using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unlocks and keeps track of gems
/// </summary>
public class GemManager : MonoBehaviour
{
    public static GemManager Instance;
    [SerializeField] Transform awardPosition;
    [SerializeField] List<Gem> gems = new List<Gem>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Gem UnlockGem(GemColours colour)
    {
        int index = (int)colour; //We want to unlock all the previous gems too
        int currGem = -1;
        Gem _gem = null;

        foreach (Gem g in gems)
        {
            int gemColour = (int)g.GemColour;

            if(gemColour <= index)
            {
                g.Unlock();
                if(gemColour >= currGem)
                {                  
                    _gem = g;
                    currGem = (int)g.GemColour;
                }
                
            }
        }

        _gem.transform.position = awardPosition.position;
        _gem.gameObject.SetActive(true);
        return _gem;
    }
}
