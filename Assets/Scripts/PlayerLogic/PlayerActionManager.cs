
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerActionManager : MonoBehaviour
{
    public UnityEvent jump;
    public UnityEvent jumpHold;
    public UnityEvent<int> moveCheck;

    /*** Keyboard Callbacks ***/

    void Start()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.ActivateInput();
        }
        else
        {
            Debug.LogError("PlayerInput not found on this GameObject!");
        }
    }

    void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            // Debug.Log("KEY PRESSED - Input System is working!");
        }
    }


    public void OnJumpHoldAction(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("JumpHold was started");
        else if (context.performed)
        {
            Debug.Log("JumpHold was performed");
            Debug.Log(context.duration);
            jumpHold.Invoke();
        }
        else if (context.canceled)
            Debug.Log("JumpHold was cancelled");
    }

    // called twice, when pressed and unpressed
    public void OnJumpAction(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("Jump was started");
        else if (context.performed)
        {
            jump.Invoke();
            Debug.Log("Jump was performed");
        }
        else if (context.canceled)
            Debug.Log("Jump was cancelled");

    }

    // called twice, when pressed and unpressed
    public void OnMoveAction(InputAction.CallbackContext context)
    {
        // Debug.Log("OnMoveAction callback invoked");
        if (context.started || context.performed)
        {
            // Debug.Log("move started");
            float move = context.ReadValue<float>();
            // Debug.Log($"move value: {move}");
            int faceRight = context.ReadValue<float>() > 0 ? 1 : -1;
            moveCheck.Invoke(faceRight);
        }
        if (context.canceled)
        {
            // Debug.Log("move stopped");
            moveCheck.Invoke(0);
        }

    }

    /*** Mouse Callbacks ***/

    public void OnClickAction(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("mouse click started");
        else if (context.performed)
        {
            Debug.Log("mouse click performed");
        }
        else if (context.canceled)
            Debug.Log("mouse click cancelled");
    }

    public void OnPointAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 point = context.ReadValue<Vector2>();
            Debug.Log($"Point detected: {point}");

        }
    }

}
