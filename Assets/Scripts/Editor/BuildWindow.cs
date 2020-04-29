using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor
{
    public class BuildWindow : EditorWindow
    {
        #region Variables

        private const BuildOptions DebugBuildOptions = BuildOptions.Development | BuildOptions.CompressWithLz4;
        private const BuildOptions ReleaseBuildOptions = BuildOptions.CompressWithLz4HC;

        private GUIStyle _labelCenterBold;
        private GUIStyle _buttonLeft;

        private SettingsScriptableObject _settings;

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
            _buttonLeft = new GUIStyle(EditorStyles.miniButton) {alignment = TextAnchor.MiddleLeft, fixedHeight = 24};
            
            _settings = SettingsScriptableObject.GetOrCreateSettings();
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

            if (GUILayout.Button("Debug Windows", _buttonLeft) && !BuildPipeline.isBuildingPlayer)
            {
                Build(ScriptingImplementation.Mono2x, BuildTarget.StandaloneWindows64, "Debug");
            }

            if (GUILayout.Button("Debug Windows (IL2CPP)", _buttonLeft) && !BuildPipeline.isBuildingPlayer)
            {
                Build(ScriptingImplementation.IL2CPP, BuildTarget.StandaloneWindows64, "Debug");
            }

            if (GUILayout.Button("Release Windows | Mac | Linux", _buttonLeft) && !BuildPipeline.isBuildingPlayer)
            {
                Build(ScriptingImplementation.Mono2x, BuildTarget.StandaloneWindows64, "Release");
                Build(ScriptingImplementation.Mono2x, BuildTarget.StandaloneOSX, "Release");
                Build(ScriptingImplementation.Mono2x, BuildTarget.StandaloneLinux64, "Release");
            }

            if (GUILayout.Button("Release Windows | Mac | Linux (IL2CPP)", _buttonLeft) &&
                !BuildPipeline.isBuildingPlayer)
            {
                Build(ScriptingImplementation.IL2CPP, BuildTarget.StandaloneWindows64, "Release");
                Build(ScriptingImplementation.IL2CPP, BuildTarget.StandaloneOSX, "Release");
                Build(ScriptingImplementation.IL2CPP, BuildTarget.StandaloneLinux64, "Release");
            }
        }

        #region Methods

        private void Build(ScriptingImplementation backend, BuildTarget target, string buildType)
        {
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(target);
            PlayerSettings.SetScriptingBackend(targetGroup, backend);

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.locationPathName = $"Builds/{buildType}/";

            switch (target)
            {
                case BuildTarget.StandaloneWindows64:
                    buildPlayerOptions.locationPathName += $"Windows/{PlayerSettings.productName}.exe";
                    break;
                case BuildTarget.StandaloneOSX:
                    buildPlayerOptions.locationPathName += $"Mac/{PlayerSettings.productName}.app";
                    break;
                case BuildTarget.StandaloneLinux64:
                    buildPlayerOptions.locationPathName += $"Linux/{PlayerSettings.productName}.x86_64";
                    break;
                default:
                    throw new NotImplementedException();
                    return;
            }

            if (buildType == "Debug")
            {
                buildPlayerOptions.options = DebugBuildOptions;
            }
            else if (buildType == "Release")
            {
                buildPlayerOptions.options = ReleaseBuildOptions;
            }
            else
            {
                throw new NotImplementedException();
            }

            buildPlayerOptions.scenes = _settings.GetSceneAssetsPathArray();
            buildPlayerOptions.target = target;
            buildPlayerOptions.targetGroup = targetGroup;

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Unknown:
                    break;
                case BuildResult.Succeeded:
                    Debug.Log($"Build Succeeded: {summary.totalSize} bytes");
                    break;
                case BuildResult.Failed:
                    Debug.Log("Build Failed");
                    break;
                case BuildResult.Cancelled:
                    Debug.Log("Build Cancelled");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion Methods
    }
}