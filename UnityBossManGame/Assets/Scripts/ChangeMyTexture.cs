using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMyTexture : MonoBehaviour {

    [SerializeField]
    public Texture[] tex;
	// Use this for initialization
	void Start () {
        Debug.Assert(tex != null, "No texture specified for ChangeMyTexture");
        GetComponent<Renderer>().enabled = false;
    }
	
    public void ShowTexture(int id)
    {
        GetComponent<Renderer>().enabled = true;
        GetComponent<Renderer>().material.SetTexture("_MainTex", tex[id]);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
