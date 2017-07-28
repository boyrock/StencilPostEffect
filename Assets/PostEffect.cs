using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffect : MonoBehaviour {

    public Shader shader;

    Material _mat;
    Material mat
    {
        get
        {
            if(_mat == null)
            {
                _mat = new Material(shader);
            }

            return _mat;
        }
    }

    RenderTexture rt;
	// Use this for initialization
	void Start () {
        rt = new RenderTexture(Screen.width, Screen.height, 24);
	}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
