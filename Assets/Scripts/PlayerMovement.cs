using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Transform boneLegLeft, boneLegRight, boneHip, boneSpine1, boneSpine2, boneNeck;//boneHip parents all other bones
    public GameObject partArmLeft, partArmRight, partUpperTorso, partLowerTorso, partHips, partHead, partLegLeft, partLegRight;
    GameObject[] bodyParts = new GameObject[8];
    Vector3 boneHipStart, boneSpine1Start, boneSpine2Start, boneNeckStart;
    Quaternion boneLegLeftStart, boneLegRightStart;
    public Camera cam;
    CharacterController pawn;
    PlayerTargeting targetingScript;
    public float walkSpeed = 5f;
    [Range(-10, -1)]
    public float gravity = -1f;
    Vector3 inputDir;
    float velocityVertical = 0f;
    float cooldownJumpWindow = 0f;
    float deathAnimTime = 5f;
    public int health = 3;
    public bool isGrounded
    {
        get
        {
            //return pawn.isGrounded || cooldownJumpWindow > 0;//
            return CheckGrounded();
        }
    }
    bool fromIdle = true;
    bool dieOnce = false;

    public float idleSpeed = .01f;
    public float idleSpace = .1f;

    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();
        targetingScript = GetComponent<PlayerTargeting>();

        boneSpine1Start = boneSpine1.localPosition;
        boneSpine2Start = boneSpine2.localPosition;
        boneLegLeftStart = boneLegLeft.localRotation;
        boneLegRightStart = boneLegRight.localRotation;
        boneHipStart = boneHip.localPosition;
        boneNeckStart = boneNeck.localPosition;

        bodyParts[0] = partArmLeft;
        bodyParts[1] = partArmRight;
        bodyParts[2] = partUpperTorso;
        bodyParts[3] = partLowerTorso;
        bodyParts[4] = partHips;
        bodyParts[5] = partHead;
        bodyParts[6] = partLegLeft;
        bodyParts[7] = partLegRight;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            AnimateDeath();
            deathAnimTime -= Time.deltaTime;
            return;
        }

        if (cooldownJumpWindow > 0) cooldownJumpWindow -= Time.deltaTime;
        //lateral movement
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        bool playerIsAiming = (targetingScript && targetingScript.playerWantsToAim && targetingScript.target);

        if(playerIsAiming)
        {
            if (fromIdle)
            {
                fromIdle = false;
                BreakIdle();
            }
            Vector3 toTarget = targetingScript.target.transform.position - transform.position;
            toTarget.Normalize();
            Quaternion worldRot = Quaternion.LookRotation(toTarget);
            Vector3 euler = worldRot.eulerAngles;
            euler.x = 0;
            euler.z = 0;
            worldRot.eulerAngles = euler;

            transform.rotation = AnimMath.Ease(transform.rotation, worldRot, .01f);
        }
        //rotate to match camera rotation
        else if (cam && (v != 0 || h != 0))
        {
            float playerYaw = transform.eulerAngles.y;
            float camYaw = cam.transform.eulerAngles.y;
            while (camYaw > playerYaw + 180f) camYaw -= 360f;
            while (camYaw < playerYaw - 180f) camYaw += 360f;

            Quaternion playerRotation = Quaternion.Euler(0, playerYaw, 0);
            Quaternion targetRotation = Quaternion.Euler(0, camYaw, 0);
            transform.rotation = AnimMath.Ease(playerRotation, targetRotation, .01f);
        }

        inputDir = (transform.forward * v) + (transform.right * h);
        if(inputDir.sqrMagnitude > 1f) inputDir.Normalize();

        //vertical movement
        bool wantsToJump = Input.GetButtonDown("Jump");
        if (isGrounded)
        {
            if(wantsToJump)
            {
                cooldownJumpWindow = 0f;
                velocityVertical = 5;
            }
        }
        velocityVertical += gravity * Time.deltaTime;

        //move player
        Vector3 moveAmount = inputDir * walkSpeed + Vector3.up * velocityVertical;
        pawn.Move(moveAmount * Time.deltaTime);
        if(isGrounded)
        {
            cooldownJumpWindow = .5f;
            velocityVertical = 0;

            if (inputDir == Vector3.zero && !playerIsAiming)
            {
                fromIdle = true;
                AnimateIdle();
            }
            else
            {
                if (fromIdle)
                {
                    fromIdle = false;
                    BreakIdle();
                }
                WalkAnimation();
            }
        }
        else
        {
            if(fromIdle)
            {
                fromIdle = false;
                BreakIdle();
            }
            AirAnimation();
        }
    }

    bool CheckGrounded()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position - new Vector3(0, 1, 0), Vector3.down, out hit, .1f))
        {
            return true;
        }
        return false;
    }

    void AnimateIdle()
    {
        boneLegLeft.localRotation = boneLegLeftStart;
        boneLegRight.localRotation = boneLegRightStart;

        float wave = Mathf.Abs(Mathf.Sin(Time.time * idleSpeed)) * idleSpace;
        boneSpine1.localPosition = boneSpine1Start + new Vector3(0, wave, 0);
        boneSpine2.localPosition = boneSpine2Start + new Vector3(0, wave, 0);
        boneNeck.localPosition = boneNeckStart + new Vector3(0, wave, 0);
    }

    void BreakIdle()
    {
        boneSpine1.localPosition = boneSpine1Start;
        boneSpine2.localPosition = boneSpine2Start;
        //boneLegLeft.localRotation = boneLegLeftStart;
        //boneLegRight.localRotation = boneLegRightStart;
        //boneHip.localPosition = boneHipStart;
        boneNeck.localPosition = boneNeckStart;
    }

    void WalkAnimation()
    {
        float speed = 10;
        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDir);
        Vector3 axis = Vector3.Cross(inputDirLocal, Vector3.up);

        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);
        alignment = Mathf.Abs(alignment);
        float degrees = AnimMath.Lerp(10, 30, alignment, false);
        float wave = Mathf.Sin(Time.time * speed) * degrees;

        //bone alignment will mess up axis
        boneLegLeft.localRotation = Quaternion.AngleAxis(wave, axis);
        boneLegRight.localRotation = Quaternion.AngleAxis(-wave, axis);

        //hips
        if (boneHip)
        {
            float walkAmount = axis.magnitude;
            float offsetY = Mathf.Cos(Time.time * speed) * walkAmount * .05f;
            boneHip.localPosition = new Vector3(0, boneHipStart.y + offsetY, 0);
        }
    }

    void AirAnimation()
    {
        //
    }

    void AnimateDeath()
    {
        if (deathAnimTime <= 0f) gameObject.SetActive(false);

        if (!dieOnce)
        {
            dieOnce = true;

            //set the capsule collider and character controller to inactive so the head can move
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            gameObject.GetComponent<CharacterController>().enabled = false;

            //set all the rigid bodies to use gravity, all the body part box colliders to active, and addForce
            foreach (GameObject b in bodyParts)
            {
                b.GetComponent<BoxCollider>().enabled = true;
                b.GetComponent<Rigidbody>().useGravity = true;
                b.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * 500f);
            }
        }
    }

    public void TakeDamage()
    {
        --health;
    }
}
