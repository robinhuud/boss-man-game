//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

using UnityEngine;

// First I extend GameObject to add 3 methods:
// these default methods work for my custom highlight shader

public static class GameObjectExtension
{
    public static bool highlighted = false;
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
        highlighted = true;
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
        highlighted = false;
    }

    public static bool isHighlighted(this GameObject go)
    {
        return highlighted;
    }
}


public class Gaze_Check : MonoBehaviour
{
    public float sightLength = 100f;
    public GameObject selectedObj;
    public GameObject cursor;
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
        // make a new shader for the selection timer
        Shader sh = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(sh);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        lineMaterial.SetInt("_ZTest", 0);
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
            if (seen.collider.tag == "TOUCHABLE") // touchable tagged objects support highlighting
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
                    waiting = true;
                }
                else
                {
                    if(!hasToggled && Time.time - selectedTime > activation_time) // has it been 2 or more seconds since i first highlighted this object?
                    {
                        IDeviceControl dc = selectedObj.GetComponent<IDeviceControl>();
                        if(dc != null)
                        {
                            dc.activate();
                            hasToggled = true;
                            waiting = false;
                        }
                        else
                        {
                            Debug.Log("can't get devicecontrol object");
                        }
                    }
                }
            }
            
            // we hit something that has the correct tag.
            // now look to see if this is a new object (different from "selectedObj")

            else
            {
                // Raycast returns false if we didn't hit any colliders tagged "TOUCHABLE"
                // we must tell any highlighted object to un-highlight itslef
                if (selectedObj != null)
                {
                    selectedObj.DeactivateHighlight();
                    hasToggled = false;
                    waiting = false;
                    selectedObj = null;
                }
            }

        }
        else // didn't hit a raycast-visible object
        {
            if (selectedObj != null)
            {
                selectedObj.DeactivateHighlight();
                hasToggled = false;
                waiting = false;
                selectedObj = null;
            }
            cursor.SetActive(false);
        }
    }

    void OnPostRender()
    {
        // this is where I draw my little timer clock thingy
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
