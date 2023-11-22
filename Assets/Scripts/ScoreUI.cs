using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI scoreText;

    void Start()
    {
        UpdateScore();
    }

    public void UpdateScore()
    {
        //Done in a coroutine to give all logic time to run before updating the score
        StartCoroutine(UpdateScoreCoroutine());
    }

    IEnumerator UpdateScoreCoroutine()
    {
        yield return null;
        scoreText.text = GameManager.Instance.Score.ToString();
    }
}
