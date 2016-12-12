using RoyTheunissen.PlaytestRecording.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace RoyTheunissen.PlaytestRecording.UI
{
    /// <summary>
    /// Manages watermarks to help you identify gameplay footage.
    /// </summary>
    public sealed class Watermark : MonoBehaviour 
    {
        private const int ShaLength = 12;

        [SerializeField]
        private string addressee;

        [Space]
        [SerializeField]
        private Text text;

        [SerializeField]
        private PlaytestService playtestService;

        private void Awake()
        {
            string format = text.text;

            string applicationName = playtestService.ApplicationName;
            string versionName = playtestService.ApplicationVersion;
            string sha = GitUtilities.Sha.Substring(0, ShaLength);

            text.text = string.Format(format, addressee, applicationName, versionName, sha);
        }
    }
}