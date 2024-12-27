using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aim : MonoBehaviour
{
    public Transform cameraAim;
    public Transform lookAtMe;
    public Transform model;
    public Transform pc;
    private Vector3 newPos;
    private Vector3 newPosDamp;
    private Vector3 velocity;
    private Vector3 camVelocity;
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, cameraAim.transform.position, ref camVelocity, 0.2f);
        newPos = (model.transform.position + pc.transform.position) /2;
        lookAtMe.transform.position = Vector3.SmoothDamp(lookAtMe.transform.position, newPos, ref velocity, 0.1f);
    }
}
