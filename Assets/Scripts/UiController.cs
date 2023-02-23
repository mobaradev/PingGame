using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{
    public GameObject GameOverScreen;
    public GameObject ConnectionPendingText;
    public GameObject ConnectionBadText;
    public GameObject ConnectionGoodText;

    private void Start()
    {
        this.SetConnectionStatus(ConnectionStatus.Pending);
    }

    public enum ConnectionStatus
    {
        Pending,
        Bad,
        Good
    }
    
    public void SetConnectionStatus(ConnectionStatus status)
    {
        this.ConnectionPendingText.SetActive(false);
        this.ConnectionBadText.SetActive(false);
        this.ConnectionGoodText.SetActive(false);
        
        if (status == ConnectionStatus.Pending) this.ConnectionPendingText.SetActive(true);
        if (status == ConnectionStatus.Bad) this.ConnectionBadText.SetActive(true);
        if (status == ConnectionStatus.Good) this.ConnectionGoodText.SetActive(true);
    }

    public void ShowGameOverScreen()
    {
        this.GameOverScreen.SetActive(true);
    }
    
}
