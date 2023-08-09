using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class NPCBrain : MonoBehaviour
{
    public float difficulty = 100;
    public VehicleMovement vm;
    public Material defaultMat;
    public GameObject vehicle;
    public GameObject bot;
    public GameObject target;
    public Color[] randomColors;
    public TrailRenderer[] trailRenderer;
    public GameObject pathFinder;
    public GameObject pathFinder_m;
    public List<GameObject> NavPoints;

    Quaternion previousRotation;

    public float sightDistance;
    public float FOVDistance;

    bool obsticalF = false;
    bool obsticalL = false;
    bool obsticalR = false;
    bool canDriftStop = false;
    bool canDrift = true;
    bool newTarget = true;
    bool overridetarget = false;
    int wallHits = 0;
    float turnStart;

    int[] addWeight = new int[] { 0, 0, 0 };
    bool searchBox = true;
    int targetLevel = 0;

    //RacerIcon
    public GameObject racerIcon;

    //Save Data
    public int NPCID;
    public string NPCname;
    public int currentPoints;

    // Start is called before the first frame update
    void Start()
    {
        //Pathfinder Spawn
        Vector3 spawnPosition = transform.position + transform.forward * -50.0f;
        pathFinder_m = Instantiate(pathFinder, spawnPosition, Quaternion.identity);
        target = pathFinder_m.GetComponent<Navigation>().m_NavFollow;
        pathFinder_m.GetComponent<Navigation>().parent = gameObject;
    }

    public void GetStats()
    {
        randomColors = GameBrain.generatedColors[NPCID];
        difficulty = GameBrain.generatedDifficulty[NPCID];
        NPCname = GameBrain.generatedNames[NPCID];
        currentPoints = GameBrain.Points[NPCID];
        ColorRandomizer();
        DifficultyStats();
    }

    public void DifficultyStats()
    {
        //difficulty = Random.Range(1, 100);
        vm.maxSpeed = 90f + difficulty/1.3f;
        vm.accelerationSpeed = 90f + difficulty / 1.3f;
        pathFinder_m.GetComponent<Navigation>().NavOffset = 10f - difficulty / 5f;
        vm.turnSpeed = 40f + difficulty / 5f;
    }

    private void FixedUpdate()
    {
        AwarenessChecker();
        TargetChecker();
        Think();
        pathFinder_m.GetComponent<Navigation>().movementSpeed = vm.curSpeed+30;
        if ((pathFinder_m.transform.position - transform.position).magnitude > 150.0f)
        {
            pathFinder_m.GetComponent<Navigation>().movementSpeed = 0;
        }
    }

    void Think()
    {
        //Attack
        if (vm.racerInfront && GetComponent<itemHandler>().itemType == 0 && vm.curPosition > 1 && (vm.racerInfront.transform.position - transform.position).magnitude < 30f && targetLevel == GetComponent<itemHandler>().itemLevel)
        {
            newTarget = false;
            target = vm.racerInfront;
            Invoke("UseItem", Random.Range(2, 5));
        }
        //Use Item
        else if (GetComponent<itemHandler>().itemType > 0 && targetLevel == GetComponent<itemHandler>().itemLevel)
        {
            int aggression = Random.Range(1, 200);

            if (aggression < difficulty)//Use Item
            {
                Invoke("UseItem", Random.Range(2, 10));
            }
        }

        //Box Search
        if (searchBox == true && newTarget == true)
        {
            addWeight = new int[] { 0, 0, 0 }; //Clear weight values
            int itemLevel = GetComponent<itemHandler>().itemLevel;
            int itemType = GetComponent<itemHandler>().itemType;

            if (itemType >= 0)
            {
                addWeight[itemType] = itemLevel + 1; //Add an additional value so that the max is 3 and min is 0
            }
            float attackBox = 99;
            float defenseBox = 99;
            float supportBox = 99;
            if (vm.curPosition > 1) //If in first, don't worry about getting items
            {
                attackBox = Random.Range(1, 5 - addWeight[0]);
            }
            supportBox = Random.Range(1, 5 + vm.curPosition - addWeight[1]); //Lower priority since defense is much better
            defenseBox = Random.Range(1, 5 - addWeight[2]); //Highest Priority

            int Pref = -1;
            if (attackBox < defenseBox && attackBox < supportBox)
            {
                Pref = 0;
            }
            else if (defenseBox < attackBox && defenseBox < supportBox)
            {
                Pref = 1;
            }
            else
            {
                Pref = 2;
            }

            //Now that it has a preference check for any nearby boxes
            itemBox[] itemBoxes = FindObjectsOfType<itemBox>();
            itemBox[] filteredBoxes = itemBoxes
                .Where(box => Vector3.Distance(box.transform.position, gameObject.transform.position) <= 1000)
                .Where(box => box.type == Pref)
                .Where(box => IsInFront(box.transform.position, gameObject.transform, 10))
                .Where(box => box.grabbable == true)
                .ToArray();


            foreach (itemBox box in filteredBoxes)
            {
                Debug.Log(box.name);
            }

            if (newTarget == true && filteredBoxes.Count() > 0)
            {
                newTarget = false;
                target = filteredBoxes[Random.Range(0, filteredBoxes.Length)].gameObject;
            }
        }
        if (target.GetComponent<itemBox>() && target.GetComponent<itemBox>().grabbable == false || !IsInFront(target.transform.position, gameObject.transform,30))
        {
            Invoke("SearchItem", 10.0f);
            searchBox = false;
            newTarget = true;
            target = pathFinder_m.GetComponent<Navigation>().m_NavFollow; ;
        }
    }

    void UseItem()
    {
        newTarget = true;
        GetComponent<itemHandler>().UseItem();
        targetLevel = Random.Range(0, 2);
    }

    bool IsInFront(Vector3 boxPosition, Transform referenceObject, float angleSize)
    {
        // Calculate the direction from the referenceObject to the boxPosition
        Vector3 directionToBox = (boxPosition - referenceObject.position).normalized;

        // Calculate the forward direction of the referenceObject
        Vector3 forwardDirection = -1*referenceObject.forward;

        // Calculate the angle between the two directions
        float angle = Vector3.Angle(forwardDirection, directionToBox);

        //Finally check if target is not behind a wall
        Ray Ray = new Ray(transform.position, boxPosition);
        RaycastHit hit;

        bool isBehindWall = false;
        if (Physics.Raycast(Ray, out hit, 500) && (hit.collider != null && (hit.collider.CompareTag("World")))) // Ground Checker
        {
            isBehindWall = true;
        }

        // If the angle is within the threshold, the box is in front
        return angle <= angleSize && !isBehindWall;
    }

    void SearchItem()
    {
        searchBox = true;
    }
    void TargetChecker()
    {
        //Accelerate
        if ((target.transform.position - transform.position).magnitude > 15.0f && !obsticalF)
        {
            vm.Accelerate(1);
        }

        //Turn
        // Get the direction from referenceObject to targetObject
        Vector3 directionToTarget = target.transform.position - transform.position;

        // Get dot product
        float dotProduct = Vector3.Dot(transform.right, directionToTarget);

        if (dotProduct > 1f && overridetarget == false)
        {
            if (turnStart == 0)
            {
                turnStart = Time.time;
            }
            vm.TurnDirection(-1); // Turn left
            if (dotProduct < 20f || wallHits < 10)
            //if ((Time.time - turnStart) < 0.1f)
            {
                if (vm.isDrifting && canDriftStop)
                {
                    vm.EndDrift();
                    vm.isDrifting = false;
                }
            }
            else if (canDrift && target == pathFinder_m.GetComponent<Navigation>().m_NavFollow)
            {
                vm.StartDrift(-1);
                canDriftStop = false;
                Invoke("StopDrift", 0.3f);
            }
        }
        else if (dotProduct < -1f && overridetarget == false)
        {
            if (turnStart == 0)
            {
                turnStart = Time.time;
            }
            vm.TurnDirection(1); // Turn right
            if (dotProduct > -20f || wallHits < 10)
            //if ((Time.time - turnStart) < 0.1f)
            {
                if (vm.isDrifting && canDriftStop)
                {
                    vm.isDrifting = false;
                    vm.EndDrift();
                }
            }
            else if (canDrift && target == pathFinder_m.GetComponent<Navigation>().m_NavFollow)
            {
                vm.StartDrift(1);
                canDriftStop = false;
                Invoke("StopDrift", 0.3f);
            }
        }
        else
        {
            turnStart = 0;
        }

        if (overridetarget == true)
        {
            if (obsticalF)
            {
                vm.Accelerate(-1);
            }
            if (obsticalL)
            {
                vm.TurnDirection(1); // Turn left
            }
            else if (obsticalR)
            {
                vm.TurnDirection(-1f); // Turn Right
            }
            else
            {
                vm.TurnDirection(-1f); // Turn Right
            }
        }
    }

    void StopDrift()
    {
        canDriftStop = true;
        canDrift = false;
        Invoke("AllowDrift", 1.0f);
    }

    void AllowDrift()
    {
        canDrift = true;
    }

    void AwarenessChecker()
    {
        float awareness = 15.0f;

        // Front
        Ray frontRay = new Ray(transform.position, transform.TransformDirection(Quaternion.Euler(0, Random.Range(-50, 50), 0) * Vector3.back));
        RaycastHit fronthit;
        if (Physics.Raycast(frontRay, out fronthit, awareness) && (fronthit.collider != null && (fronthit.collider.CompareTag("World") || fronthit.collider.CompareTag("Vehicle")))) // Ground Checker
        {
            obsticalF = true;
        }
        else
        {
            obsticalF = false;
        }

        // Right
        Vector3 rightDirection = Quaternion.AngleAxis(Random.Range(-50, 50), transform.up) * transform.TransformDirection(Vector3.left);
        Ray rightRay = new Ray(transform.position, rightDirection);
        RaycastHit righthit;
        if (Physics.Raycast(rightRay, out righthit, awareness) && (righthit.collider != null && (righthit.collider.CompareTag("World") || righthit.collider.CompareTag("Vehicle")))) // Ground Checker
        {
            obsticalR = true;
        }
        else
        {
            obsticalR = false;
        }

        // Left
        Vector3 leftDirection = Quaternion.AngleAxis(Random.Range(-50, 50), transform.up) * transform.TransformDirection(Vector3.right);
        Ray leftRay = new Ray(transform.position, leftDirection);
        RaycastHit lefthit;
        if (Physics.Raycast(leftRay, out lefthit, awareness) && (lefthit.collider != null && (lefthit.collider.CompareTag("World") || lefthit.collider.CompareTag("Vehicle")))) // Ground Checker
        {
            obsticalL = true;
        }
        else
        {
            obsticalL = false;
        }

        if (obsticalF || obsticalL || obsticalR)
        {
            wallHits++;
            if (wallHits < 10)
            {
                overridetarget = true;
            }
            else
            {
                Invoke("Unstuck",3.0f);
                overridetarget = false;
            }
        }
        else
        {
            overridetarget = false;
        }

        Debug.DrawRay(frontRay.origin, frontRay.direction * awareness, obsticalF ? Color.red : Color.green);
        Debug.DrawRay(leftRay.origin, leftRay.direction * awareness, obsticalL ? Color.red : Color.green);
        Debug.DrawRay(rightRay.origin, rightRay.direction * awareness, obsticalR ? Color.red : Color.green);
    }

    void Unstuck()
    {
        wallHits = 0;
    }

    public void ColorRandomizer()
    {
        //Vehicle Changer
        Material[] materials = vehicle.GetComponent<MeshRenderer>().sharedMaterials;
        materials[0] = new Material(defaultMat);
        materials[0].color = randomColors[0]; //Mat1

        materials[5] = new Material(defaultMat);
        materials[5].color = randomColors[1]; //Mat2

        materials[2] = new Material(defaultMat);
        materials[2].color = randomColors[2]; //Mat4

        vehicle.GetComponent<MeshRenderer>().sharedMaterials = materials;

        //NPC Changer
        MeshRenderer[] meshRenderers = bot.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            materials = meshRenderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                Debug.Log(materials[i]);
                if (materials[i].name == "Material.001 (Instance)")
                {
                    materials[i] = new Material(defaultMat);
                    materials[i].color = randomColors[0]; // Mat1
                }
                else if (materials[i].name == "Material.002 (Instance)")
                {
                    materials[i] = new Material(defaultMat);
                    materials[i].color = randomColors[1]; // Mat2
                }
                else if (materials[i].name == "Material.004 (Instance)")
                {
                    materials[i] = new Material(defaultMat);
                    materials[i].color = randomColors[2]; // Mat4
                }
            }

            meshRenderer.materials = materials;

            //Change Trail
            foreach (TrailRenderer trail in GetComponentsInChildren<TrailRenderer>())
            {
                // Get the material used by the TrailRenderer
                Material trailMaterial = trail.material;

                // Set the new color property
                trailMaterial.SetColor("_StartColor", randomColors[1]);
                trailMaterial.SetColor("_EndColor", randomColors[1]);
            }
        }

        //HUD Creator
        racerIcon = Instantiate(racerIcon, GameObject.Find("HUD").transform);
        racerIcon.GetComponent<racerHUDTarget>().target = gameObject;
        racerIcon.GetComponent<racerHUDTarget>().offsetX = 6889;
        racerIcon.GetComponent<racerHUDTarget>().offsetY = 2199;
        racerIcon.GetComponent<racerHUDTarget>().percentage = 0.26f;
        racerIcon.GetComponent<racerHUDTarget>().color1 = randomColors[0];
        racerIcon.GetComponent<racerHUDTarget>().color3 = randomColors[1];
        racerIcon.GetComponent<racerHUDTarget>().color2 = randomColors[2];
    }
}
