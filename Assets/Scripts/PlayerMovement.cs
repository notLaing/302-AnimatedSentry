using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera cam;
    CharacterController pawn;
    public float walkSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        //rotate to match camera rotation
        if (cam && (v != 0 || h != 0))
        {
            float playerYaw = transform.eulerAngles.y;
            float camYaw = cam.transform.eulerAngles.y;
            while (camYaw > playerYaw + 180f) camYaw -= 360f;
            while (camYaw < playerYaw - 180f) camYaw += 360f;

            Quaternion targetRotation = Quaternion.Euler(0, camYaw, 0);
            transform.rotation = AnimMath.Ease(transform.rotation, targetRotation, .01f);
        }

        Vector3 moveDir = (transform.forward * v) + (transform.right * h);
        if(moveDir.sqrMagnitude > 1f) moveDir.Normalize();
        moveDir *= walkSpeed;

        pawn.SimpleMove(moveDir);
    }
}
