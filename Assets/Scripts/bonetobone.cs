using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bonetobone : MonoBehaviour
{
    public GameObject bone1;
    public GameObject bone2;
    void Update()
    {
        bone1.transform.position = bone2.transform.position;
        bone1.transform.rotation = bone2.transform.rotation;
    }
}
