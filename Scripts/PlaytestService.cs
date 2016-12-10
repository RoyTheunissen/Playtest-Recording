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
            StopCurrentPlayTestSession(HandlePlaytestDataUploaded);
        }

        private void HandlePlaytestDataUploaded()
        {
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
            string filePath = currentSessionData.DataPath + FootageName;
            recordingService.StartRecording(filePath, recordFaceCam);
        }

        private void StopCurrentPlayTestSession(Action completionCallback = null)
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

            // Upload the file to a webserver.
            hostingService.Upload(zipPath, completionCallback);
        }
    }
}