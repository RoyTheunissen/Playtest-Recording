using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RoyTheunissen.PlaytestRecording.Building.Editor
{
    /// <summary>
    /// A convenient window for editing build information. The fields are displayed using 
    /// reflection, so you can add new fields to the build info and it will be added dynamically.
    /// </summary>
    public class BuildWindow : EditorWindow
    {
        private const string KeyPrefix = "BuildWindow/";
        private const string BuildTargetKey = KeyPrefix + "BuildTarget";
        private const string BuildOptionsKey = KeyPrefix + "BuildOptions";
        private const string BuildPathKey = KeyPrefix + "BuildPath";

        private List<FieldInfo> fieldsInternal;
        private List<FieldInfo> fields
        {
            get
            {
                if (fieldsInternal == null)
                    CacheFieldInfo();
                return fieldsInternal;
            }
        }

        private BuildInformation buildInformationInternal;
        private BuildInformation buildInformation
        {
            get
            {
                if (buildInformationInternal == null)
                    buildInformationInternal = BuildInformation.Current;
                return buildInformationInternal;
            }
        }

        private bool editedBuildInformation;

        private BuildTarget buildTarget
        {
            get
            {
                return (BuildTarget)EditorPrefs.GetInt(BuildTargetKey);
            }
            set
            {
                EditorPrefs.SetInt(BuildTargetKey, (int)value);
            }
        }

        private BuildOptions buildOptions
        {
            get
            {
                return (BuildOptions)EditorPrefs.GetInt(BuildOptionsKey);
            }
            set
            {
                EditorPrefs.SetInt(BuildOptionsKey, (int)value);
            }
        }

        private string buildPath
        {
            get
            {
                return EditorPrefs.GetString(BuildPathKey);
            }
            set
            {
                EditorPrefs.SetString(BuildPathKey, value);
            }
        }

        [MenuItem("Window/Build Window")]
        public static void Init()
        {
            GetWindow<BuildWindow>(false, "Build Window");
        }

        private void SaveIfNecessary()
        {
            if (buildInformationInternal != null)
                buildInformation.Save();

            editedBuildInformation = false;
        }

        private void CacheFieldInfo()
        {
            fieldsInternal = new List<FieldInfo>();

            // Find all the fields of the build information class.
            FieldInfo[] fields = typeof(BuildInformation)
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            this.fields.Clear();

            for (int i = 0; i < fields.Length; i++)
            {
                // Only do string fields for now.
                if (fields[i].FieldType != typeof(string))
                    continue;

                this.fields.Add(fields[i]);
            }
        }

        private string GetTargetExtension(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneOSXUniversal:
                case BuildTarget.StandaloneOSXIntel:
                    return "app";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "exe";
                default:
                    return string.Empty;
            }
        }
        private string GetTargetExtension(BuildTarget buildTarget, bool includeDot)
        {
            string extension = GetTargetExtension(buildTarget);
            if (includeDot && !string.IsNullOrEmpty(extension))
                return "." + extension;
            return extension;
        }

        private string GetSuggestedBuildName()
        {
            return "{0} {1} - {2} - ({3})";
        }

        private string GetFormattedBuildName()
        {
            // Fill in the chosen path with some dynamic information.
            return string.Format(buildPath,
                buildInformation.ApplicationName, buildInformation.VersionName,
                buildInformation.BuildNumber, buildInformation.Addressee);
        }

        private void Build()
        {
            buildInformation.FillInAutomaticValues();

            SaveIfNecessary();

            // Figure out the build options.
            BuildPlayerOptions buildOptions = new BuildPlayerOptions();

            buildOptions.options = this.buildOptions;
            buildOptions.target = buildTarget;
            buildOptions.locationPathName = GetFormattedBuildName();

            buildOptions.scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
                buildOptions.scenes[i] = EditorBuildSettings.scenes[i].path;

            // Start a build and keep track of how long it took.
            UnityEngine.Debug.Log("Starting build for platform " + buildTarget + ".");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            BuildPipeline.BuildPlayer(buildOptions);

            // Report on how long it took.
            stopwatch.Stop();
            UnityEngine.Debug.Log("Build finished. " + stopwatch.Elapsed);
        }

        private bool HasAttribute<T>(FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttributes(typeof(T), true).Length > 0;
        }

        private void DisplayBuildInformationGui()
        {
            EditorGUILayout.BeginVertical("Box");

            string label, oldValue, newValue;
            for (int i = 0; i < fields.Count; i++)
            {
                // Show a space if necessary.
                if (HasAttribute<SpaceAttribute>(fields[i]))
                    EditorGUILayout.Space();

                oldValue = fields[i].GetValue(buildInformation) as string;

                // Show a field for editing the value.
                label = ObjectNames.NicifyVariableName(fields[i].Name);

                GUI.enabled = !HasAttribute<ReadOnlyAttribute>(fields[i]);
                newValue = EditorGUILayout.TextField(label, oldValue);
                GUI.enabled = true;

                // If the value changed, apply it to the current build information.
                if (newValue != oldValue)
                {
                    fields[i].SetValue(buildInformation, newValue);
                    editedBuildInformation = true;
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void BrowseToTargetPath()
        {
            string extension = GetTargetExtension(buildTarget);
            if (string.IsNullOrEmpty(extension))
            {
                buildPath = EditorUtility.SaveFolderPanel(
                    "Build directory", buildPath, GetSuggestedBuildName());
            }
            else
            {
                buildPath = EditorUtility.SaveFilePanel(
                    "Build file", buildPath, GetSuggestedBuildName(), extension);
            }
        }

        private void TranslatePathIfNecessary(BuildTarget oldTarget, BuildTarget newTarget)
        {
            if (oldTarget == buildTarget)
                return;

            string oldExtension = GetTargetExtension(oldTarget, true);
            string newExtension = GetTargetExtension(buildTarget, true);

            if (oldExtension != newExtension && buildPath.EndsWith(oldExtension))
            {
                // Remove the old extension.
                buildPath = buildPath.Substring(0, buildPath.Length - oldExtension.Length);

                // Add the new extension.
                buildPath += newExtension;
            }
        }

        private void OnGUI()
        {
            if (editedBuildInformation)
            {
                EditorGUILayout.HelpBox(
                    "The build information will be saved when you build.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Some information is updated automatically. To access the SHA of the current "
                    + "revision, make sure Git.exe is in your PATH environment variable.",
                    MessageType.Info);
            }

            EditorGUILayout.Space();

            DisplayBuildInformationGui();

            // Show dropdowns to adjust the build settings.
            EditorGUILayout.Space();

            BuildTarget oldBuildTarget = buildTarget;
            buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Target", buildTarget);

            // Change the build path if you change targets that have a different extension.
            TranslatePathIfNecessary(oldBuildTarget, buildTarget);

            buildOptions = (BuildOptions)EditorGUILayout.EnumMaskPopup("Options", buildOptions);

            // Also allow the user to specify a path with a nice browse button next to it.
            EditorGUILayout.BeginHorizontal();
            {
                buildPath = EditorGUILayout.TextField("Path", buildPath);
                if (GUILayout.Button("...", EditorStyles.miniButton, GUILayout.Width(22)))
                    BrowseToTargetPath();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Show buttons to build or optionally just save without building.
            EditorGUILayout.BeginHorizontal();
            {
                bool canBuild = !string.IsNullOrEmpty(buildPath)
                    && buildTarget != BuildTarget.NoTarget;
                GUI.enabled = canBuild;

                bool shouldBuild = GUILayout.Button("Build", GUILayout.Height(32));
                if (shouldBuild)
                    Build();

                GUI.enabled = canBuild && editedBuildInformation;
                bool shouldSave = GUILayout.Button("Save", GUILayout.Width(48), GUILayout.Height(32));
                if (shouldSave)
                    SaveIfNecessary();
                GUI.enabled = true;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}