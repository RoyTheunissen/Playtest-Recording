using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

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
            UnityWebRequest uploadRequest = UnityWebRequest.Post(url, form);
            uploadRequest.SendWebRequest();

            Debug.Log("Going to wait for playtest data upload completion now...");

            StartCoroutine(UploadResultRoutine(uploadRequest, completionCallback, progressCallback));
        }

        private IEnumerator UploadResultRoutine(
            UnityWebRequest request, Action completionCallback, UploadProgressHandler progressCallback)
        {
            while (!request.isDone)
            {
                yield return null;

                if (progressCallback != null)
                    progressCallback(request.uploadProgress);
            }

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError("Playtest data upload error: " + request.error);
            else
                Debug.Log("Playtest data upload completed: " + request.downloadHandler.text);

            request.Dispose();

            if (completionCallback != null)
                completionCallback();
        }
    }
}