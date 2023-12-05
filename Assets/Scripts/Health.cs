using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;

    [SerializeField]int currentHealth; //viewable for debugging

    public bool IsDead => currentHealth <= 0;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void OnEnable()
    {
        EventManager<int>.StartListening(Events.DAMAGE.ToString(), TakeDamage);
    }

    void OnDisable()
    {
        EventManager<int>.StopListening(Events.DAMAGE.ToString(), TakeDamage);
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

    void UpdateHealth()
    {
        if(gameObject.tag == "Player")
        {
            EventManager<float>.TriggerEvent(Events.UPDATEPLAYERHEALTH.ToString(), currentHealth / (float)maxHealth);
        }     
    }
}
