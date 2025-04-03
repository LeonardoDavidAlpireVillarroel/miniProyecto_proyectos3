using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchCamera : MonoBehaviour

{
    [SerializeField]
    private PlayerInput playerInput;
    private CinemachineCamera virtualCamera;
    private InputAction aimAction;
    private int priorityBoostAmount = 10;
    [SerializeField]
    private Canvas thirdPersonCanvas;    
    [SerializeField]
    private Canvas aimCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
        aimAction = playerInput.actions["Aim"];
    }

    private void OnEnable()
    {
        aimAction.performed += _ => StartAim();
        aimAction.canceled += _ => CancelAim();

    }

    private void OnDisable()
    {
        aimAction.performed -= _ => StartAim();
        aimAction.canceled -= _ => CancelAim();
    }

    private void StartAim()
    {
        virtualCamera.Priority += priorityBoostAmount;

        if (aimCanvas != null)
            aimCanvas.enabled = false;

        if (thirdPersonCanvas != null)
            thirdPersonCanvas.enabled = true;
    }
    private void CancelAim()
    {
        virtualCamera.Priority -= priorityBoostAmount;

        if (aimCanvas != null)
            aimCanvas.enabled = true;

        if (thirdPersonCanvas != null)
            thirdPersonCanvas.enabled = false;
    }



}
