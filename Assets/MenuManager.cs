using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    bool Settings = false;
    bool Cup = false;
    bool Logo = false;
    public void Start()
    {
        MoveMain();
    }
    public void MoveSettings()
    {
        if (Settings)
            return;
        Settings = true;
        StartCoroutine(MoveObject(GameObject.Find("Offscreen").transform.position, GameObject.Find("CupSelectorPos").transform.position, 0.5f, GameObject.Find("SettingsMenu")));
        if (Logo)
            ReturnMain();
        if (Cup)
            ReturnCup();
    }
    void ReturnSettings()
    {
        Settings = false;
        StartCoroutine(MoveObject(GameObject.Find("CupSelectorPos").transform.position, GameObject.Find("Offscreen").transform.position, 0.5f, GameObject.Find("SettingsMenu")));
    }
    public void MoveMain()
    {
        Logo = true;
        StartCoroutine(MoveObject(GameObject.Find("Offscreen").transform.position, GameObject.Find("CupSelectorPos").transform.position, 0.5f, GameObject.Find("StartMenu")));
    }
    void ReturnMain()
    {
        Logo = false;
        StartCoroutine(MoveObject(GameObject.Find("CupSelectorPos").transform.position, GameObject.Find("Offscreen").transform.position, 0.5f, GameObject.Find("StartMenu")));
    }
    public void MoveCup()
    {
        if (Cup)
            return;
        Cup = true;
        StartCoroutine(MoveObject(GameObject.Find("Offscreen").transform.position, GameObject.Find("CupSelectorPos").transform.position, 0.5f, GameObject.Find("CupSelector")));
        if (Logo)
            ReturnMain();
        if (Settings)
            ReturnSettings();
    }
    void ReturnCup()
    {
        Cup = false;
        StartCoroutine(MoveObject(GameObject.Find("CupSelectorPos").transform.position, GameObject.Find("Offscreen").transform.position, 0.5f, GameObject.Find("CupSelector")));
    }
    IEnumerator MoveObject(Vector2 pos1, Vector2 pos2, float time, GameObject objectRef)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < time)
        {
            // Calculate the normalized time (a value between 0 and 1) based on the elapsed time and duration
            float t = elapsedTime / time;

            // Use Lerp to interpolate the position between the start and end points
            objectRef.transform.position = Vector2.Lerp(pos1, pos2, t);

            // Increment the elapsed time based on the time since the last frame
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the object reaches the exact end position
        objectRef.transform.position = pos2;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
