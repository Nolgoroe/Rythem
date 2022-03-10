using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAnimEvent : MonoBehaviour
{

    public void CallDeactivateWeaponPlayer()
    {
        GetComponentInParent<PlayerController>().DeactivateWeapon();
    }
    public void CallDeactivateWeaponEnemy()
    {
        GetComponentInParent<Dummy>().DeactivateWeapon();
    }
}
