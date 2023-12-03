using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

//Not a scriptable obj so we can use Awake/Update functions to keep track of our own timers.
public abstract class BaseAttack : MonoBehaviour, IAttack, IHitboxListener
{
    [SerializeField] Hitbox hitbox;
    [SerializeField] int attackDamage;
    [SerializeField] protected float cooldownTime;
    [SerializeField] string targetTag; //who do we hit, e.g. enemies etc

    protected CountdownTimer attackTimer = null;

    public bool IsRunning => attackTimer != null && attackTimer.IsRunning;

    protected void Awake()
    {
        SetupAttackTimer();
    }

    protected void SetupAttackTimer()
    {
        attackTimer = new CountdownTimer(cooldownTime);

        attackTimer.OnTimerStart += () => { hitbox.ActivateHitbox(); };
        attackTimer.OnTimerStop += () => { hitbox.DeactivateHitbox(); };
    }

    protected virtual void Update()
    {
        attackTimer.Tick(Time.deltaTime);
        if(attackTimer.IsRunning)
        {
            hitbox.UpdateHitbox();
        }
    }

    public virtual void StartAttackTimer()
    {
        if (!attackTimer.IsRunning)
        {         
            attackTimer.Start();
        }  
    }

    // Start is called before the first frame update
    public virtual void Attack()
    {
        hitbox.AddResponder(this);

        /*Vector3 origin = transform.position + transform.forward;
        Collider[] hitEnemies = Physics.OverlapSphere(origin, attackDistance);

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log(enemy.name);
            if (enemy.CompareTag(targetTag))
            {
                enemy.GetComponent<Health>().TakeDamage(attackDamage);
            }
        }*/
    }

    public void CollidingWith(Collider collider)
    {
        
        if (collider.CompareTag(targetTag))
        {
            //Debug.Log("COLLIDER COLLIDING WITH " + collider.name); //for debugging.
            collider.GetComponent<Health>()?.TakeDamage(attackDamage);
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            hurtbox?.GetHit(attackDamage);
        }   
    }
}
