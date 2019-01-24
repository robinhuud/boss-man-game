using UnityEngine.AI;
using UnityEngine;
using System.Collections;


/// <summary>
/// Coordinates and controls the NavMeshAgent and the Animator
/// 
/// </summary>

public class employeeBehavior : MonoBehaviour {

    public Animator animator; // The Animator which controls the animation of the employee
    public NavMeshAgent agent; // The NavMeshAgent which controls where the employee can move in the scene
    public Transform spawnPoint; // world coordinates of the spawn point (and reset point) of the employee
    public Transform target; // world coordinates to move toward for the NavMeshAgent
    public Transform lookTarget; // world coordinates to look toward for the animator (player's face)
    public Transform benchTarget; // world coordinates of benchfor hallway navigation.
    public Transform headBone; // the head bone object container

    private bool stopped = true; // Is the NavMeshAgent currently stopped?
    private float brakingDistance = .35f; // fudge factor to trigger the stop walking animation
    private bool isInHall = true;
    private bool staring = false;

    private static System.Random random = new System.Random();

    // Called from the editor from the cog icon, useful for hooking up dependant scene attributes
    // in this case, that's the required public variables of Animator, and NavMeshAgent
    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();

        Debug.Assert(animator != null, "Can't find an Animator attached to object, please assign it in the inspector");
        Debug.Assert(agent != null, "Can't find a NavMeshAgent attached to object, please assign it in the inspector");
    }

    // Use this for initialization
    void Start ()
    {
        // Debug.Log("TRACE: Start method");
        // make sure we have our scene references
        Debug.Assert(animator != null, "No Animator assigned, cannot continue.");
        Debug.Assert(agent != null, "No NavMeshAgent assigned, cannot continue.");
        // prepare the NavMeshAgent
        Debug.Assert(agent.isOnNavMesh, "NavMeshAgent is not attached to a navmesh");
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(benchTarget.position, path);
        if(path.status == NavMeshPathStatus.PathComplete)
        {
            //Debug.Log("Complete Path found.");
            agent.Warp(benchTarget.position);
        }
        else
        {
            Debug.Log("No complete path found");
        }
        // Tell the NavMeshAgent that we are ready to go.
        ArrivedNearChair();
    }

	
	// Update is called once per rendered frame
	void Update ()
    {

	}

    // FixedUpdate is called once per simulation tick (fixed frame rate)
    void FixedUpdate()
    {

    }

    // Called after Update() is done
    private void LateUpdate()
    {
        // Let's get the head pointed toward the viewer
        if(staring)
        {
            staring = false;
            StartCoroutine(StareAt(headBone.rotation, lookTarget, 0.5f));
        }
    }

    // employee has arrived at it's destination, it will look for a chair to sit down if possible.
    private void ArrivedNearChair()
    {
        //Debug.Log("Arrived");
        agent.isStopped = true;
        FindChair();
        //Debug.Log("Arrived at destination, stopping walk cycle");    
    }

    public void DoorOpened()
    {
        // this is called when the door has opened, 
        //Debug.Log("Door has opened at " + Time.time);
        animator.SetBool("Sitting", false);
        stopped = false;
    }


    void FindChair()
    {
        // Prevent the NavMeshAgent from overriding our rotation.
        agent.updateRotation = false;
        // The assumption is that the chair will be facing toward the lookTarget, so facing that way
        // will align us with the chair. In order for this to work, the walkTarget must be  directly
        // in front of the chair in the direction of the lookTarget
        StartCoroutine(LerpTurn(transform.rotation, lookTarget, 0.5f));
    }

    // Slowly turns object toward target over duration seconds, then calls ready_to_sit
    IEnumerator LerpTurn(Quaternion startRotation, Transform target, float duration)
    {
        float startTime = Time.time;
        float thisTime = startTime;
        float fraction = 0;
        Quaternion destinationRotation = Quaternion.LookRotation(new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z));
        while ( thisTime < startTime + duration + Time.deltaTime)
        {
            thisTime += Time.deltaTime;
            fraction = (thisTime - startTime) / duration;
            transform.localRotation = Quaternion.Slerp(startRotation, destinationRotation, fraction);
            yield return null;
        }
        ReadyToSit();
    }

    IEnumerator StareAt(Quaternion startRotation, Transform target, float duration)
    {
        float startTime = Time.time;
        float thisTime = startTime;
        float fraction = 0;
        Quaternion destinationRotation = Quaternion.LookRotation(new Vector3(target.position.x - headBone.position.x, target.position.y - headBone.position.y, target.position.z - headBone.position.z));
        while (thisTime < startTime + duration + Time.deltaTime)
        {
            thisTime += Time.deltaTime;
            fraction = (thisTime - startTime) / duration;
            headBone.localRotation = Quaternion.Slerp(startRotation, destinationRotation, fraction);
            yield return null;
        }
    }

    void ReadyToSit()
    {
        animator.SetTrigger("step_back");
        // Prevent the navmeshagent from messing up our step-back animation any more than it already is
        agent.updatePosition = false;
    }

    // All of these functions are called by the animator based on events in the various timelines of the animation clips
    // these are used to string animations together, and trigger other events

    // This is called by the animator after the "step_back" trigger has finished.
    // In order for this to work the NavMeshAgent must have it's updatePosition set to false for at least one frame
    // so it is set in the ready_to_sit() method.
    void SteppedBack()
    {
        // After we step back toward the chair, we must trigger the sitting animation.
        // offset to get employee into correct position for sitting down to land his butt in the chair.
        // This is because the stepping back animation (unlike the walk cycle) does not have root bone offset baked in.
        transform.position += new Vector3(-.41f, 0, 0.01f); 
        // After we step back toward the chair, we must trigger the sitting animation.
        animator.SetBool("Sitting", true);
    }

    void StandLoop()
    {
        // This is the moment when we decide whether we should start walking or just continue to stand
        //Debug.Log("StandLoop");
        if (!stopped) // This is the "Start Walking" case
        {
            //Debug.Log("Setting destination to new walk target at " + Time.time);
            animator.SetBool("Walking", true); // should trigger state change, then StartedWalking() callback
            //agent.updatePosition = true;

            if (isInHall)
            {
                agent.SetDestination(target.position);
                isInHall = false;
            }
            else
            {
                agent.SetDestination(benchTarget.position);
                isInHall = true;
            }
        }
    }

    void StartedWalking()
    {
        // Turns on the Nav mesh agent which takes over the root motion.
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;
    }

    void WalkLoop()
    {
        // after each step we check to see if we are close enough to the destinatiopn to trigger the stop walking animation transition.
        if (!stopped && agent.remainingDistance <= agent.stoppingDistance + brakingDistance)
        {
            //Debug.Log("Walk Loop at destination, stop walking at " + Time.time);
            animator.SetBool("Walking", false); // should trigger state transition, then StoppedWalking() callback
            stopped = true;
            ArrivedNearChair();
        }
    }

    void StoppedWalking()
    {
        // turn off navigation because step back animation moves the gameobject and nav would try to move it back
        agent.isStopped = true; 
    }

	void SatDown()
	{
        // Just string the next animation clip, which will start calling SitLoop() after each cycle
        //animator.SetTrigger("gesture_wide");
	}

    void SitLoop()
    {
        // every time we finish a loop animation, we pick what to do next randomly
        switch (random.Next(0,5)) // 1 in 6 chance of each tap foot or cross legs
        {
            case 0:
                animator.SetTrigger("tap_foot");
                break;
            case 1:
                animator.SetTrigger("cross_legs");
                break;
            case 2:
                staring = false;
                // In this case we just stay in the same loop, and continue to sit
                break;
        }
    }

    void CrossedLegs()
    {
        // every time we finish a loop animation, we pick what to do next randomly
        switch (random.Next(0,9)) // 1 in 10 chance of uncrossing legs immediately instead of swinging foot
        {
            case 0:
                animator.SetTrigger("uncross");
                break;
        }
    }

    void TapLoop()
    {
        // every time we finish a loop animation, we pick what to do next randomly
        switch (random.Next(0, 5)) // 1 in 6 chance to stop tapping foot every cycle
        {
            case 0:
                animator.SetTrigger("stop_tapping");
                break;
        }
    }

    void SwingLoop()
    {
        // every time we finish a loop animation, we pick what to do next randomly
        switch (random.Next(0, 5)) // 1 in 6 chance of leaving foot-swing animation, uncrossing legs.
        {
            case 0:
                animator.SetTrigger("uncross");
                break;
            case 1:
                animator.SetTrigger("gesture_inward");
                break;
        }
    }

    void UncrossedLegs()
    {
        //animator.SetTrigger("gesture_inward");
        staring = true;
    }

    void FinishedFalling()
    {
        stopped = false;
        agent.Warp(spawnPoint.position);
        animator.SetBool("Walking", true);
        agent.SetDestination(benchTarget.position);
        isInHall = true;
    }
}
