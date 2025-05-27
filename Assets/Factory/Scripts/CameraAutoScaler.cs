using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Factory
{
    public class CameraAutoScaler : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private Vector2 _targetResolution = new Vector2(1080, 1920);

        [SerializeField]
        private float _targetOrthographicSize = 10;

        [SerializeField]
        private float _maxOrthographicSize = 10;

        [SerializeField]
        private float _minOrthographicSize = 10;

        private void Start()
        {
            _camera = GetComponent<Camera>();
            AdjustCameraSize();
        }

        private void AdjustCameraSize()
        {
            float currentWidth = _camera.pixelWidth;
            float ratioDesired = _targetResolution.x / _targetResolution.y;
            float ratioCurrent = _camera.aspect;
            float scaleX = ratioDesired / ratioCurrent;
            _camera.orthographicSize = Mathf.Min(
                Mathf.Max(_targetOrthographicSize * scaleX, _minOrthographicSize),
                _maxOrthographicSize
            );
        }
    }
}
