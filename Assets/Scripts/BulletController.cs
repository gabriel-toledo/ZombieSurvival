using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public int bulletDamage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<PlayerController>().TakeDamage(bulletDamage);
            Debug.Log("Player Collision");
        }
        else if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyAi>().TakeDamage(bulletDamage);
            Debug.Log("Enemy Collision");
        }
        Invoke(nameof(DestroyDelay), 0.05f);
    }
    private void DestroyDelay()
    {
        Destroy(gameObject);
    }
}
