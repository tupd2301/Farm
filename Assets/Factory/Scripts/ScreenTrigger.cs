using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Athena.Common.UI;
using DG.Tweening;
using Factory;
using UnityEngine;

public class ScreenTrigger : MonoBehaviour
{
    public List<GameObject> _particles;

    public Touch _touch;

    public GameObject GetObject()
    {
        return _particles.FirstOrDefault(x => x.activeSelf == false);
    }

    void Update()
    {
        if (Input.touchCount > 0 && _touch.phase == TouchPhase.Began)
        {
            _touch = Input.GetTouch(0);
            SpawnParticle(_touch.position);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SpawnParticle(Input.mousePosition);
        }
    }

    public void SpawnParticle(Vector3 position)
    {
        Debug.Log("SpawnParticle");
        var particle = GetObject();
        if (particle != null && position.y < Screen.height / 2f)
        {
            // Convert screen position to world position in the UI canvas
            Vector2 localPoint;
            RectTransform canvasRect = particle.transform.parent.GetComponent<RectTransform>();

            // Use RectTransformUtility for proper UI coordinate conversion
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                position,
                UIManager.Instance.CameraUI, // Use the proper UI camera
                out localPoint
            );
            AudioManager.Instance.PlaySound("Tap");
            particle.GetComponent<RectTransform>().localPosition = localPoint;
            particle.SetActive(true);
            DOVirtual.DelayedCall(
                1f,
                () =>
                {
                    particle.SetActive(false);
                }
            );
        }
    }
}
