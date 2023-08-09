using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class itemBox : MonoBehaviour
{
    // Rotate Model
    public GameObject boxModel;
    public GameObject gibs;
    float rotationSpeed = 60f; // Higher means faster
    public Material[] newMats;
    public int type;
    public bool grabbable;
    float rechargeTime = 20.0f;

    // Update is called once per frame
    void Update()
    {
        // Rotate the object on all three axes
        boxModel.transform.Rotate(Vector3.right * (rotationSpeed*0.9f) * Time.deltaTime);
        boxModel.transform.Rotate(Vector3.up * (rotationSpeed * 0.7f) * Time.deltaTime);
        boxModel.transform.Rotate(Vector3.forward * (rotationSpeed * 0.6f) * Time.deltaTime);
    }

    void Start()
    {
        Material newMat = new Material(newMats[type]);
        Material[] materials = boxModel.GetComponent<MeshRenderer>().materials;

        materials[1] = newMat;
        boxModel.GetComponent<MeshRenderer>().materials = materials;
    }

    void OnTriggerEnter(UnityEngine.Collider collision)
    {
        if (collision.gameObject.CompareTag("Vehicle") && grabbable)
        {
            GameObject m_gib = Instantiate(gibs);
            m_gib.transform.position = boxModel.transform.position;
            m_gib.transform.rotation = boxModel.transform.rotation;
            m_gib.GetComponent<decay>().valMat = type;

            //Give Vehicle Information
            itemHandler iH = collision.GetComponent<itemHandler>();

            if (iH.itemType >= 0 && iH.itemType == type && iH.itemLevel < 2) //Check if player already has item. If so, rank up.
            {
                iH.itemLevel++;
            }
            else if (iH.itemType < 0) //If no item, just give the item to the player.
            {
                Debug.Log("Test");
                iH.itemType = type;
                iH.itemLevel = 0;
            }
            else if (iH.itemType >= 0 && iH.itemType != type) //Check if player has conflicting item. If so, derank.
            {
                iH.itemLevel--;
                if (iH.itemLevel < 0) //Swaps item if the degrade would make the item less than 0.
                {
                    iH.itemType = type;
                    iH.itemLevel = 0;
                }
            }
            gameObject.transform.localScale = Vector3.zero;
            grabbable = false;
            Invoke("Recharge", rechargeTime);

            //Play Break Sound
            collision.GetComponent<SoundHandler>().PlaySFX("break");
        }
    }

    void Recharge()
    {
        grabbable = true;
        gameObject.transform.localScale = Vector3.one;
    }
}
