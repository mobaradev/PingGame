using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public int points = 0;
    public int lives;

    public List<int> ballsHit = new List<int>();
    public List<int> ballsMissed = new List<int>();

    public LifeIndicatorsManager LifeIndicatorsManager;
    public BallsGenerator BallsGenerator;
    public SoundsManager SoundsManager;
    public bool IsGameOn = true;

    // public RandomNumbers r1;
    public UnifiedRandom r1;
    // public RandomNumbers r2;
    public UnifiedRandom r2;
    public List<string> rSeeds = new List<string>();
    private List<string> _x = new List<string>();

    public float GameTime = 0.0f;
    
    // Ratio of game time to the number of balls generated
    // if ratio is too high (higher than about 1.0)
    // it means that player cheated the game to make it easier
    // by having less balls at the same time to hit
    private float ratio = 0;

    public UiController UiController;
    public AudioSource AudioSource;
    
    // UI elements
    public TextMeshPro pointsText;
    public TextMeshProUGUI pointsGameOverText;
    
    
    // settings
    public bool UseCameraShake = true;
    private string _serverUrl;
    
    void Start()
    {
        // Generate two rSeeds (random seeds) and place them to the rSeeds list
        this.rSeeds.Add(HashString(Random.Range(1, 250000).ToString()));
        this.rSeeds.Add(HashString(Random.Range(1, 250000).ToString()));

        // Assign instances of RandomNumbers() to r1 and r2
        // note that the values are swapped (r1 = 2nd; r2 = 1st)
        // to make it harder for the potential cheater
        this.r1 = new UnifiedRandom(rSeeds[1]);
        this.r2 = new UnifiedRandom(rSeeds[0]);
     
        // Set proper server url
        this._serverUrl = PlayerPrefs.GetString("serverUrl");
        
        // Set proper target frame rate
        Application.targetFrameRate = PlayerPrefs.GetInt("fps");

        // Use camera shake only for 'high' graphics
        this.UseCameraShake = PlayerPrefs.GetString("graphics") == "high";
        
        // set audio mode
        string audioMode = PlayerPrefs.GetString("audio_music");
        if (audioMode == "full")
        {
            this.AudioSource.volume = 0.6f;
        } else if (audioMode == "quiet")
        {
            this.AudioSource.volume = 0.2f;
        }
        else
        {
            this.AudioSource.volume = 0.0f;
        }
    }

    public void OnBallMissed()
    {
        if (this.IsGameOn) this.lives--;

        if (this.lives == 0 && this.IsGameOn)
        {
            this.GameOver();
        }
    }

    public string PrepareSecretCode()
    {
        UnifiedRandom rn = new UnifiedRandom(this.gameObject.tag);
        int x = (int)((GameObject.FindGameObjectWithTag("Verifier").transform.position.y * rn.GetNumber(0, 500)) +
                      GameObject.FindGameObjectWithTag("Verifier").transform.position.z + rn.GetNumber(2, 250));
        string text = this.points + "-" + (double)(x - rn.GetNumber(0, x + (int)GameObject.FindGameObjectWithTag("Verifier").transform.position.x));

        text = HashString(text);
        
        return text;
    }
    
    public static string HashString(string text, string salt = "")
    {
        if (String.IsNullOrEmpty(text))
        {
            return String.Empty;
        }
    
        // Uses SHA256 to create the hash
        using (var sha = new System.Security.Cryptography.SHA256Managed())
        {
            // Convert the string to a byte array first, to be processed
            byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
            byte[] hashBytes = sha.ComputeHash(textBytes);
        
            // Convert back to a string, removing the '-' that BitConverter adds
            string hash = BitConverter
                .ToString(hashBytes)
                .Replace("-", String.Empty);

            return hash;
        }
    }

    private void FixedUpdate()
    {
        if (!this.IsGameOn)
        {
            if (!this.AudioSource.mute)
            {
                this.AudioSource.volume -= 0.01f;
                if (this.AudioSource.volume <= 0) this.AudioSource.mute = true;
            }
        }
    }
    
    void Update()
    {
        this.GameTime += Time.deltaTime;

        this.ratio = this.GameTime / GameObject.FindObjectsOfType<BallController>().Length;
        
        this.pointsText.text = this.points.ToString();

        string nx = this._gX();
        this._x.Add(nx);
        this._x = this._x.Distinct().ToList();

        if (this._x.Count > 1)
        {
            //GameObject.FindGameObjectWithTag("Verifier").transform.Translate(1, 1, 1);
            int z = this.r1.GetNumber(1, 100);
            this._x.Add(z.ToString());
            this._x.Clear();
        }
    }

    private void GameOver()
    {
        // set the number of points in the game over screen
        if (this.points == 1)
        {
            this.pointsGameOverText.SetText("1 point");
        }
        else
        {
            this.pointsGameOverText.SetText(this.points.ToString() + " points");
        }
        
        // mark that game is finished
        this.IsGameOn = false;
        this.BallsGenerator.isOpen = false;
        
        // show game over screen & enable the cursor
        this.UiController.ShowGameOverScreen();
        Cursor.visible = true;
        
        // save result to the local ranking
        RankingManager.AddResultToLocalRanking(PlayerPrefs.GetString("nick", ""), this.points);
        
        // save the result to the online ranking
        Invoke(nameof(this.UploadTrigger), 0.5f);
    }

    public void UploadTrigger()
    {
        StartCoroutine(this.Upload());
    }
    
    // JSON response expected from the server for the post '/send_score'
    class JsonResponseFull
    {
        public bool success;
        public bool flagGranted;
        public string flag;
    }
    
    IEnumerator Upload()
    {
        string nick = PlayerPrefs.GetString("nick", "");
        string jerseyNumber = PlayerPrefs.GetInt("jersey_number", 1).ToString();
        
        WWWForm form = new WWWForm();
        form.AddField("points", this.points);
        form.AddField("nick", nick);
        form.AddField("jerseyNumber", jerseyNumber);
        form.AddField("list", String.Join("", this.ballsHit.ToArray()));
        form.AddField("list2", String.Join("", this.ballsMissed.ToArray()));
        string s = this.PrepareSecretCode();
        string ss = "";
        for (int i = 0; i < s.Length; i++)
        {
            ss = ss + s[i] + this.rSeeds[1][i] + this.rSeeds[0][i];
        }
        form.AddField("ss", ss);

        using (UnityWebRequest www = UnityWebRequest.Post(this._serverUrl + "/send_score", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                this.UiController.SetConnectionStatus(UiController.ConnectionStatus.Bad);
            }
            else
            {
                Debug.Log("Form upload complete!");
                Debug.Log(www.downloadHandler.text);
                
                JsonResponseFull jsonResponseFull = JsonUtility.FromJson<JsonResponseFull>(www.downloadHandler.text);
                if (jsonResponseFull.success)
                {
                    this.UiController.SetConnectionStatus(UiController.ConnectionStatus.Good);

                    if (jsonResponseFull.flagGranted)
                    {
                        // FLAG GRANTED
                        // Debug.Log("FLAG GRANTED");
                        PlayerPrefs.SetString("flag", jsonResponseFull.flag);
                        PlayerPrefs.Save();
                        SceneManager.LoadScene("Scenes/Flag");
                    }
                    // else
                    // {
                    //     Debug.Log("FLAG NOT GRANTED");
                    // }
                }
                else
                {
                    this.UiController.SetConnectionStatus(UiController.ConnectionStatus.Bad);
                }
            }
        }
    }

    // function with meaningless name (to confuse potential cheaters)
    // that returns the unique value that is calculated by checking different
    // objects in the game (their position, size)
    // the function should return the same value for the entire game
    // if function returns different value, than expected
    // it means that player tried to cheat the game by for example increasing
    // the size of paddle to make hitting the balls easier
    private string _gX()
    {
        string x = "";
        GameObject b = GameObject.FindGameObjectWithTag("Barrier");
        x = x + Math.Round(b.transform.position.x, 2).ToString() + ", " + Math.Round(b.transform.position.y, 2).ToString() + ", " + Math.Round(b.transform.position.z, 2).ToString();
        x = x + Math.Round(b.transform.rotation.x, 2).ToString() + ", " + Math.Round(b.transform.rotation.y, 2).ToString() + ", " + Math.Round(b.transform.rotation.z, 2).ToString();
        x = x + Math.Round(b.transform.localScale.x, 2).ToString() + ", " + Math.Round(b.transform.localScale.y, 2).ToString() + ", " + Math.Round(b.transform.localScale.z, 2).ToString();
        GameObject r = GameObject.FindGameObjectWithTag("Racket");
        x = x + Math.Round(r.transform.localScale.x, 2).ToString() + ", " + Math.Round(r.transform.localScale.y, 2).ToString() + ", " + Math.Round(r.transform.localScale.z, 2).ToString();
        GameObject r2 = FindObjectOfType<RacketController>().gameObject;
        x = x + Math.Round(r2.transform.localScale.x, 2).ToString() + ", " + Math.Round(r2.transform.localScale.y, 2).ToString() + ", " + Math.Round(r2.transform.localScale.z, 2).ToString();

        return x;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Scenes/Game");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Scenes/Menu");
    }
}
