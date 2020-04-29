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

        #region Methods
        
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

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("None", GUILayout.MaxWidth(position.width / 2), GUILayout.Height(24)))
                {
                    _sceneAssets.Clear();
                }

                if (GUILayout.Button("All", GUILayout.MaxWidth(position.width / 2), GUILayout.Height(24)))
                {
                    foreach (string assetGuid in AssetDatabase.FindAssets("t:Scene", new[] {"Assets/Scenes"}))
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                        if (string.IsNullOrEmpty(assetPath)) continue;

                        SceneAsset asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
                        if (_sceneAssets.Contains(asset)) continue;

                        _sceneAssets.Add(asset);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        #endregion Methods
    }
}