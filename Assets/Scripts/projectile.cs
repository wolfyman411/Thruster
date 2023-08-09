using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class projectile : MonoBehaviour
{
    public float speed = 200;
    public float damage = 75;
    public bool explosive = false;
    Rigidbody rb;
    public GameObject parent;
    public GameObject target;
    public GameObject particle;
    bool Active = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.transform.parent.GetComponent<Rigidbody>();
        Invoke("Activate", 0.5f);
    }

    void Activate()
    {
        Active = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            // Calculate the direction towards the target
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

            // Calculate the distance to the target
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            // If the distance is greater than a small threshold, move towards the target
            if (distanceToTarget > 0.1f)
            {
                // Calculate the new position to move towards the target
                Vector3 newPosition = transform.position + directionToTarget * (speed * 5) * Time.deltaTime;

                // Move towards the new position
                transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * 10f);
            }

            // Rotate towards the target
            Quaternion targetRotation = Quaternion.LookRotation(-directionToTarget, transform.up); // Invert the target direction
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100f * Time.deltaTime);
        }
        else
        {
            Vector3 force = transform.forward * -1 * speed; //Accelerate
            force = Vector3.ClampMagnitude(force, speed);
            rb.AddForce(force);
            Vector3 newVelocity = rb.velocity.normalized * speed;
            rb.velocity = newVelocity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("World"))
        {
            if (explosive)
            {
                GameObject particle_m = Instantiate(particle);
                particle_m.transform.position = gameObject.transform.position;
            }
            Destroy(gameObject.transform.parent.gameObject);
        }
        else if (other.CompareTag("Vehicle") && Active == true)
        {
            if (explosive)
            {
                GameObject particle_m = Instantiate(particle);
                particle_m.transform.position = gameObject.transform.position;
                other.GetComponentInParent<VehicleMovement>().curSpeed = 0;
            }
            other.GetComponentInParent<VehicleMovement>().DealDamage(damage);

            //Audio/Animation Handler

            if (other.GetComponent<NPCBrain>()) //Check if NPC
            {
                if (other.GetComponent<VehicleMovement>().isDead) //Check if Dead
                {
                    if (parent == GameObject.Find("Thruster")) //Check if Player
                    {
                        GameObject.Find("GameManager").GetComponent<AnnouncerSystem>().PlaySound("App", 0);
                    }
                }
            }
            else
            {
                transform.parent.gameObject.GetComponent<ItemBase>().vm.GetComponent<SoundHandler>().PlayAudio("taunt"); //Play hit audio
            }
            transform.parent.gameObject.GetComponent<ItemBase>().vm.GetComponent<AnimationHandler>().CallAnim("Taunt"); //Play taunt anim
            Destroy(gameObject.transform.parent.gameObject);
        }
    }

}
