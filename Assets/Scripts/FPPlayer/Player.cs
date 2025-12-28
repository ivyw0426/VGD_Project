using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(FPController))]
public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] FPController FPController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    #region Input Handling
    void onMove(InputValue value)
    {
        FPController.moveInput = value.Get<Vector2>();
    }

    void onLook(InputValue value)
    {
        FPController.moveInput = value.Get<Vector2>();
    }
    #endregion

    #region Unity Methods
    void OnValidate()
    {
        if(FPController == null) FPController = GetComponent<FPController>();
    }

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    #endregion

}
