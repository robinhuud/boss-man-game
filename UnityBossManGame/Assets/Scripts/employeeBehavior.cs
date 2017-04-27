using UnityEngine.AI;
using UnityEngine;

public class employeeBehavior : MonoBehaviour {

    public Animator animator;
    public NavMeshAgent agent;
    public Transform target;
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
		// check to see if we have arrived at our navmesh target
        if(agent.remainingDistance <= agent.stoppingDistance )
        {
            animator.SetBool("Walking", false);
            agent.updateRotation = false;
            //transform.rotation
        }
	}

    public void TriggerNextEmployee()
    {
        agent.SetDestination(target.position);
        animator.SetBool("Walking", true);
    }
}
