using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualDistraction : Distraction
{
    private GameObject DistractionObject;
    private Vector3 objectPosition;
    private Vector3 objectSize;
    private Vector3 force;

    public VisualDistraction(GameObject go, Vector3 op, Vector3 os, Vector3 f)
    {

        DistractionObject = go;
        objectPosition = op;
        objectSize = os;
        force = f;

        DistractionObject.SetActive(false);
        DistractionObject.transform.localScale = objectSize;
    }

    public override void StartDistraction()
    {
        DistractionObject.transform.position = objectPosition;
        DistractionObject.SetActive(true);
        DistractionObject.GetComponent<Rigidbody>().AddForce(force.x, force.y, force.z, ForceMode.Impulse);
    }

    public override void StopDistraction()
    {
        DistractionObject.SetActive(false);
        DistractionObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
