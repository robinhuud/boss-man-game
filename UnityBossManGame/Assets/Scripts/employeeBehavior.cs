using UnityEngine.AI;
using UnityEngine;

public class employeeBehavior : MonoBehaviour {

    public Animator animator;
    public NavMeshAgent agent;
    public Transform target;

    private bool stopped = false;
	// Use this for initialization
	void Start ()
    {
		
	}

    // Called from the editor from the cog icon, useful for hooking up dependant scene attributes
    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();
        Debug.Log("Resetting animator = "+ animator);
        Debug.Assert(animator != null,"Can't find an animator attached to object");
    }
	
	// Update is called once per frame
	void Update ()
    {
		// check to see if we have arrived at our navmesh target using a fudge factor of .75 meters to allow stopping animation
        if(!stopped && agent.remainingDistance <= agent.stoppingDistance + .75f )
        {
            stopped = true;
            animator.SetBool("Walking", false);
            Debug.Log("Arrived at destination, stopping walk cycle");
            //agent.updateRotation = false;
            //transform.rotation
        }
	}

    public void TriggerNextEmployee()
    {
        Debug.Log("Triggered next employee");
        agent.SetDestination(target.position);
        animator.SetBool("Walking", true);
        stopped = false;
    }
}
