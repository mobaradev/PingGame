using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Random = UnityEngine.Random;

public class BallController : MonoBehaviour
{
    public int ballId;
    private GameManager _gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        this._gameManager = FindObjectOfType<GameManager>();
        this.AddForce();
    }
    
    public void AddForce()
    {
        float randomX = Random.Range(-0.3f, 0.3f);
        float randomZ = (-1) * Random.Range(1f, 1.3f);
        
        this.GetComponent<Rigidbody>().AddForce(new Vector3(randomX, 0, randomZ), ForceMode.Impulse);
    }

    public void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("Collision enter = " + collision.gameObject.tag);
        if (collision.gameObject.tag == "Racket")
        {
            if (this._gameManager.IsGameOn)
            {
                this._gameManager.points += 1 * this.ballId;
                this._gameManager.SoundsManager.PlaySound();
                this._gameManager.ballsHit.Add(this.ballId);
                this.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                int pts = (int)Math.Round(this.transform.localScale.x * 10) + (int)Math.Round(this.transform.localScale.y * 20) + (int)Math.Round(this.transform.localScale.z * 30);
                if (this.ballId == 1) pts += 34;
                else if (this.ballId == 2) pts += -11;
                else if (this.ballId == 3) pts += 2;
                else if (this.ballId == 4) pts += 5;
                else if (this.ballId == 5) pts += 6;

                List<int> pList = new List<int>();
                for (int i = 0; i < 5; i++)
                {
                    pList.Add(this._gameManager.r1.GetNumber(1, 301));
                }

                pts += pList[this.ballId - 1];

                GameObject.FindGameObjectWithTag("Verifier").transform.Translate(0, (pts), 0);
                
                // camera shake effect
                if (this._gameManager.UseCameraShake) CameraShake.Shake(0.2f, 0.1f);
            }
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Barrier")
        {
            if (this._gameManager.IsGameOn)
            {
                int pts = GameObject.FindObjectsOfType<BallController>().Length <= 3 ? this.ballId : this.ballId * (int)(Math.Ceiling(this._gameManager.GameTime / GameObject.FindObjectsOfType<BallController>().Length));

                if (this.ballId == 1) pts += 9;
                else if (this.ballId == 2) pts += 3;
                else if (this.ballId == 3) pts += -2;
                else if (this.ballId == 4) pts += 7;
                else if (this.ballId == 5) pts += 0;
                
                List<int> pList = new List<int>();
                for (int i = 0; i < 5; i++)
                {
                    pList.Add(this._gameManager.r2.GetNumber(1, this._gameManager.lives * 101 + this.ballId * 3));
                }
                
                pts += pList[this.ballId - 1];
                
                GameObject.FindGameObjectWithTag("Verifier").transform.Translate(pts, 0, 19f);
                
                this._gameManager.ballsMissed.Add(this.ballId);
                this._gameManager.LifeIndicatorsManager.SetOffRandomIndicator();
                this._gameManager.OnBallMissed();
            }
        }
    }
}
