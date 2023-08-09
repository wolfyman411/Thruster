using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosionforce : MonoBehaviour
{
    // Start is called before the first frame update
    public float minForce = 5;
    public float maxForce = 100;
    public float forceRadius = 10;
    public float fragScale = 1;
    public bool explodeOnStart = true;

    void Start()
    {
        if (explodeOnStart)
        {
            Explode();
        }
    }
    public void Explode()
    {
        foreach (Transform t in transform)
        {
            var rb = t.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(Random.Range(minForce, maxForce), gameObject.transform.position, forceRadius);
        }
    }

    IEnumerator Shrink(Transform t, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 newScale = t.localScale;

        while(newScale.x >= 0)
        {
            newScale -= new Vector3(fragScale, fragScale, fragScale);

            t.localScale = newScale;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
