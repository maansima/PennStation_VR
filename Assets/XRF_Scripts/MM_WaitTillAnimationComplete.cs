using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MM_WaitTillAnimationComplete : MonoBehaviour
{
    public BoxCollider colliderToEnable;
    public MeshRenderer rendererToEnable;

    void Start()
    {
        // Make sure collider starts OFF
        if (colliderToEnable != null && rendererToEnable != null)
            colliderToEnable.enabled = false;
            rendererToEnable.enabled = false;
    }

    // This function will be called by an Animation Event
    public void TurnOnCollider()
    {
        if (colliderToEnable != null)
            colliderToEnable.enabled = true;
            rendererToEnable.enabled = true;
    }
}
