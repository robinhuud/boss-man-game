//
// All code written by Robin Hayes in 2016-17
// all code is written intended to be used under GPL
// no warranties are expressed or implied
//

using UnityEngine;
using UnityEngine.AI;

public class SceneControl : MonoBehaviour {

    [SerializeField] LightButton[] scripts;
    public employeeBehavior employeeScript;

    // This method is called from the editor cog menu, useful for attaching
    // serializable fileds with default values.
    void Reset()
    {
        if(this.transform.childCount > 0)
        {
            scripts = GetComponentsInChildren<LightButton>();
        }
    }

    // Use this for initialization
    void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
    }

    // Need to define an interface for this?? WTF??
    public void react(LightButton caller)
    {
        if (caller.GetComponentInParent<MonoBehaviour>().name.Equals("Door"))
        {
            employeeScript.TriggerNextEmployee();
        }
    }
}
