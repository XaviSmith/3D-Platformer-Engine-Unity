using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// More performant shadow that will always render beneath the player or target object. Projectors are too costly for slower devices.
/// </summary>
public class ShadowCast : MonoBehaviour
{
    [SerializeField] bool drawDebugRay;
    [SerializeField] LayerMask layers;
    [SerializeField] SpriteRenderer shadowSprite; //We can have multiple shadow sprites and raycasts if we care about shadows hanging off ledges. For this project that's acceptable.
    [SerializeField] float shadowDist;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 shadowOffset;
    [SerializeField, Range(0, 255)] float maxShadowAlpha = 178;
    RaycastHit hit;
    Color32 spriteColor;

    private void OnEnable()
    {
        spriteColor = shadowSprite.color;    
    }

    void LateUpdate()
    {
        if(drawDebugRay)
        {
            Debug.DrawRay(transform.position + offset, Vector3.down * shadowDist, Color.blue);
        }
        
        if (Physics.Raycast(transform.position + offset, Vector3.down, out hit, shadowDist, layers))
        {
            shadowSprite.transform.position = new Vector3(hit.point.x, hit.point.y + shadowOffset.y, hit.point.z); //raise it a teeny bit off the floor
            spriteColor.a = (byte)(Mathf.Lerp(maxShadowAlpha, 0, hit.distance / shadowDist)); //scale shadow alpha with how close we are to the ground

        } else
        {
            spriteColor.a = 0;
        }
        shadowSprite.color = spriteColor;
    }

}
