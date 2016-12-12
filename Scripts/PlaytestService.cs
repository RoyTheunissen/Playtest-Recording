using Ionic.Zip;
using System;
using UnityEngine;

namespace RoyTheunissen.PlaytestRecording
{
    /// <summary>
    /// Manages data recording of playtests. Also works in-editor but this is disabled by default
    /// to prevent the server from being cluttered with editor footage. Define 
    /// GATHER_PLAYTEST_DATA_IN_EDITOR if you want to temporarily re-enable this feature. Note 
    /// that you can have the game start recording automatically but you can disable this so you 
    /// can prompt the user first. The same goes for the face cam.
    /// </summary>
    public sealed class PlaytestService : MonoBehaviour 
    {
        private const string ZipNameFormat = "PlayTest_{0}_{1}_{2}_{3}";
        private const string FootageName = "Footage";

        [SerializeField]
        private string applicationName = "TestApplication";
        public string ApplicationName { get { return applicationName; } }

        [SerializeField]
        private string applicationVersion = "alpha";
        public string ApplicationVersion { get { return applicationVersion; } }

        [Tooltip("Set this to false if you want to ask users for permission first.")]
        [SerializeField]
        private bool startRecordingImmediately = false;

        [Tooltip("I advise to disable this by default. Ask players first!")]
        [SerializeField]
        private bool includeFaceCamByDefault = false;

        [Space]
        [SerializeField]
        private RecordingService recordingService;

        [SerializeField]
        private HostingService hostingService;

        [NonSerialized]
        private PlaytestSessionData currentSessionData;

        public delegate void DataUploadStartedHandler(PlaytestService service);
        public event DataUploadStartedHandler DataUploadStartedEvent;
        private void DispatchDataUploadStartedEvent()
        {
            if (DataUploadStartedEvent != null)
                DataUploadStartedEvent(this);
        }

        public delegate void DataUploadProgressedHandler(
            PlaytestService service, float fraction);
        public event DataUploadProgressedHandler DataUploadProgressedEvent;
        private void DispatchDataUploadProgressedEvent(float fraction)
        {
            if (DataUploadProgressedEvent != null)
                DataUploadProgressedEvent(this, fraction);
        }

        public delegate void DataUploadCompletedHandler(PlaytestService service);
        public event DataUploadCompletedHandler DataUploadCompletedEvent;
        private void DispatchDataUploadCompletedEvent()
        {
            if (DataUploadCompletedEvent != null)
                DataUploadCompletedEvent(this);
        }

#if !UNITY_EDITOR || GATHER_PLAYTEST_DATA_IN_EDITOR
        private bool hasStartedUploading;
        private bool hasFinishedUploading;

        private void Awake()
        {
            StartNewPlayTestSession();
        }

        private void OnApplicationQuit()
        {
            // Intercept and cancel every application quit until we're finished uploading.
            if (!hasFinishedUploading)
                Application.CancelQuit();

            // If we've already started uploading, go no further.
            if (hasStartedUploading)
                return;

            // Otherwise start uploading the playtest session.
            hasStartedUploading = true;
            StopCurrentPlayTestSession(HandleDataUploaded, HandleDataUploadProgress);
        }

        private void HandleDataUploadProgress(float fraction)
        {
            DispatchDataUploadProgressedEvent(fraction);
        }

        private void HandleDataUploaded()
        {
            DispatchDataUploadCompletedEvent();

            hasFinishedUploading = true;
            Application.Quit();
        }
#endif

        private void StartNewPlayTestSession()
        {
            currentSessionData = new PlaytestSessionData();
            currentSessionData.StartSession();

            if (startRecordingImmediately)
                StartRecording(includeFaceCamByDefault);
        }

        public void StartRecording(bool recordFaceCam)
        {
            if (currentSessionData == null)
                return;

            string filePath = currentSessionData.DataPath + FootageName;
            recordingService.StartRecording(filePath, recordFaceCam);
        }

        private void StopCurrentPlayTestSession(Action completionCallback = null,
            HostingService.UploadProgressHandler progressCallback = null)
        {
            // Stop gathering data.
            string recordingFileName = recordingService.StopRecording();
            string sessionDataFileName = currentSessionData.StopSession();

            // Save the compressed archive to a file.
            string zipName = string.Format(ZipNameFormat, applicationName,
                currentSessionData.Device.name,
                currentSessionData.User.name, currentSessionData.Id);
            string zipPath = ZipUtility.Zip(currentSessionData.DataPath + zipName,
                recordingFileName, sessionDataFileName);

            DispatchDataUploadStartedEvent();

            // Upload the file to a webserver.
            hostingService.Upload(zipPath, completionCallback, progressCallback);
        }
    }
}