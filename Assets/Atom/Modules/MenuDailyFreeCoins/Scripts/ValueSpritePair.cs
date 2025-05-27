using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ValueSpritePair
{
    public int Value;
    public Sprite Sprite;
}

[System.Serializable]
public class ListValueSpritePair
{
    public List<ValueSpritePair> MultiplierViewDefines = new List<ValueSpritePair>();

    public int Count { get => MultiplierViewDefines.Count; }

    public Sprite GetSprite(int multiplier)
    {
        foreach (ValueSpritePair multiplierDefine in MultiplierViewDefines)
        {
            if (multiplier == multiplierDefine.Value)
            {
                return multiplierDefine.Sprite;
            }
        }
        return null;
    }
}

