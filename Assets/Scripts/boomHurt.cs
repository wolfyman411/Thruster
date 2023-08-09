using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boomHurt : MonoBehaviour
{
    bool canDamage = true;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("NoMoreDamage", 0.1f);
    }

    void NoMoreDamage()
    {
        canDamage = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Vehicle") && canDamage == true)
        {
            other.GetComponent<VehicleMovement>().DealDamage(25);
        }
    }
}
