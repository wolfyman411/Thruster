using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetAxis("Quit") > 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
