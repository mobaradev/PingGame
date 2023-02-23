using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Button FullScreenButton;
    public Button GraphicsButton;
    public Button FpsButton;
    public Button AudioMusicButton;
    public TMP_InputField NickInput;
    public TMP_InputField ServerInput;

    public string DefaultServerUrl;

    public enum GraphicsSelector
    {
        High,
        Low
    }
    
    public enum AudioMusicSelector
    {
        Full,
        Quiet,
        Off
    }

    public bool FullScreenSelected;
    public GraphicsSelector GraphicsSelected;
    public int FpsSelected;
    public AudioMusicSelector AudioMusicSelected;
    public string NickSelected;
    public string ServerSelected;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadSettingsFromPlayerPrefs()
    {
        // full screen
        this.FullScreenSelected = Screen.fullScreen;
        if (this.FullScreenSelected)
        {
            this.FullScreenButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("fullscreen: true");
        }
        else
        {
            this.FullScreenButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("fullscreen: false");
        }
        
        
        // graphics
        if (PlayerPrefs.GetString("graphics") == "") PlayerPrefs.SetString("graphics", "high");
        if (PlayerPrefs.GetString("graphics") == "high")
        {
            GraphicsSelected = GraphicsSelector.High;
            this.GraphicsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("graphics: high");
        } else if (PlayerPrefs.GetString("graphics") == "low")
        {
            GraphicsSelected = GraphicsSelector.Low;
            this.GraphicsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("graphics: low");
        }
        
        // fps limit
        if (PlayerPrefs.GetInt("fps") == 0) PlayerPrefs.SetInt("fps", 60);
        this.FpsSelected = PlayerPrefs.GetInt("fps");
        this.FpsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("limit fps: " + this.FpsSelected.ToString());
        
        // audio - music
        if (PlayerPrefs.GetString("audio_music", "") == "") PlayerPrefs.SetString("audio_music", "full");
        if (PlayerPrefs.GetString("audio_music") == "full") this.AudioMusicSelected = AudioMusicSelector.Full;
        else if (PlayerPrefs.GetString("audio_music") == "quiet") this.AudioMusicSelected = AudioMusicSelector.Quiet;
        else if (PlayerPrefs.GetString("audio_music") == "off") this.AudioMusicSelected = AudioMusicSelector.Off;

        this.AudioMusicButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("sounds & music: " + PlayerPrefs.GetString("audio_music"));
        
        // nick
        this.NickSelected = PlayerPrefs.GetString("nick");
        this.NickInput.SetTextWithoutNotify(this.NickSelected);
        
        // nick
        this.ServerSelected = PlayerPrefs.GetString("serverUrl");
        if (this.ServerSelected == this.DefaultServerUrl) this.ServerSelected = "default";
        else if (this.ServerSelected == "")
        {
            this.ServerSelected = "default";
            PlayerPrefs.SetString("serverUrl", this.DefaultServerUrl);
        }
        this.ServerInput.SetTextWithoutNotify(this.ServerSelected);
    }

    public void OnFullScreenButtonClick()
    {
        this.FullScreenSelected = !this.FullScreenSelected;
        if (this.FullScreenSelected)
        {
            this.FullScreenButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("fullscreen: true");
        }
        else
        {
            this.FullScreenButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("fullscreen: false");
        }

        Screen.fullScreen = this.FullScreenSelected;
    }

    public void OnGraphicsButtonClick()
    {
        if (this.GraphicsSelected == GraphicsSelector.High)
        {
            GraphicsSelected = GraphicsSelector.Low;
            this.GraphicsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("graphics: low");
            PlayerPrefs.SetString("graphics", "low");
        }
        else
        {
            GraphicsSelected = GraphicsSelector.High;
            this.GraphicsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("graphics: high");
            PlayerPrefs.SetString("graphics", "high");
        }
    }

    public void OnFpsButtonClick()
    {
        if (this.FpsSelected == 30)
        {
            this.FpsSelected = 60;
        } else if (this.FpsSelected == 60)
        {
            this.FpsSelected = 120;
        }
        else
        {
            this.FpsSelected = 30;
        }
        
        this.FpsButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("limit fps: " + this.FpsSelected.ToString());
        PlayerPrefs.SetInt("fps", this.FpsSelected);
    }

    public void OnAudioMusicButtonClick()
    {
        if (PlayerPrefs.GetString("audio_music") == "full")
        {
            PlayerPrefs.SetString("audio_music", "quiet");
            this.AudioMusicSelected = AudioMusicSelector.Quiet;
        } else if (PlayerPrefs.GetString("audio_music") == "quiet")
        {
            PlayerPrefs.SetString("audio_music", "off");
            this.AudioMusicSelected = AudioMusicSelector.Off;
        } else if (PlayerPrefs.GetString("audio_music") == "off")
        {
            PlayerPrefs.SetString("audio_music", "full");
            this.AudioMusicSelected = AudioMusicSelector.Full;
        }
        PlayerPrefs.Save();
        
        Debug.Log("Audio Music = " + PlayerPrefs.GetString("audio_music"));
        this.AudioMusicButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("sounds & music: " + PlayerPrefs.GetString("audio_music"));
    }

    public void OnNickInputChange()
    {
        string value = this.NickInput.text;
        PlayerPrefs.SetString("nick", value);
    }
    
    public void OnServerInputChange()
    {
        string value = this.ServerInput.text;
        if (value == "default")
        {
            value = this.DefaultServerUrl;
        }
        PlayerPrefs.SetString("serverUrl", value);
    }

    public void Reset()
    {
        PlayerPrefs.SetString("graphics", "high");
        PlayerPrefs.SetString("serverUrl", this.DefaultServerUrl);
        PlayerPrefs.SetInt("fps", 60);
        
        this.LoadSettingsFromPlayerPrefs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
