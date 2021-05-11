using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachineExample : MonoBehaviour
{
    public void Hello()
    {
        Debug.Log("Hello");
    }
 

    //Estado actual del enemigo
    public UnitStates currentState;
    private Transform patrolTarget;

    public Transform target;

    public float moveSpeed = 3.0f;
    public float rotateSpeed = 3.0f;

    //idleRange >= followRange
    public float followRange = 10.0f; // Distancia de detecci�n del enemigo
    public float idleRange = 10.0f; // Distancia de vuelta a estado idle


    //VIDA DEL ENEMIGO
    public float health = 100.0f;
    private float currentHealth;
 
    private NavMeshAgent agent;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(nameof(IdleState));
        this.currentHealth = this.health;
        agent.speed = moveSpeed;
    }


    IEnumerator IdleState()
    {
        Debug.Log("Idle: Enter");

        while (this.currentState == UnitStates.Idle)
        {
            this.currentState = GetDistance() < followRange ? UnitStates.Follow : UnitStates.Idle;

            yield return new WaitForSeconds(1);
        }

        Debug.Log("Idle: Exit");
        GoToNextState();
    }

    IEnumerator FollowState()
    {
        Debug.Log("Follow: Enter");
        agent.destination = target.position;
        while (this.currentState == UnitStates.Follow)
        {
            agent.destination = target.position;


            RotateTowardsTarget();

            if (GetDistance() > idleRange)
            {
                this.currentState = UnitStates.Idle;
            }
            else if (GameManager._instance.isPlayerHiding)
            {
                this.currentState = UnitStates.Idle;
            }

            yield return 0;
        }

        Debug.Log("Follow: Exit");
        GoToNextState();
    }

    IEnumerator AttackState()
    {
        Debug.Log("Attack: Enter");

        patrolTarget = GameManager._instance.GetARandomTarget();
        agent.destination = patrolTarget.position;


        while (this.currentState == UnitStates.Attack)
        {
            RotateTowardsTarget(patrolTarget);

            if (GetDistance() < followRange && !GameManager._instance.isPlayerHiding)
            {
                Debug.Log("aHORA SEGUIRA A PLAYER");
                this.currentState = UnitStates.Follow;
            }

            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        Debug.Log("PATH COMPLETE");
                        this.currentState = UnitStates.Idle;

                    }
                }
            }

            yield return 0;
        }


        Debug.Log("Attack: Exit");
        GoToNextState();
    }

    IEnumerator DieState()
    {
        Debug.Log("Die: Enter");
        GameManager._instance.killGhost();
        Destroy(this.gameObject);


        yield return 0;
    }

    private IEnumerator OnCollisionEnter(Collision other)
    {
      

        yield return new WaitForEndOfFrame();
    }

    //Calcula la distancia entre el enemigo y su objetivo
    float GetDistance()
    {
        Vector3 director = new Vector3(this.transform.position.x - this.target.position.x, 0,
            this.transform.position.z - this.target.position.z);
        return director.magnitude;
    }

    void GoToNextState()
    {
        string methodName = this.currentState.ToString() + "State";
        Debug.Log("NOMBRE ESTADO " + methodName);
        SendMessage(methodName);
    }

    void RotateTowardsTarget()
    {
        Vector3 direction = this.target.position - this.transform.position;
        Quaternion directionToFace = Quaternion.LookRotation(direction);
        float angleToRotate = this.rotateSpeed * Time.deltaTime;

        this.transform.rotation = Quaternion.Slerp(
            this.transform.rotation,
            directionToFace,
            angleToRotate
        );
    }

    void RotateTowardsTarget(Transform target)
    {
        Vector3 direction = target.position - this.transform.position;
        Quaternion directionToFace = Quaternion.LookRotation(direction);
        float angleToRotate = this.rotateSpeed * Time.deltaTime;

        this.transform.rotation = Quaternion.Slerp(
            this.transform.rotation,
            directionToFace,
            angleToRotate
        );
    }

    public IEnumerator TakeDamage()
    {
        float damageToTake = 100.0f - GetDistance() * 5.0f;

        if (damageToTake < 0)
        {
            damageToTake = 0;
        }

        if (damageToTake > health)
        {
            damageToTake = health;
        }


        this.currentHealth -= damageToTake;

        if (this.currentHealth <= 0)
        {
            this.currentState = UnitStates.Dead;
        }
        else
        {
            this.followRange = Mathf.Max(GetDistance(), this.followRange);
            this.currentState = UnitStates.Follow;
        }

        print("Vida actual del enemigo: " + this.currentHealth.ToString());

        yield return new WaitForEndOfFrame();
    }
}
