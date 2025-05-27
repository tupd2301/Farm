using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Athena.Common.UI;
using DG.Tweening;
using TMPro;

namespace Atom
{
    public class PiggyBankLevelUpUI : UIController
    {
        public event System.Action OnLevelUpCompleted;

        [SerializeField]
        private Animator _levelupAnimator;
        [SerializeField]
        private Image _piggyImage;
        [SerializeField]
        private TextMeshProUGUI _piggyMaxCoin;
        [SerializeField]
        private List<ParticleSystem> _piggyBankLevelUp;
        [SerializeField]
        private List<Sprite> _piggyLevelSprites;
        [SerializeField]
        private List<Color> _piggyLevelColor;
        [SerializeField]
        private float _startLevelUpAnimationTime;

        private AnimationClip[] _animationClips;

        private int _toPiggyLevelIndex;
        private int _currentMaxCoin;
        private int _levelUpMaxCoin;

        public void Setup(int fromLevel, int toLevel)
        {
            setPiggySprite(fromLevel - 1);
            _toPiggyLevelIndex = toLevel - 1;
            _currentMaxCoin = PiggyBankManager.Instance.PiggyBankLogic.Config.Levels[fromLevel - 1].MaxCoin;
            _levelUpMaxCoin = PiggyBankManager.Instance.PiggyBankLogic.Config.Levels[toLevel - 1].MaxCoin;
            setMaxValue(_currentMaxCoin);
            _animationClips = _levelupAnimator.runtimeAnimatorController.animationClips;
            StartCoroutine(runFunctionAfterTime(_startLevelUpAnimationTime, playLevelUpAnimation));
        }

        private void setPiggySprite(int spriteIndex)
        {
            _piggyImage.sprite = _piggyLevelSprites[spriteIndex];
        }

        private void playLevelUpAnimation()
        {
            AnimationClip levelupAnimation = getAnimationByName("LevelUp");
            if (levelupAnimation != null)
            {
                StartCoroutine(runFunctionAfterTime(1f / 3f, () => changePiggyBankLevel(_toPiggyLevelIndex)));
                StartCoroutine(runFunctionAfterTime(1f / 3f, changeMaxPiggyBankCoinValue));
                StartCoroutine(playAnimation(levelupAnimation, onLevelUpCompleted));
            }
        }

        private void onLevelUpCompleted()
        {
            StartCoroutine(playAnimationByName("PiggyBankLevelUpDone"));
            StartCoroutine(runFunctionAfterTime(2f, () => OnLevelUpCompleted?.Invoke()));
        }

        private void changePiggyBankLevel(int spriteIndex)
        {
            setParticleColor(spriteIndex);
            setPiggySprite(spriteIndex);
        }

        private void setParticleColor(int spriteIndex)
        {
            foreach (ParticleSystem particle in _piggyBankLevelUp)
            {
                var mainParticle = particle.main;
                mainParticle.startColor = _piggyLevelColor[spriteIndex];
            }
        }

        private void changeMaxPiggyBankCoinValue()
        {
            DOTween.To(x => setMaxValue((int)x), _currentMaxCoin, _levelUpMaxCoin, 1f);
        }

        private void setMaxValue(int value)
        {
            _piggyMaxCoin.text = $"Max:{value}";
        }

        private IEnumerator runFunctionAfterTime(float time, System.Action function)
        {
            yield return Yielders.Get(time);
            function?.Invoke();
        }

        private IEnumerator playAnimationByName(string animationName)
        {
            AnimationClip animationClip = getAnimationByName(animationName);
            if (animationClip != null)
            {
                yield return playAnimation(animationClip);
            }
        }

        private AnimationClip getAnimationByName(string animationName)
        {
            foreach (AnimationClip clip in _animationClips)
            {
                if (clip.name == animationName)
                {
                    return clip;
                }
            }
            throw new System.ArgumentNullException($"animation not exist in array {nameof(_animationClips)}");
        }

        private IEnumerator playAnimation(AnimationClip clip, System.Action onCompleteCallBack = null)
        {
            _levelupAnimator.Play(clip.name);
            yield return Yielders.Get(clip.length);
            onCompleteCallBack?.Invoke();
        }
    }
}

