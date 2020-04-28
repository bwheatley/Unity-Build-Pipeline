using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ScenesWindow : EditorWindow
    {
        #region Variables

        private GUIStyle _labelCenterBold;

        private SettingsScriptableObject _settings;
        private List<SceneAsset> _sceneAssets;

        #endregion Variables

        private void Awake()
        {
            _labelCenterBold = new GUIStyle {alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold};

            _settings = SettingsScriptableObject.GetOrCreateSettings();
            _sceneAssets = _settings.sceneAssets;
        }

        private void OnDestroy()
        {
            AssetDatabase.SaveAssets();
        }

        private void OnGUI()
        {
            EditorGUILayout.Separator();
            GUILayout.Label("Scenes", _labelCenterBold);
            EditorGUILayout.Separator();

            for (int i = 0; i < _sceneAssets.Count; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    _sceneAssets[i] =
                        (SceneAsset) EditorGUILayout.ObjectField(_sceneAssets[i], typeof(SceneAsset), true,
                            GUILayout.Height(24));

                    if (GUILayout.Button("X", GUILayout.Width(24), GUILayout.Height(24)))
                    {
                        _sceneAssets.RemoveAt(i);
                    }
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add", GUILayout.Height(24)))
            {
                _sceneAssets.Add(null);
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Apply to Build Settings", GUILayout.Height(24)))
            {
                SetEditorBuildSettingsScenes();
            }
        }

        #region Methods

        private void SetEditorBuildSettingsScenes()
        {
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
            foreach (SceneAsset sceneAsset in _sceneAssets)
            {
                string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                if (!string.IsNullOrEmpty(scenePath))
                {
                    editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                }
            }

            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        }

        #endregion Methods
    }
}