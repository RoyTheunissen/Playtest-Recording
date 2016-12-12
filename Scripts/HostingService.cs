using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace RoyTheunissen.PlaytestRecording
{
    /// <summary>
    /// Uploads files to a webserver.
    /// </summary>
    public sealed class HostingService : MonoBehaviour 
    {
        [SerializeField]
        private string targetUrl = "http://example.com/playtests/";

        public delegate void UploadProgressHandler(float fraction);

        private string UploadUrl
        {
            get { return targetUrl + "Upload.php"; }
        }

        public void Upload(string filePath,
            Action completionCallback = null, UploadProgressHandler progressCallback = null)
        {
            byte[] data = File.ReadAllBytes(filePath);
            string url = UploadUrl;
            WWWForm form = new WWWForm();
            form.AddBinaryData("fileToUpload", data, Path.GetFileNameWithoutExtension(filePath));
            form.AddField("submit", "Upload File");
            WWW uploadRequest = new WWW(url, form);

            Debug.Log("Going to wait for playtest data upload completion now...");

            StartCoroutine(WwwResultRoutine(uploadRequest, completionCallback, progressCallback));
        }

        private IEnumerator WwwResultRoutine(
            WWW www, Action completionCallback, UploadProgressHandler progressCallback)
        {
            while (!www.isDone)
            {
                yield return null;

                if (progressCallback != null)
                    progressCallback(www.uploadProgress);
            }

            if (!string.IsNullOrEmpty(www.error))
                Debug.LogError("Playtest data upload error: " + www.error);
            else
                Debug.Log("Playtest data upload completed: " + www.text);

            if (completionCallback != null)
                completionCallback();
        }
    }
}