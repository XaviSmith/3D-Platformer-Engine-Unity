using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField]int currentHealth; //viewable for debugging

    bool squashed = false; //we got jumped on etc.
    public bool IsSquashed => squashed;
    public bool IsDead => !squashed && currentHealth <= 0;

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

    public void SquashDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            squashed = true;
        }

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealth();
    }

    void UpdateHealth()
    {
        if(gameObject.CompareTag("Player"))
        {
            EventManager<float>.TriggerEvent(Events.UPDATEPLAYERHEALTH.ToString(), currentHealth / (float)maxHealth);
        }     
    }
}
