using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Transform boneLegLeft, boneLegRight, boneHip, boneSpine;
    public Camera cam;
    CharacterController pawn;
    PlayerTargeting targetingScript;
    public float walkSpeed = 5f;
    [Range(-10, -1)]
    public float gravity = -1f;
    Vector3 inputDir;
    float velocityVertical = 0f;
    float cooldownJumpWindow = 0f;
    float deathAnimTime = 2f;
    public int health = 3;
    public bool isGrounded
    {
        get
        {
            return pawn.isGrounded || cooldownJumpWindow > 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();
        targetingScript = GetComponent<PlayerTargeting>();
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
        if (pawn.isGrounded)
        {
            cooldownJumpWindow = .5f;
            velocityVertical = 0;
            WalkAnimation();
        }
        else
        {
            AirAnimation();
        }

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
            boneHip.localPosition = new Vector3(0, offsetY, 0);
        }
    }

    void AirAnimation()
    {
        //
    }

    void AnimateDeath()
    {
        if (deathAnimTime <= 0f) gameObject.SetActive(false);
    }

    public void TakeDamage()
    {
        --health;
    }
}
