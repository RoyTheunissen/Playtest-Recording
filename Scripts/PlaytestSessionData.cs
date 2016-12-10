using System;
using System.IO;
using UnityEngine;

namespace RoyTheunissen.PlaytestRecording
{
    /// <summary>
    /// Maintains data about a playtest session.
    /// </summary>
    [Serializable]
    public class PlaytestSessionData
    {
        private const int CurrentSerializationFormat = 1;
        private const string SessionIdKey = "PlaytestSessionData/SessionId";
        private const string FileName = "data";
        private const string Extension = ".txt";

        [SerializeField]
        private int serializationFormat = CurrentSerializationFormat;
        public int SerializationFormat { get { return serializationFormat; } }

        [SerializeField]
        private int id;
        public int Id { get { return id; } }

        public string DataPath
        {
            get
            {
#if UNITY_EDITOR
                string basePath = "C:/Videos";
#else
                string basePath = Application.dataPath;
#endif
                return basePath + "/playtests/" + id + "/";
            }
        }

        [Serializable]
        private struct TimeData
        {
            [SerializeField]
            private string startTime;
            public DateTime StartTime
            {
                get { return DateTime.Parse(startTime); }
                set { startTime = value.ToString(); }
            }

            [SerializeField]
            private string endTime;
            public DateTime EndTime
            {
                get { return DateTime.Parse(endTime); }
                set { endTime = value.ToString(); }
            }

            [SerializeField]
            private string duration;
            public TimeSpan Duration
            {
                get { return TimeSpan.Parse(duration); }
                set { duration = value.ToString(); }
            }
        }

        [Serializable]
        public struct DeviceData
        {
            public string name;
            public string model;
            public string type;
            public string operatingSystem;
            public int systemMemorySize;

            public int gpuId;
            public string gpuName;
            public string gpuType;
            public string gpuVendor;
            public string gpuVersion;
            public int gpuMemorySize;
            public bool gpuMultiThreaded;
            public int gpuShaderLevel;

            public int processorCount;
            public int processorFrequency;
            public string processorType;

            public bool supportsImageEffects;
            public bool supportsInstancing;
            public bool supportsMotionVectors;
            public bool supportsRawShadowDepthSampling;
            public bool supportsShadows;

            public static DeviceData GetCurrentDeviceData()
            {
                DeviceData result = new DeviceData();
                result.name = SystemInfo.deviceName;
                result.model = SystemInfo.deviceModel;
                result.operatingSystem = SystemInfo.operatingSystem;
                result.systemMemorySize = SystemInfo.systemMemorySize;

                result.gpuId = SystemInfo.graphicsDeviceID;
                result.gpuName = SystemInfo.graphicsDeviceName;
                result.gpuType = SystemInfo.graphicsDeviceType.ToString();
                result.gpuVendor = SystemInfo.graphicsDeviceVendor;
                result.gpuVersion = SystemInfo.graphicsDeviceVersion;
                result.gpuMemorySize = SystemInfo.graphicsMemorySize;
                result.gpuMultiThreaded = SystemInfo.graphicsMultiThreaded;
                result.gpuShaderLevel = SystemInfo.graphicsShaderLevel;

                result.processorCount = SystemInfo.processorCount;
                result.processorFrequency = SystemInfo.processorFrequency;
                result.processorType = SystemInfo.processorType;

                result.supportsImageEffects = SystemInfo.supportsImageEffects;
                result.supportsInstancing = SystemInfo.supportsInstancing;
                result.supportsMotionVectors = SystemInfo.supportsMotionVectors;
                result.supportsRawShadowDepthSampling = SystemInfo.supportsRawShadowDepthSampling;
                result.supportsShadows = SystemInfo.supportsShadows;

                return result;
            }
        }

        [Serializable]
        public struct UserData
        {
            public string name;

            public static UserData GetCurrentUserData()
            {
                UserData userData = new UserData();
                userData.name = Environment.UserName;
                return userData;
            }
        }

        [SerializeField]
        private TimeData time;

        [SerializeField]
        private DeviceData device;
        public DeviceData Device { get { return device; } }

        [SerializeField]
        private UserData user;
        public UserData User { get { return user; } }

        private int SessionId
        {
            get { return PlayerPrefs.GetInt(SessionIdKey); }
            set { PlayerPrefs.SetInt(SessionIdKey, value); }
        }

        public PlaytestSessionData()
        {
            id = ++SessionId;
        }

        public void StartSession()
        {
            time.StartTime = DateTime.Now;
            device = DeviceData.GetCurrentDeviceData();
            user = UserData.GetCurrentUserData();

            Directory.CreateDirectory(DataPath);
        }

        public string StopSession()
        {
            time.EndTime = DateTime.Now;

            time.Duration = time.EndTime - time.StartTime;

            return WriteToFile();
        }

        private string WriteToFile()
        {
            string json = JsonUtility.ToJson(this, true);
            string fileName = DataPath + FileName + Extension;
            File.WriteAllText(fileName, json);
            return fileName;
        }
    }
}
