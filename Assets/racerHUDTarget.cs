using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class racerHUDTarget : MonoBehaviour
{
    public GameObject target;
    public float offsetX;
    public float offsetY;
    public float percentage;
    public Color color1;
    public Color color2;
    public Color color3;

    public Image Image1;
    public Image Image2;
    public Image Image3;

    // Update is called once per frame
    private void Start()
    {
        Image1.color = color1;
        Image2.color = color2;
        Image3.color = color3;
    }
    void Update()
    {
        transform.position = new Vector3(target.transform.position.z + offsetX, -1 * target.transform.position.x + offsetY) * percentage;
    }




}
