using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class smoothCam : MonoBehaviour
{
    public Transform target;
    public float movementSpeed = 5f;
    public float rotationSpeed = 5f;
    bool follow = true;

    private void FixedUpdate()
    {
        // Calculate the desired position and rotation
        Vector3 desiredPosition = target.position;
        Quaternion desiredRotation = target.rotation;

        // Smooth interpolation
        if (follow)
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, movementSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }

        //Check if dead
        if (target.transform.parent.gameObject.GetComponent<VehicleMovement>() && target.transform.parent.gameObject.GetComponent<VehicleMovement>().isDead)
        {
            follow = false;
        }
        else
        {
            follow = true;
        }
    }

}
