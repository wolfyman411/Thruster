using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public int Timer = 10;
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Decay", 1, 1);
    }

    private void Update()
    {
        if (target != null)
        {
            gameObject.transform.position = target.transform.position;
        }
    }

    void Decay()
    {
        if (Timer > 0 && Timer < 999)
        {
            Timer--;
        }
        else if (Timer <= 0)
        {
            target.GetComponent<VehicleMovement>().isShielded = false;
            Destroy(gameObject);
        }
    }
}
