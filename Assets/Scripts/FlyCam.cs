using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCam : MonoBehaviour
{
    Vector2 rot;

    private void Update()
    {
        rot = new Vector2(
            rot.x + Input.GetAxis("Mouse X") * 2,
            rot.y + Input.GetAxis("Mouse Y") * 2);

        transform.localRotation = Quaternion.AngleAxis(rot.x, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rot.y, Vector3.left);

        transform.position += transform.forward * 2 * Input.GetAxis("Vertical");
        transform.position += transform.right * 2 * Input.GetAxis("Horizontal");
    }
}
