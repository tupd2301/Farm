using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Factory
{
    [CreateAssetMenu(fileName = "BoxConfigSO", menuName = "Factory/BoxConfigSO")]
    public class BoxConfigSO : ScriptableObject
    {
        public List<BoxConfig> boxConfigs;

        void OnValidate()
        {
            int index = 0;
            foreach (var boxConfig in boxConfigs)
            {
                boxConfig.boxType = (BoxType)index;
                index++;
                if (index > 51)
                {
                    boxConfigs.Remove(boxConfig);
                    Debug.LogError("BoxConfig duplicate: " + boxConfig.boxType);
                }
            }
        }
    }

    [System.Serializable]
    public enum BoxType
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5,
        G = 6,
        H = 7,
        I = 8,
        J = 9,
        K = 10,
        L = 11,
        M = 12,
        N = 13,
        O = 14,
        P = 15,
        Q = 16,
        R = 17,
        S = 18,
        T = 19,
        U = 20,
        V = 21,
        W = 22,
        X = 23,
        Y = 24,
        Z = 25,
        A1 = 26,
        B1 = 27,
        C1 = 28,
        D1 = 29,
        E1 = 30,
        F1 = 31,
        G1 = 32,
        H1 = 33,
        I1 = 34,
        J1 = 35,
        K1 = 36,
        L1 = 37,
        M1 = 38,
        N1 = 39,
        O1 = 40,
        P1 = 41,
        Q1 = 42,
        R1 = 43,
        S1 = 44,
        T1 = 45,
        U1 = 46,
        V1 = 47,
        W1 = 48,
        X1 = 49,
        Y1 = 50,
        Z1 = 51,
    }

    [System.Serializable]
    public class BoxConfig
    {
        [ReadOnly]
        public BoxType boxType;
        public int weight;
        public int boxCurrencyValue;
        public int patienceValue;

        public void Copy(BoxConfig other)
        {
            boxType = other.boxType;
            weight = other.weight;
            boxCurrencyValue = other.boxCurrencyValue;
            patienceValue = other.patienceValue;
        }
    }
}
