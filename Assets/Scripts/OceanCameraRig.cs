using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanCameraRig : MonoBehaviour
{
    public int oceanSize;

    public Transform MainCamera;

    private void Update()
    {
        transform.position = new Vector3(MainCamera.position.x - (oceanSize / 2f), transform.position.y, MainCamera.position.z - (oceanSize / 2f));
    }
}
