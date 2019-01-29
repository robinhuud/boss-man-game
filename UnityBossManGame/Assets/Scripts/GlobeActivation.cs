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
        StartCoroutine(RotateAroundZAxis(1f));
    }

    IEnumerator RotateAroundZAxis(float duration)
    {
        float startTime = Time.time;
        float nowTime = startTime;
        Vector3 locals = this.transform.localEulerAngles;
        while (nowTime < startTime + duration)
        {
            float angle = 720f * ((nowTime - startTime) / duration);
            this.transform.localEulerAngles = new Vector3(locals.x, locals.y, angle);
            yield return null;
            nowTime = Time.time;
        }
        this.transform.localEulerAngles = new Vector3(locals.x, locals.y, locals.z);
    }

    public bool IsActive()
    {
        return true;
    }
}