using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public Animator botModel;
    public Animator vehicleModel;
    public float turnDirection = 0;
    // Start is called before the first frame update
    // Update is called once per frame

    public void TurnPos(float amount)
    {
        float result = (amount + 0.5f);
        botModel.SetFloat("TurnPos", result);
    }

    public void CallAnim(string anim)
    {
        if (anim == "Hurt")
        {
            StartCoroutine(PlayHurt());
        }
        else if (anim == "Boost")
        {
            StartCoroutine(PlayBoost());
        }
        else if (anim == "Taunt")
        {
            StartCoroutine(PlayTaunt());
        }
        else if (anim == "Win")
        {
            PlayWin();
        }
        else if (anim == "Lose")
        {
            PlayLose();
        }
    }

    IEnumerator PlayTaunt()
    {
        botModel.SetBool("CanTaunt?", true);
        yield return new WaitForSeconds(0.3f);
        botModel.SetBool("CanTaunt?", false);
    }

    IEnumerator PlayHurt()
    {
        botModel.SetBool("IsHurt?", true);
        vehicleModel.SetBool("IsHurt?", true);
        yield return new WaitForSeconds(0.3f);
        botModel.SetBool("IsHurt?", false);
        vehicleModel.SetBool("IsHurt?", false);
    }

    IEnumerator PlayBoost()
    {
        botModel.SetBool("IsBoosting?", true);
        vehicleModel.SetBool("IsBoosting?", true);
        yield return new WaitForSeconds(0.3f);
        botModel.SetBool("IsBoosting?", false);
        vehicleModel.SetBool("IsBoosting?", false);
    }

    void PlayLose()
    {
        vehicleModel.SetBool("RaceOver?", true);
        vehicleModel.SetBool("IsMad?", true);
    }

    void PlayWin()
    {
        vehicleModel.SetBool("RaceOver?", true);
        vehicleModel.SetBool("IsMad?", false);
    }
}
