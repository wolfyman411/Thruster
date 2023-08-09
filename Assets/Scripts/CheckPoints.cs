using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    public GameObject[] CPS;
    public GameObject[] NPCPATH;
    public List<GameObject> Places;
    public bool endGame = false;

    private void Start()
    {
        Places.AddRange(GameObject.FindGameObjectsWithTag("Vehicle")); //Find all vehicles
        InvokeRepeating("PlaceChecker", 0.5f, 0.5f); //Update Placements
    }

    void PlaceChecker()
    {
        //List<GameObject> tempList = new List<GameObject>();
        if (endGame)
        {
            return;
        }
        for (int i = 0; i < Places.Count - 1; i++)
        {
            for (int j = 0; j < Places.Count - 1 - i; j++)
            {
                if (Places[j].GetComponent<VehicleMovement>().lap == Places[j + 1].GetComponent<VehicleMovement>().lap) //Check if same lap
                {
                    if (Places[j].GetComponent<VehicleMovement>().CheckpointVal > Places[j + 1].GetComponent<VehicleMovement>().CheckpointVal) //Check who has more checkpoints
                    {
                        GameObject temp = Places[j];
                        Places[j] = Places[j + 1];
                        Places[j + 1] = temp;
                    }
                    else if (Places[j].GetComponent<VehicleMovement>().CheckpointVal == Places[j + 1].GetComponent<VehicleMovement>().CheckpointVal) //Check who is closer to checkpoint 
                    {
                        float distance1 = 0;
                        float distance2 = 0;
                        if (Places[j].GetComponent<VehicleMovement>().CheckpointVal < CPS.Length)
                        {
                            distance1 = Vector3.Distance(Places[j].transform.position, CPS[Places[j].GetComponent<VehicleMovement>().CheckpointVal].transform.position);
                        }
                        if (Places[j + 1].GetComponent<VehicleMovement>().CheckpointVal < CPS.Length)
                        {
                            distance2 = Vector3.Distance(Places[j + 1].transform.position, CPS[Places[j + 1].GetComponent<VehicleMovement>().CheckpointVal].transform.position);
                        }
                        if (distance2 > distance1)
                        {
                            GameObject temp = Places[j];
                            Places[j] = Places[j + 1];
                            Places[j + 1] = temp;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < Places.Count; i++)
        {
            Places[i].GetComponent<VehicleMovement>().curPosition = Places.Count - i;

            // Check if there's a racer in front of the current racer
            if (i < Places.Count - 1)
            {
                // Set the racerInfront property of the current racer
                Places[i].GetComponent<VehicleMovement>().racerInfront = Places[i + 1];
            }
            else
            {
                // If the current racer is the last one in the list, there is no racer in front
                Places[i].GetComponent<VehicleMovement>().racerInfront = null;
            }
        }
    }
    [ContextMenu("ShowLeaderboard")]
    public void EndRace() //Ends the Game
    {
        endGame = true;
        GameObject.Find("Thruster").GetComponent<PlayerController>().enabled = false; //Turn off player driving
        GameObject[] vehicleMovements = GameObject.FindGameObjectsWithTag("Vehicle");

        foreach (GameObject vehicleMovement in vehicleMovements) //Turn off all driving
        {
            if (vehicleMovement.GetComponent<VehicleMovement>().curPosition > 1)
            {
                vehicleMovement.GetComponent<AnimationHandler>().CallAnim("Lose");
            }
            else
            {
                vehicleMovement.GetComponent<AnimationHandler>().CallAnim("Win");
            }
            vehicleMovement.GetComponent<VehicleMovement>().enabled = false;
        }

        //Get Points
        GameBrain.playerPoints += GameObject.Find("Thruster").GetComponent<VehicleMovement>().curPosition;
        foreach (GameObject vehicleMovement in vehicleMovements)
        {
            if (vehicleMovement.GetComponent<NPCBrain>())
            {
                Debug.Log(vehicleMovement.GetComponent<NPCBrain>().currentPoints);
                GameBrain.Points[vehicleMovement.GetComponent<NPCBrain>().NPCID] += vehicleMovement.GetComponent<VehicleMovement>().curPosition;
            }
        }


            GetComponent<HUD>().StartCoroutine(GetComponent<HUD>().MoveLeaderboard());
        Invoke("NextMap", 5.0f);
    }

    [ContextMenu("NextMap")]
    void NextMap()
    {
        GameBrain.LoadNextScene();
    }
}
