using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class VehicleMovement : MonoBehaviour
{
    Rigidbody rb;
    public float turnSpeed;
    public float accelerationSpeed;
    public float maxSpeed;
    public float bounceForce;

    float maxSpeedTemp;
    public float deceleration;
    public float curSpeed;
    float turnSpeedTemp;

    public float bonusSpeed;

    float dragTemp;

    //Drifting
    public bool isDrifting = false;
    public float driftForce = 10f;
    public float driftTorque = 5f;
    public float driftPower;
    public float driftStart;
    float initialDriftDirection;
    public float[] driftTimes;

    //Rotation
    Quaternion initialRotation;
    public float rotationSpeed;
    public float maxTiltAngle;
    float tiltAngleTemp;
    public bool isTurning = false;

    //Rotate
    public GameObject shipModel;

    //Racing
    public List<GameObject> collectedCheckPoints;
    public GameObject GameManager;
    public int CheckpointVal = 0;
    public int curPosition;
    public GameObject racerInfront;
    public int lap;
    float AccelerationStart = 999;

    //Health
    public float HP = 100;
    public bool isInvincible = false;
    public bool isDead = false;
    public bool isShielded = false;
    public GameObject shield;
    public GameObject gibs;
    public GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager");
        rb = GetComponent<Rigidbody>();
        maxSpeedTemp = maxSpeed;
        initialRotation = shipModel.transform.rotation;

        tiltAngleTemp = maxTiltAngle;

        turnSpeedTemp = turnSpeed;
        Invoke("BonusDecay", 0.1f);
        InvokeRepeating("UpdateDrag", 0.1f, 0.1f);

        dragTemp = rb.drag;
    }

    // Update is called once per frame
    void Update()
    {
        Hover();
        DriftChecker();

        //Speed Checker
        curSpeed = rb.velocity.magnitude;
        if (curSpeed > maxSpeed)
        {
            curSpeed = maxSpeed;
            Vector3 newVelocity = rb.velocity.normalized * curSpeed;
            rb.velocity = newVelocity;
        }
    }

    void FixedUpdate() //Drift System
    {
        if (isDrifting)
        {
            float sidewaysForce = initialDriftDirection * driftForce;
            float turnTorque = initialDriftDirection * driftTorque;

            rb.AddForce(transform.right * sidewaysForce, ForceMode.Acceleration);
            rb.AddTorque(transform.up * turnTorque, ForceMode.Acceleration);
        }
    }

    void UpdateDrag()
    {
        //Drag Calculation
        if (!isDrifting && !isTurning)
        {
            rb.drag = Mathf.Abs(1 * Mathf.Pow(.985f, curSpeed));
        }
    }

    void BonusDecay()
    {
        if (bonusSpeed > 1 && !isDrifting)
        {
            rb.AddForce(-(transform.forward) * bonusSpeed, ForceMode.Impulse);
            bonusSpeed -= 0.5f;
            GetComponent<AnimationHandler>().CallAnim("Boost"); //Play Boost Anim
            //GetComponent<SoundHandler>().PlaySFX("boost");
        }
        else
        {
            bonusSpeed = 0;
        }
        Invoke("BonusDecay", 0.1f);
    }

    void Hover()
    {
        Ray ray = new Ray(transform.position, Vector3.down); //Draws a ray

        float maxDistance = Mathf.Infinity;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance) && hit.collider.CompareTag("World")) //Ground Checker
        {
            float distanceToGround = hit.distance;
            if (distanceToGround < 6.0f)
            {
                rb.AddForce(0, 40, 0); //Add upwards force
            }
        }
    }

    public void StartDrift(float amount)
    {
        if (!GameManager.GetComponent<HUD>().raceStarted) //If race not started, don't allow
        {
            return;
        }
        if (amount > 0.5 || amount < -0.5)
        {
            if (!isDrifting)
            {
                rb.drag = dragTemp;
                driftStart = Time.time;
                isDrifting = true;
                initialDriftDirection = amount;
            }
        }
    }

    public void EndDrift()
    {
        if (Time.time - driftStart > driftTimes[0] && Time.time - driftStart < driftTimes[1])
        {
            bonusSpeed = 10;
        }
        else if (Time.time - driftStart > driftTimes[1])
        {
            bonusSpeed = 20;
        }
        driftStart = 0;
    }

    void DriftChecker()
    {
        if (isDrifting)
        {
            rb.angularDrag = 1f;
            maxTiltAngle = tiltAngleTemp*1.1f;
            turnSpeed = turnSpeedTemp * 1.1f;
        }
        else
        {
            rb.angularDrag = 0.5f;
            rb.angularVelocity = Vector3.zero;
            maxTiltAngle = tiltAngleTemp;
            turnSpeed = turnSpeedTemp;
        }
    }

    public void TurnDirection(float direction)
    {
        if (!GameManager.GetComponent<HUD>().raceStarted) //If race not started, don't allow
        {
            return;
        }

        //Animation
        GetComponent<AnimationHandler>().TurnPos(direction);

        if (direction > 0.5f || direction < -0.5f)
        {
            //if (!isDrifting)
            {
                if (!isDrifting)
                {
                    rb.drag = dragTemp * 0.6f;
                }
                float rotationAmount = direction * turnSpeed;
                transform.Rotate(Vector3.up, rotationAmount * Time.deltaTime); //Turn Vehicle

                RotateVehicle(direction, true);
            }
        }
        else
        {
            maxSpeed = maxSpeedTemp+bonusSpeed;
            UpdateDrag();
        }
    }

    public void Accelerate(float amount)
    {
        if ((AccelerationStart == 999 || GetComponent<NPCBrain>() && GetComponent<NPCBrain>().difficulty > 50) && !GameManager.GetComponent<HUD>().raceStarted)
        {
            AccelerationStart = Time.time;
        }
        else if (Mathf.Abs(GameManager.GetComponent<HUD>().startTime - AccelerationStart) < 0.5f)
        {
            Debug.Log("Gas:" + AccelerationStart);
            Debug.Log("Start:" + GameManager.GetComponent<HUD>().startTime);
            Debug.Log("Diff:" + Mathf.Abs(GameManager.GetComponent<HUD>().startTime - AccelerationStart));
            bonusSpeed = 30f;
            AccelerationStart = 999;
        }
        if (!GameManager.GetComponent<HUD>().raceStarted) //If race not started, don't allow
        {
            return;
        }

        if (isDead)
        {
            return;
        }
        if (amount > 0.5f || amount < -0.5f)
        {
            amount *= -1;
            Vector3 force = transform.forward * amount * accelerationSpeed; //Accelerate
            force = Vector3.ClampMagnitude(force, maxSpeed);
            rb.AddForce(force);

            RotateVehicle(amount,false);
        }
    }

    public void FixRotation()
    {
        RotateVehicle(0, false);
    }

    void RotateVehicle(float amount, bool side)
    {
        if (isDead)
        {
            return;
        }
        // Rotation
        float targetTiltAngle = amount * maxTiltAngle;
        Quaternion targetRotation;

        if (amount > 0.5f || amount < -0.5f)
        {
            if (side)
            {
                targetRotation = Quaternion.Euler(targetTiltAngle * -1, targetTiltAngle * -1, targetTiltAngle);
            }
            else
            {
                targetRotation = Quaternion.Euler(targetTiltAngle, 0f, 0f);
            }
        }
        else
        {
            targetRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        Quaternion initialOffset = Quaternion.Euler(-90f, 0f, 0f);
        Quaternion newRotation = Quaternion.LookRotation(transform.forward, Vector3.up) * targetRotation * initialOffset;

        shipModel.transform.rotation = Quaternion.Lerp(shipModel.transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
    }

    //Bounce Penalty
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle") || collision.gameObject.CompareTag("World"))
        {
            //Check Speed and Deal Damage
            if (curSpeed > 70)
            {
                DealDamage(curSpeed/10);
            }
            // Calculate the collision normal vector
            Vector3 collisionNormal = collision.contacts[0].normal;

            // Bounce and remove some speed
            float bounceForceMagnitude = curSpeed * 0.7f;

            // Apply the bounce force
            rb.AddForce(collisionNormal * bounceForceMagnitude, ForceMode.Impulse);

            bonusSpeed = 0;

            GetComponent<SoundHandler>().PlaySFX("bonk");
        }
    }

    //Damage Handler
    public void DealDamage(float amount)
    {
        if (GameManager.GetComponent<CheckPoints>().endGame == true)
        {
            return;
        }
        if (isInvincible == false && HP > 0)
        {
            if (isShielded == false)
            {
                HP -= amount;
            }
            else
            {
                Destroy(shield);
                isShielded = false;
            }
        }

        if (HP <= 0 && isDead == false)
        {
            Explode();
        }
        else if (HP > 0)
        {
            GetComponent<AnimationHandler>().CallAnim("Hurt"); //Play Hurt Anim
            GetComponent<SoundHandler>().PlayAudio("hurt");
        }
    }
    public void Explode()
    {
        GetComponent<SoundHandler>().PlaySFX("boom");
        //Explode
        GameObject particle_m = Instantiate(explosion);
        GetComponent<SphereCollider>().enabled = false; //Disable Collision
        particle_m.transform.position = gameObject.transform.position;
        isDead = true;

        //Gib
        GameObject m_gib = Instantiate(gibs);
        m_gib.transform.rotation = shipModel.transform.rotation;
        m_gib.transform.position = shipModel.transform.position;

        //Hide Ship and stop
        gameObject.transform.localScale = Vector3.zero;
        maxSpeed = 0;

        //Launch Gibs
        Rigidbody[] rigidbodies = m_gib.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigidbodies)
        {
            Vector3 randomDirection = Random.insideUnitSphere;
            rb.AddForce(randomDirection * 0.00001f, ForceMode.Impulse);
        }
        Invoke("Respawn", 5);

        //Play Announcer
        if (!GetComponent<NPCBrain>())
        {
            GameManager.GetComponent<AnnouncerSystem>().PlaySound("Dis",0);
        }
    }

    void Respawn()
    {
        GetComponent<SphereCollider>().enabled = true; //Enable Collision
        if (collectedCheckPoints.Count > 0)
        {
            transform.position = collectedCheckPoints[collectedCheckPoints.Count - 1].transform.position;
            Transform targetPointer = collectedCheckPoints[collectedCheckPoints.Count - 1].transform.Find("Pointer");//Look Straight
            Vector3 lookDirection = transform.position - targetPointer.position;
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
        else
        {
            transform.position = GameManager.GetComponent<CheckPoints>().CPS[0].transform.position;
            Transform targetPointer = GameManager.GetComponent<CheckPoints>().CPS[0].transform.Find("Pointer");//Look Straight
            Vector3 lookDirection = transform.position - targetPointer.position;
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }

        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.rotation = new Quaternion(0f, gameObject.transform.rotation.y, 0f, 0f);

        isDead = false;
        HP = 100;
        maxSpeed = maxSpeedTemp;

        if (GetComponent<NPCBrain>()) //Check if NPC
        {
            GetComponent<NPCBrain>().pathFinder_m.GetComponent<Navigation>().collectedCheckPoints = GetComponent<NPCBrain>().NavPoints; //Remember collected Nav Points
            GetComponent<NPCBrain>().pathFinder_m.GetComponent<Navigation>().m_NavFollow.transform.position = gameObject.transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            if (!collectedCheckPoints.Contains(other.gameObject))
            {
                CheckpointVal++;
                collectedCheckPoints.Add(other.gameObject);
            }

            if (CheckpointVal == GameManager.GetComponent<CheckPoints>().CPS.Length && collectedCheckPoints[0] == other.gameObject && GetComponent<NPCBrain>()) // Check if NPC
            {
                GetComponent<NPCBrain>().NavPoints = GetComponent<NPCBrain>().pathFinder_m.GetComponent<Navigation>().collectedCheckPoints; // Remember collected Nav Points
                CheckpointVal = 0;
                collectedCheckPoints.Clear();
                lap++;
            }
            else if (CheckpointVal == GameManager.GetComponent<CheckPoints>().CPS.Length && collectedCheckPoints[0] == other.gameObject) // Check if hit all checkpoints
            {
                CheckpointVal = 0;
                collectedCheckPoints.Clear();
                lap++;
                GameObject.Find("Splits").GetComponent<Text>().text = string.Format("{0:00}:{1:00}:{2:00}", GameManager.GetComponent<HUD>().s_minutes, GameManager.GetComponent<HUD>().s_seconds, GameManager.GetComponent<HUD>().s_miliseconds);
                GameManager.GetComponent<HUD>().s_miliseconds = 0;
                GameManager.GetComponent<HUD>().s_seconds = 0;
                GameManager.GetComponent<HUD>().s_minutes = 0;
                if (lap > 2) //End Game Logic
                {
                    GameManager.GetComponent<CheckPoints>().EndRace();
                    if (curPosition > 1)
                    {
                        GetComponent<SoundHandler>().PlayAudio("lose");
                    }
                    else
                    {
                        GetComponent<SoundHandler>().PlayAudio("win");
                    }
                }
            }
        }
    }
}
