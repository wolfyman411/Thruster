using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class decay : MonoBehaviour
{
    public GameObject[] gameObjects;
    public Material[] changeMaterial;
    public int valMat;
    public bool changeTex = false;
    void Start()
    {
        if (changeTex)
        {
            foreach (var obj in gameObjects)
            {
                Material[] materials = obj.GetComponent<MeshRenderer>().materials;
                int loop = 0;
                foreach (Material material in materials)
                {
                    loop++;
                    if (material.name == "Material.001 (Instance)")
                    {
                        materials[loop - 1] = changeMaterial[valMat];
                    }
                }
                obj.GetComponent<MeshRenderer>().materials = materials;
            }
        }
        Invoke("Decay", 0.1f);
        Invoke("Remove", 25.0f);
    }

    public void changeId()
    {
        foreach (var obj in gameObjects)
        {
            Material[] materials = obj.GetComponent<MeshRenderer>().materials;
            int loop = 0;
            foreach (Material material in materials)
            {
                loop++;
                if (material.name == "Material.001 (Instance)")
                {
                    materials[loop - 1] = changeMaterial[valMat];
                }
            }
            obj.GetComponent<MeshRenderer>().materials = materials;
        }
    }

    void Decay()
    {
        foreach(var obj in gameObjects)
        {
            obj.transform.localScale *= 0.99f;
        }
        Invoke("Decay", 0.1f);
    }

    void Remove()
    {
        Destroy(gameObject);
    }
}
