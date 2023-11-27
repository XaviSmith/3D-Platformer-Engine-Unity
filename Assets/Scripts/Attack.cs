using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    [SerializeField] float attackDistance;
    [SerializeField] int attackDamage;

    // Start is called before the first frame update
    public virtual void StartAttack()
    {
        Vector3 attackPos = transform.position + transform.forward;
        Collider[] hitEnemies = Physics.OverlapSphere(attackPos, attackDistance);

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log(enemy.name);
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<Health>().TakeDamage(attackDamage);
            }
        }
    }
}
