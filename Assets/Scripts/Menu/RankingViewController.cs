using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class RankingViewController : MonoBehaviour
{
    public List<RankingManager.Result> LocalRanking;
    public List<RankingManager.Result> OnlineRanking;

    public GameObject RankingItemOnListPrefab;
    public GameObject LocalRankingList;
    public GameObject OnlineRankingList;

    public GameObject LocalRankingTab;
    public GameObject OnlineRankingTab;
    
    public void LoadLocalRanking()
    {
        this.LocalRanking = RankingManager.GetLocalRanking();

        for (int i = 0; i < this.LocalRanking.Count; i++)
        {
            GameObject newRankingItem = Instantiate(this.RankingItemOnListPrefab, this.LocalRankingList.transform);
            newRankingItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(16, -16 - 32 * i);
            if (this.LocalRanking[i].points > 9999)
            {
                newRankingItem.GetComponentInChildren<TextMeshProUGUI>().SetText((i+1) + ". " + this.LocalRanking[i].nick + " (9999+)");
            }
            else
            {
                newRankingItem.GetComponentInChildren<TextMeshProUGUI>().SetText((i+1) + ". " + this.LocalRanking[i].nick + " (" + this.LocalRanking[i].points + "p)");
            }
        }
        
    }
    
    [System.Serializable]
    public struct RankingJsonResponse
    {
        public bool success;
        public RankingManager.Result[] ranking;
    }

    public IEnumerator LoadOnlineRanking()
    {
        // clear the online list
        foreach (Transform child in this.OnlineRankingList.transform) {
            GameObject.Destroy(child.gameObject);
        }

        Debug.Log(PlayerPrefs.GetString("serverUrl") + "/get_ranking");
        using (UnityWebRequest www = UnityWebRequest.Get(PlayerPrefs.GetString("serverUrl") + "/get_ranking"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                GameObject errorItem = Instantiate(this.RankingItemOnListPrefab, this.OnlineRankingList.transform);
                errorItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(16, -16 - 32 * 0);
                errorItem.GetComponentInChildren<TextMeshProUGUI>().SetText("Could not load list from the server");
                errorItem.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                errorItem.GetComponentInChildren<TextMeshProUGUI>().fontSize = 15;
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                
                RankingJsonResponse rankingJsonResponse = JsonUtility.FromJson<RankingJsonResponse>(www.downloadHandler.text);
                if (rankingJsonResponse.success)
                {
                    this.OnlineRanking = rankingJsonResponse.ranking.ToList();
                    
                    for (int i = 0; i < this.OnlineRanking.Count; i++)
                    {
                        GameObject newRankingItem = Instantiate(this.RankingItemOnListPrefab, this.OnlineRankingList.transform);
                        newRankingItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(16, -16 - 32 * i);

                        if (this.OnlineRanking[i].points > 9999)
                        {
                            newRankingItem.GetComponentInChildren<TextMeshProUGUI>().SetText((i+1) + ". " + this.OnlineRanking[i].nick + " (9999+)");
                        }
                        else
                        {
                            newRankingItem.GetComponentInChildren<TextMeshProUGUI>().SetText((i+1) + ". " + this.OnlineRanking[i].nick + " (" + this.OnlineRanking[i].points + "p)");                
                        }
            
                    }
                }
                else
                {
                    
                }
            }
        }
    }

    public void OnLocalRankingTabClick()
    {
        StartCoroutine(this._setLocalView());
    }

    public void OnOnlineRankingTabClick()
    {
        StartCoroutine(this._setOnlineView());
    }
    
    private IEnumerator _setLocalView()
    {
        FindObjectOfType<MenuController>().Cover.GetComponent<Animator>().SetTrigger("ShowAndHide");
        
        yield return new WaitForSeconds(0.5f);
        
        this.LocalRankingList.SetActive(true);
        this.OnlineRankingList.SetActive(false);
        
        this.LocalRankingTab.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Underline | FontStyles.Bold;
        this.OnlineRankingTab.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
    }
    
    private IEnumerator _setOnlineView()
    {
        FindObjectOfType<MenuController>().Cover.GetComponent<Animator>().SetTrigger("ShowAndHide");
        
        yield return new WaitForSeconds(0.5f);
        
        this.LocalRankingList.SetActive(false);
        this.OnlineRankingList.SetActive(true);

        this.LocalRankingTab.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
        this.OnlineRankingTab.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Underline | FontStyles.Bold;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        this.LoadLocalRanking();
    }

    public void OnActive()
    {
        StartCoroutine(this.LoadOnlineRanking());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
