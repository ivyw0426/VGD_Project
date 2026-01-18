using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;

    public float moveSpeed;
    public float walkSpeed = 6f;
    public float sprintSpeed = 6f;
    public float gravity = -15f;
    public float jumpHeight = 1f;

    public KeyCode sprintKey = KeyCode.LeftShift;
    
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    Vector3 velocity;
    bool isGrounded;
    
    public MovementState state;
    //state of movement
    public enum MovementState
    {
        walking,
        sprinting,
        air
    }

    private void StateHandler()
    {
        if(isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        } else if (isGrounded){
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        } else
        {
            state = MovementState.air;
        }
    }

    void Update()
    {
        //check if player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * moveSpeed * Time.deltaTime);


        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        StateHandler();
    }
}
