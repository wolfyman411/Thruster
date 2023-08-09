using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorIcon : MonoBehaviour
{
    public Color color1;
    public Color color2;
    public Color color3;
    public Color[] defaultColors;
    public GameObject colorRef;
    public int posID;

    public Image Image1;
    public Image Image2;
    public Image Image3;

    public Text Name;
    public Text Score;

    void Update()
    {
        colorRef = GameObject.Find("GameManager").GetComponent<CheckPoints>().Places[posID]; //Get Position
        if (colorRef != null && GameObject.Find("GameManager").GetComponent<CheckPoints>().endGame == false)
        {
            //Get Color
            if (colorRef.GetComponent<NPCBrain>() != null) //If not player get NPC colors
            {
                color1 = colorRef.GetComponent<NPCBrain>().randomColors[0];
                color2 = colorRef.GetComponent<NPCBrain>().randomColors[1];
                color3 = colorRef.GetComponent<NPCBrain>().randomColors[2];
            }
            else //Player colors
            {
                color1 = defaultColors[0];
                color2 = defaultColors[1];
                color3 = defaultColors[2];
            } //Set Colors
            Image1.color = color1;
            Image2.color = color2;
            Image3.color = color3;

            //Get Name
            if (colorRef.GetComponent<NPCBrain>() != null)
            {
                Name.text = colorRef.GetComponent<NPCBrain>().NPCname;
            }
            else
            {
                Name.text = "Steele Steele";
            }

            //Get Points
            if (colorRef.GetComponent<NPCBrain>() != null)
            {
                Score.text = (colorRef.GetComponent<NPCBrain>().currentPoints).ToString()+" + "+ (colorRef.GetComponent<VehicleMovement>().curPosition).ToString();
            }
            else
            {
                Score.text = (GameBrain.playerPoints).ToString() + " + " + (colorRef.GetComponent<VehicleMovement>().curPosition).ToString();
            }
        }
    }
}
