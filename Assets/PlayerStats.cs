using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum AttackColor { Green,Blue,Red,White}
public class PlayerStats : MonoBehaviour
{
    public float maxHP;
    public float currentHP;
    public AttackColor currentAttackColor;
    public int maxAmmo;
    public int currentAmmo;
    public float reloadTime;
    public int attackPower;

    public TMP_Text currentAmmoText, attackPowerText;

    private void Start()
    {
        currentHP = maxHP;
        currentAmmo = maxAmmo;
    }



    public void ReduceAmmo()
    {
        currentAmmo--;
    }

    private void Update()
    {
        UpdateAmmoText();

        UpdateAttackPowerText();
    }

    void UpdateAmmoText()
    {
        currentAmmoText.text = "current Ammo : " + currentAmmo.ToString();
    }
    void UpdateAttackPowerText()
    {
        attackPowerText.text = "attack power : " + attackPower.ToString();
    }
}
