using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    [SerializeField] RawImage imageRef;
    [SerializeField] float x;
    [SerializeField] float y;
    void Update()
    {
        imageRef.uvRect = new Rect(imageRef.uvRect.position + new Vector2(x, y)*Time.deltaTime,imageRef.uvRect.size);
    }
}
