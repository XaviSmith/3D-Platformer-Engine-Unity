using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] LayerMask mask;
    [Tooltip("Uses boxes if false")][SerializeField] bool useSphere = false;
    [SerializeField] Vector3 hitboxSize = Vector3.one;
    [SerializeField] float radius;

    [Header("Debug Settings")]
    [SerializeField] bool drawHitbox = true;
    [SerializeField] Color32 inactiveColor = Color.red;
    [SerializeField] Color32 activeColor = Color.blue;
    [SerializeField] Color32 collidingColor = Color.green;
    [SerializeField] ColliderState colliderState;
    IHitboxListener listener = null;

    /// <summary>
    /// Called by whatever is managing our hitbox (e.g. BaseAttack)
    /// </summary>
    public void UpdateHitbox()
    {
        if(colliderState == ColliderState.INACTIVE) { return; }

        Collider[] colliders = Physics.OverlapBox(transform.position, hitboxSize, transform.rotation, mask);

        if(colliders.Length > 0)
        {
            colliderState = ColliderState.COLLIDING;

            for (int i = 0; i < colliders.Length; i++)
            {
                Collider _collider = colliders[i];
                //Debug.Log("WE HIT " + _collider.gameObject.name); //For debugging
                listener?.CollidingWith(_collider);
            }
        }
        else
        {
            colliderState = ColliderState.ACTIVE;
        }
    }

    public void ActivateHitbox()
    {
        colliderState = ColliderState.ACTIVE;
    }

    public void DeactivateHitbox()
    {
        colliderState = ColliderState.INACTIVE;
    }

    public void AddResponder(IHitboxListener _listener)
    {
        listener = _listener;
    }

    void OnDrawGizmos()
    {
        if(drawHitbox)
        {
            switch (colliderState)
            {
                case ColliderState.INACTIVE:
                    Gizmos.color = inactiveColor;
                    break;
                case ColliderState.ACTIVE:
                    Gizmos.color = activeColor;
                    break;
                case ColliderState.COLLIDING:
                    Gizmos.color = collidingColor;
                    break;

            }
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(hitboxSize.x * 2, hitboxSize.y * 2, hitboxSize.z * 2));
        }
        
    }
}
