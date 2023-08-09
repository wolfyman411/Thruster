using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    public AudioClip[] hurt;
    public AudioClip[] lose;
    public AudioClip[] pass;
    public AudioClip[] taunt;
    public AudioClip[] win;
    public AudioClip[] miscSounds;
    public AudioSource voiceBox;
    public AudioSource sfxBox;
    public AudioSource engineBox;

    int PreviousPos;
    VehicleMovement vm;
    bool makeSound = false;
    // Start is called before the first frame update
    void Start()
    {
        vm = GetComponent<VehicleMovement>();
        PreviousPos = vm.curPosition;
        Invoke("PassChecker", Random.Range(1f, 3f));
        Invoke("CanMakeSound", Random.Range(4f, 10f));
    }

    private void Update() //Change engine volume based on speed
    {
        engineBox.volume = (vm.curSpeed / vm.maxSpeed)/2f;
    }

    void PassChecker()
    {
        if (PreviousPos < vm.curPosition)
        {
            PlayAudio("pass");
        }
        PreviousPos = vm.curPosition;
        Invoke("PassChecker", Random.Range(1f, 3f));
    }

    public void PlayAudio(string type)
    {
        if (!makeSound)
        {
            return;
        }
        AudioClip chosenAudio = taunt[0];
        if (type == "pass")
        {
            //int playSound = Random.Range(1, 3);
            //if (playSound == 3)
            {
                chosenAudio = pass[Random.Range(0, pass.Length)];
            }
        }
        else if (type == "hurt")
        {
            chosenAudio = hurt[Random.Range(0, hurt.Length)];
        }
        else if (type == "taunt")
        {
            chosenAudio = taunt[Random.Range(0, taunt.Length)];
        }
        else if (type == "win")
        {
            chosenAudio = win[Random.Range(0, win.Length)];
        }
        else if (type == "lose")
        {
            chosenAudio = lose[Random.Range(0, lose.Length)];
        }
        voiceBox.clip = chosenAudio;
        voiceBox.Play();
        makeSound = false;
        Invoke("CanMakeSound", Random.Range(4f, 10f));
    }

    public void PlaySFX(string type)
    {
        AudioClip chosenAudio = taunt[0];
        if (type == "bonk")
        {
            chosenAudio = miscSounds[0];
        }
        else if (type == "boom")
        {
            chosenAudio = miscSounds[1];
        }
        else if (type == "boost")
        {
            chosenAudio = miscSounds[2];
        }
        else if (type == "break")
        {
            chosenAudio = miscSounds[3];
        }
        else if (type == "laser")
        {
            chosenAudio = miscSounds[4];
        }
        sfxBox.clip = chosenAudio;
        sfxBox.Play();
    }

    void CanMakeSound()
    {
        makeSound = true;
    }
}
