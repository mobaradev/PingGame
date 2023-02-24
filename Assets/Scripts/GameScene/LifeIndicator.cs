using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeIndicator : MonoBehaviour
{
    public bool IsOn = true;

    public Material MaterialOn;
    public Material MaterialOff;

    public void SetOn()
    {
        this.GetComponent<Renderer>().material = this.MaterialOn;
        this.IsOn = true;
    }

    public void SetOff()
    {
        this.GetComponent<Renderer>().material = this.MaterialOff;
        this.IsOn = false;
    }
}
