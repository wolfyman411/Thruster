using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Navigation : MonoBehaviour
{
    GameObject GameManager;
    int CheckpointPos = 0;
    public List<GameObject> collectedCheckPoints;
    GameObject[] points;
    Vector3 initialPos;
    Vector3 targetPos;
    public float movementSpeed = 5.0f;
    public float NavOffset;
    public GameObject parent;
    public GameObject m_NavFollow;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager");
        if (GameManager)
        {
            points = GameManager.GetComponent<CheckPoints>().NPCPATH;
            m_NavFollow = Instantiate(m_NavFollow, gameObject.transform.position, Quaternion.identity);
            m_NavFollow.GetComponent<NavFollow>().target = gameObject;

            GotoNextPoint();
        }
    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set agent destination
        initialPos = gameObject.transform.position;
        targetPos = points[CheckpointPos].transform.position;

        float xOffset = Random.Range(-NavOffset, NavOffset);
        float zOffset = Random.Range(-NavOffset, NavOffset);
        targetPos += new Vector3(xOffset, 0f, zOffset);

        // Get the next point
        CheckpointPos = (CheckpointPos + 1) % points.Length;
    }

    void Update()
    {
        //Lerp
        float step = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        // Choose the next destination point when the agent gets close to destination
        if ((gameObject.transform.position - targetPos).magnitude < 20.0f)
            GotoNextPoint();
    }
}