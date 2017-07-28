using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour {

    [SerializeField]
    int stencil;

    [SerializeField]
    Color color;
    
    Renderer renderer;

    [SerializeField]
    float radius;

    Vector3 initPos;

    [SerializeField]
    float rotationSpeed;

    // Use this for initialization
    void Start () {
        renderer = this.GetComponent<Renderer>();
        initPos = this.transform.localPosition;
    }
	
	// Update is called once per frame
	void Update () {

        renderer.material.SetInt("_Stencil", stencil);
        renderer.material.SetColor("_Color", color);

        float x = Mathf.Cos(Time.time * rotationSpeed) * radius;
        float z = Mathf.Sin(Time.time * rotationSpeed) * radius;
        this.transform.localPosition = initPos + new Vector3(x, 0, z);

    }
}
