using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis
{
    Forward,
    Backward,
    Right,
    Left,
    Up,
    Down
}

public class PointAt : MonoBehaviour
{
    public Axis aimOrientation;
    PlayerTargeting playerTargeting;
    Quaternion startRotation;

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
        if(playerTargeting && playerTargeting.target && playerTargeting.isTargeting)
        {
            Vector3 vToTarget = playerTargeting.target.transform.position - transform.position;
            //transform.rotation = Quaternion.LookRotation(vToTarget, Vector3.up);

            Vector3 fromVector = Vector3.forward;
            switch (aimOrientation)
            {
                case Axis.Forward:
                    fromVector = Vector3.forward;
                    break;
                case Axis.Backward:
                    fromVector = Vector3.back;
                    break;
                case Axis.Right:
                    fromVector = Vector3.right;
                    break;
                case Axis.Left:
                    fromVector = Vector3.left;
                    break;
                case Axis.Up:
                    fromVector = Vector3.up;
                    break;
                case Axis.Down:
                    fromVector = Vector3.down;
                    break;
            }

            transform.rotation = Quaternion.FromToRotation(fromVector, vToTarget);
        }
        else
        {
            transform.localRotation = startRotation;
        }
    }
}
