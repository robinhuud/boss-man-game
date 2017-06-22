using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class employeeBehavior : MonoBehaviour {

    public Animator animator;
    public NavMeshAgent agent;
    public Transform target;
    public Transform lookTarget;
    public Transform chairTarget;

    private bool stopped = false;
	// Use this for initialization
	void Start ()
    {
        agent.isStopped = true;
        //animator.SetBool("Walking", false);
    }

    // Called from the editor from the cog icon, useful for hooking up dependant scene attributes
    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();
        Debug.Assert(animator != null,"Can't find an Animator attached to object");
        Debug.Assert(agent != null, "Can't find a NavMeshAgent attached to object");
    }
	
	// Update is called once per frame
	void Update ()
    {
        // check to see if we have arrived at our navmesh target using a fudge factor of .75 meters to allow stopping animation
        if (!stopped)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                arrived();
            }
        }
	}

    private void arrived()
    {
        stopped = true;
        animator.SetBool("Walking", false);
        StartCoroutine("queued_trigger");
        Debug.Log("Arrived at destination, stopping walk cycle");    
    }

    public void DoorOpened()
    {
        Debug.Log("Door has opened");
        agent.SetDestination(target.position);
        animator.SetBool("Walking", true);
        stopped = false;
    }

    IEnumerator queued_trigger()
    {
        AnimatorStateInfo a_info = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(a_info.length);
        agent.updateRotation = false;
        transform.LookAt(lookTarget);
        animator.SetTrigger("step_back");
        while (!a_info.IsName("StepBack"))
        {
            yield return null;
            a_info = animator.GetCurrentAnimatorStateInfo(0);
        }
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        agent.updatePosition = false;
        animator.SetBool("Sitting", true);
    }

}
