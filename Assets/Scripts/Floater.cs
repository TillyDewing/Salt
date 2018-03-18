using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    public float LiftAcceleration = 1;
    public float MaxDistance = 3;
    public Rigidbody Body;

    public float waveEffectDepth = -1.5f;
    private Ocean ocean;

    void Start()
    {
        ocean = (Ocean)GameObject.FindObjectOfType(typeof(Ocean));
    }

    void FixedUpdate()
    {
        Vector3 p = transform.position;
        float waterHeight = ocean.GetHeightAtLocation(p.x, p.z);
        Vector3 waterNormal = ocean.GetNormalAtLocation(p.x, p.z);

        if(transform.position.y > waveEffectDepth)
        {
            waterNormal = Vector3.Lerp(waterNormal, Vector3.up, .5f);
        }
        else
        {
            waterNormal = Vector3.up;
        }

        float forceFactor = Mathf.Clamp(1f - (p.y - waterHeight) / MaxDistance, 0, 1);
        transform.parent.GetComponent<Rigidbody>().AddForceAtPosition(waterNormal * forceFactor * LiftAcceleration * Body.mass, p);

        //if (!Debug.isDebugBuild)
        //    return;
        Debug.DrawLine(p, p + waterNormal * forceFactor * 5, Color.green);
        Debug.DrawLine(p + waterNormal * forceFactor * 5, p + waterNormal * 5, Color.red);
    }

    public bool isInWater()
    {
        Vector3 p = transform.position;
        float waterHeight = ocean.GetHeightAtLocation(p.x, p.z);
        if (p.y <= waterHeight + 1)
        {
            //Debug.Log("In Water");
            return true;
        }

        return false;
    }
}
