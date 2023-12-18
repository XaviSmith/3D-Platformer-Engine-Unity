using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Platformer;

//For attacks we want to keep out like a jump hitbox or bee hitbox
public class JumpAttack : PersistentAttack
{
    [SerializeField] PlayerController player;

    public override void CollidingWith(Collider collider)
    {
        //Debug.Log("COLLIDER COLLIDING WITH " + collider.tag); //for debugging.
        if (collider.CompareTag(targetTag))
        {
            player?.BounceJump();
            collider.GetComponent<Health>()?.TakeDamage(attackDamage);
            Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
            hurtbox?.GetHit(attackDamage);
        }   
    }
}
