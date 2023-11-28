using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitboxListener
{
    void CollidingWith(Collider collider);
}
