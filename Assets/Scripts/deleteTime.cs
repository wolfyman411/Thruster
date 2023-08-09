using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deleteTime : MonoBehaviour
{
    // Start is called before the first frame update
    public float time;
    void Start()
    {
        Invoke("Delete", time);
    }

    void Delete()
    {
        Destroy(gameObject);
    }
}
