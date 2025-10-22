using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_PositionMatcher : MonoBehaviour
{
    public Transform objectToFollow;
    public Transform objectFollowing;



    // Update is called once per frame
    void Update()
    {
        objectFollowing.position = objectToFollow.position;
    }
}
