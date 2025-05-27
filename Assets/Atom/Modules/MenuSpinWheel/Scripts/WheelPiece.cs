using UnityEngine;

namespace Atom.Modules.SpinWheel
{
   [System.Serializable]
   public class WheelPiece
   {
      public string Id;
      public string Icon;
      public string Label;

      [Tooltip("Reward amount")] public int Amount;

      [Tooltip("Probability in %")]
      [Range(0f, 100f)]
      public float Chance = 100f;

      [HideInInspector] public int Index;
      [HideInInspector] public double weight = 0f;

      [Space, Header("Background")]
      [Tooltip("Background color of the piece")]
      public Color Color;
      [Tooltip("Path of background sprite of the piece, will display over the background color if set")]
      public string BGSprite;
   }
}
