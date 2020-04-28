using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BuildWindow : EditorWindow
    {
        #region Variables

        private GUIStyle _labelCenterBold;
        private GUIStyle _buttonLeft;

        #endregion Variables

        [MenuItem("Build/Build")]
        private static void OpenWindow()
        {
            BuildWindow window = GetWindow<BuildWindow>(false, "Build");
            window.Show();
        }

        private void Awake()
        {
            _labelCenterBold = new GUIStyle {alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold};
            _buttonLeft = EditorStyles.miniButton;
            _buttonLeft.alignment = TextAnchor.MiddleLeft;
            _buttonLeft.fixedHeight = 24;
        }

        private void OnDestroy()
        {
            // throw new NotImplementedException();
        }

        private void OnGUI()
        {
            EditorGUILayout.Separator();
            GUILayout.Label("Scenes", _labelCenterBold);
            EditorGUILayout.Separator();

            if (GUILayout.Button("Scenes", _buttonLeft))
            {
                ScenesWindow scenesWindow = GetWindow<ScenesWindow>(false, "Scenes");
                scenesWindow.ShowUtility();
            }

            EditorGUILayout.Separator();
            GUILayout.Label("Build Configs", _labelCenterBold);
            EditorGUILayout.Separator();

            if (GUILayout.Button("Debug Windows", _buttonLeft))
            {
            }

            if (GUILayout.Button("Debug Windows (IL2CPP)", _buttonLeft))
            {
            }

            if (GUILayout.Button("Release Windows | Mac | Linux", _buttonLeft))
            {
            }

            if (GUILayout.Button("Release Windows | Mac | Linux (IL2CPP)", _buttonLeft))
            {
            }
        }

        #region Methods

        #endregion Methods
    }
}