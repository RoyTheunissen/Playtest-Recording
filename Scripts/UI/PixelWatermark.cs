using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RoyTheunissen.PlaytestRecording.UI
{
    /// <summary>
    /// Generates a surreptitious pixel watermark to identify screenshots/footage.
    /// </summary>
    public sealed class PixelWatermark : MonoBehaviour 
    {
        [SerializeField]
        private RectTransform pixelPrefab;

        [SerializeField]
        private Color onePixelColor = Color.white;

        [SerializeField]
        private Color zeroPixelColor = Color.black;

        private List<RectTransform> pixelInstances = new List<RectTransform>();

        private float spacing
        {
            get
            {
                return pixelPrefab.sizeDelta.x;
            }
        }

        private void Awake()
        {
            Watermark.Create(CreatePixel);
        }

        private void CreatePixel(int index, int count, bool bit)
        {
            RectTransform pixelInstance = Instantiate(pixelPrefab, transform);
            pixelInstance.anchoredPosition = new Vector3(index * spacing, 0);

            Image image = pixelInstance.GetComponent<Image>();
            image.color *= bit ? onePixelColor : zeroPixelColor;

            pixelInstances.Add(pixelInstance);
        }
    }
}