using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Atom
{
    public class Price : MonoBehaviour
    {
        public TextMeshProUGUI Amount;
        public TextMeshProUGUI OptCrossPriceTxt;

        public void Setup(PriceData data)
        {
            if (data.Type == PriceData.CurrencyType.RM)
            {
                Amount.text = string.Format("${0:0.00}", (float)data.Amount / 100f);
            }
            else
            {
                Amount.text = data.Amount.ToString();
            }
            if (OptCrossPriceTxt != null && data.CrossPrice.HasValue)
            {
                if (data.Type == PriceData.CurrencyType.RM)
                {
                    OptCrossPriceTxt.text = string.Format("${0:0.00}", (float)data.CrossPrice / 100f);
                }
                else
                {
                    OptCrossPriceTxt.text = data.CrossPrice.ToString();
                }
            }
            else
            {
                if (OptCrossPriceTxt != null)
                {
                    OptCrossPriceTxt.gameObject.SetActive(false);
                }
            }
        }
    }
}