using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavFollow : MonoBehaviour
{
    NavMeshAgent agent;
    public GameObject target;
    public float maxSpeed;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        agent.SetDestination(target.transform.position);
        agent.speed = maxSpeed;
    }
}
