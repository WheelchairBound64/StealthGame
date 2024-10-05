using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum GuardStates
{
    PATROL,
    INVESTIGATE,
    PURSUE
}
public class GuardBehavior : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform moveTarget;
    [SerializeField] Transform losPlayer;
    //Rigidbody rb;
    CharacterController charCont;
    [SerializeField] float speed;
    NavMeshPath navPath;
    Queue<Vector3> remainingPoints;
    Vector3 currentTargetPoint;
    GuardStates state = GuardStates.PATROL;
    float suspicion = 0f;
    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();

        charCont = GetComponent<CharacterController>();
        navPath = new NavMeshPath();
    }

    // Update is called once per frame
    void Update()
    {
        var new_forward = (currentTargetPoint - transform.position).normalized;
        new_forward.y = 0;
        transform.forward = new_forward;

        remainingPoints = new Queue<Vector3>();
        if (agent.CalculatePath(moveTarget.position, navPath))
        {
            foreach (Vector3 p in navPath.corners)
            {
                remainingPoints.Enqueue(p);
            }

            currentTargetPoint = remainingPoints.Dequeue();
        }

        float distToPoint = Vector3.Distance(transform.position, currentTargetPoint);
        if (distToPoint < 1.1)
        {
            if (remainingPoints.Count > 0)
            {
                currentTargetPoint = remainingPoints.Dequeue();
            }
        }

        charCont.Move(new_forward * speed * Time.deltaTime);

        //line of sight
        Vector3 directionToTarget = (losPlayer.position - transform.position).normalized;
        Vector3 forwardDirection = transform.forward;

        float dot = Vector3.Dot(directionToTarget, forwardDirection);

        if (dot > .6) 
        {
            agent.CalculatePath(losPlayer.position, navPath);
            state = GuardStates.PURSUE;
        }
        else if (dot < .6 && suspicion > 0)
        {

            state = GuardStates.INVESTIGATE;
        }
        else
        {
            state = GuardStates.PATROL;
        }

        switch(state)
        {
            case GuardStates.PURSUE:
                UpdatePursue(dot);
                break;
            case GuardStates.PATROL:
                UpdatePatrol();
                break;
            case GuardStates.INVESTIGATE:
                UpdateInvestigate(dot);
                break;
        }
    }

    private void FixedUpdate()
    {
        //rb.velocity = transform.forward * speed;
    }

    private void OnDrawGizmos()
    {
        if (navPath == null)
            return;
        
        Gizmos.color = Color.red;
        foreach(Vector3 node in navPath.corners)
        {
            Gizmos.DrawWireSphere(node, .5f);
        }
    }

    void UpdatePatrol()
    {
        Debug.Log("I am unaware of what is happening at this current time");
    }

    void UpdateInvestigate(float d)
    {
        if(d < .6f)
        {
            suspicion -= Time.deltaTime;
            Debug.Log("Investigating area");
        }
        if(suspicion <= 0)
        {
            state = GuardStates.PATROL;
        }
    }

    void UpdatePursue(float d)
    {
        suspicion = 3;
        if (d > .6)
        {
            Debug.Log("Target being pursued");
        }
    }
}
