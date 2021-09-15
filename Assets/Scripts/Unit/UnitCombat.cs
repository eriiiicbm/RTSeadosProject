using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitCombat : Unit
{
    public float damage;
    float attackDistance;
    float attackSpeed;
    public LayerMask layerMask = ~8;
    [SerializeField] private AudioClip attackSound;
    float attackTimer;

    public bool automaticAttack = true;

// Start is called before the first frame update
    void Start()
    {
        base.Start();
        attackSpeed = rtsEntity.AttackTimer;
        attackDistance = rtsEntity.AttackRange;
        damage = rtsEntity.Damage;
        attackTimer = rtsEntity.AttackTimer;
        if (attackDistance <= defaultDistance)
        {
            attackDistance = defaultDistance;
        }

        audioList.Insert(4, attackSound);

        accessibleMethodStatesList.Add(nameof(AttackState));
    }

    // Update is called once per frame


    [Server]
    private void Attack()
    {
        attackSpeed += Time.deltaTime;
        if (target == null)
        {
            navMeshAgent.stoppingDistance = defaultDistance;
            return;
        }

        if (target.GetComponent<Resource>() != null)
        {
            return;
        }

        if (target.connectionToClient == null)
        {
            goto damage;
        }

        if (connectionToClient.connectionId == target.connectionToClient.connectionId)
        {
            return;
        }

        damage:
        navMeshAgent.stoppingDistance = rtsEntity.AttackRange;

        Vector3 pos = target.transform.position;
        navMeshAgent.destination = pos;

        var distance = (transform.position - pos).magnitude;
        Quaternion targetRotation = Quaternion.LookRotation(pos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180 * Time.deltaTime);
        Debug.Log(
            $"ha authority on ta1rget {target.hasAuthority}  attack distance is {attackDistance} distance is {distance} attaclspeed is {attackSpeed} attacktimer is {attackTimer} the rest {distance - navMeshAgent.stoppingDistance}");

        if (((distance - defaultDistance) >= attackDistance) || !(attackSpeed >= attackTimer)) return;
        Debug.Log($"ha authority on ta2rget {target.hasAuthority}");

        unitStates = UnitStates.Attack;
        StopCoroutine(nameof(AttackAnim));
        StartCoroutine(nameof(AttackAnim));
        Debug.Log("ESTA PEGANDO ");
        GetComponent<ComponentAbility>()?.active(target.GetComponent<RTSBase>(), damage);
        PlayListSoundEffect(4, 1, false);
        attackSpeed = 0;


        navMeshAgent.stoppingDistance = defaultDistance;
    }

    IEnumerator AttackAnim()
    {
        yield return new WaitForEndOfFrame();
        /*do
        {
            
        } while (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"));
*/
        if (animator == null)
        {
            yield break;
        }

        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        unitStates = UnitStates.Idle;
    }

/*    public virtual  IEnumerator MoveState()
    {
        while (unitStates == UnitStates.Walk)
        {
            NavMeshToTarget();
            yield return 0;
        }

        yield return new WaitForEndOfFrame();
    }
  */
    public override IEnumerator IdleState()
    {
        base.IdleState();
        while (unitStates == UnitStates.Idle)
        {
            if (!automaticAttack)
            {
                Debug.Log("Automatic attack is disabled");

                yield return 0;
            }

            DetectEnemy(attackDistance);
            yield return 0;
        }

        yield return new WaitForEndOfFrame();
    }

    public virtual IEnumerator AttackState()
    {
        while (unitStates == UnitStates.Attack)
        {
            if (target == null)
            {
                DetectEnemy(attackDistance);
            }

            yield return 0;
        }

        yield return new WaitForEndOfFrame();
    }

    private void Update()
    {
        Attack();
    }

    public void DetectEnemy(float attackRange)
    {
        Collider[] collider = Physics.OverlapSphere(this.gameObject.transform.position, attackRange * 3, layerMask);
        if (collider.Length == 0)
        {
            return;
        }

        Collider nearCollider = collider[0];
        foreach (var col in collider)
        {
            if (col.GetComponent<RTSBase>()?.connectionToClient == null)
            {
                continue;
            }

            if (col.GetComponent<RTSBase>().connectionToClient.connectionId ==
                NetworkClient.connection.connectionId) continue;
            if (Vector3.Distance(col.transform.position, this.transform.position) <=
                Vector3.Distance(nearCollider.transform.position, this.transform.position))
            {
                nearCollider = col;
            }
        }

        if (nearCollider == null) return;

        RTSBase enemy = nearCollider.GetComponent<RTSBase>();
        if (enemy == null) return;
        if (enemy.connectionToClient == null || connectionToClient == null)
        {
            return;
        }

        if (enemy.connectionToClient.connectionId ==
            connectionToClient.connectionId)
        {
            return;
        }

        transform.LookAt(enemy.transform.position);

        Vector3 eulerAngles = transform.rotation.eulerAngles;
        eulerAngles.x = 0;
        eulerAngles.z = 0;
        eulerAngles.y -= 180;
        transform.rotation = Quaternion.Euler(eulerAngles);
        if (target == null)
        {
            GetTargeter().CmdSetTarget(enemy.gameObject);
            target = enemy.GetComponent<Targetable>();
            CmdMove(target.transform.position);
            return;
        }

        if (!(Vector3.Distance(enemy.transform.position, this.transform.position) <
              Vector3.Distance(target.transform.position, this.transform.position))) return;
        Debug.Log("Updated target");
        GetTargeter().CmdSetTarget(enemy.gameObject);
        target = enemy.GetComponent<Targetable>();
        CmdMove(target.transform.position);
    }
}