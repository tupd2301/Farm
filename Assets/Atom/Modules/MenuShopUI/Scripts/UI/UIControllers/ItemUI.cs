using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Athena.Common;

namespace Atom
{
    public class ItemUI : MonoBehaviour
    {
        [SerializeField]
        private Image _iconImage;
        public Image IconImage { get { return _iconImage; } }
        [SerializeField]
        private TextMeshProUGUI _amountText;
        [SerializeField]
        private GameObject _glowObject;

        private Item _item;
        public Item Item { get { return _item; } }

        public void Setup(Item item, string amountTextFormat = "+{0:N0}")
        {
            _item = item;
            //_iconImage.sprite = ResourceManager.Instance.GetSpriteOfUserItem(item);
            _amountText.text = string.Format(amountTextFormat, _item.Quantity);
        }

        public void SetupSmallIcon(Item item, string amountTextFormat = "{0:N0}")
        {
            _item = item;
            //_iconImage.sprite = ResourceManager.Instance.GetSpriteOfUserItemSmall(item);
            _amountText.text = string.Format(amountTextFormat, _item.Quantity);
        }

        public void SetIconImage(Sprite sprite)
        {
            _iconImage.sprite = sprite;
        }

        public void SetActiveGlowObject(bool isActive)
        {
            _glowObject.SetActive(isActive);
        }

        public void AppearAt(Vector2 position)
        {
            transform.DOKill();
            transform.localScale = Vector2.zero;
            transform.position = position;

            //_glowObject.SetActive(false);
            //_amountText.gameObject.SetActive(false);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(1f, .1f));
            sequence.Append(transform.DOPunchScale(Vector2.one * .5f, .1f, 8));
            sequence.AppendCallback(() => {
                _glowObject.SetActive(true);
                _amountText.gameObject.SetActive(true);
            });
        }

        public void FlyToTarget(Transform target, float finalScale, System.Action<Item> onReached)
        {
            StartCoroutine(flyToTargetProcess(target, finalScale, onReached));
        }

        private IEnumerator flyToTargetProcess(Transform target, float finalScale, System.Action<Item> onReached)
        {
            _glowObject.SetActive(false);
            _amountText.gameObject.SetActive(false);

            transform.DOKill();
            transform.DOMove(target.position, .5f).SetEase(Ease.InCubic);
            transform.DOScale(finalScale, .5f);

            yield return Yielders.Get(.5f);

            _iconImage.gameObject.SetActive(false);

            target.DOKill();
            target.DOPunchScale(Vector2.one * .25f, .25f);

            onReached?.Invoke(_item);

            Destroy(gameObject);
        }
    }
}