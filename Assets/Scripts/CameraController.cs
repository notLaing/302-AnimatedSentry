using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    Camera cam;
    public float mouseSensitivityX = 5;
    public float mouseSensitivityY = -5;
    public float mouseSensitivityScroll = 5;
    float pitch = 0f;
    float yaw = 0f;
    float dollyDist = 10f;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //move (with easing)
        if (target)
        {
            transform.position = AnimMath.Ease(transform.position, target.position + new Vector3(0, 1, 0), .01f);
        }
        
        //rotate (TODO: ease)
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        yaw += mx * mouseSensitivityX;
        pitch += my * mouseSensitivityY;
        pitch = Mathf.Clamp(pitch, -10f, 89f);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        //dolly camera
        dollyDist += Input.mouseScrollDelta.y * mouseSensitivityScroll;
        dollyDist = Mathf.Clamp(dollyDist, 3, 20);
        cam.transform.localPosition = AnimMath.Ease(cam.transform.localPosition, new Vector3(0, 0, -dollyDist), .02f);
    }

    void OnDrawGizmos()
    {
        if (!cam) cam = GetComponentInChildren<Camera>();
        if (!cam) return;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
        Gizmos.DrawLine(transform.position, cam.transform.position);
    }
}
