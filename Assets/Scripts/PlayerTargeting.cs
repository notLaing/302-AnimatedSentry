using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    public TargetableObject target { get; private set; }
    float cooldownScan = 0f;
    public float visionDist = 10f;
    public bool isTargeting { get; private set; }

    List<TargetableObject> validTargets = new List<TargetableObject>();

    // Start is called before the first frame update
    void Start()
    {
        isTargeting = false;
    }

    // Update is called once per frame
    void Update()
    {
        //isTargeting = Input.GetButton("Fire2");
        if (Input.GetButtonDown("Fire2"))
        {
            isTargeting = !isTargeting;
            print("Target: " + target);
        }

        cooldownScan -= Time.deltaTime;
        if(isTargeting)
        {
            if (cooldownScan <= 0) ScanForTargets();
        }
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
