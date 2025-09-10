using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualButtonCreator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject vb = GameObject.CreatePrimitive(PrimitiveType.Cube);
        vb.name = "VirtualButtonZone";
        vb.transform.parent = transform;  // Gắn lên chính GameObject này (ImageTarget)
        vb.transform.localPosition = new Vector3(0f, 0.01f, 0f);
        vb.transform.localScale = new Vector3(0.2f, 0.01f, 0.2f);
        vb.GetComponent<Collider>().isTrigger = false;
        vb.GetComponent<Renderer>().material.color = Color.yellow;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
