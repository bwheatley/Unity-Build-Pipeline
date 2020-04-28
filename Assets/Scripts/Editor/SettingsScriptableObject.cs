using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class SettingsScriptableObject : ScriptableObject
    {
        #region Variables

        // NOTE: Update IsValidFolder and CreateFolder calls below if you change folders.
        private const string SettingsPath = "Assets/Resources/BuildSettings.asset";

        public List<SceneAsset> sceneAssets;

        #endregion Variables

        public static SettingsScriptableObject GetOrCreateSettings()
        {
            SettingsScriptableObject settings = AssetDatabase.LoadAssetAtPath<SettingsScriptableObject>(SettingsPath);

            if (settings == null)
            {
                settings = CreateInstance<SettingsScriptableObject>();
                settings.sceneAssets = new List<SceneAsset>();

                // TODO: Hard coded.
                if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }

                AssetDatabase.CreateAsset(settings, SettingsPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        #region Methods

        public string[] GetSceneAssetsPathArray()
        {
            List<string> sceneAssetPaths = new List<string>();
            foreach (SceneAsset sceneAsset in sceneAssets)
            {
                if (sceneAsset != null)
                {
                    sceneAssetPaths.Add(AssetDatabase.GetAssetPath(sceneAsset));
                }
            }

            return sceneAssetPaths.ToArray();
        }

        #endregion Methods
    }
}