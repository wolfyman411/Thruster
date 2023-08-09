using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class itemHandler : MonoBehaviour
{
    public int itemType = -1;
    public int itemLevel = -1;
    public GameObject[] attackItems;
    public GameObject[] supportItems;
    public GameObject[] defenseItems;
    public GameObject shootbone;
    
    public void UseItem()
    {
        if (itemType < 0 || itemLevel < 0)
        {
            return;
        }
        GetComponent<SoundHandler>().PlaySFX("laser");
        if (itemType == 0) //Attack Items
        {
            GameObject itemRef = Instantiate(attackItems[itemLevel]);
            itemRef.transform.position = shootbone.transform.position;
            itemRef.transform.rotation = gameObject.transform.rotation;
            itemRef.GetComponentInChildren<projectile>().parent = gameObject;
            itemRef.GetComponent<ItemBase>().vm = GetComponent<VehicleMovement>();

            if (itemLevel != 1 && GetComponent<VehicleMovement>().curPosition > 1)
            {
                //itemRef.GetComponentInChildren<projectile>().target = GameObject.Find("GameManager").GetComponent<CheckPoints>().Places[GetComponent<VehicleMovement>().curPosition - GameObject.Find("GameManager").GetComponent<CheckPoints>().Places.Count + 1]; //Finds the target ahead
                itemRef.GetComponentInChildren<projectile>().target = GetComponent<VehicleMovement>().racerInfront;
            }
        }
        else if (itemType == 1) //Boosters
        {
            GameObject boostRef = Instantiate(supportItems[itemLevel]);
            boostRef.GetComponent<ItemBase>().vm = GetComponent<VehicleMovement>();
        }
        else if (itemType == 2) //Defense
        {
            GameObject defenseRef = Instantiate(defenseItems[itemLevel]);
            defenseRef.GetComponent<ItemBase>().vm = GetComponent<VehicleMovement>();
            if (defenseRef.GetComponent<Shield>())
            {
                defenseRef.GetComponent<Shield>().target = gameObject;
                GetComponent<VehicleMovement>().isShielded = true;
                GetComponent<VehicleMovement>().shield = defenseRef;
            }
            else
            {
                defenseRef.transform.position = gameObject.transform.position - gameObject.transform.forward * -10;
            }
        }
        itemType = -1;
        itemLevel = -1;
    }
}
