using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBrain : MonoBehaviour
{
    //Racer Variables
    public static List<Color[]> generatedColors;
    public static List<int> generatedDifficulty;
    public static List<string> generatedNames;
    public static List<int> Points;
    public static List<string> chosenRaceNames;
    public static int playerPoints;
    public static int currentRace = 0;
    public GameObject refNPC;
    //Generate Racers and Move to a Cup
    public void StartRace()
    {
        generatedColors = new List<Color[]>();
        generatedDifficulty = new List<int>();
        generatedNames = new List<string>();
        Points = new List<int>();

        for (int i = 0; i < 8; i++)
        {
            //RandomColors
            Color[] randomColors = { Color.black, Color.black, Color.black };
            float red = Random.Range(0.2f, 0.8f);
            float blue = Random.Range(0.2f, 0.8f);
            float green = Random.Range(0.2f, 0.8f);
            randomColors[0] = new Color(red, blue, green);
            randomColors[1] = new Color(red * 1.3f, blue * 1.3f, green * 1.3f);
            int specialColor = Random.Range(1, 3);
            if (specialColor == 2)
            {
                randomColors[2] = new Color(green * 0.5f, red * 0.5f, blue * 0.5f);
            }
            else
            {
                randomColors[2] = new Color(red * 0.5f, blue * 0.5f, green * 0.5f);
            }
            generatedColors.Add(randomColors);

            //Generate difficulty
            int randomDifficulty = 60 + Random.Range(0, 40);
            generatedDifficulty.Add(randomDifficulty);

            //Generate Names
            string[] randomNames1 = new string[]{"Steel","Stratched","Shiny","Polished","Bot","Bolted","Winged","Star","Overcharged","Unstable","Captain"};
            string[] randomNames2 = new string[]{"Undertaker","Warrior","Nuke","Pilot","Champion","Alpha","Beta","Bot","Commander","Raven"};
            string randomName = randomNames1[Random.Range(0,randomNames1.Length)] + " " + randomNames2[Random.Range(0, randomNames2.Length)];
            generatedNames.Add(randomName);

            //Set Points to zero
            Points.Add(0);
        }
        LoadNextScene();
    }

    public static void LoadNextScene()
    {
        currentRace++;
        if (currentRace < chosenRaceNames.Count)
        {
            SceneManager.LoadScene(chosenRaceNames[currentRace]);
        }
        else
        {
            SceneManager.LoadScene("EndScreen");
        }
    }
}