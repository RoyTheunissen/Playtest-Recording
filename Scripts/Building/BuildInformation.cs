using System;
using UnityEngine;
using RoyTheunissen.PlaytestRecording.Utilities;

#if UNITY_EDITOR
using System.IO;
#endif // UNITY_EDITOR

namespace RoyTheunissen.PlaytestRecording.Building
{
    /// <summary>
    /// An object that wraps all the build information with helpers to get and set the current 
    /// build information. If you have an automated build process, this is a useful class to use 
    /// in order to update the build information text file automatically when creating a build.
    /// </summary>
    [Serializable]
    public class BuildInformation
    {
        private const string Extension = ".json";
        private const string FileName = "BuildInformation";

        private static readonly string AbsolutePath
            = Application.dataPath + "/PlaytestRecording/Resources/" + FileName + Extension;

        private static readonly string ResourcesPath = FileName;

        [SerializeField]
        private string applicationName;
        public string ApplicationName
        {
            get { return applicationName; }
        }

        [SerializeField]
        private string versionName;
        public string VersionName
        {
            get { return versionName; }
        }

        [SerializeField]
        private string addressee;
        public string Addressee
        {
            get { return addressee; }
        }

        [Space]
        [ReadOnly]
        [SerializeField]
        private string sha;
        public string Sha
        {
            get { return sha; }
        }

        [ReadOnly]
        [SerializeField]
        private string buildTime;
        public DateTime BuildTime
        {
            get { return DateTime.Parse(buildTime); }
            set { buildTime = value.ToString(); }
        }

        [ReadOnly]
        [SerializeField]
        private string buildNumber;
        public int BuildNumber
        {
            get { return int.Parse(buildNumber); }
            set { buildNumber = value.ToString(); }
        }

        [ReadOnly]
        [SerializeField]
        private string uniqueBuildIdentifier;
        public string UniqueBuildIdentifier
        {
            get { return uniqueBuildIdentifier; }
        }

        public void FillInAutomaticValues()
        {
            sha = GitUtilities.Sha;

            BuildTime = DateTime.Now;
            BuildNumber++;
            uniqueBuildIdentifier = Guid.NewGuid().ToString();
        }

        public void Save()
        {
            Current = this;
        }

        public static BuildInformation Current
        {
            get { return GetBuildInformation(); }
            set { SetBuildInformation(value); }
        }

        private static BuildInformation GetBuildInformation()
        {
            TextAsset textAsset = Resources.Load<TextAsset>(ResourcesPath);

            if (textAsset == null)
                return null;

            return JsonUtility.FromJson<BuildInformation>(textAsset.text);
        }

        private static void SetBuildInformation(BuildInformation buildInformation)
        {
#if UNITY_EDITOR
            string json = JsonUtility.ToJson(buildInformation, true);
            File.WriteAllText(AbsolutePath, json);
            UnityEditor.AssetDatabase.Refresh();
#endif // UNITY_EDITOR
        }
    }
}
