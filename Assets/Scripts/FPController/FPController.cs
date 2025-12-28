using UnityEngine;
using Unity.Cinemachine;
[RequireComponent(typeof(CharacterController))]
public class FPController : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float maxSpeed = 3.5f;
    public float acceleration = 15f;

    public Vector3 currentVelocity {get; private set; }
    public float currentSpeed {get; private set; }

    [Header("Looking Parameters")]
    public Vector2 lookSensitivity = new Vector2(0.1f, 0.1f);
    //limit pitch angle, e.g. how much you can look up and down
    public float pitchLimit = 85f;

    [SerializeField] float currentPitch = 0f;

    public float CurrentPitch
    { 
        get => currentPitch;
        set
        {
            currentPitch = Mathf.Clamp(value, -pitchLimit, pitchLimit);
        }
    }


    [Header("Input")]
    public Vector2 moveInput;
    public Vector2 lookInput;
    //to see in inspector
    [Header("Components")]
   [SerializeField]CharacterController characterController;
   [SerializeField]CinemachineCamera fpCamera;

   #region Unity Methods

   void OnValidate()
    {
        if(characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        MoveUpdate();
        LookUpdate();
    }
    #endregion

    #region Contorller Methods
    void MoveUpdate()
    {
        Vector3 motion = transform.forward * moveInput.y + transform.right * moveInput.x;
        motion.y = 0f;
        motion.Normalize();

        if(motion.sqrMagnitude >= 0.01f)
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, motion * maxSpeed, acceleration * Time.deltaTime);
        } else
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, acceleration * Time.deltaTime);
        }

        
        float verticalVelocity = Physics.gravity.y * 20f * Time.deltaTime;

        Vector3 fullVelocity = new Vector3(currentVelocity.x, verticalVelocity, currentVelocity.z);


        characterController.Move(fullVelocity * Time.deltaTime);

        //updating speed
        currentSpeed = currentVelocity.magnitude;
    }

    void LookUpdate()
    {
        Vector2 input = new Vector2(lookInput.x * lookSensitivity.x, lookInput.y * lookSensitivity.y);
        //lookig up and down
        CurrentPitch -= input.y;
        fpCamera.transform.localRotation = Quaternion.Euler(CurrentPitch, 0f, 0f);

        //looking lefet and right
        transform.Rotate(Vector3.up * input.x);
    }
    #endregion

    
}
