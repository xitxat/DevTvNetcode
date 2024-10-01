using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

//  TUTORIAL: Primary fire == Attack

[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]

public class InputReader : ScriptableObject, IPlayerActions
{
    // enables button hold down
    public event Action<bool> AttactEvent;
    public event Action<Vector2> MoveEvent;

    private Controls controls;

    // SO's exist via :
    private void OnEnable()
    {
        controls ??= new Controls();
        controls.Player.SetCallbacks(this);

        controls.Player.Enable();

    }

    // Disable controls to prevent leaks and performance issues
    private void OnDisable()
    {
        controls.Player.Disable();
    }

    // Context holds data re the event
    public void OnAttack(InputAction.CallbackContext context)
    {
        // null check + bool assignment
        AttactEvent?.Invoke(context.performed);

        //AttactEvent.Invoke(context.performed ? true : context.canceled ? false : default);
        // rfkt to
        //if (context.performed) AttactEvent.Invoke(true);
        //else if (context.canceled) AttactEvent.Invoke(false);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }



    public void OnCrouch(InputAction.CallbackContext context)
    {
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
    }

    public void OnJump(InputAction.CallbackContext context)
    {
    }

    public void OnLook(InputAction.CallbackContext context)
    {
    }



    public void OnNext(InputAction.CallbackContext context)
    {
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
    }
}
