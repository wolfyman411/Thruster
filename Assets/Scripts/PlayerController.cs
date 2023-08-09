using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    VehicleMovement vm;
    itemHandler iH;
    bool isMovingHor;
    bool isMovingVer;
    // Start is called before the first frame update
    void Start()
    {
        iH = GetComponent<itemHandler>();
        vm = GetComponent<VehicleMovement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        vm.TurnDirection(horizontalInput); //Turner
        if (Mathf.Abs(verticalInput) > 0.5f)
        {
            vm.Accelerate(verticalInput); //Accelerator
        }

        if (horizontalInput > 0.5 || horizontalInput < -0.5)//Rotation
        {
            vm.isTurning = true;
            isMovingHor = true;
        }
        else
        {
            vm.isTurning = false;
            isMovingHor = false;
        }

        if (verticalInput > 0.5 || verticalInput < -0.5)//Accelerate
        {
            isMovingVer = true;
        }
        else
        {
            isMovingVer = false;
        }

        if (isMovingHor == false && isMovingVer == false)//Fix Rotation
        {
            vm.FixRotation();
        }

        //Drift Checker
        if (Input.GetAxis("Drift") > 0)
        {
            vm.StartDrift(horizontalInput);
        }
        else if (vm.isDrifting == true)
        {
            vm.isDrifting = false;
            vm.EndDrift();
        }

        //Use Checker
        if (Input.GetAxis("UseItem") > 0)
        {
            iH.UseItem();
        }
    }
}
