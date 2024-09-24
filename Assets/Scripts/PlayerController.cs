using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    
    [Header("variable - you can change these to feel right")]

    #region movement
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 3f;
    private float moveSpeed;
    private Vector3 targetVelocity;

    public float maxStamina = 5f;
    private float stamina;
    private bool allowSprint = true;
    public float jumpStrength = 10f;
    public float airSpeed = 0.1f;

    private Vector3 moveVector;
    #endregion


    #region ground and gravity
    public float groundCheckDistance;
    public float gravity = 1f;
    public float maxSlopeAngle = 40f;
    private RaycastHit slopeHit;
    private float playerHeight;
    #endregion


    [Header("static - DO N0T TOUCH THESE")]

    #region transforms
    public Transform cameraDirection;
    public Transform player;
    public CapsuleCollider playerCollider;
    #endregion

    #region controls
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    #endregion

    #region UI
    private RawImage staminaColor;
    #endregion

    #region private ground and gravity
    private bool onPlatform = false;
    private Vector3 deltaPos;
    private Vector3 oldPos;
    private Rigidbody rb;
    private bool isGrounded = true;
    #endregion

    #region hidden variables
    [HideInInspector]
    [Header("invisible - everything here should not be visible")]

    public bool crouchMode = false;
    private float headCheckDist = 2f;
    #endregion



    #region functions
    private Vector3 origin()
    {
        return new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * 0.5f), transform.position.z);
    }
    private static bool isMoving()
    {
        return (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);
    }

    private bool OnSlope()
    {
        //Debug.Log("I run");
        //Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance);
        if (Physics.Raycast(origin(), Vector3.down, out slopeHit, groundCheckDistance))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            //Debug.Log(angle);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDir(Vector3 input)
    {
        return Vector3.ProjectOnPlane(input, slopeHit.normal).normalized;
    }
    #endregion



    // Start is called before the first frame update
    void Start()
    {
        playerHeight = transform.localScale.y;
        groundCheckDistance = playerHeight * 0.5f + 0.3f;
        rb = GetComponent<Rigidbody>();
        stamina = maxStamina;
        moveSpeed = walkSpeed;
        staminaColor = GetComponentInChildren<RawImage>();
    }


    private void Update()
    {
        #region Platform Velocity

        deltaPos = transform.position - oldPos;
        oldPos = transform.position;

        #endregion  

        #region Jumping

        if (isGrounded && Input.GetKeyDown(jumpKey))
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(0f, jumpStrength, 0f, ForceMode.Impulse);
            isGrounded = false;
        }
        //Debug.Log(isGrounded);

        #endregion

        #region Sprinting
        //Debug.Log("my stamina is: " + stamina);
        if (Input.GetKey(sprintKey) && !crouchMode && stamina > 0 && allowSprint && isMoving())
        {
            if (moveSpeed < sprintSpeed)
            {
                moveSpeed += 0.1f;
            }

            stamina -= Time.deltaTime;
        }
        else
        {
            if (moveSpeed > walkSpeed)
            {
                moveSpeed -= 0.1f;
            }
            
            if (stamina < 0)
            {
                allowSprint = false;
            }
            if (!allowSprint && stamina > maxStamina / 5)
            {
                allowSprint = true;
            }
            if (stamina < maxStamina)
            {
                if (moveSpeed <= walkSpeed)
                    stamina += Time.deltaTime/2;
            }
            else
                stamina = maxStamina;
            
        }
        #endregion

        #region Crouching
/*        if (!crouchMode)
        {
            Debug.DrawRay(transform.position, Vector3.up * 1f, Color.red);
        }
        else
        {
            Debug.DrawRay(origin(), Vector3.up * headCheckDist, Color.red);
        }*/

        if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit2, 1f) && isGrounded)
        {
            player.localScale = new Vector3(1, 0.5f, 1);
            playerCollider.height = 1f;
            crouchMode = true;
            if (!crouchMode && isGrounded)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            }
        }
        if (Input.GetKey(crouchKey))
        {
            if (!crouchMode && isGrounded)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            }
            crouchMode = true;
            player.localScale = new Vector3(1, 0.5f, 1);
            playerCollider.height = 1f;
            //Debug.DrawRay(transform.position, Vector3.up * headCheckDist, Color.red);
        }
        else if (!Physics.Raycast(origin(), Vector3.up, out RaycastHit hit1, headCheckDist) || hit1.collider.gameObject.CompareTag("Player") || hit1.collider.gameObject.CompareTag("FirstPersonArea") || !crouchMode)
        {
            crouchMode = false;
            player.localScale = new Vector3(1, 1, 1);
            playerCollider.height = 2;
            if (moveSpeed < walkSpeed)
            {
                moveSpeed += 0.1f;
            }
        }
        if (crouchMode && moveSpeed > crouchSpeed)
        {
            moveSpeed -= 0.1f;
        }
        #endregion

        #region UI

        if (stamina < 2f)
        {
            
            staminaColor.color = new Color(0f, 0f, 0f, (stamina * -0.5f + 1));
        }
        else
        {
            staminaColor.color = new Color(0f, 0f, 0f, 0);
        }


        #endregion
    }
    private void FixedUpdate()
    {
        #region Player Movement
        //creates the initial direction you want to go to according to inputs
        targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (targetVelocity.magnitude > 1f)
        {
            targetVelocity = targetVelocity.normalized;
        }


        //rotates this direction towards the camera rotation
        targetVelocity = Quaternion.AngleAxis(cameraDirection.localRotation.eulerAngles.y, Vector3.up) * transform.TransformDirection(targetVelocity) * moveSpeed;


        //if the velocity is above 0, it rotates the player accordingly
        if (targetVelocity.magnitude > 0)
        {
            if (isGrounded)
            {
                player.forward = Vector3.Slerp(player.forward, -targetVelocity.normalized, 0.1f) * Time.deltaTime;
            }
            else
            {
                player.forward = Vector3.Slerp(player.forward, -targetVelocity.normalized, 0.1f * airSpeed) * Time.deltaTime;
            }
        }



        //rotates the moveVector towards the forward vector of the player (I think this is better for immersive player controls
        //targetVelocity = -player.forward * targetVelocity.magnitude;



        //limiting the speed?
        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.y = 0;

        //projects the movement of the player according to a slope if player is on a slope
        if (OnSlope())
        {
            targetVelocity = GetSlopeMoveDir(targetVelocity);
            if (rb.velocity.y > 0 && isGrounded)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
                //Debug.Log("pushing down");
            }
            else if (isGrounded && rb.velocity.y <= 0 && isMoving())
            {
                rb.AddForce(-slopeHit.normal * 80f, ForceMode.Force);
            }
        }
        //turns gravity off while on slope so that you dont slowly slide downwards
        rb.useGravity = !OnSlope();


        if (isGrounded)
        {
            //adds force to the player, thus moves the player
            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        else
        {
            //adds force to the player, thus moves the player
            rb.AddForce(velocityChange * airSpeed, ForceMode.VelocityChange);
            Debug.DrawRay(origin(), velocityChange * airSpeed);
        }
        #endregion

        #region Ground Check
        Vector3 direction = Vector3.down;

        if (Physics.Raycast(origin(), direction, out RaycastHit hit, groundCheckDistance))
        {
            //Debug.DrawRay(origin, direction * groundCheckDistance, Color.red);
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if (angle < maxSlopeAngle)
                isGrounded = true;
            else
            {
                Debug.Log("TOO STEEP");
                isGrounded = false;
                rb.AddForce(Vector3.Project(hit.normal, targetVelocity) * airSpeed * moveSpeed * 2f, ForceMode.VelocityChange);
            }
        }
        else
        {
            if (Physics.Raycast(origin() + player.transform.forward * -0.5f, direction, out RaycastHit hit1, groundCheckDistance) && Vector3.Angle(Vector3.up, hit1.normal) > maxSlopeAngle)
            {

                    Debug.Log("TOO STEEP");
                    isGrounded = false;
                    rb.AddForce(Vector3.down * airSpeed * moveSpeed/4, ForceMode.VelocityChange);

            }
            isGrounded = false;
        }
        Debug.DrawRay(origin(), direction * groundCheckDistance, Color.red);
        Debug.DrawRay(origin() + player.transform.forward * -0.5f, direction * groundCheckDistance, Color.red);

        #endregion
    }


    #region Platform and ConveyerBelt

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Platform") && other.transform.position.y < transform.position.y)
        {
            transform.parent = other.transform;
            onPlatform = true;
        }
        if (other.CompareTag("ConveyorBelt"))
        {
            Vector3 pushForce = other.transform.forward * other.GetComponent<ConveyorScript>().conveyorSpeed;
            pushForce.y = 0;
            Debug.Log("Im conveyoing");
            if (isGrounded)
            {
                Debug.Log("as grounded");
                rb.AddForce(pushForce, ForceMode.VelocityChange);
            }
            else
            {
                Debug.Log("NOT as grounded");

                rb.AddForce(pushForce * airSpeed , ForceMode.VelocityChange);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Platform"))
        {
            transform.parent = null;
            rb.AddForce(deltaPos.normalized * 5, ForceMode.Impulse);
            onPlatform = false;
        }

    }

    #endregion
}