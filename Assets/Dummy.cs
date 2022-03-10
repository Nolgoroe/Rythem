using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dummy : MonoBehaviour
{
    public Color originalColor;
    public Color hitColor;
    public Renderer meshRenderer;
    public float flashTime;

    public float attackRange;

    public GameObject focusedIndicator;

    public LayerMask playerLayer;

    public GameObject targetPlayer;

    public bool isMelee, isRange, isGhost, isArmor, isOriginallyArmor;

    //public float meleeAttackCoolDown;
    //public float currentmeleeAttackTimer;

    public bool canAttack;

    public Animator meleeWeaponAnim;
    public GameObject meleeWeaponParent;

    public AttackColor enemyColor;
    public AttackColor armorColor;
    public GameObject armor;

    public float maxHP;
    public float currentHP;

    public float maxArmorHP;
    public float currentArmorHP;

    public Slider enemyHealth;
    public Slider enemyArmorHealth;

    public GameObject bulletPrefab;
    public Transform bulletOrigin;

    public bool midAttack;
    public bool isStunned;
    public float StunDuration;

    Coroutine countdownStunCoroutine;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
        focusedIndicator.SetActive(false);

        //meleeAttackCoolDown = Conductor.instance.secPerBeat;

        if (isMelee)
        {
            meleeWeaponParent.SetActive(false);
        }

        currentHP = maxHP;

        if (isOriginallyArmor)
        {
            currentArmorHP = maxArmorHP;
        }
    }

    private void Update()
    {
        DetectPlayer();
        UpdateHP();

        if (isArmor)
        {
            UpdateArmorHP();
        }

        //if (isMelee)
        //{
        //    if (currentmeleeAttackTimer > 0)
        //    {
        //        currentmeleeAttackTimer -= Time.deltaTime;
        //        canAttack = false;
        //    }
        //    else
        //    {
        //        canAttack = true;
        //    }
        //}
    }

    public void CallFlash()
    {
       StartCoroutine(Flash());
    }
    public IEnumerator Flash()
    {
        meshRenderer.material.color = hitColor;

        yield return new WaitForSeconds(flashTime);

        meshRenderer.material.color = originalColor;
    }

    public void ToggleFocusIndicator()
    {
        if (focusedIndicator.activeInHierarchy)
        {
            focusedIndicator.SetActive(false);
        }
        else
        {
            focusedIndicator.SetActive(true);
        }
    }

    public void DetectPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, playerLayer);

        if(hitColliders.Length > 0)
        {
            targetPlayer = hitColliders[0].gameObject;
        }
        else
        {
            targetPlayer = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        Gizmos.DrawSphere(transform.position, attackRange);
    }

    public void DeactivateWeapon()
    {
        meleeWeaponParent.SetActive(false);
        meleeWeaponAnim.SetBool("Attack", false);
    }


    public void UpdateHP()
    {
        enemyHealth.value = currentHP / maxHP;
    }
    public void UpdateArmorHP()
    {
        enemyArmorHealth.value = currentArmorHP / maxArmorHP;
    }

    public void TakeDamage(int amount)
    {
        if (isStunned)
        {
            StartCoroutine(GloryKill());
            return;
        }

        CallFlash();

        if (midAttack)
        {
            GetStunned();
        }

        if (isArmor)
        {
            currentArmorHP -= amount;

            if (currentArmorHP <= 0)
            {
                isArmor = false;
                enemyArmorHealth.gameObject.SetActive(false);
            }
        }
        else
        {
            currentHP -= amount;
        }

        if (currentHP <= 0)
        {
            currentHP = maxHP;
        }
    }
    public void TakeMeleeDamage(int amount)
    {
        if (isStunned)
        {
            StartCoroutine(GloryKill());
            return;
        }

        if (isArmor)
        {
            if (PlayerController.instance.stats.currentAttackColor == armorColor || PlayerController.instance.stats.currentAttackColor == AttackColor.White)
            {
                CallFlash();

                if (midAttack)
                {
                    GetStunned();
                }

                currentArmorHP -= amount;

                if(currentArmorHP <= 0)
                {
                    isArmor = false;
                    enemyArmorHealth.gameObject.SetActive(false);
                    armor.SetActive(false);
                }
            }
            else
            {
                PlayerController.instance.GetStunned();
            }

        }
        else
        {
            if (PlayerController.instance.stats.currentAttackColor == enemyColor || PlayerController.instance.stats.currentAttackColor == AttackColor.White)
            {
                if (midAttack)
                {
                    GetStunned();
                }

                CallFlash();

                currentHP -= amount;
            }
            else
            {
                PlayerController.instance.GetStunned();
            }

        }

        if (currentHP <= 0)
        {
            currentHP = maxHP;

            if (isOriginallyArmor)
            {
                isArmor = true;
                currentArmorHP = maxArmorHP;
                enemyArmorHealth.gameObject.SetActive(true);
                armor.SetActive(true);
            }
        }
    }

    public void CheckCanAttackPlayer()
    {
        if (targetPlayer /*&& canAttack*/)
        {
            midAttack = true;
            Debug.Log("ATTACKING");

            meleeWeaponParent.SetActive(true);
            meleeWeaponAnim.SetBool("Attack", true);
        }
        else
        {
            midAttack = false;
        }
    }
    public void CheckCanAttackPlayerRange()
    {
        if (targetPlayer /*&& canAttack*/)
        {
            Debug.Log("RANGE ATTACKING");

            bullet b = Instantiate(bulletPrefab, bulletOrigin.transform.position, bulletOrigin.transform.rotation).GetComponent<bullet>();
            b.SetAttackPlayer(bulletOrigin);
        }
    }

    public void GetStunned()
    {
        isStunned = true;
        countdownStunCoroutine = StartCoroutine(CountDownStun());
    }

    IEnumerator CountDownStun()
    {
        yield return new WaitForSeconds(StunDuration);
        isStunned = false;
    }

    public IEnumerator GloryKill()
    {
        if (countdownStunCoroutine != null)
        {
            StopCoroutine(countdownStunCoroutine);
            countdownStunCoroutine = null;
        }

        currentHP = maxHP;

        if (isOriginallyArmor)
        {
            currentArmorHP = maxArmorHP;
            enemyArmorHealth.gameObject.SetActive(true);
            armor.SetActive(true);
            isArmor = true;
        }

        Material mat = GetComponent<MeshRenderer>().materials[0];
        mat.color = Color.magenta;

        PlayerController.instance.AddHP(1);
        yield return new WaitForSeconds(2f);
        isStunned = false;
        meshRenderer.material.color = originalColor;
    }
}
