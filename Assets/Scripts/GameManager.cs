using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Score { get; private set; }

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

    private void OnEnable()
    {
        //EventManager<int>.StartListening(Events.UPDATESCORE.ToString(), AddScore);
    }

    public void AddScore(int score)
    {
        Debug.Log("Adding " + score + " to score.");
        Score += score;
    }
}
