using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

//Not a scriptable obj so we can use Awake/Update functions to keep track of our own timers.
public abstract class BaseAttack : MonoBehaviour, IAttack
{
    [SerializeField] float attackDistance;
    [SerializeField] int attackDamage;
    [SerializeField] protected float cooldownTime;
    [SerializeField] string targetTag; //who do we hit, e.g. enemies etc

    protected CountdownTimer attackTimer = null;

    public bool IsRunning => attackTimer != null && attackTimer.IsRunning;

    protected void Awake()
    {
        attackTimer = new CountdownTimer(cooldownTime);    
    }

    protected virtual void Update()
    {
        attackTimer.Tick(Time.deltaTime);
    }

    public virtual void StartAttack()
    {
        if (!attackTimer.IsRunning)
        {
            attackTimer.Start();
        }  
    }

    // Start is called before the first frame update
    public virtual void Attack()
    {
        Vector3 origin = transform.position + transform.forward;
        Collider[] hitEnemies = Physics.OverlapSphere(origin, attackDistance);

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log(enemy.name);
            if (enemy.CompareTag(targetTag))
            {
                enemy.GetComponent<Health>().TakeDamage(attackDamage);
            }
        }
    }

}
