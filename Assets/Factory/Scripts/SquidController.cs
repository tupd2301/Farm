using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Factory;
using UnityEngine;

public class SquidController : FishController
{
    public Animator _animator;

    public bool isAttacking = false;

    public float attackTime = 15f;

    public void DecreaseHPByClick()
    {
        // base.DecreaseHPByTime();
        currentTotalTickValue -= fishConfig.fishCurrencyValue * fishConfig.percentDecrease / 100;
        UpdateHpBar();
        if (currentTotalTickValue < 0)
        {
            state = FishState.Dead;
            _moveTween.Kill();
            FlipWithDirection(new Vector3(-1, -1, 1));
            _hpBarMask.gameObject.SetActive(false);
            _spriteRenderer.material.DOFade(0, 3f);
            transform
                .DOLocalMoveY(5, 3f)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            _spriteRenderer.material.SetFloat("_SwaySpeed", 0);
            FishManager.Instance.CheckWinLose();
        }
    }

    public override void Move()
    {
        System.Random random = new System.Random();
        if (!isAttacking)
        {
            isAttacking = true;
            _animator.Play("Attack");
            var particleSystem = PoolSystem.Instance.GetObject("Ink");
            particleSystem.transform.position = transform.position;
            var main = particleSystem.GetComponent<ParticleSystem>().main;
            main.startLifetime = attackTime;
            particleSystem.SetActive(true);
            particleSystem.GetComponent<ParticleSystem>().Play();
            PoolSystem.Instance.ReturnObject(particleSystem, "Ink", attackTime);
            Invoke(nameof(StopAttack), attackTime);
        }
        if (targetPosition == transform.position)
        {
            float randomX = random.Next(-30, 30) * 0.1f;
            float randomY = random.Next(-40, -20) * 0.1f;
            targetPosition = new Vector3(randomX, randomY, 0);
        }
        base.Move();
    }

    public void StopAttack()
    {
        isAttacking = false;
    }

    public override void OnClick()
    {
        // base.OnClick();
        Debug.Log("Squid OnClick");
        DecreaseHPByClick();
    }

    public override async Task FindTarget() { }

    public override void CheckFull() { }

    public override void Update() { }
}
