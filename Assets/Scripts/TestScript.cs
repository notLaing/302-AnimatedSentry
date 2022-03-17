using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Quaternion rot;
    public Vector3 angles;
    public float targetAngle = -60f;
    public float t = 3f;
    // Start is called before the first frame update
    void Start()
    {
        rot = gameObject.transform.localRotation;
        angles = gameObject.transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            t = 4;
            targetAngle *= -1;
        }
        t -= Time.deltaTime;
        angles.z = AnimMath.Lerp(targetAngle, angles.z, Mathf.Clamp((t - 1f), 0, 1));
        rot.eulerAngles = angles;
        gameObject.transform.localRotation = rot;
    }
}
