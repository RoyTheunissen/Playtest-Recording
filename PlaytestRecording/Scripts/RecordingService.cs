using System.Diagnostics;
using UnityEngine;

namespace RoyTheunissen.PlaytestRecording
{
    /// <summary>
    /// Manages the recording of gameplay footage via an external application. I made a customized 
    /// version of Open Broadcasting Software Studio which works out-of-the-box and records as soon 
    /// as the application is launched. It also has extended launch parameters so you can specify 
    /// the exact path and filename of the file to which should be recorded. A build of this 
    /// application is intended to be shipped alongside the game so the game can speak to it.
    /// </summary>
    public sealed class RecordingService : MonoBehaviour 
    {
        private const string RecordingApplicationName = "obs64";
        private const string RecordingApplicationExtension = ".exe";

        private const string ExtensionPrefix = ".";
        private const string VideoFormat = "flv";

        private const string ArgumentsFormat = " --startrecording \"{0}\" --runinbackground";

        private string RecordingApplicationDirectoryPath
        {
            get
            {
#if UNITY_EDITOR
                return "C:/Git/OBSS/build/rundir/Release/bin/64bit/";
#else
                return Application.dataPath + "/obs/bin/64bit/";
#endif
            }
        }

        private string RecordingApplicationPath
        {
            get
            {
                return RecordingApplicationDirectoryPath
                    + RecordingApplicationName + RecordingApplicationExtension;
            }
        }

        public bool IsRecording
        {
            get
            {
                return recordingProcess != null && !recordingProcess.HasExited;
            }
        }

        private Process recordingProcess;
        private string path;

        /// <summary>
        /// Starts recording to the specified file.
        /// </summary>
        /// <param name="path">The file name. May include a directory but no extension.</param>
        public void StartRecording(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(RecordingApplicationPath);
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = RecordingApplicationDirectoryPath;
            startInfo.Arguments = string.Format(ArgumentsFormat, path);

            try
            {
                recordingProcess = Process.Start(startInfo);
            }
            catch
            {
                recordingProcess = null;
            }

            this.path = path;
        }

        /// <summary>
        /// Stops the recording.
        /// </summary>
        /// <returns>The full path to the file to which the recording was output.</returns>
        public string StopRecording()
        {
            if (IsRecording)
                recordingProcess.Kill();
            recordingProcess = null;

            return path + ExtensionPrefix + VideoFormat;
        }
    }
}