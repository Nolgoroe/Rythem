using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

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

    public LayerMask layersToHitWalls;
    public LayerMask groundLayer;

    public bool isGrounded;
    public bool hasJumped;
    public bool canShoot;

    public string hitTag;

    public GameObject bulletPrefab;
    public Transform rayOrigin;
    public Transform bulletOrigin;

    private void Start()
    {
        currentShootTimer = shootCoolDown;
        currentDashTimer = originalDashTimer;
    }

    public void Update()
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


        CheckIsGrounded();
        GatherInput();
        Look();

        if (currentDashTimer > 0)
        {
            currentDashTimer -= Time.deltaTime;
        }

        if (currentShootTimer > 0)
        {
            currentShootTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    void GatherInput()
    {

        playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetMouseButtonDown(0))
        {
            Conductor.instance.CheckActionToBPM();

            if (currentShootTimer <= 0)
            {
                currentShootTimer = shootCoolDown;
                bullet b = Instantiate(bulletPrefab, bulletOrigin.transform.position, bulletOrigin.transform.rotation).GetComponent<bullet>();
                b.SetForward(bulletOrigin);
            }
        }

        if (currentDashTimer <= 0 && Input.GetKeyDown(KeyCode.LeftShift) && hitTag != "Wall")
        {
            currentDashTimer = originalDashTimer;
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
}
