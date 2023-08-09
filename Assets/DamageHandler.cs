using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    public GameObject Smoke1;
    public GameObject Smoke2;
    VehicleMovement vm;
    // Start is called before the first frame update
    void Start()
    {
        vm = GetComponent<VehicleMovement>();
        Smoke1.SetActive(false);
        Smoke2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (vm.HP <= 50)
        {
            Smoke1.SetActive(true);
        }
        if (vm.HP <= 25)
        {
            Smoke2.SetActive(true);
        }
        if (vm.HP > 50)
        {
            Smoke1.SetActive(false);
            Smoke2.SetActive(false);
        }
    }
}
