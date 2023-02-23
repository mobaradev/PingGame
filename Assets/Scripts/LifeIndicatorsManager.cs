using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeIndicatorsManager : MonoBehaviour
{
    public List<LifeIndicator> LifeIndicators = new List<LifeIndicator>();

    public LifeIndicator GetRandomIndicator()
    {
        if (this.LifeIndicators.Count == 0) return null;
        
        int randomIndex = Random.Range(0, this.LifeIndicators.Count);
        return this.LifeIndicators[randomIndex];
    }

    public void SetOffRandomIndicator()
    {
        int randomIndex = Random.Range(0, this.LifeIndicators.Count);

        for (int i = 0; i < this.LifeIndicators.Count; i++)
        {
            if (this.LifeIndicators[(i + randomIndex) % this.LifeIndicators.Count].IsOn) {
                this.LifeIndicators[(i + randomIndex) % this.LifeIndicators.Count].SetOff();
                return;
            }
        }
    }
}
