using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atom
{
    public class ProcessBarUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI[] _itemTexts;
        [SerializeField]
        private Slider _slider;

        public void Setup(int maxValue, int currentValue, int purchasableValue)
        {
            _slider.maxValue = maxValue;
            _itemTexts[0].text = "0";

            _itemTexts[1].text = purchasableValue.ToString();

            _slider.value = currentValue;

            _itemTexts[2].text = maxValue.ToString();
        }
    }
}