using System.Collections;
using UnityEngine;

namespace Atom
{
    public class PayoutItemSFX : MonoBehaviour
    {
        [SerializeField]
        private string _flyStartSoundId;
        //[SerializeField]
        //private string _flyCompleteSoundId = C.AudioIds.Sound.BoosterCollected;

        public void PlayFlyStartSound()
        {
            if (!string.IsNullOrEmpty(_flyStartSoundId))
            {
                //AudioPlayer.sharedInstance.PlaySound(_flyStartSoundId);
            }
        }

        public void PlayFlyCompleteSound()
        {
            //if(!string.IsNullOrEmpty(_flyCompleteSoundId))
            //{
            //AudioPlayer.sharedInstance.PlaySound(_flyCompleteSoundId);
            //}
        }
    }
}