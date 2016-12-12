using System.Diagnostics;
using UnityEngine;

namespace RoyTheunissen.PlaytestRecording.Utilities
{
    /// <summary>
    /// Utilities for interacting with the Git repository. Editor only.
    /// For any of this to work your git.exe needs to be added to your PATH environment variable.
    /// http://stackoverflow.com/questions/26620312/installing-git-in-path-with-github-client-for-windows
    /// </summary>
    public static class GitUtilities
    {
        public static string Sha
        {
            get
            {
                string sha;
                try
                {
                    sha = GetGitProcessOutput("rev-parse HEAD");
                }
                catch
                {
                    sha = "0000000000000000000000000000000000000000";
                }
                return sha;
            }
        }

        public static string GetCurrentRepositoryPath()
        {
            return Application.dataPath + "/../";
        }

        private static Process StartGitProcess(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("git.exe");

            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = GetCurrentRepositoryPath();
            startInfo.CreateNoWindow = true;

            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.Arguments = arguments;

            Process process = new Process();
            process.StartInfo = startInfo;
            return process;
        }

        private static string GetGitProcessOutput(string arguments)
        {
            Process process = StartGitProcess(arguments);
            process.Start();

            string error = process.StandardError.ReadLine();
            if (!string.IsNullOrEmpty(error))
                UnityEngine.Debug.LogError(error);

            string output = process.StandardOutput.ReadLine();

            process.WaitForExit();

            return output;
        }

        private static void ExecuteGitProcess(string arguments)
        {
            Process process = StartGitProcess(arguments);
            process.Start();

            string error = process.StandardError.ReadLine();
            if (!string.IsNullOrEmpty(error))
                UnityEngine.Debug.LogError(error);

            process.WaitForExit();
        }
    }
}
