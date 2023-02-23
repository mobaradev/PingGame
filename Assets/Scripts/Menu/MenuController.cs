using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject Settings;
    public GameObject Ranking;
    public GameObject Credits;
    public GameObject Cover;

    public SettingsController SettingsController;
    public RankingViewController RankingViewController;

    public enum PanelSelector
    {
        Main,
        Settings,
        Ranking,
        Credits
    }
    // Start is called before the first frame update
    void Start()
    {
        this.SettingsController.LoadSettingsFromPlayerPrefs();

        Cursor.visible = true;
    }

    private void _hideAllPanels()
    {
        this.MainMenu.SetActive(false);
        this.Settings.SetActive(false);
        this.Ranking.SetActive(false);
        this.Credits.SetActive(false);
    }

    public void SetPanel(string panelName)
    {
        if (panelName == "Main") StartCoroutine(this.SetPanel(PanelSelector.Main));
        if (panelName == "Settings") StartCoroutine(this.SetPanel(PanelSelector.Settings));
        if (panelName == "Ranking") StartCoroutine(this.SetPanel(PanelSelector.Ranking));
        if (panelName == "Credits") StartCoroutine(this.SetPanel(PanelSelector.Credits));
    }

    public IEnumerator SetPanel(PanelSelector panel)
    {
        this.Cover.GetComponent<Animator>().SetTrigger("ShowAndHide");
        
        yield return new WaitForSeconds(0.5f);

        if (this.Settings.activeSelf)
        {
            Debug.Log("PP saved;");
            PlayerPrefs.Save();
        }
        
        this._hideAllPanels();
        if (panel == PanelSelector.Main)
        {
            this.MainMenu.SetActive(true);
        } else if (panel == PanelSelector.Settings)
        {
            this.Settings.SetActive(true);
        } else if (panel == PanelSelector.Ranking)
        {
            this.Ranking.SetActive(true);
            this.RankingViewController.OnActive();
        } else if (panel == PanelSelector.Credits)
        {
            this.Credits.SetActive(true);
        }
    }

    public void OnPlayButtonClick()
    {
        this.Cover.GetComponent<Animator>().SetTrigger("ShowAndHide");
        StartCoroutine(this.LoadGame());
    }

    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }

    public IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Scenes/Game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
