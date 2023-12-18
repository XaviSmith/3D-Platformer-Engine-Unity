using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

//For attacks we want to keep out like a jump hitbox or bee hitbox
public class PersistentAttack : MonoBehaviour, IAttack, IHitboxListener
{
    [SerializeField] protected bool startActive = true;
    [SerializeField] protected Hitbox hitbox;
    [SerializeField] protected int attackDamage;
    [SerializeField] protected string targetTag; //who do we hit, e.g. enemies etc

    public bool IsRunning { get; private set; }

    protected void OnEnable()
    {
        if(startActive)
        {
            StartAttack();
        }
    }

    protected virtual void Update()
    {
        if(IsRunning)
        {
            hitbox.UpdateHitbox();
        }
    }

    public virtual void StartAttack()
    {
        IsRunning = true;
        hitbox.ActivateHitbox();
        hitbox.AddResponder(this);
    }

    public virtual void StopAttack()
    {
        IsRunning = false;
        hitbox.DeactivateHitbox();
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

    public virtual void CollidingWith(Collider collider)
    {
        //Debug.Log("COLLIDER COLLIDING WITH " + collider.tag); //for debugging.
        if (collider.CompareTag(targetTag))
        {
            collider.GetComponent<Health>()?.TakeDamage(attackDamage);
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            hurtbox?.GetHit(attackDamage);
        }   
    }
}
