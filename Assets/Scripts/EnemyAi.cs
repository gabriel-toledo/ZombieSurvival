using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public MainGameController gameController;

    public float health;
    public int attackDamage;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerSighted, playerInAttackRange;

    private void Awake()
    {
        gameController = GameObject.Find("MainGameController").GetComponent<MainGameController>();
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerSighted && !playerInAttackRange) Patroling();
        if ((playerInSightRange || playerSighted) && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange) AttackPlayer(player.GetComponent<PlayerController>());
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        else agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        playerSighted = true;
        agent.SetDestination(player.position);
    }

    private void AttackPlayer(PlayerController playerController)
    {
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            playerController.TakeDamage(attackDamage);
            alreadyAttacked = true;
            agent.SetDestination(transform.position);
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
        agent.SetDestination(player.position);
    }

    public void TakeDamage(int damage)
    {
        playerSighted = true;
        health -= damage;

        if (health <= 0) { 
            playerSighted = false;
            playerInSightRange = false;
            agent.SetDestination(transform.position);
            Invoke(nameof(DestroyEnemy), 0.5f); 
        }
        else ChasePlayer();
    }
    private void DestroyEnemy()
    {
        gameController.CountEnemy(1);
        Destroy(gameObject);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        AttackPlayer(other.GetComponentInParent<PlayerController>());
    //    }
    //}
}
