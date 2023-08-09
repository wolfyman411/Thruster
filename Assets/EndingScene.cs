using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingScene : MonoBehaviour
{
    public RawImage backgroundRef;
    public RawImage foregroundRef;

    public Texture[] lose;
    public Texture[] win;
    // Start is called before the first frame update

    private void Start()
    {
        //Check for player win
        int lowestNPC = 999;
        for (int i = 0; i < GameBrain.Points.Count; i++)
        {
            if (GameBrain.Points[i] < lowestNPC && GameBrain.Points[i] != 0)
            {
                lowestNPC = GameBrain.Points[i];
            }
        }
        if (GameBrain.playerPoints < lowestNPC)
        {
            WinGame();
        }
        else
        {
            LoseGame();
        }
        Debug.Log(lowestNPC);
    }

    public void LoseGame()
    {
        backgroundRef.texture = lose[0];
        foregroundRef.texture = lose[1];
    }
    public void WinGame()
    {
        backgroundRef.texture = win[0];
        foregroundRef.texture = win[1];
    }

    private void Update()
    {
        
    }
}
