using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerTargeting player;
    Camera cam;
    public float mouseSensitivityX = 5;
    public float mouseSensitivityY = -5;
    public float mouseSensitivityScroll = 5;
    float pitch = 0f;
    float yaw = 0f;
    float dollyDist = 10f;

    float shakeTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        if(player == null)
        {
            PlayerTargeting script = FindObjectOfType<PlayerTargeting>();
            if (script != null) player = script;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool isAiming = (player && player.target && player.playerWantsToAim);

        //move rig position (with easing)
        if (player)
        {
            transform.position = AnimMath.Ease(transform.position, player.transform.position + new Vector3(0, 1, 0), .01f);
        }

        //rotate rig (TODO: ease)
        float playerYaw = AnimMath.AngleWrapDegrees(yaw, player.transform.eulerAngles.y);

        if (isAiming)
        {
            Quaternion tempTarget = Quaternion.Euler(0, playerYaw, 0);
            transform.rotation = AnimMath.Ease(transform.rotation, tempTarget, .001f);
        }
        else
        {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");
            yaw += mx * mouseSensitivityX;
            pitch += my * mouseSensitivityY;
            //if (yaw > 360f) yaw -= 360f;
            //if (yaw < 0) yaw += 360;
            pitch = Mathf.Clamp(pitch, -10f, 89f);
            //transform.rotation = Quaternion.Euler(pitch, yaw, 0);
            transform.rotation = AnimMath.Ease(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
        }

        //dolly camera
        dollyDist += Input.mouseScrollDelta.y * mouseSensitivityScroll;
        dollyDist = Mathf.Clamp(dollyDist, 3, 20);
        //ease
        float tempZ = isAiming ? 2 : dollyDist;
        cam.transform.localPosition = AnimMath.Ease(cam.transform.localPosition, new Vector3(0, 0, -tempZ), .02f);

        //rotate the camera object (not the rig)
        if(isAiming)
        {
            Vector3 vToAimTarget = player.target.transform.position - cam.transform.position;
            Quaternion worldRot = Quaternion.LookRotation(vToAimTarget);
            Quaternion localRot = worldRot;

            if(cam.transform.parent)
            {
                localRot = Quaternion.Inverse(cam.transform.parent.rotation) * worldRot;
            }

            Vector3 euler = localRot.eulerAngles;
            euler.z = 0;
            localRot.eulerAngles = euler;
            
            cam.transform.localRotation = AnimMath.Ease(cam.transform.localRotation, localRot, .001f);
        }
        else
        {
            cam.transform.localRotation = AnimMath.Ease(cam.transform.localRotation, Quaternion.identity, .001f);
        }

        UpdateShake();
    }

    void UpdateShake()
    {
        if (shakeTimer < 0) return;

        shakeTimer -= Time.deltaTime;
        float p = shakeTimer / 1f;
        p *= p;
        p = AnimMath.Lerp(1f, .98f, p);
        Quaternion randomRot = AnimMath.Lerp(Random.rotation, Quaternion.identity, p);
        
        cam.transform.localRotation *= randomRot;
    }

    public void Shake(float time)
    {
        if (time > shakeTimer) shakeTimer = time;
    }

    void OnDrawGizmos()
    {
        if (!cam) cam = GetComponentInChildren<Camera>();
        if (!cam) return;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
        Gizmos.DrawLine(transform.position, cam.transform.position);
    }
}
