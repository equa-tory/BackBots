using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    //PlayerInput input;

    public Vector2 movementInput;

    public bool meleeInventory_Input;
    public bool recoveryInventory_Input;
    public bool sprint_Input;
    
    [Header("Inventory")]
    public bool left_Input;
    public bool right_Input;

    private void Awake()
    {
        Instance = this;
    }

    /*private void OnEnable()
    {
        if (input == null)
        {
            input = new PlayerInput();

            input.Main.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();

            input.Inventory.MeleeInventory.performed += ctx => meleeInventory_Input = true;
            input.Inventory.MeleeInventory.canceled += ctx => meleeInventory_Input = false;

            input.Inventory.RecoveryInventory.performed += ctx => recoveryInventory_Input = true;
            input.Inventory.RecoveryInventory.canceled += ctx => recoveryInventory_Input = false;

            input.Actions.Sprint.performed += ctx => sprint_Input = true;
            input.Actions.Sprint.canceled += ctx => sprint_Input = false;

            input.Inventory.Left.performed += ctx => left_Input = true;
            input.Inventory.Left.canceled += ctx => left_Input = false;

            input.Inventory.Right.performed += ctx => right_Input = true;
            input.Inventory.Right.canceled += ctx => right_Input = false;

        }

        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }*/

    private void Update()
    {
        AllInputs();
    }

    public void AllInputs()
    {

    }


}