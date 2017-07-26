using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour {

    [SerializeField]
    int stencil;

    [SerializeField]
    Color color;
    
    Renderer renderer;

    // Use this for initialization
    void Start () {
        renderer = this.GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {

        renderer.material.SetInt("_Stencil", stencil);
        renderer.material.SetColor("_Color", color);

    }
}
