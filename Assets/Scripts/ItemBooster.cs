using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBooster : MonoBehaviour
{
    // Start is called before the first frame update
    ItemBase ib;
    public float duration;
    public float boostAmount;
    void Start()
    {
        ib = GetComponent<ItemBase>();
        ib.vm.bonusSpeed += boostAmount; //Apply Boost
        InvokeRepeating("Decay",0.1f,0.1f);
    }

    void Decay()
    {
        duration -= 0.1f;
        ib.vm.bonusSpeed += boostAmount;
        if (duration < 0)
        {
            Destroy(gameObject);
        }
    }
}
