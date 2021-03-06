﻿//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

/////////////////////////////////////////////////////
// Gaze_Check.cs
// Responsible for keeping track of where the HMD is looking,
// and doing the following:
// if gaze hits a collider, render a cursor object at the collision point
// if gaze hits a collider tagged with the selectableTag string:
//      send a message to the object helling it to highlight itself
//      keep track of the object the gaze is hitting, and start a timer
//      display a timer graphic around the cursor while the timer is running
//      if the timer expires while the cursor is still on the same object
//      call the activate method from the iDeviceControl interface on all
//      iDeviceControl objects attached to the object

using UnityEngine;

// First I extend GameObject to add 2 methods:
// ActivateHighlight, and DeactivateHighlight
// these default methods work for my custom highlight shader

public static class GameObjectExtension
{
    public static void ActivateHighlight(this GameObject go)
    {
        //Debug.Log("Activating Highlight!!");
        Material[] mat = go.GetComponent<Renderer>().materials;
        for (int i = 0; i < mat.Length; i++)
        {
            if (mat[i].HasProperty("_HighlightColor"))
            {
                Color glow = mat[i].GetColor("_HighlightColor");
                glow.a = 1f;
                mat[i].SetColor("_HighlightColor", glow);
            }
#if UNITY_ANDROID // android has some weird scaling issues sometimes in VR, so I set the scale of the highlight to fix it (kind of)
            if(mat[i].HasProperty("_HighlightWidth"))
            {
                mat[i].SetFloat("_HighlightWidth", mat[i].GetFloat("_HighlightWidth") * .05f);
            }
#endif
        }
    }

    public static void DeactivateHighlight(this GameObject go)
    {
        //Debug.Log("DE-Activating Highlight!!");
        Material[] mat = go.GetComponent<Renderer>().materials;
        for (int i = 0; i < mat.Length; i++)
        {
            if (mat[i].HasProperty("_HighlightColor"))
            {
                Color glow = mat[i].GetColor("_HighlightColor");
                glow.a = 0f;
                mat[i].SetColor("_HighlightColor", glow);
            }
#if UNITY_ANDROID
            if (mat[i].HasProperty("_HighlightWidth"))
            {
                mat[i].SetFloat("_HighlightWidth", mat[i].GetFloat("_HighlightWidth") * 20f);
            }
#endif
        }
    }
}


public class Gaze_Check : MonoBehaviour
{
    public float sightLength = 100f;
    public GameObject selectedObj;
    public GameObject cursor;
    public string selectableTag = "TOUCHABLE";
    public float activation_time = 2f;
    private float selectedTime;
    private Vector3 focalPoint;
    private float focalDistance;
    private bool hasToggled = false;
    private bool waiting = false;

    static Material lineMaterial;

    // called once on startup
    void Awake()
    {
        makeShader();
    }
	// Use this for initialization
	void Start ()
    {
	    if(cursor == null)
        {
            cursor = GetComponentInChildren<GameObject>();
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    // fixedUpdate is called once per physics sim tick
    void FixedUpdate()
    {
        // In fixedupdate we are looking to see if the gaze-vector of this object
        // (raycast from object origin in "forward" direction) is intersecting with any
        // objects that are tagged "TOUCHABLE"
        RaycastHit seen;
        Ray forwardRay = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(forwardRay, out seen, sightLength)) // populates "seen" with a RaycastHit object
        {
            // we hit something that is not invisible to raycast
            focalPoint = transform.position + (transform.forward * seen.distance);
            focalDistance = seen.distance;
            cursor.transform.position = focalPoint;
            cursor.SetActive(true);
            if (seen.collider.tag.Equals(selectableTag)) // touchable tagged objects support highlighting
            {
                if (seen.transform.gameObject != selectedObj) // new object?
                {
                    if(selectedObj != null)
                    {
                        selectedObj.DeactivateHighlight();
                    }
                    selectedObj = seen.transform.gameObject;
                    selectedObj.ActivateHighlight();
                    selectedTime = Time.time;
                    hasToggled = false;
                    waiting = true;
                }
                else
                {
                    if(!hasToggled && Time.time - selectedTime > activation_time) // has it been activation_time or more seconds since i first highlighted this object?
                    {
                        IDeviceControl[] dcs = selectedObj.GetComponents<IDeviceControl>();
                        if(dcs != null)
                        {
                            foreach(IDeviceControl dc in dcs)
                            {
                                dc.Activate();
                                hasToggled = true;
                                waiting = false;
                            }
                        }
                        else
                        {
                            Debug.Log("can't get devicecontrol object");
                        }
                    }
                }
            }
            else
            {
                // Raycast returns false if we didn't hit any colliders tagged with selectableTag
                // we must tell any highlighted object to un-highlight itslef
                if (selectedObj != null)
                {
                    selectedObj.DeactivateHighlight();
                    waiting = false;
                    hasToggled = false;
                    selectedObj = null;
                }
            }

        }
        else // didn't hit a raycast-visible object
        {
            if (selectedObj != null)
            {
                selectedObj.DeactivateHighlight();
                waiting = false;
                hasToggled = false;
                selectedObj = null;
            }
            cursor.SetActive(false);
        }
    }

    void makeShader()
    {
        // make a new shader for the selection timer
        Shader sh = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(sh);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        lineMaterial.SetInt("_ZTest", 0);
    }

    void OnPostRender()
    {
        // this is where I draw my little timer clock thingy
        // to show how long you've beel looking at a selectableTag object
        float t = Time.time;
        float a, angle, x, y, z, f;
        if(waiting && t > selectedTime && t < selectedTime + activation_time)
        {
            f = (t - selectedTime) / activation_time; // 0-1 start-finish of activation time
            lineMaterial.SetPass(0);
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix); // transofrm to camera space
            GL.Begin(GL.TRIANGLE_STRIP);
            for(int i = 0; i < (int) (100 * f); i++)
            {
                a = i / 100f;
                angle = a * Mathf.PI * 2;
                x = Mathf.Sin(angle);
                y = Mathf.Cos(angle);
                z = focalDistance - .02f;
                GL.Color(new Color(a, 1f - a/2, a, .5f + a /2));
                GL.Vertex3(x * .01f, y * .01f, z);
                GL.Vertex3(x * .02f, y * .02f, z);
            }
            GL.End();
            GL.PopMatrix();
        }
    }
}
