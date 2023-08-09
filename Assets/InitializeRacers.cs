using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeRacers : MonoBehaviour
{
    void Start()
    {
        Invoke("SetRacerStats", 0.2f);
    }

    void SetRacerStats()
    {
        GameObject[] vehicles = GameObject.FindGameObjectsWithTag("Vehicle");
        int currentPos = 0;
        foreach (GameObject vehicle in vehicles)
        {
            if (vehicle.GetComponent<NPCBrain>())
            {
                //vehicle.GetComponent<NPCBrain>().NPCID = currentPos;
                vehicle.GetComponent<NPCBrain>().GetStats();
                currentPos++;
            }
        }
    }
}
