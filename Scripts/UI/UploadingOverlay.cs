using UnityEngine;
using UnityEngine.UI;

namespace RoyTheunissen.PlaytestRecording.UI
{
    /// <summary>
    /// Displays an overlay that communicates to the player how the data upload has progressed.
    /// </summary>
    public sealed class UploadingOverlay : MonoBehaviour 
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private Slider progressBar;

        [SerializeField]
        private PlaytestService hostingService;

        private void Awake()
        {
            canvasGroup.alpha = 0.0f;
            canvasGroup.blocksRaycasts = false;

            hostingService.DataUploadStartedEvent += HandleUploadStartedEvent;
            hostingService.DataUploadProgressedEvent += HandleDataUploadProgressedEvent;
        }

        private void OnDestroy()
        {
            hostingService.DataUploadStartedEvent -= HandleUploadStartedEvent;
            hostingService.DataUploadProgressedEvent -= HandleDataUploadProgressedEvent;
        }

        private void HandleUploadStartedEvent(PlaytestService service)
        {
            canvasGroup.alpha = 1.0f;
            canvasGroup.blocksRaycasts = true;
        }

        private void HandleDataUploadProgressedEvent(PlaytestService service, float fraction)
        {
            progressBar.normalizedValue = fraction;
        }
    }
}