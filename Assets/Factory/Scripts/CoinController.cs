using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Factory
{
    public class CoinController : MonoBehaviour
    {
        public int value;

        public bool isCollected = false;

        public void Awake()
        {
            var canvas = GetComponentInChildren<Canvas>();
            canvas.worldCamera = GameManager.Instance.mainCamera;
        }

        public void OnEnable()
        {
            isCollected = false;
            transform.DOComplete();
            transform.DOLocalMoveY(-5.7f, 1f).SetEase(Ease.InSine);
            if (isCollected || gameObject.activeSelf == false)
                return;
            CancelInvoke("OnClick");
            Invoke("OnClick", 5f);
        }

        public void OnClick()
        {
            if (isCollected || gameObject.activeSelf == false)
                return;
            CancelInvoke("OnClick");
            isCollected = true;
            transform.DOComplete();
            transform
                .DOMove(GameManager.Instance.homeUI.TotalGoldText.transform.position, 0.5f)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    FishManager.Instance.SpawnTextFloating(value.ToString(), transform.position);
                    GameManager.Instance.AddGold(value);
                    PoolSystem.Instance.ReturnObject(gameObject, "Coin");
                });
        }
    }
}
