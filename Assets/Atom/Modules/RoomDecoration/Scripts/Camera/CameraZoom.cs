using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomDecoration.CameraUtilities
{
    public class CameraZoom : MonoBehaviour
    {
        [Header("Zoom Settings")]
        [SerializeField] private float zoomDuration = 0.5f;
        [SerializeField, Range(1.1f, 2f)] private float zoomMultiplier = 1.2f;
        [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private float minZoomDistance = 5f;
        [SerializeField] private float maxZoomDistance = 20f;

        [Header("Rotation Settings")]
        [SerializeField] private bool enableRotation = true;
        [SerializeField] private float maxRotationAngle = 30f; // Maximum rotation angle to either side
        [SerializeField] private float rotationThreshold = 0.1f; // How far from center before rotation starts

        private Camera mainCamera;
        private bool isOrthographic;
        private float originalSize;
        private float originalFOV;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Coroutine zoomCoroutine;

        private void Awake()
        {
            mainCamera = GetComponent<Camera>();
            isOrthographic = mainCamera.orthographic;

            originalRotation = transform.rotation;
            originalPosition = transform.position;
            if (isOrthographic)
            {
                originalSize = mainCamera.orthographicSize;
            }
            else
            {
                originalFOV = mainCamera.fieldOfView;
            }
        }

        private Vector2 CalculateRotationAngles(Vector3 targetPosition)
        {
            // Get the direction from the camera to the target
            Vector3 directionToTarget = (targetPosition - mainCamera.transform.position).normalized;

            // Calculate horizontal offset (left/right) using the camera's right vector
            float horizontalOffset = Vector3.Dot(directionToTarget, mainCamera.transform.right);

            // Calculate vertical offset (up/down) using the camera's up vector
            float verticalOffset = Vector3.Dot(directionToTarget, mainCamera.transform.up);

            // Only rotate if offsets exceed the thresholds
            float horizontalAngle = Mathf.Abs(horizontalOffset) > rotationThreshold
                ? Mathf.Clamp(horizontalOffset * maxRotationAngle, -maxRotationAngle, maxRotationAngle)
                : 0f;

            float verticalAngle = Mathf.Abs(verticalOffset) > rotationThreshold
                ? Mathf.Clamp(verticalOffset * maxRotationAngle, -maxRotationAngle, maxRotationAngle)
                : 0f;

            // Return both angles as a Vector2
            return new Vector2(horizontalAngle, verticalAngle);
        }

        public void ZoomTo(Vector3 furniturePosition)
        {
            if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
            }

            zoomCoroutine = StartCoroutine(ZoomSequence(furniturePosition));
        }

        private float CalculateZoomLevel(float currentZoom, float distanceToTarget)
        {
            float distanceRatio = Mathf.Clamp01((distanceToTarget - minZoomDistance) / (maxZoomDistance - minZoomDistance));
            float zoomFactor = Mathf.Lerp(1.1f, zoomMultiplier, distanceRatio);
            return currentZoom / zoomFactor;
        }

        private IEnumerator ZoomSequence(Vector3 targetPosition)
        {
            float elapsedTime = 0f;
            Vector3 startPosition = transform.position;
            Quaternion startRotation = transform.rotation;

            float distanceToTarget = Vector3.Distance(startPosition, targetPosition);

            float startZoom = isOrthographic ? mainCamera.orthographicSize : mainCamera.fieldOfView;
            float targetZoom = CalculateZoomLevel(startZoom, distanceToTarget);

            // Calculate dynamic rotation based on target position
            Vector2 rotationAngles = enableRotation ? CalculateRotationAngles(targetPosition) : Vector2.zero;
            Quaternion targetRotation = Quaternion.Euler(
                originalRotation.eulerAngles.x - rotationAngles.y,
                originalRotation.eulerAngles.y + rotationAngles.x,
                originalRotation.eulerAngles.z
            );

            Vector3 directionToTarget = (targetPosition - startPosition).normalized;
            float zoomDistance = Mathf.Clamp(distanceToTarget / 2f, minZoomDistance, maxZoomDistance);
            Vector3 zoomTarget = targetPosition - (directionToTarget * zoomDistance);
            zoomTarget.y = transform.position.y;

            // Zoom in
            while (elapsedTime < zoomDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / zoomDuration;
                float curveValue = zoomCurve.Evaluate(t);

                transform.position = Vector3.Lerp(startPosition, zoomTarget, curveValue);
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, curveValue);

                if (isOrthographic)
                {
                    mainCamera.orthographicSize = Mathf.Lerp(startZoom, targetZoom, curveValue);
                }
                else
                {
                    mainCamera.fieldOfView = Mathf.Lerp(startZoom, targetZoom, curveValue);
                }

                yield return null;
            }

            CameraZoomInStopEvent.Trigger();

            yield return new WaitForSeconds(0.5f);

            // Store zoomed values for zooming back out
            startPosition = transform.position;
            startRotation = transform.rotation;
            startZoom = isOrthographic ? mainCamera.orthographicSize : mainCamera.fieldOfView;
            elapsedTime = 0f;

            // Zoom back out
            while (elapsedTime < zoomDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / zoomDuration;
                float curveValue = zoomCurve.Evaluate(t);

                transform.position = Vector3.Lerp(startPosition, originalPosition, curveValue);
                transform.rotation = Quaternion.Lerp(startRotation, originalRotation, curveValue);

                if (isOrthographic)
                {
                    mainCamera.orthographicSize = Mathf.Lerp(startZoom, originalSize, curveValue);
                }
                else
                {
                    mainCamera.fieldOfView = Mathf.Lerp(startZoom, originalFOV, curveValue);
                }

                yield return null;
            }

            zoomCoroutine = null;

            CameraZoomOutStopEvent.Trigger();
        }

        public void ResetCamera()
        {
            if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
                zoomCoroutine = null;
            }

            transform.rotation = originalRotation;
            if (isOrthographic)
            {
                mainCamera.orthographicSize = originalSize;
            }
            else
            {
                mainCamera.fieldOfView = originalFOV;
            }
        }
        private void OnEnable()
        {
            CameraZoomEvent.Register(ZoomTo);
        }

        private void OnDisable()
        {
            CameraZoomEvent.Unregister(ZoomTo);
        }
    }

}
