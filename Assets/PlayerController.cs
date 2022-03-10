using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public Rigidbody rb;

    public Slider playerHPSlider;

    private Vector3 playerInput;

    public float speed;
    public float turnSpeed = 360;
    public float dashDistance;

    public float originalDashTimer;
    public float currentDashTimer;

    public float jumpPower;
    public float ySpeed;
    public int maxNumberOfJumps;
    public int currentNumberOfJumps;

    public float shootCoolDown;
    public float currentShootTimer;

    public float meleeAttackCoolDown;
    public float currentmeleeAttackTimer;

    public LayerMask layersToHitWalls;
    public LayerMask groundLayer;
    public LayerMask layerForEnemies;

    public bool isGrounded;
    public bool hasJumped;
    public bool canShoot;
    //public bool canMeleeAttack;

    public string hitTag;

    public GameObject bulletPrefab;
    public Transform rayOrigin;
    public Transform bulletOrigin;

    public Collider[] enemies;

    public float enemyDetectRadius;
    public Transform currentlyFocusedEnemy;
    public Transform closestEnemy;

    public PlayerStats stats;

    public Animator meleeWeaponAnim;
    public GameObject meleeWeaponParent;

    public int numberOfColors;

    public float StunDuration;
    public bool isStunned;


    public Image dashImage, StunnedImage, reloadImage;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentShootTimer = shootCoolDown;
        currentDashTimer = originalDashTimer;
        currentmeleeAttackTimer = meleeAttackCoolDown;

        canShoot = true;
        DeactivateWeapon();

        numberOfColors = System.Enum.GetValues(typeof(AttackColor)).Length;
    }

    public void Update()
    {
        if (!isStunned)
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;

            Ray ray = new Ray(rayOrigin.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10, layersToHitWalls))
            {
                hitTag = hit.transform.tag;
            }
            else
            {
                hitTag = "";
            }

            UpdateHPSlider();
            CheckIsGrounded();
            GatherInput();
            Look();
            DetectEnemies();

            if (currentDashTimer > 0)
            {
                currentDashTimer -= Time.deltaTime;
            }

            if (currentShootTimer > 0)
            {
                currentShootTimer -= Time.deltaTime;
            }

            if (currentmeleeAttackTimer > 0)
            {
                currentmeleeAttackTimer -= Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isStunned)
        {
            Move();
        }
    }

    void GatherInput()
    {

        playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetMouseButtonDown(0))
        {
            Conductor.instance.CheckActionToBPM();

            if (canShoot)
            {
                if (currentShootTimer <= 0)
                {
                    currentShootTimer = shootCoolDown;
                    bullet b = Instantiate(bulletPrefab, bulletOrigin.transform.position, bulletOrigin.transform.rotation).GetComponent<bullet>();
                    b.SetForward(bulletOrigin);
                    stats.ReduceAmmo();

                    if(stats.currentAmmo <= 0)
                    {
                        canShoot = false;

                        StartCoroutine(Reload());
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Conductor.instance.CheckActionToBPM();

            if (currentmeleeAttackTimer <= 0)
            {
                currentmeleeAttackTimer = meleeAttackCoolDown;

                meleeWeaponParent.SetActive(true);
                meleeWeaponAnim.SetBool("Attack", true);

                SetAttackColor();
            }
        }

        if (currentDashTimer <= 0 && Input.GetKeyDown(KeyCode.LeftShift) && hitTag != "Wall")
        {
            currentDashTimer = originalDashTimer;

            dashImage.fillAmount = 0;
            LeanTween.value(dashImage.gameObject, 0, 1, originalDashTimer).setOnUpdate((float val) =>
            {
                dashImage.fillAmount = val;
            });

            Dash();
        }


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Conductor.instance.CheckActionToBPM();

            ySpeed = jumpPower;

            Vector3 newVelocity = new Vector3(rb.velocity.x, rb.velocity.y + ySpeed, rb.velocity.z);
            rb.velocity = newVelocity;
            hasJumped = true;
            currentNumberOfJumps--;
        }

        if(Input.GetKeyDown(KeyCode.Space) && !isGrounded)
        {
            if(currentNumberOfJumps > 0)
            {
                Conductor.instance.CheckActionToBPM();
                ySpeed = jumpPower;

                Vector3 newVelocity = new Vector3(rb.velocity.x, rb.velocity.y + ySpeed, rb.velocity.z);
                rb.velocity = newVelocity;

                currentNumberOfJumps--;
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeEnemyFocus();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SwtichAttackColor();
        }

    }

    private void SetAttackColor()
    {
        Material mat = meleeWeaponAnim.GetComponent<MeshRenderer>().materials[0];

        switch (stats.currentAttackColor)
        {
            case AttackColor.Green:
                mat.color = Color.green;
                break;
            case AttackColor.Blue:
                mat.color = Color.blue;
                break;
            case AttackColor.Red:
                mat.color = Color.red;
                break;
            case AttackColor.White:
                mat.color = Color.white;
                break;
            default:
                break;
        }
    }

    IEnumerator Reload()
    {
        reloadImage.fillAmount = 0;
        LeanTween.value(reloadImage.gameObject, 0, 1, stats.reloadTime).setOnUpdate((float val) =>
        {
            reloadImage.fillAmount = val;
        });

        yield return new WaitForSeconds(stats.reloadTime);
        stats.currentAmmo = stats.maxAmmo;
        canShoot = true;
    }
    void Look()
    {
        if(playerInput != Vector3.zero)
        {
            Vector3 relative = (transform.position + playerInput.ToIsometricMatrix()) - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relative, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, turnSpeed * Time.deltaTime);
        }
    }

    void Move()
    {
        rb.MovePosition(transform.position + (transform.forward * playerInput.magnitude) * speed * Time.fixedDeltaTime);
    }

    void Dash()
    {
        Conductor.instance.CheckActionToBPM();

        transform.position += transform.forward * dashDistance;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayOrigin.position, transform.forward * 10);

        Gizmos.DrawSphere(transform.position, enemyDetectRadius);
    }

    void CheckIsGrounded()
    {
        Ray ray = new Ray(rayOrigin.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1f, groundLayer))
        {
            if (!hasJumped)
            {
                isGrounded = true;
                ySpeed = 0;
                currentNumberOfJumps = maxNumberOfJumps;
            }
        }
        else
        {
            hasJumped = false;
            isGrounded = false;
        }
    }

    public void DetectEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyDetectRadius, layerForEnemies);

        enemies = hitColliders;

        if(enemies.Length > 0)
        {
            closestEnemy = GetClosestEnemy(enemies);
        }
        else
        {
            closestEnemy = null;
        }
    }

    Transform GetClosestEnemy (Collider[] enemies)
    {
        Transform bestTarget = null;

        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach(Collider potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }

        if (!currentlyFocusedEnemy)
        {
            currentlyFocusedEnemy = bestTarget;

            currentlyFocusedEnemy.GetComponent<Dummy>().ToggleFocusIndicator();
        }

        return bestTarget;
    }
    Transform GetClosestEnemy (Collider[] enemies, Transform currentFocusedEnemy)
    {
        Transform bestTarget = null;

        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach(Collider potentialTarget in enemies)
        {
            if(potentialTarget.transform != currentFocusedEnemy)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.transform;
                }
            }
        }
     
        return bestTarget;
    }

    public void ChangeEnemyFocus()
    {
        currentlyFocusedEnemy.GetComponent<Dummy>().ToggleFocusIndicator();

        currentlyFocusedEnemy = GetClosestEnemy(enemies, currentlyFocusedEnemy);

        currentlyFocusedEnemy.GetComponent<Dummy>().ToggleFocusIndicator();
    }

    public void DeactivateWeapon()
    {
        meleeWeaponParent.SetActive(false);
        meleeWeaponAnim.SetBool("Attack", false);
    }

    public void TakeDamage(int amount)
    {
        stats.currentHP -= amount;

        if(stats.currentHP <= 0)
        {
            stats.currentHP = stats.maxHP;
        }
    }

    public void SwtichAttackColor()
    {
        int attackColor = (int)stats.currentAttackColor;
        attackColor++;

        if (attackColor == numberOfColors)
        {
            attackColor = 0;
        }


        stats.currentAttackColor = (AttackColor)attackColor;

        Material playerMat = GetComponent<MeshRenderer>().materials[0];
        switch (stats.currentAttackColor)
        {
            case AttackColor.Green:
                playerMat.color = Color.green;
                break;
            case AttackColor.Blue:
                playerMat.color = Color.blue;
                break;
            case AttackColor.Red:
                playerMat.color = Color.red;
                break;
            case AttackColor.White:
                playerMat.color = Color.white;
                break;
            default:
                break;
        }
    }

    public void UpdateHPSlider()
    {
        playerHPSlider.value = stats.currentHP / stats.maxHP;
    }

    public void GetStunned()
    {
        isStunned = true;
        StartCoroutine(CountDownStun());
    }

    IEnumerator CountDownStun()
    {
        StunnedImage.fillAmount = 0;
        LeanTween.value(StunnedImage.gameObject, 0, 1, StunDuration).setOnUpdate((float val) =>
        {
            StunnedImage.fillAmount = val;
        });

        yield return new WaitForSeconds(StunDuration);
        isStunned = false;
    }

    public void AddHP(float amount)
    {
        stats.currentHP += amount;
    }


    public void AddAttackPower()
    {
        if(stats.attackPower < 3)
        {
            stats.attackPower++;
        }
    }

    public void ResetAttackPower()
    {
        stats.attackPower = 0;
    }
}
