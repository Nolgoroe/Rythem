using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public Rigidbody rb;
    public float bulletSpeed;
    public float enemyBulletSpeed;

    public float lifeTime;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dummy"))
        {
            Debug.Log("Hit Dummy");
            other.GetComponent<Dummy>().TakeDamage(1 + PlayerController.instance.stats.attackPower);
            Destroy(gameObject);

        }

        if (other.CompareTag("Armor"))
        {
            other.GetComponentInParent<Dummy>().TakeDamage(1 + PlayerController.instance.stats.attackPower);
            Destroy(gameObject);

        }

        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().TakeDamage(3);
            Destroy(gameObject);

        }

        if (other.CompareTag("puzzle cube"))
        {
            other.GetComponent<PuzzleCube>().DoAction();
            Destroy(gameObject);
        }
    }

    public void SetForward(Transform origin)
    {
        transform.forward = origin.forward;

        Move();
    }
    public void SetAttackPlayer(Transform origin)
    {
        Vector3 direction = PlayerController.instance.gameObject.transform.position - origin.position;

        Move(direction);
    }

    public void Move()
    {
        rb.velocity = transform.forward * bulletSpeed;
    }
    public void Move(Vector3 direction)
    {
        rb.velocity = direction.normalized * enemyBulletSpeed;
        transform.localScale *= 3;
    }
}
