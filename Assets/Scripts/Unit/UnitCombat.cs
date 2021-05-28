using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitCombat : Unit
{
    public float damage;
    float attackDistance;
    float attackSpeed;

    [SerializeField]
    private  AudioClip attackSound;
    float attackTimer;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        attackSpeed = rtsEntity.AttackTimer;
        attackDistance = rtsEntity.AttackRange;
        damage = rtsEntity.Damage;
        attackTimer = rtsEntity.AttackTimer;
        if (attackDistance<=defaultDistance)
        {
            attackDistance = defaultDistance;
        }
        audioList.Insert(3,attackSound);  
    }

    // Update is called once per frame
 [ServerCallback]
    void Update()
    {
        base.Update();
         Attack();

    }

    [Server]
    private void Attack()
    {
        
        attackSpeed += Time.deltaTime;
        if (target != null&&!target.hasAuthority)
        {
            navMeshAgent.stoppingDistance = rtsEntity.AttackRange;

            Vector3 pos = target.transform.position;
            navMeshAgent.destination = pos;

            var distance = (transform.position - pos).magnitude;
            Quaternion targetRotation = Quaternion.LookRotation(pos - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180 * Time.deltaTime);
            Debug.Log($"ha authority on ta1rget {target.hasAuthority}  attack distance is {attackDistance} distance is {distance} attaclspeed is {attackSpeed} attacktimer is {attackTimer} the rest {distance -  navMeshAgent.stoppingDistance }");
            NavMeshAgent navMeshAgentTarget = target.gameObject.GetComponent<NavMeshAgent>();
            if (navMeshAgentTarget==null)
            {
                return;
            }
            if (((distance - navMeshAgent.stoppingDistance )>= attackDistance) || !(attackSpeed >= attackTimer)) return;
            Debug.Log($"ha authority on ta2rget {target.hasAuthority}");

            unitStates = UnitStates.Attack;
            StopCoroutine(nameof(AttackAnim));
           StartCoroutine(nameof(AttackAnim));
           Debug.Log("ESTA PEGANDO ");
            GetComponent<ComponentAbility>()?.active(target.GetComponent<RTSBase>(), damage);
            PlayListSoundEffect(3,1,false);
            attackSpeed = 0;
            
            
        }
        navMeshAgent.stoppingDistance = defaultDistance;
    }
    IEnumerator AttackAnim()
    {
        yield return new WaitForEndOfFrame();
        /*do
        {
            
        } while (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"));
*/

        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        unitStates = UnitStates.Idle;

        
    }
}