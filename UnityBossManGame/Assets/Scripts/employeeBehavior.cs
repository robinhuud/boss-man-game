using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class employeeBehavior : MonoBehaviour {

    public Animator animator;
	// Use this for initialization
	void Start () {
		
	}

    // Called from the editor from the cog icon, useful for hooking up dependant scene attributes
    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        Debug.Log("Resetting animator = "+ animator);
        Debug.Assert(animator != null,"Can't find an animator attached to object");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void TriggerNextEmployee()
    {

    }
}
