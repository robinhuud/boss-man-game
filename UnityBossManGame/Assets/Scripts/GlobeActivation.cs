using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeActivation : MonoBehaviour, IDeviceControl {

    [SerializeField]
    public Texture[] mapList;
    int mapId = 0;
	// Use this for initialization
	void Start ()
    {
        Debug.Assert(mapList.Length > 0, "No textures in map list");
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Activate()
    {
        Material mat = GetComponent<Renderer>().material;
        if(++mapId > mapList.Length)
        {
            mapId = 0;
        }
        mat.SetTexture("_MainTex", mapList[mapId]);
    }

    public bool IsActive()
    {
        return true;
    }
}