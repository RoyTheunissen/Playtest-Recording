using RoyTheunissen.PlaytestRecording.Building;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RoyTheunissen.PlaytestRecording.UI
{
    /// <summary>
    /// Displays a watermark based on the current build information to help you identify footage.
    /// </summary>
    public sealed class Watermark : MonoBehaviour 
    {
        private const int ShaLength = 12;

        [Space]
        [SerializeField]
        private Text text;

        public delegate void WatermarkHandler(int index, int count, bool bit);

        private void Awake()
        {
            string format = text.text;

            BuildInformation info = BuildInformation.Current;

            text.text = string.Format(format,
                info.Addressee, info.ApplicationName, info.VersionName,
                info.Sha.Substring(0, ShaLength));
        }

        public static void Create(int identifier, WatermarkHandler handler)
        {
            BitArray bits = new BitArray(new int[] { identifier });

            int count = bits.Length;
            for (int i = 0; i < count; i++)
                handler(i, count, bits[i]);
        }

        public static void Create(WatermarkHandler handler)
        {
            // Create a watermark for the current build number, which is a nice small number.
            Create(BuildInformation.Current.BuildNumber, handler);
        }
    }
}