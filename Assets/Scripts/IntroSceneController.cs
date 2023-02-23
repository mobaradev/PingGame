using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneController : MonoBehaviour
{
    public GameObject Warning;
    public GameObject Logo1;
    public GameObject Logo2;
    public GameObject DataPanel;

    public TextMeshProUGUI JerseyNumber;
    public TextMeshProUGUI JerseyNick;

    public TMP_InputField NickInput;
    
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("nick", "") != "")
        {
            this.JerseyNick.SetText(PlayerPrefs.GetString("nick"));
            this.NickInput.text = PlayerPrefs.GetString("nick");
        }
        
        this.JerseyNumber.SetText(PlayerPrefs.GetInt("jersey_number", 0).ToString());
        
        Cursor.visible = true;

        StartCoroutine(this._hideWarning());
        StartCoroutine(this._showLogo1());
        StartCoroutine(this._hideLogo1());
        StartCoroutine(this._showLogo2());
        StartCoroutine(this._hideLogo2());
        StartCoroutine(this._showDataPanel());
    }

    private IEnumerator _hideWarning()
    {
        yield return new WaitForSeconds(2.5f);
        this.Warning.GetComponent<Animator>().SetTrigger("WarningOut");
    }

    private IEnumerator _showLogo1()
    {
        yield return new WaitForSeconds(7f);
        this.Logo1.GetComponent<Animator>().SetTrigger("LogoIn");
    }
    
    private IEnumerator _hideLogo1()
    {
        yield return new WaitForSeconds(10f);
        this.Logo1.GetComponent<Animator>().SetTrigger("LogoOut");
    }
    
    private IEnumerator _showLogo2()
    {
        yield return new WaitForSeconds(15f);
        this.Logo2.GetComponent<Animator>().SetTrigger("LogoIn");
    }
    
    private IEnumerator _hideLogo2()
    {
        yield return new WaitForSeconds(18f);
        this.Logo2.GetComponent<Animator>().SetTrigger("LogoOut");
    }
    
    private IEnumerator _showDataPanel()
    {
        yield return new WaitForSeconds(21.5f);
        this.DataPanel.SetActive(true);
    }
    
    private void _hideDataPanel()
    {
        this.DataPanel.GetComponent<Animator>().SetTrigger("DataOut");
    }

    public void OnJerseyClick()
    {
        int number = Int32.Parse(this.JerseyNumber.text);
        number += 1;
        if (number > 99) number = 1;
        
        this.JerseyNumber.SetText(number.ToString());
        PlayerPrefs.SetInt("jersey_number", number);
    }

    public void OnNickInputChange()
    {
        string value = this.NickInput.text;
        PlayerPrefs.SetString("nick", value);
        this.JerseyNick.SetText(value);
    }

    public void Continue()
    {
        PlayerPrefs.Save();
        this._hideDataPanel();
        StartCoroutine(this._goToMainMenu());
    }

    private IEnumerator _goToMainMenu()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Scenes/Menu");
    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            if (PlayerPrefs.GetString("nick", "") != "" && PlayerPrefs.GetInt("jersey_number", 0) != 0)
            {
                SceneManager.LoadScene("Scenes/Menu");
            }
        }
    }
}
