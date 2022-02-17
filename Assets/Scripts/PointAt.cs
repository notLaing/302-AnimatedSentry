using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAt : MonoBehaviour
{
    //public Axis aimOrientation;
    PlayerTargeting playerTargeting;
    Quaternion startRotation;
    Quaternion goalRotation;
    public bool lockAxisX = false;
    public bool lockAxisY = false;
    public bool lockAxisZ = false;

    // Start is called before the first frame update
    void Start()
    {
        playerTargeting = GetComponentInParent<PlayerTargeting>();
        startRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        TurnTowardsTarget();
    }

    void TurnTowardsTarget()
    {
        if(playerTargeting && playerTargeting.target && playerTargeting.playerWantsToAim)
        {
            Vector3 vToTarget = playerTargeting.target.transform.position - transform.position;
            //Vector3 fromVector = Vector3.forward;
            //transform.rotation = Quaternion.LookRotation(vToTarget);//, Vector3.up);

            Quaternion worldRot = Quaternion.LookRotation(vToTarget, Vector3.up);
            Quaternion prevRot = transform.rotation;

            Vector3 eulerBefore = transform.localEulerAngles;
            transform.rotation = worldRot;
            Vector3 eulerAfter = transform.localEulerAngles;
            transform.rotation = prevRot;

            /*Quaternion localRot = worldRot;
            if(transform.parent)
            {
                localRot = Quaternion.Inverse(transform.parent.rotation) * worldRot;
            }*/

            if (lockAxisX) eulerAfter.x = eulerBefore.x;
            if (lockAxisY) eulerAfter.y = eulerBefore.y;
            if (lockAxisZ) eulerAfter.z = eulerBefore.z;

            goalRotation = Quaternion.Euler(eulerAfter);

        }
        else
        {
            goalRotation = startRotation;
        }

        transform.localRotation = AnimMath.Ease(transform.localRotation, goalRotation, .001f);
    }
}
