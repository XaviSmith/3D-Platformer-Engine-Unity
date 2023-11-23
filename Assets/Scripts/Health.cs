using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] FloatEventChannel playerHealthChannel;

    int currentHealth;

    public bool IsDead => currentHealth <= 0;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void OnEnable()
    {
        EventManager.StartListening(Events.TEST.ToString(), Test);
        EventManager<int>.StartListening(Events.DAMAGE.ToString(), TakeDamage);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.TEST.ToString(), Test);
        EventManager<int>.StopListening(Events.DAMAGE.ToString(), TakeDamage);
    }

    public void Test()
    {
        Debug.Log("TEST CALLED");
    }

    void Start()
    {
        UpdateHealth();
    }

    /*public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        PublishHealthPercentage();
    }*/

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealth();
    }

    void PublishHealthPercentage()
    {
        if(playerHealthChannel != null)
        {
            playerHealthChannel.Invoke(currentHealth / (float) maxHealth);
        }
    }

    void UpdateHealth()
    {
        if(gameObject.tag == "Player")
        {
            EventManager<float>.TriggerEvent(Events.UPDATEPLAYERHEALTH.ToString(), currentHealth / (float)maxHealth);
        }     
    }
}
