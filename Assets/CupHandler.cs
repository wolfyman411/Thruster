using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupHandler : MonoBehaviour
{
    public List<string> cupNames;
    public void SetCup()
    {
        GameBrain.chosenRaceNames = cupNames;
        GameBrain.currentRace = 0;
        GameObject.Find("GameBrain").GetComponent<GameBrain>().StartRace();
        //GameBrain.LoadNextScene();
    }
}
