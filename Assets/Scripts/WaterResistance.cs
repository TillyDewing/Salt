using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterResistance : MonoBehaviour
{
    public float waterDragMul;

    private float baseDrag;
    private float baseAngularDrag;

    private List<Floater> floaters = new List<Floater>();
    private Rigidbody rigidBody;
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child.tag == "Floater")
            {
                floaters.Add(child.GetComponent<Floater>());
            }
        }
        rigidBody = GetComponent<Rigidbody>();
        baseDrag = rigidBody.drag;
        baseAngularDrag = rigidBody.angularDrag;
    }

    public void Update()
    {
        if (InWater())
        {
            rigidBody.drag = baseDrag * waterDragMul;
            rigidBody.angularDrag = baseAngularDrag * waterDragMul;
        }
        else
        {
            rigidBody.drag = baseDrag;
            rigidBody.angularDrag = baseAngularDrag;
        }
    }

    public bool InWater()
    {
        foreach(Floater floater in floaters)
        {
            if (floater.isInWater())
            {
                return true;
            }
        }

        return false;
    }
}
