using Ionic.Zip;
using System;
using UnityEngine;

namespace RoyTheunissen.PlaytestRecording
{
    /// <summary>
    /// Manages data recording of playtests. Also works in-editor but this is disabled by default
    /// to prevent the server from being cluttered with editor footage. Define 
    /// GATHER_PLAYTEST_DATA_IN_EDITOR if you want to temporarily re-enable this feature.
    /// </summary>
    public sealed class PlaytestService : MonoBehaviour 
    {
        private const string ZipNameFormat = "PlayTest_{0}_{1}_{2}_{3}";
        private const string FootageName = "Footage";

        [SerializeField]
        private string applicationName = "TestApplication";

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

            string filePath = currentSessionData.DataPath + FootageName;
            recordingService.StartRecording(filePath);
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