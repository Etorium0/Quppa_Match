using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class MultiHueShiftEditor : EditorWindow
{
    private List<Texture2D> sourceTextures = new List<Texture2D>();
    private float hueShift = 0f;
    private float saturationShift = 0f;
    private float lightnessShift = 0f;
    private Dictionary<Texture2D, Texture2D> previewMap = new Dictionary<Texture2D, Texture2D>();
    private Vector2 scrollPos;
    private const float previewAreaMaxHeight = 400f;

    [MenuItem("Tools/Multi Sprite HSL Shifter")]
    public static void ShowWindow()
    {
        GetWindow<MultiHueShiftEditor>("Multi HSL Shifter");
    }

    void OnGUI()
    {
        GUILayout.Label("🎨 Kéo nhiều sprite textures vào vùng dưới", EditorStyles.boldLabel);
        DrawDropArea();

        EditorGUI.BeginChangeCheck();

        hueShift = EditorGUILayout.Slider("Hue Shift (°)", hueShift, -180f, 180f);
        saturationShift = EditorGUILayout.Slider("Saturation Shift", saturationShift, -1f, 1f);
        lightnessShift = EditorGUILayout.Slider("Lightness Shift", lightnessShift, -1f, 1f);

        if (EditorGUI.EndChangeCheck())
        {
            UpdatePreviews();
        }


        if (previewMap.Count > 0)
        {
            GUILayout.Space(10);
            GUILayout.Label("👁️ Preview", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(previewAreaMaxHeight));
            foreach (var tex in sourceTextures.ToArray())
            {
                if (!previewMap.ContainsKey(tex)) continue;

                GUILayout.BeginHorizontal("box");

                GUILayout.BeginVertical(GUILayout.Width(70));
                GUILayout.Label("Gốc", EditorStyles.miniBoldLabel);
                GUILayout.Label(tex, GUILayout.Width(64), GUILayout.Height(64));
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.Width(70));
                GUILayout.Label("HSL", EditorStyles.miniBoldLabel);
                GUILayout.Label(previewMap[tex], GUILayout.Width(64), GUILayout.Height(64));
                GUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                GUILayout.BeginVertical();
                GUILayout.Label(tex.name);
                if (GUILayout.Button("❌ Xoá", GUILayout.Width(50)))
                {
                    sourceTextures.Remove(tex);
                    previewMap.Remove(tex);
                    GUIUtility.ExitGUI();
                }
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("💾 Ghi đè tất cả"))
            {
                if (EditorUtility.DisplayDialog("Xác nhận", $"Ghi đè {previewMap.Count} texture?", "Ghi đè", "Hủy"))
                {
                    foreach (var kv in previewMap)
                    {
                        OverwriteTexture(kv.Key, kv.Value);
                    }
                    AssetDatabase.Refresh();
                    Debug.Log("✅ Đã ghi đè tất cả texture.");
                }
            }
            GUI.backgroundColor = Color.white;
        }

        if (GUILayout.Button("🗑 Xoá toàn bộ danh sách"))
        {
            sourceTextures.Clear();
            previewMap.Clear();
        }
    }

    void DrawDropArea()
    {
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 80.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "KÉO SPRITE HOẶC TEXTURE VÀO ĐÂY", EditorStyles.helpBox);

        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (dropArea.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    bool anyAdded = false;

                    foreach (Object obj in DragAndDrop.objectReferences)
                    {
                        Texture2D tex = null;

                        if (obj is Texture2D t)
                            tex = t;

                        else if (obj is Sprite s && s.texture != null)
                            tex = s.texture;

                        if (tex != null && !sourceTextures.Contains(tex))
                        {
                            sourceTextures.Add(tex);
                            anyAdded = true;
                        }
                    }

                    if (anyAdded)
                        UpdatePreviews();

                    evt.Use();
                }
            }
        }
    }

    void UpdatePreviews()
    {
        previewMap.Clear();
        foreach (var tex in sourceTextures)
        {
            previewMap[tex] = ApplyHSLShift(tex, hueShift, saturationShift, lightnessShift);
        }
    }

    Texture2D ApplyHSLShift(Texture2D original, float hueDeg, float satShift, float lightShift)
    {
        MakeTextureReadable(original);

        Texture2D tempTex = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
        tempTex.filterMode = original.filterMode;

        Color[] pixels = original.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            Color.RGBToHSV(pixels[i], out float h, out float s, out float v);

            h = (h + hueDeg / 360f) % 1f;
            if (h < 0) h += 1f;

            s = Mathf.Clamp01(s + satShift);
            v = Mathf.Clamp01(v + lightShift);

            Color newColor = Color.HSVToRGB(h, s, v);
            newColor.a = pixels[i].a;
            pixels[i] = newColor;
        }

        tempTex.SetPixels(pixels);
        tempTex.Apply();
        return tempTex;
    }

    void OverwriteTexture(Texture2D original, Texture2D modified)
    {
        string path = AssetDatabase.GetAssetPath(original);

        // Lưu lại setting cũ
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        bool wasReadable = importer.isReadable;
        TextureImporterCompression compression = importer.textureCompression;
        TextureImporterType textureType = importer.textureType;
        TextureImporterNPOTScale npot = importer.npotScale;

        // Ghi đè file PNG
        byte[] pngData = modified.EncodeToPNG();
        File.WriteAllBytes(path, pngData);

        // Force reimport
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer is TextureImporter texImporter)
        {
            // Khôi phục setting gốc
            texImporter.isReadable = wasReadable;
            texImporter.textureCompression = compression;
            texImporter.textureType = textureType;
            texImporter.npotScale = npot;

            // ⚠️ Xoá override trên tất cả platform để giữ nguyên kích thước
            texImporter.ClearPlatformTextureSettings("Android");
            texImporter.ClearPlatformTextureSettings("iPhone");
            texImporter.ClearPlatformTextureSettings("Standalone");
            texImporter.ClearPlatformTextureSettings("WebGL");

            texImporter.SaveAndReimport();
        }
    }


    void MakeTextureReadable(Texture2D tex)
    {
        string path = AssetDatabase.GetAssetPath(tex);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer != null && !importer.isReadable)
        {
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }
}
