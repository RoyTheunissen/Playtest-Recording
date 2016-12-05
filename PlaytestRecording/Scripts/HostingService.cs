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

        private string UploadUrl
        {
            get { return targetUrl + "Upload.php"; }
        }

        public void Upload(string filePath, Action completionCallback = null)
        {
            byte[] data = File.ReadAllBytes(filePath);
            string url = UploadUrl;
            WWWForm form = new WWWForm();
            form.AddBinaryData("fileToUpload", data, Path.GetFileNameWithoutExtension(filePath));
            form.AddField("submit", "Upload File");
            WWW uploadRequest = new WWW(url, form);

            Debug.Log("Going to wait for playtest data upload completion now...");

            StartCoroutine(WwwResultRoutine(uploadRequest, completionCallback));
        }

        private IEnumerator WwwResultRoutine(WWW www, Action completionCallback)
        {
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
                Debug.LogError("Playtest data upload error: " + www.error);
            else
                Debug.Log("Playtest data upload completed: " + www.text);

            if (completionCallback != null)
                completionCallback();
        }
    }
}