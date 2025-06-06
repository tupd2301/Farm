using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.U2D;

namespace Factory
{
    public class FishTank : MonoBehaviour
    {
        public SpriteShapeController _spriteShapeController;

        [Header("Wave Settings")]
        [SerializeField]
        private float waveHeight = 0.1f;

        [SerializeField]
        private float waveSpeed = 1f;

        [SerializeField]
        private float waveFrequency = 2f;

        [SerializeField]
        private float secondaryWaveHeight = 0.05f;

        [SerializeField]
        private float secondaryWaveFrequency = 3f;

        private List<Vector3> _basePoints;
        private float _time;

        void Start()
        {
            // Store the original points as base positions
            _basePoints = new List<Vector3>();
            for (int i = 0; i < _spriteShapeController.spline.GetPointCount(); i++)
            {
                if (_spriteShapeController.spline.GetPosition(i).y == 0)
                {
                    _basePoints.Add(_spriteShapeController.spline.GetPosition(i));
                }
            }
        }

        void Update()
        {
            _time += Time.deltaTime * waveSpeed;

            for (int i = 0; i < _basePoints.Count; i++)
            {
                Vector3 point = _basePoints[i];

                // Calculate position in wave cycle (0 to 1)
                float wavePhase = (float)i / (_basePoints.Count - 1);

                // Primary wave
                float primaryWave =
                    Mathf.Sin((_time + wavePhase * waveFrequency) * Mathf.PI * 2) * waveHeight;

                // Secondary wave for more complexity
                float secondaryWave =
                    Mathf.Sin((_time * 1.1f + wavePhase * secondaryWaveFrequency) * Mathf.PI * 2)
                    * secondaryWaveHeight;

                // Combine waves
                Vector3 newPosition = point;
                newPosition.y += primaryWave + secondaryWave;

                _spriteShapeController.spline.SetPosition(i, newPosition);
            }
        }
    }
}
