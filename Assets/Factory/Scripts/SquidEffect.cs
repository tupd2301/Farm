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

    public void Update()
    {
        //raycast to find fish
        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            transform.position,
            new Vector2(range, range),
            0,
            Vector2.zero
        );
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Fish"))
            {
                var fishController = hit.collider.GetComponent<FishController>();
                Debug.Log("SquidEffect");
                if (fishController.state != FishState.Moving)
                {
                    return;
                }
                System.Random random = new System.Random();
                if (fishController != null)
                {
                    fishController.state = FishState.Confused;
                    fishController.KillAllTween();
                }
            }
        }
    }
}
