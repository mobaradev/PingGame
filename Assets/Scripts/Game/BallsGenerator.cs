using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsGenerator : MonoBehaviour
{
    public List<GameObject> ballPrefabs;
    public bool isOpen = true;
    
    void Start()
    {
        this.InvokeRepeating(nameof(this.GenerateBall), 1f, 0.5f);
    }

    void GenerateBall()
    {
        if (!this.isOpen) return;
        
        int ballPrefabIndex = 0;
        float randomValue = Random.Range(0f, 1f);
        if (randomValue <= 0.2f)
        {
            ballPrefabIndex = 1;
        } else if (randomValue <= 0.35f)
        {
            ballPrefabIndex = 2;
        } else if (randomValue <= 0.45f)
        {
            ballPrefabIndex = 3;
        } else if (randomValue <= 0.5f)
        {
            ballPrefabIndex = 4;
        }
        
        Instantiate(this.ballPrefabs[ballPrefabIndex], new Vector3(0, 4.12f, 7.53f), Quaternion.identity);
    }
}
