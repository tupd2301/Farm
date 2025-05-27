using System.Collections;
using System.Collections.Generic;
using Atom;
using UnityEngine;

namespace Farm
{
    public class GameManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            AppManager.Instance.ShowUI<HomeUI>("HomeUI", 0);
        }

        // Update is called once per frame
        void Update() { }
    }
}
