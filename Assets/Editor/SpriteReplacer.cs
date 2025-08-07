using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class SpriteReplacer : EditorWindow
{
    private List<Sprite> oldSprites = new List<Sprite>();
    private List<Sprite> newSprites = new List<Sprite>();
    private DefaultAsset targetFolder;
    private Vector2 scroll;

    [MenuItem("Tools/Multi Sprite Replacer (Drag & Drop)")]
    public static void ShowWindow()
    {
        GetWindow<SpriteReplacer>("Sprite Replacer (Drag & Drop)");
    }

    private void OnGUI()
    {
        GUILayout.Label("Replace Sprites in Folder & Scenes (Multi-drag Support)", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        targetFolder = (DefaultAsset)EditorGUILayout.ObjectField("🎯 Target Folder", targetFolder, typeof(DefaultAsset), false);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("🎨 Old Sprites (Drag Multiple Here):", EditorStyles.boldLabel);
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag Sprites Here", EditorStyles.helpBox);

        HandleDragAndDrop(dropArea);

        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(300));
        for (int i = 0; i < oldSprites.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            DrawSpritePreview(oldSprites[i]);
            oldSprites[i] = (Sprite)EditorGUILayout.ObjectField(oldSprites[i], typeof(Sprite), false, GUILayout.Width(100));

            GUILayout.Label("→", GUILayout.Width(20));

            DrawSpritePreview(newSprites[i]);
            newSprites[i] = (Sprite)EditorGUILayout.ObjectField(newSprites[i], typeof(Sprite), false, GUILayout.Width(100));

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                oldSprites.RemoveAt(i);
                newSprites.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        if (GUILayout.Button("🔁 Replace Sprites In Folder & Scenes"))
        {
            if (!ValidateInputs()) return;
            ReplaceInFolder();
            ReplaceInScenes();
        }
    }

    private void HandleDragAndDrop(Rect dropArea)
    {
        Event evt = Event.current;
        if ((evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform) && dropArea.Contains(evt.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                int added = 0;

                foreach (Object draggedObj in DragAndDrop.objectReferences)
                {
                    Sprite sprite = draggedObj as Sprite;

                    // Nếu là Texture2D, thử load Sprite đầu tiên trong đó
                    if (sprite == null && draggedObj is Texture2D texture)
                    {
                        string path = AssetDatabase.GetAssetPath(texture);
                        Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
                        foreach (var subAsset in subAssets)
                        {
                            if (subAsset is Sprite s && !oldSprites.Contains(s))
                            {
                                oldSprites.Add(s);
                                newSprites.Add(null);
                                added++;
                            }
                        }
                        continue;
                    }

                    if (sprite != null && !oldSprites.Contains(sprite))
                    {
                        oldSprites.Add(sprite);
                        newSprites.Add(null);
                        added++;
                    }
                }

                evt.Use();
                if (added > 0) Repaint();
            }
        }
    }

    private void DrawSpritePreview(Sprite sprite)
    {
        if (sprite != null)
        {
            Texture2D tex = AssetPreview.GetAssetPreview(sprite) ?? AssetPreview.GetMiniThumbnail(sprite);
            GUILayout.Box(tex, GUILayout.Width(40), GUILayout.Height(40));
        }
        else
        {
            GUILayout.Box("", GUILayout.Width(40), GUILayout.Height(40));
        }
    }

    private bool ValidateInputs()
    {
        if (oldSprites.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No sprite pairs to replace.", "OK");
            return false;
        }

        if (targetFolder == null)
        {
            EditorUtility.DisplayDialog("Error", "Please select a target folder.", "OK");
            return false;
        }

        for (int i = 0; i < oldSprites.Count; i++)
        {
            if (oldSprites[i] == null || newSprites[i] == null)
            {
                EditorUtility.DisplayDialog("Error", $"Sprite pair at row {i + 1} is incomplete.", "OK");
                return false;
            }
        }

        return true;
    }

    // Replace Sprite reference in ScriptableObject/Prefab
    private void ReplaceInFolder()
    {
        string folderPath = AssetDatabase.GetAssetPath(targetFolder);
        string[] guids = AssetDatabase.FindAssets("", new[] { folderPath });

        int replaceCount = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Prefab asset
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab != null)
            {
                bool modified = false;
                // Replace in SpriteRenderer
                foreach (var sr in prefab.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    for (int i = 0; i < oldSprites.Count; i++)
                    {
                        if (sr.sprite == oldSprites[i])
                        {
                            sr.sprite = newSprites[i];
                            modified = true;
                            replaceCount++;
                        }
                    }
                }
                // Replace in UI Image
                foreach (var img in prefab.GetComponentsInChildren<Image>(true))
                {
                    for (int i = 0; i < oldSprites.Count; i++)
                    {
                        if (img.sprite == oldSprites[i])
                        {
                            img.sprite = newSprites[i];
                            modified = true;
                            replaceCount++;
                        }
                    }
                }
                if (modified)
                {
                    EditorUtility.SetDirty(prefab);
                    PrefabUtility.SavePrefabAsset(prefab);
                }
            }
            else // ScriptableObject hoặc các loại asset khác
            {
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                bool modified = false;

                foreach (Object asset in assets)
                {
                    if (asset == null) continue;

                    SerializedObject so = new SerializedObject(asset);
                    SerializedProperty prop = so.GetIterator();

                    while (prop.NextVisible(true))
                    {
                        if (prop.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            for (int i = 0; i < oldSprites.Count; i++)
                            {
                                if (prop.objectReferenceValue == oldSprites[i])
                                {
                                    prop.objectReferenceValue = newSprites[i];
                                    modified = true;
                                    replaceCount++;
                                    break;
                                }
                            }
                        }
                    }
                    if (modified)
                    {
                        so.ApplyModifiedProperties();
                        EditorUtility.SetDirty(asset);
                    }
                }
                if (modified)
                {
                    AssetDatabase.SaveAssets();
                }
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done", $"Replaced {replaceCount} sprite reference(s) in folder '{folderPath}'.", "OK");
    }

    // Replace Sprite reference in all Scene files in the folder
    private void ReplaceInScenes()
    {
        string folderPath = AssetDatabase.GetAssetPath(targetFolder);
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { folderPath });

        int sceneReplaceCount = 0;
        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            bool modified = false;

            foreach (var go in scene.GetRootGameObjects())
            {
                foreach (var sr in go.GetComponentsInChildren<SpriteRenderer>(true))
                {
                    for (int i = 0; i < oldSprites.Count; i++)
                    {
                        if (sr.sprite == oldSprites[i])
                        {
                            sr.sprite = newSprites[i];
                            modified = true;
                            sceneReplaceCount++;
                        }
                    }
                }
                foreach (var img in go.GetComponentsInChildren<Image>(true))
                {
                    for (int i = 0; i < oldSprites.Count; i++)
                    {
                        if (img.sprite == oldSprites[i])
                        {
                            img.sprite = newSprites[i];
                            modified = true;
                            sceneReplaceCount++;
                        }
                    }
                }
            }
            if (modified)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }
        EditorUtility.DisplayDialog("Done", $"Replaced {sceneReplaceCount} sprite reference(s) in scenes under '{folderPath}'.", "OK");
        AssetDatabase.Refresh();
    }
}