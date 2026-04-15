using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class IAPCatalogAndButtonRenamer : EditorWindow
{
    string oldPrefix = "slimegoo_inapp_";
    string newPrefix = "tilupop_inapp_";
    const string CatalogPath = "Assets/Resources/IAPProductCatalog.json";

    [MenuItem("Tools/IAP/Rename Catalog + Buttons")]
    static void Open() => GetWindow<IAPCatalogAndButtonRenamer>("IAP Renamer");

    void OnGUI()
    {
        oldPrefix = EditorGUILayout.TextField("Old prefix", oldPrefix);
        newPrefix = EditorGUILayout.TextField("New prefix", newPrefix);

        if (GUILayout.Button("Run"))
        {
            // 1) Load catalog JSON
            var root = LoadCatalogJson(CatalogPath, out JArray products);
            if (root == null) return;

            // 2) Rename IDs trong catalog theo prefix cũ -> mới
            int a = RenameCatalogProducts(products, oldPrefix, newPrefix);

            // 3) Xử lý Scene: rename các nút có prefix cũ, và GÁN ID CHO NÚT TRỐNG
            int b = ProcessButtonsInOpenScenes(products, oldPrefix, newPrefix);

            // 4) Xử lý tất cả Prefab tương tự
            int c = ProcessButtonsInAllPrefabs(products, oldPrefix, newPrefix);

            // 5) Lưu catalog lại (đã thêm các ID còn thiếu)
            SaveCatalogJson(CatalogPath, root);

            AssetDatabase.ImportAsset(CatalogPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Catalog renamed {a} items. Scene buttons {b}. Prefab buttons {c}. Done.");
        }
    }

    // ---------- Catalog JSON ----------
    static JObject LoadCatalogJson(string path, out JArray products)
    {
        products = null;
        if (!File.Exists(path))
        {
            Debug.LogError($"Catalog not found at: {path}");
            return null;
        }
        var json = File.ReadAllText(path);
        var root = JObject.Parse(json);
        products = (JArray)(root["products"] ?? new JArray());
        root["products"] ??= products;
        return root;
    }

    static void SaveCatalogJson(string path, JObject root)
    {
        File.WriteAllText(path, root.ToString(Newtonsoft.Json.Formatting.Indented));
    }

    static int RenameCatalogProducts(JArray products, string oldPref, string newPref)
    {
        int changed = 0;
        foreach (var p in products.OfType<JObject>())
        {
            var id = p["id"]?.Value<string>();
            if (!string.IsNullOrEmpty(id) && id.StartsWith(oldPref))
            {
                p["id"] = newPref + id.Substring(oldPref.Length);
                changed++;
            }

            var allStoreIDs = p["allStoreIDs"] as JArray;
            if (allStoreIDs != null)
            {
                foreach (var sid in allStoreIDs.OfType<JObject>())
                {
                    var sidVal = sid["id"]?.Value<string>();
                    if (!string.IsNullOrEmpty(sidVal) && sidVal.StartsWith(oldPref))
                        sid["id"] = newPref + sidVal.Substring(oldPref.Length);
                }
            }
        }
        return changed;
    }

    static void EnsureProductExists(JArray products, string id, JObject template = null)
    {
        if (products.OfType<JObject>().Any(p => string.Equals(p["id"]?.Value<string>(), id)))
            return;

        JObject newProd;
        if (template != null)
        {
            newProd = (JObject)template.DeepClone();
            newProd["id"] = id;
            // dọn bớt mô tả/override nếu muốn sạch
            newProd["descriptions"] = new JArray();
            newProd["allStoreIDs"] = new JArray();
            newProd["payouts"] = new JArray();
        }
        else
        {
            // tối thiểu: id + type
            newProd = new JObject
            {
                ["id"] = id,
                ["type"] = "Consumable",   // an toàn
                ["descriptions"] = new JArray(),
                ["allStoreIDs"] = new JArray(),
                ["payouts"] = new JArray()
            };
        }
        products.Add(newProd);
        Debug.Log($"[Catalog] Added missing product: {id}");
    }

    // ---------- Buttons (scene & prefab) ----------
    static Object[] FindIAPLikeComps(GameObject root)
        => root.GetComponentsInChildren<MonoBehaviour>(true)
               .Where(mb =>
               {
                   if (mb == null) return false;
                   var so = new SerializedObject(mb);
                   return so.FindProperty("m_ProductID") != null ||
                          so.FindProperty("productId")  != null;
               })
               .Cast<Object>().ToArray();

    static string ExtractIndexFromName(string goName)
    {
        // Ưu tiên dạng "name (12)"
        var m = Regex.Match(goName, @"\((\d+)\)");
        if (m.Success) return m.Groups[1].Value;

        // Thử "_12" hoặc "name12" ở cuối
        m = Regex.Match(goName, @"(?:_|)(\d+)$");
        if (m.Success) return m.Groups[1].Value;

        return null;
    }

    static bool SetProductId(Object comp, string targetId)
    {
        var so = new SerializedObject(comp);
        var prop = so.FindProperty("m_ProductID") ?? so.FindProperty("productId");
        if (prop == null || prop.propertyType != SerializedPropertyType.String) return false;

        prop.stringValue = targetId;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(comp);
        return true;
    }

    static int ProcessButtonsInContainer(GameObject container, JArray products, string oldPref, string newPref, JObject templateForNew = null)
    {
        int count = 0;
        foreach (var comp in FindIAPLikeComps(container))
        {
            var so = new SerializedObject(comp);
            var prop = so.FindProperty("m_ProductID") ?? so.FindProperty("productId");
            string val = prop?.stringValue ?? string.Empty;

            if (!string.IsNullOrEmpty(val))
            {
                // rename theo prefix cũ -> mới
                if (val.StartsWith(oldPref))
                {
                    var newId = newPref + val.Substring(oldPref.Length);
                    EnsureProductExists(products, newId, templateForNew);
                    SetProductId(comp, newId);
                    count++;
                }
            }
            else
            {
                // TRƯỜNG HỢP TRỐNG: suy số từ tên GO -> gán newPrefix + number
                var go = ((Component)comp).gameObject;
                var num = ExtractIndexFromName(go.name);
                if (!string.IsNullOrEmpty(num))
                {
                    var newId = $"{newPref}{num}";
                    EnsureProductExists(products, newId, templateForNew);
                    SetProductId(comp, newId);
                    count++;
                }
            }
        }
        return count;
    }

    static int ProcessButtonsInOpenScenes(JArray products, string oldPref, string newPref)
    {
        // chọn 1 sản phẩm làm template khi cần tạo mới
        var template = products.OfType<JObject>().FirstOrDefault();
        int total = 0;
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        foreach (var root in scene.GetRootGameObjects())
            total += ProcessButtonsInContainer(root, products, oldPref, newPref, template);
        return total;
    }

    static int ProcessButtonsInAllPrefabs(JArray products, string oldPref, string newPref)
    {
        var template = products.OfType<JObject>().FirstOrDefault();
        int total = 0;

        foreach (var guid in AssetDatabase.FindAssets("t:Prefab"))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var root = PrefabUtility.LoadPrefabContents(path);

            int before = total;
            total += ProcessButtonsInContainer(root, products, oldPref, newPref, template);

            if (total > before) PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
        }
        return total;
    }
}
