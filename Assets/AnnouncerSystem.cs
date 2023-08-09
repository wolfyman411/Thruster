using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnouncerSystem : MonoBehaviour
{
    public AudioSource AnnouncerVO;
    public AudioClip[] Disappointed;
    public AudioClip[] Approval;
    public AudioClip[] Countdown;

    public void PlaySound(string soundName, int specific)
    {
        AudioClip clipRef = null;
        if (soundName == "Dis")
        {
            clipRef = Disappointed[Random.Range(0, Disappointed.Length)];
        }
        else if (soundName == "App")
        {
            clipRef = Approval[Random.Range(0, Approval.Length)];
        }
        else if (soundName == "Count")
        {
            clipRef = Countdown[specific];
        }

        //Check if audio valid
        if (clipRef != null)
        {
            AnnouncerVO.clip = clipRef;
            AnnouncerVO.Play();
        }
    }
}
