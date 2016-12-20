using RoyTheunissen.PlaytestRecording.Building;
using UnityEngine;
using UnityEngine.UI;

namespace RoyTheunissen.PlaytestRecording.UI
{
    /// <summary>
    /// Displays a watermark based on the current build information to help you identify footage.
    /// </summary>
    public sealed class Watermark : MonoBehaviour 
    {
        [Space]
        [SerializeField]
        private Text text;

        private void Awake()
        {
            string format = text.text;

            BuildInformation info = BuildInformation.Current;

            text.text = string.Format(format,
                info.Addressee, info.ApplicationName, info.VersionName, info.Sha);
        }
    }
}