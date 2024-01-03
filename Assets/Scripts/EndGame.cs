using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Platformer;
using System;
using UnityEngine.SceneManagement;

//Hardcoding ending to get project finished within time limit
public class EndGame : MonoBehaviour
{
    public static EndGame Instance;
    GameManager gameManager; //reference to the instance we set OnEnable.
    [Header("References")]
    [SerializeField] string endScene;
    [SerializeField] GameObject EndGameContainerObj;
    [SerializeField] Camera _camera;
    [SerializeField] TypewriterText textBox;
    [SerializeField] Animator animator;
    [SerializeField] GameTimer gameTimer;
    [SerializeField] ScreenFader fader;
    [SerializeField] AudioClip drumrollSFX;
    [SerializeField] AudioClip applauseSFX;

    [SerializeField] float textsDuration = 2.5f;

    [Header("AwardTimes")]
    [SerializeField] float bronzeTime;
    [SerializeField] float silverTime;
    [SerializeField] float goldTime;
    [SerializeField] float platTime;

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

    void OnEnable()
    {
        gameManager = GameManager.Instance;
        EventManager.StartListening(Events.ENDGAME.ToString(), EndTheGame);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.ENDGAME.ToString(), EndTheGame);
    }


    public void EndTheGame()
    {
        gameManager.HideTexts = true;
        gameTimer.PauseTimer();         
        StartCoroutine(ShowTexts());
    }

    IEnumerator ShowTexts()
    {
        EndGameContainerObj.SetActive(true);
        gameManager.GetPlayerController.LockMovement();
        fader.FadeInAndOut();

        while (!fader.FadeInFinished)
        {
            yield return null;
        }

        _camera.depth = 1;
        SceneManager.LoadScene(endScene);

        Gem unlockedGem = null;
             
        while(!fader.IsFinished)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        textBox.SetText("Great Job! You collected all the stars in... ");
        SoundManager.Instance.PlaySFX(drumrollSFX);
        yield return new WaitForSeconds(3.25f);
        textBox.AddText(gameTimer.FormattedTime + "!");

        yield return new WaitForSeconds(3f);

        if(gameTimer.CurrTime < bronzeTime)
        {
            textBox.SetText("That means you've earned..."); 
            yield return new WaitForSeconds(3f);

            animator.Play("WaveStart");

            if (gameTimer.CurrTime < platTime)
            {
                unlockedGem = GemManager.Instance.UnlockGem(GemColours.PLAT);
                textBox.SetText("The Platinum Gem!");
                SoundManager.Instance.PlaySFX(applauseSFX);
                yield return new WaitForSeconds(3f);
                textBox.SetText("The best award I have to offer!");
                yield return new WaitForSeconds(1f);
                textBox.AddText(" Congrats!!");
                yield return new WaitForSeconds(3.5f);
                textBox.SetText("I hope you enjoyed the game!");
                yield return new WaitForSeconds(4f);
                yield break;
            } else if(gameTimer.CurrTime < goldTime)
            {
                unlockedGem = GemManager.Instance.UnlockGem(GemColours.GOLD);
                textBox.SetText("The Gold Gem!");
                yield return new WaitForSeconds(2f);
                textBox.SetText("Nice!");
                yield return new WaitForSeconds(3f);
                textBox.SetText("Thanks for playing!");
            } else if (gameTimer.CurrTime < silverTime)
            {
                unlockedGem = GemManager.Instance.UnlockGem(GemColours.SILVER);
                textBox.SetText("The Silver Emerald!");
                yield return new WaitForSeconds(2f);
                textBox.SetText("Congratulations!");
                yield return new WaitForSeconds(2f);
                textBox.SetText("If you can do it again in under " + TimeSpan.FromSeconds(goldTime).ToString(@"hh\:mm\:ss") + " I'll give you an even better prize!");
            } else
            {
                unlockedGem = GemManager.Instance.UnlockGem(GemColours.BRONZE);
                textBox.SetText("The Bronze Gem!");
                yield return new WaitForSeconds(2f);
                textBox.SetText("Good going!");
                yield return new WaitForSeconds(2f);
                textBox.SetText("If you can do it again in under " + TimeSpan.FromSeconds(silverTime).ToString(@"hh\:mm\:ss") + " I'll give you an even better prize!");
            }


        } else
        {
            textBox.SetText("If you can do it again in under " + TimeSpan.FromSeconds(bronzeTime).ToString(@"hh\:mm\:ss") + " I'll give you a prize!");
        }

        yield return new WaitForSeconds(2f);
    }

}
