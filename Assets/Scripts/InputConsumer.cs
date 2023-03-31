using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputConsumer : MonoBehaviour
{
    private Controls controls;
    private LSystemGenerator generator;

    private void Awake()
    {
        controls = new Controls();
    }

    private void OnEnable()
    {
        controls.Player.Tap.performed += Tap;
        controls.Player.Enable();
    }

    private void Tap(InputAction.CallbackContext obj)
    {
        generator.Generate(clean: true);
    }

    private void OnDisable()
    {
        controls.Player.Tap.performed -= Tap;
    }

}
