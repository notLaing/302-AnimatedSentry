using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    Moving,
    Aiming,
    Attacking,
    Death
}

public class EnemyController : MonoBehaviour
{
    CharacterController pawn;
    NavMeshAgent agent;
    Transform navTarget;
    public Transform rightOddInnerFrontJoint, rightOddInnerBackJoint, rightEvenInnerFrontJoint, rightEvenInnerBackJoint,
        leftOddInnerFrontJoint, leftOddInnerBackJoint, leftEvenInnerFrontJoint, leftEvenInnerBackJoint,
        rightOddOuterFrontJoint, rightOddOuterBackJoint, rightEvenOuterFrontJoint, rightEvenOuterBackJoint,
        leftOddOuterFrontJoint, leftOddOuterBackJoint, leftEvenOuterFrontJoint, leftEvenOuterBackJoint;
    Transform[] legJoints = new Transform[16];
    public PointAt pointScript;
    public Shooter shootScript;
    public EnemyState state = EnemyState.Idle;
    Vector3 targetLocation;
    Quaternion leftInStart, rightInStart, leftOutStart, rightOutStart;
    float huntDistSqrd = 2500f;
    float curDistToPlayer;
    float deathAnimTime = 3f;
    public int health = 25;
    bool dieOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 2;

        PlayerTargeting player = FindObjectOfType<PlayerTargeting>();
        navTarget = player.transform;

        //fill legJoints in order of similarly moving legs
        legJoints[0] = rightOddInnerFrontJoint;
        legJoints[1] = rightOddInnerBackJoint;
        legJoints[2] = leftEvenInnerFrontJoint;
        legJoints[3] = leftEvenInnerBackJoint;

        legJoints[4] = rightEvenInnerFrontJoint;
        legJoints[5] = rightEvenInnerBackJoint;
        legJoints[6] = leftOddInnerFrontJoint;
        legJoints[7] = leftOddInnerBackJoint;

        legJoints[8] = rightOddOuterFrontJoint;
        legJoints[9] = rightOddOuterBackJoint;
        legJoints[10] = leftEvenOuterFrontJoint;
        legJoints[11] = leftEvenOuterBackJoint;

        legJoints[12] = rightEvenOuterFrontJoint;
        legJoints[13] = rightEvenOuterBackJoint;
        legJoints[14] = leftOddOuterFrontJoint;
        legJoints[15] = leftOddOuterBackJoint;

        leftInStart = leftOddInnerBackJoint.localRotation;
        rightInStart = rightOddInnerBackJoint.localRotation;
        leftOutStart = leftOddOuterBackJoint.localRotation;
        rightOutStart = rightOddOuterBackJoint.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        targetLocation = navTarget.transform.position - transform.position;
        curDistToPlayer = targetLocation.sqrMagnitude;

        if(health <= 0)
        {
            state = EnemyState.Death;
        }
        //move only if the player is within distance
        else if (navTarget && curDistToPlayer < huntDistSqrd)
        {
            agent.destination = navTarget.transform.position;
            state = EnemyState.Moving;
            
            //aim if within aiming distance (also move). Should be same as in Shooter script
            if(curDistToPlayer <= 900f)
            {
                state = EnemyState.Aiming;

                if(shootScript.shootTime <= 0f)
                {
                    state = EnemyState.Attacking;
                }
            }
            else
            {
                pointScript.target = null;
            }
        }
        else
        {
            state = EnemyState.Idle;
            pointScript.target = null;
        }
        

