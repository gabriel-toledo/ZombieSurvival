using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;
    private Animator anim;

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
    public bool playerInSightRange, playerSighted, playerInAttackRange, dead = false;

    private void Awake()
    {
        gameController = GameObject.Find("MainGameController").GetComponent<MainGameController>();
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerSighted && !playerInAttackRange && !dead) Patroling();
        if ((playerInSightRange || playerSighted) && !playerInAttackRange && !dead) ChasePlayer();
        if (playerInAttackRange && !dead) AttackPlayer(player.GetComponentInParent<PlayerController>());
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        else agent.SetDestination(walkPoint);

        anim.SetFloat("Speed", 0.6f, 0.3f, Time.deltaTime);
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
        anim.SetFloat("Speed", 0.6f, 0.3f, Time.deltaTime);
        playerSighted = true;
        agent.SetDestination(player.position);
    }

    private void AttackPlayer(PlayerController playerController)
    {
        anim.SetFloat("Speed", 1f, 0.3f, Time.deltaTime);
        transform.LookAt(player.position);
        agent.SetDestination(transform.position);
        if (!alreadyAttacked)
        {
            playerController.TakeDamage(attackDamage);
            alreadyAttacked = true;
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

        if (health <= 0)
        {
            dead = true;
            anim.SetFloat("Speed", 1.5f, 0.001f, Time.deltaTime);
            playerSighted = false;
            playerInSightRange = false;
            agent.SetDestination(transform.position);
            Invoke(nameof(DestroyEnemy), 1.12f);
        }
        else ChasePlayer();
    }
    private void DestroyEnemy()
    {
        gameController.CountEnemy(1);
        Destroy(gameObject);
    }
}