using UnityEngine;

namespace RoyTheunissen.PlaytestRecording.Example
{
    /// <summary>
    /// Rotates the transform a number of times per second, per axis.
    /// </summary>
    public sealed class Rotator : MonoBehaviour 
    {
        [SerializeField]
        private Vector3 rotationsPerSecond = Vector3.up;

        private void Update()
        {
            transform.Rotate(rotationsPerSecond * 360 * Time.deltaTime);
        }
    }
}