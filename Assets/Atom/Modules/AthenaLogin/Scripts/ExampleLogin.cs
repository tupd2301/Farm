using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace OneID
{
    public class ExampleLogin : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            GameObject.Instantiate(Resources.Load<GameObject>("LoginManager"));
        }
    }
}
