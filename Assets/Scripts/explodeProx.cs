using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class explodeProx : MonoBehaviour
{
    float damage = 50f;
    public GameObject particle;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<VehicleMovement>())
        {
            GameObject particle_m = Instantiate(particle);
            particle_m.transform.position = gameObject.transform.position;
            other.GetComponentInParent<VehicleMovement>().DealDamage(damage);
            other.GetComponentInParent<VehicleMovement>().curSpeed = 0;

            if (other.GetComponentInParent<VehicleMovement>() != gameObject.transform.parent.gameObject.GetComponent<ItemBase>().vm) //If not parent then play taunt
            {
                //Debug.Log(GetComponentInParent<ItemBase>().vm);
                GetComponentInParent<ItemBase>().vm.GetComponent<SoundHandler>().PlayAudio("taunt"); //Play hit audio
                GetComponentInParent<ItemBase>().vm.GetComponent<AnimationHandler>().CallAnim("Taunt"); //Play taunt anim
            }

            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
