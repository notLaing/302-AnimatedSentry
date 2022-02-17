using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    public TargetableObject target { get; private set; }
    public Transform boneShoulderLeft, boneShoulderRight;
    float cooldownScan = 0f;
    public float visionDist = 10f;
    float cooldownAttack = 0;
    [Range(1, 20)]
    public int roundsPerSecond = 5;
    public bool playerWantsToAim { get; private set; }
    public bool playerWantsToAttack { get; private set; }

    List<TargetableObject> validTargets = new List<TargetableObject>();

    // Start is called before the first frame update
    void Start()
    {
        playerWantsToAim = false;
    }

    // Update is called once per frame
    void Update()
    {
        playerWantsToAttack = Input.GetButton("Fire1");
        //isTargeting = Input.GetButton("Fire2");
        if (Input.GetButtonDown("Fire2"))
        {
            playerWantsToAim = !playerWantsToAim;
            //print("Target: " + target);
        }

        cooldownScan -= Time.deltaTime;
        if(playerWantsToAim)
        {
            if(target != null)
            {
                if(!CanSeeThing(target))
                {
                    target = null;
                }
            }
            if (cooldownScan <= 0) ScanForTargets();
        }

        cooldownAttack -= Time.deltaTime;
        DoAttack();
    }

    void DoAttack()
    {
        if (cooldownAttack > 0f) return;
        if (!playerWantsToAim) return;
        if (!playerWantsToAttack) return;
        if (target == null) return;
        if (!CanSeeThing(target)) return;

        cooldownAttack = 1f / roundsPerSecond;

        //TODO: spawn projectiles
        //or hitscan/take health away from target

        boneShoulderLeft.localEulerAngles += new Vector3(-30, 0, 0);
        boneShoulderRight.localEulerAngles += new Vector3(-30, 0, 0);
    }

    void ScanForTargets()
    {
        cooldownScan = .5f;
        validTargets.Clear();
        target = null;

        //ctrl + . to change "var" type into explicit type
        TargetableObject[] things = GameObject.FindObjectsOfType<TargetableObject>();

        foreach(TargetableObject thing in things)
        {
            Vector3 vToThing = thing.transform.position - transform.position;

            //close enough to see
            if(vToThing.sqrMagnitude <= visionDist * visionDist)
            {
                if (Vector3.Dot(transform.forward, vToThing) > 0.4f) validTargets.Add(thing);
            }
        }

        if (validTargets.Count > 0) PickTarget();
    }

    private bool CanSeeThing(TargetableObject thing)
    {
        Vector3 vToThing = thing.transform.position - transform.position;;

        //is too far to see?
        if (vToThing.sqrMagnitude > visionDist * visionDist) return false;

        //how much is in-front of player?
        float alignment = Vector3.Dot(transform.forward, vToThing.normalized);

        //is within so-many degrees of forward direction?
        if (alignment < .4f) return false;

        return true;
    }

    void PickTarget()
    {
        if (target) return;

        //has not found a target
        float minDist = (visionDist * visionDist) + 1f;

        foreach(TargetableObject thing in validTargets)
        {
            Vector3 vToThing = thing.transform.position - transform.position;
            if(vToThing.sqrMagnitude < minDist)
            {
                minDist = vToThing.sqrMagnitude;
                target = thing;
            }
        }
    }
}
