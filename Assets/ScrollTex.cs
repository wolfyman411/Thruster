using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTex : MonoBehaviour
{
    public float ScrollX = 0.5f;
    public float ScrollY = 0.5f;
    public Renderer materialRef;

    // Update is called once per frame
    void Update()
    {
        float OffsetY = Time.time * ScrollY;
        float OffsetX = Time.time * ScrollX;
        materialRef.material.mainTextureOffset = new Vector2 (OffsetX, OffsetY);
    }
}