        //animations: idle, walk, aim, attack, death, one more animation. Can do multiple without moving, like Zelda: BOTW guardians
        switch(state)
        {
            case EnemyState.Idle:
                AnimateIdle();
                break;
            case EnemyState.Aiming:
                AnimateAim();
                AnimateMove();
                break;
            case EnemyState.Moving:
                AnimateMove();
                break;
            case EnemyState.Attacking:
                AnimateAttack();
                break;
            case EnemyState.Death:
                AnimateDeath();
                deathAnimTime -= Time.deltaTime;
                break;
        }
    }

    void AnimateIdle()
    {
        float wave = Mathf.Sin(Time.time * 2f) * 5f;//[-35, -25] and [80, 90]

        for (int i = 0; i < 16; ++i)
        {
            Quaternion rot = legJoints[i].localRotation;
            Vector3 euler = legJoints[i].localRotation.eulerAngles;

            //inner part of leg
            if (i < 8)
            {
                //group 1
                if (i < 4) euler.x = -30f + wave;
                //group 2
                else euler.x = -30f - wave;
            }
            else
            {
                //group 1
                if (i < 12) euler.x = 85f - wave;
                //group 2
                else euler.x = 85f + wave;
            }

            rot.eulerAngles = euler;
            legJoints[i].localRotation = rot;
        }
    }

    void AnimateMove()
    {
        float waveUp = Mathf.Sin(Time.time * 4f) * 5f;//[-35, -25] and [80, 90]
        float waveForward = Mathf.Sin(Time.time * 4f) * 10f;

        for (int i = 0; i < 16; ++i)
        {
            Quaternion rot = legJoints[i].localRotation;
            Vector3 euler = legJoints[i].localRotation.eulerAngles;

            //inner part of leg
            if (i < 8)
            {
                //group 1
                if (i < 4)
                {
                    euler.x = -30f + waveUp;
                    //left leg
                    if (i % 4 > 1) euler.y = -90f + waveForward;
                    else euler.y = 90f + waveForward;
                }
                //group 2
                else
                {
                    euler.x = -30f - waveUp;
                    //left leg
                    if (i % 4 > 1) euler.y = -90f - waveForward;
                    else euler.y = 90f - waveForward;
                }
            }
            else
            {
                //group 1
                if (i < 12)
                {
                    euler.x = 85f - waveUp;
                    //left leg
                    if (i % 4 > 1) euler.y = -90f + waveForward;
                    else euler.y = 90f + waveForward;
                }
                //group 2
                else
                {
                    euler.x = 85f + waveUp;
                    //left leg
                    if (i % 4 > 1) euler.y = -90f - waveForward;
                    else euler.y = 90f - waveForward;
                }
            }

            rot.eulerAngles = euler;
            legJoints[i].localRotation = rot;
        }
    }

    void AnimateAim()
    {
        pointScript.target = navTarget;
    }

    void AnimateAttack()
    {
        shootScript.shootTime += 5f;
        shootScript.Shoot();
        shootScript.transform.localEulerAngles += new Vector3(-30, 0, 0);
    }

    void AnimateDeath()
    {
        agent.baseOffset = AnimMath.Lerp(0.1f, 1, Mathf.Clamp((deathAnimTime - 2f), 0, 1));

        //flatten legs
        if (deathAnimTime >= 2f)
        {
            //align
            if(!dieOnce)
            {
                dieOnce = true;
                for(int i = 0; i < 16; ++i)
                {
                    Quaternion rot = legJoints[i].localRotation;
                    Vector3 euler = legJoints[i].localRotation.eulerAngles;
                    //inner part of leg
                    if (i < 8)
                    {
                        euler.x = -30f;

                        if (i % 4 > 1) euler.y = -90f;
                        else euler.y = 90f;
                    }
                    else
                    {
                        euler.x = 85f;
                    }

                    rot.eulerAngles = euler;
                    legJoints[i].localRotation = rot;
                }
            }
            //flatten
            else
            {
                for (int i = 0; i < 16; ++i)
                {
                    Quaternion rot = legJoints[i].localRotation;
                    Vector3 euler = legJoints[i].localRotation.eulerAngles;
                    euler.x = AnimMath.Lerp(0, euler.x, Mathf.Clamp((deathAnimTime - 2f), 0, 1));

                    rot.eulerAngles = euler;
                    legJoints[i].localRotation = rot;
                }
            }
        }
        else//curl legs
        {
            for(int i = 0; i < 16; ++i)
            {
                Quaternion rot = legJoints[i].localRotation;
                Vector3 euler = legJoints[i].localRotation.eulerAngles;

                /*if(euler.x < 0)
                {
                    euler.x += 360f;
                    rot.eulerAngles = euler;
                    legJoints[i].localRotation = rot;
                }*/

                //inner
                if(i < 8) euler.x = AnimMath.Lerp(-50 + 360, euler.x, Mathf.Clamp((deathAnimTime - 1f), 0, 1));
                else euler.x = AnimMath.Lerp(-120 + 360, euler.x, Mathf.Clamp((deathAnimTime - 1f), 0, 1));

                rot.eulerAngles = euler;
                legJoints[i].localRotation = rot;
            }
        }

        if (deathAnimTime <= 0f) gameObject.SetActive(false);
    }
}
