using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Factory;
using UnityEngine;
using UnityEngine.U2D;

public class SquidEffect : MonoBehaviour
{
    public float range = 10f;
    public float speedScale = 1f;

    public float duration = 1f;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fish"))
        {
            var fishController = other.GetComponent<FishController>();
            System.Random random = new System.Random();
            if (fishController != null)
            {
                fishController.targetPosition =
                    transform.position
                    + new Vector3(
                        random.Next(-(int)(range * 0.5f), (int)(range * 0.5f)),
                        random.Next(-(int)(range * 0.5f), (int)(range * 0.5f)),
                        0
                    );
                fishController.KillAllTween();
                fishController.Move();
            }
        }
    }
}
