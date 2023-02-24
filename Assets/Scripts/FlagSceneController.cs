using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlagSceneController : MonoBehaviour
{
    public TextMeshPro FlagText;
    
    void Start()
    {
        string flag = PlayerPrefs.GetString("flag");
        this.FlagText.SetText(flag);
    }
}
