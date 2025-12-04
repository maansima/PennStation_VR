using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MM_WaitTillAnimationComplete : MonoBehaviour
{
    public BoxCollider colliderToEnable;

    void Start()
    {
        // Make sure collider starts OFF
        if (colliderToEnable != null)
            colliderToEnable.enabled = false;
    }

    // This function will be called by an Animation Event
    public void TurnOnCollider()
    {
        if (colliderToEnable != null)
            colliderToEnable.enabled = true;
    }
}
