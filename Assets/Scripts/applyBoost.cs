using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class applyBoost : MonoBehaviour
{
    public GameObject target;
    void OnTriggerEnter(UnityEngine.Collider collision)
    {
        if (collision.CompareTag("Vehicle"))
        {
            VehicleMovement vehicleMovement = collision.GetComponent<VehicleMovement>();

            Vector3 toBooster = target.transform.position - collision.transform.position;
            Vector3 vehicleForward = collision.transform.forward;

            // Get forward direction of vehicle
            float dotProduct = Vector3.Dot(vehicleForward, toBooster.normalized);

            // If is the vehicle facing the booster?
            bool isFacingBooster = dotProduct > 0;

            // Apply boost based on direction
            float bonusSpeed = isFacingBooster ? 10f : -40f;
            vehicleMovement.bonusSpeed += bonusSpeed;
        }
    }
}