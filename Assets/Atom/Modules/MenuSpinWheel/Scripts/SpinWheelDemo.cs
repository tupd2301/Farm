using System.Collections;
using System.Collections.Generic;
using Atom.Modules.SpinWheel;
using UnityEngine;

public class SpinWheelDemo : MonoBehaviour
{
    public SpinWheel spinWheel;

    private async void Start() 
    {
        await spinWheel.GetConfigFromPlayfab();
        spinWheel.Show();
    }
}
