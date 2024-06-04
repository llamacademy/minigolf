using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace LlamAcademy.Minigolf.Editor
{
    public class LevelSaverWindow : EditorWindow
    {
        private Label MissingTilemapWarning => rootVisualElement.Q<Label>("missing-tilemap");
        private TextField DirectoryInput => rootVisualElement.Q<TextField>("directory-input");
        private TextField FileNameInput => rootVisualElement.Q<TextField>("file-name-input");
        private Label DuplicateFileWarning => rootVisualElement.Q<Label>("override-file-warning");
        private Label MissingFolderWarning => rootVisualElement.Q<Label>("missing-folder-warning");
        private ObjectField LoadFileField => rootVisualElement.Q<ObjectField>("load-file-field");
        private ObjectField TilesetField => rootVisualElement.Q<ObjectField>("tile-palette");
        private Button SaveButton => rootVisualElement.Q<Button>("save-button");
        private Button FindTilemapButton => rootVisualElement.Q<Button>("find-tilemap-button");
        private Button LoadButton => rootVisualElement.Q<Button>("load-button");

        //Save values on domain reload
        [SerializeField] private Object tilesetValue;
        [SerializeField] private Object loadFileValue;
        [SerializeField] private string fileName = "1";
        [SerializeField] private string directoryPath = "Assets/LlamAcademy/Minigolf/Resources/Levels/";

        private Tilemap Tilemap;
        private Grid Grid;

        [MenuItem("Tools/Tilemap Level Saver")]
        public static void ShowWindow()
        {
            LevelSaverWindow window = GetWindow<LevelSaverWindow>();
            window.titleContent = new GUIContent("🗺 Level Saver");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/LlamAcademy/Minigolf/Editor/LevelSaverWindow.uxml");
            asset.CloneTree(root);

            SaveButton.RegisterCallback<ClickEvent>(SaveToFile);
            LoadButton.RegisterCallback<ClickEvent>(LoadFromFile);
            FileNameInput.RegisterValueChangedCallback(HandleFileUpdate);
            DirectoryInput.RegisterValueChangedCallback(HandleDirectoryUpdate);

            TilesetField.value = tilesetValue;
            LoadFileField.value = loadFileValue;
            FileNameInput.value = fileName;
            DirectoryInput.value = directoryPath;

            FindTilemap();
        }

        private void HandleDirectoryUpdate(ChangeEvent<string> evt)
        {
            UpdateWarningVisibility(FileNameInput.value?.Trim(), evt.newValue.Trim());
        }

        private void HandleFileUpdate(ChangeEvent<string> evt)
        {
            UpdateWarningVisibility(evt.newValue.Trim(), DirectoryInput.value?.Trim());
        }

        private void UpdateWarningVisibility(string fileName, string directory)
        {
            if (FileExistsAtDirectory(fileName))
            {
                DuplicateFileWarning.AddToClassList("hidden");
            }
            else
            {
                DuplicateFileWarning.RemoveFromClassList("hidden");
            }

            if (Directory.Exists(directory))
            {
                MissingFolderWarning.AddToClassList("hidden");
            }
            else
            {
                MissingFolderWarning.RemoveFromClassList("hidden");
            }
        }

        private void OnDisable()
        {
            tilesetValue = TilesetField.value;
            loadFileValue = LoadFileField.value;
            fileName = FileNameInput.value;
            directoryPath = DirectoryInput.value;
        }

        private void OnFocus()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (Tilemap == null || Grid == null) return;

            Bounds bounds = new();
            bool isFirstChild = true;

            foreach (Transform child in Grid.transform)
            {
                if (child.TryGetComponent(out Renderer renderer) && renderer is not TilemapRenderer)
                {
                    if (isFirstChild)
                    {
                        bounds = renderer.bounds;
                        isFirstChild = false;
                    }
                    else
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                }
            }

            // we can lose accuracy since not all renderer bounds take up a full unit.
            BoundsInt boundsInt = new()
            {
                position = Vector3Int.CeilToInt(bounds.center - bounds.size),
                size = Vector3Int.CeilToInt(bounds.size * 2)
            };

            Handles.color = Color.red;
            Handles.DrawWireCube(boundsInt.center, boundsInt.size);
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void FindTilemap(ClickEvent _ = null)
        {
            Tilemap = FindObjectsByType<Tilemap>(FindObjectsSortMode.None).FirstOrDefault();
            Grid = FindObjectsByType<Grid>(FindObjectsSortMode.None).FirstOrDefault();
            FindTilemapButton.RegisterCallbackOnce<ClickEvent>(FindTilemap);

            if (Tilemap == null || Grid == null)
            {
                SaveButton.enabledSelf = false;
                LoadButton.enabledSelf = false;

                MissingTilemapWarning.RemoveFromClassList("hidden");
                FindTilemapButton.RemoveFromClassList("hidden");
            }
            else
            {
                MissingTilemapWarning.AddToClassList("hidden");
                FindTilemapButton.AddToClassList("hidden");
            }
        }


        private void SaveToFile(ClickEvent _)
        {
            FindTilemap();
            GameObject grid = (GameObject)TilesetField.value;
            Tilemap tilemap = grid.transform.GetChild(0).GetComponent<Tilemap>();

            List<PrefabSpawnData> saveData = new();
            foreach (Transform child in Grid.transform)
            {
                if (child.TryGetComponent(out Renderer renderer) && renderer is not TilemapRenderer)
                {
                    if (!PrefabUtility.IsPartOfAnyPrefab(child.gameObject))
                    {
                        Debug.LogWarning($"{child.gameObject} is not a Prefab! This will be lost on save");
                    }
                    else
                    {
                        Object prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
                        string path = AssetDatabase.GetAssetPath(prefabSource);
                        string resourcesPath = path[path.IndexOf("Resources/")..];
                        resourcesPath = resourcesPath.TrimStart("Resources/".ToCharArray()).TrimEnd(".prefab".ToCharArray());

                        saveData.Add(new PrefabSpawnData(resourcesPath, Vector3Int.CeilToInt(child.position), child.rotation));
                    }
                }
            }

            Level level = CreateInstance<Level>();
            level.WorldObjectPositions = saveData;

            string directory = DirectoryInput.value.Trim();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string fileName = FileNameInput.value.Trim();
            string filePath = directory.EndsWith("/") ? $"{directory}{fileName}.asset" : $"{directory}/{fileName}.asset";

            AssetDatabase.DeleteAsset(filePath);
            AssetDatabase.CreateAsset(level, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (int.TryParse(fileName, out int result))
            {
                FileNameInput.value = (result + 1).ToString();
            }
        }

        private string GetFullyQualifiedFileName(string fileName)
        {
            string directory = DirectoryInput.value.Trim();
            string filePath = directory.EndsWith("/") ? $"{directory}{fileName}.asset" : $"{directory}/{fileName}.asset";
            return filePath;
        }

        private bool FileExistsAtDirectory(string fileName)
        {
            return File.Exists(GetFullyQualifiedFileName(fileName));
        }

        private void LoadFromFile(ClickEvent _)
        {

        }
    }
}
