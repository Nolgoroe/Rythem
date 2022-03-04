using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public Rigidbody rb;
    public float bulletSpeed;

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
            other.GetComponent<Dummy>().CallFlash();
        }

        Destroy(gameObject);
    }

    public void SetForward(Transform origin)
    {
        transform.forward = origin.forward;

        Move();
    }

    public void Move()
    {
        rb.velocity = transform.forward * bulletSpeed;
    }
}
