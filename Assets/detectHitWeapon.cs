using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectHitWeapon : MonoBehaviour
{
    public bool isPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (isPlayer)
        {
            if (other.CompareTag("Armor"))
            {
                other.GetComponentInParent<Dummy>().TakeMeleeDamage(2 + PlayerController.instance.stats.attackPower);
            }
            else if (other.CompareTag("Dummy"))
            {
                other.GetComponent<Dummy>().TakeMeleeDamage(2 + PlayerController.instance.stats.attackPower);
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerController>().TakeDamage(2);
            }
        }
    }
}
