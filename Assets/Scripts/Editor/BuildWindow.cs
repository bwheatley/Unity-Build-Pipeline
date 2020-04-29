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
        private GUIStyle _numberFieldCenter;

        private SettingsScriptableObject _settings;

        private int _major;
        private int _minor;
        private int _patch;

        #endregion Variables

        [MenuItem("Build/Build")]
        private static void OpenWindow()
        {
            BuildWindow window = GetWindow<BuildWindow>(false, "Build");
            window.Show();
        }

        private static (string, string) TargetToStringAndFileExtension(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows64:
                    return ("Windows", "exe");
                case BuildTarget.StandaloneOSX:
                    return ("Mac", "app");
                case BuildTarget.StandaloneLinux64:
                    return ("Linux", "x86_64");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Methods

        private void Awake()
        {
            _labelCenterBold = new GUIStyle {alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold};
            _buttonLeft = new GUIStyle(EditorStyles.miniButton) {alignment = TextAnchor.MiddleLeft, fixedHeight = 24};
            _numberFieldCenter = new GUIStyle(EditorStyles.numberField) {alignment = TextAnchor.MiddleCenter};

            _settings = SettingsScriptableObject.GetOrCreateSettings();

            string[] version = PlayerSettings.bundleVersion.Split('.');
            if (version.Length != 3)
            {
                _major = 0;
                _minor = 1;
                _patch = 0;
                return;
            }

            _major = int.TryParse(version[0], out _major) ? _major : 0;
            _minor = int.TryParse(version[1], out _minor) ? _minor : 1;
            _patch = int.TryParse(version[2], out _patch) ? _patch : 0;
        }

        private void OnDestroy()
        {
            AssetDatabase.SaveAssets();
            PlayerSettings.bundleVersion = $"{_major}.{_minor}.{_patch}";
        }

        private void OnGUI()
        {
            EditorGUILayout.Separator();

            PlayerSettings.companyName = EditorGUILayout.TextField("Company Name", PlayerSettings.companyName);
            PlayerSettings.productName = EditorGUILayout.TextField("Product Name", PlayerSettings.productName);

            EditorGUILayout.Separator();
            GUILayout.Label("Version", _labelCenterBold);
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            {
                // TODO: I need MaxHeight to stop this and the buttons overlapping.
                _major = EditorGUILayout.IntField(_major, _numberFieldCenter, GUILayout.Height(24));
                _minor = EditorGUILayout.IntField(_minor, _numberFieldCenter, GUILayout.Height(24));
                _patch = EditorGUILayout.IntField(_patch, _numberFieldCenter, GUILayout.Height(24));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Major", GUILayout.Height(24)))
                {
                    _major++;
                    _minor = 0;
                    _patch = 0;
                }

                if (GUILayout.Button("Minor", GUILayout.Height(24)))
                {
                    _minor++;
                    _patch = 0;
                }

                if (GUILayout.Button("Patch", GUILayout.Height(24)))
                {
                    _patch++;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            GUILayout.Label("Scenes", _labelCenterBold);
            EditorGUILayout.Separator();

            if (GUILayout.Button("Scenes", _buttonLeft))
            {
                ScenesWindow scenesWindow = GetWindow<ScenesWindow>(false, "Scenes");
                scenesWindow.ShowUtility();
            }

            EditorGUILayout.Separator();
            GUILayout.Label("Debug", _labelCenterBold);
            EditorGUILayout.Separator();

            _settings.autoconnectProfiler =
                EditorGUILayout.Toggle("Autoconnect Profiler", _settings.autoconnectProfiler);
            _settings.deepProfilingSupport =
                EditorGUILayout.Toggle("Deep Profiling Support", _settings.deepProfilingSupport);
            _settings.buildAndRun = EditorGUILayout.Toggle("Build and Run", _settings.buildAndRun);

            EditorGUILayout.Separator();


            if (GUILayout.Button("Debug Windows", _buttonLeft) &&
                !BuildPipeline.isBuildingPlayer)
            {
                Build(ScriptingImplementation.Mono2x, BuildTarget.StandaloneWindows64, true);
            }

            if (GUILayout.Button("Debug Windows (IL2CPP)", _buttonLeft) && !BuildPipeline.isBuildingPlayer)
            {
                Build(ScriptingImplementation.IL2CPP, BuildTarget.StandaloneWindows64, true);
            }

            EditorGUILayout.Separator();
            GUILayout.Label("Release", _labelCenterBold);
            EditorGUILayout.Separator();

            if (GUILayout.Button("Release Windows | Mac | Linux", _buttonLeft) && !BuildPipeline.isBuildingPlayer)
            {
                Build(ScriptingImplementation.Mono2x, BuildTarget.StandaloneWindows64);
                Build(ScriptingImplementation.Mono2x, BuildTarget.StandaloneOSX);
                Build(ScriptingImplementation.Mono2x, BuildTarget.StandaloneLinux64);
            }

            if (GUILayout.Button("Release Windows | Mac | Linux (IL2CPP)", _buttonLeft) &&
                !BuildPipeline.isBuildingPlayer)
            {
                Build(ScriptingImplementation.IL2CPP, BuildTarget.StandaloneWindows64);
                Build(ScriptingImplementation.IL2CPP, BuildTarget.StandaloneOSX);
                Build(ScriptingImplementation.IL2CPP, BuildTarget.StandaloneLinux64);
            }
        }

        private void Build(ScriptingImplementation backend, BuildTarget target, bool debug = false)
        {
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(target);
            PlayerSettings.SetScriptingBackend(targetGroup, backend);

            BuildOptions debugBuildOptions = DebugBuildOptions;
            if (_settings.autoconnectProfiler) debugBuildOptions |= BuildOptions.ConnectWithProfiler;
            if (_settings.deepProfilingSupport) debugBuildOptions |= BuildOptions.EnableDeepProfilingSupport;
            if (_settings.buildAndRun) debugBuildOptions |= BuildOptions.AutoRunPlayer;

            (string targetStr, string fileExtension) = TargetToStringAndFileExtension(target);
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                locationPathName =
                    $"Builds/{PlayerSettings.bundleVersion}/{(debug ? "Debug" : "Release")}/{targetStr}/{PlayerSettings.productName}.{fileExtension}",
                options = debug ? debugBuildOptions : ReleaseBuildOptions,
                scenes = _settings.GetSceneAssetsPathArray(),
                target = target,
                targetGroup = targetGroup
            };

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