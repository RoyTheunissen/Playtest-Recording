using UnityEditor;

namespace RoyTheunissen.PlaytestRecording.Building.Editor
{
    /// <summary>
    /// Utility to help serialize build options by exposing booleans.
    /// </summary>
    public static class SerializableBuildOptions 
    {
        private const string BuildOptionsKey = "BuildOptionFlags";

        public static BuildOptions Flags
        {
            get { return (BuildOptions)EditorPrefs.GetInt(BuildWindow.KeyPrefix + BuildOptionsKey); }
            set { EditorPrefs.SetInt(BuildWindow.KeyPrefix + BuildOptionsKey, (int)value); }
        }

        public static bool DevelopmentBuild
        {
            get { return GetFlag(BuildOptions.Development); }
            set { SetFlag(BuildOptions.Development, value); }
        }

        public static bool AllowScriptDebugging
        {
            get { return GetFlag(BuildOptions.AllowDebugging); }
            set { SetFlag(BuildOptions.AllowDebugging, value); }
        }

        private static bool GetFlag(BuildOptions flag)
        {
            return (Flags & flag) == flag;
        }

        private static void SetFlag(BuildOptions flag, bool value)
        {
            if (value)
                Flags |= flag;
            else
                Flags &= ~flag;
        }
    }
}
