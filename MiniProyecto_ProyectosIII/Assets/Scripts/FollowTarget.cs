using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform followTarget;

    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float bottonClamp = -40f;
    [SerializeField] private float topClamp = 70f;

    private float cinemachineTargetPitch;
    private float cinemachineTargetYaw;

    private void LateUpdate()
    {
        CameraLogic();
    }
    private void CameraLogic()
    {
        float mouseX = GetMouseInput("Mouse X");
        float mouseY = GetMouseInput("Mouse Y");

        cinemachineTargetPitch = UpdateRotation(cinemachineTargetPitch, mouseY, bottonClamp, topClamp, true);
        cinemachineTargetYaw=UpdateRotation(cinemachineTargetYaw, mouseX, float.MinValue, float.MaxValue, false);

        ApplyRotation(cinemachineTargetPitch, cinemachineTargetYaw);
    }

    private void ApplyRotation(float pitch, float yaw)
    {
        followTarget.rotation = Quaternion.Euler(pitch, yaw, followTarget.eulerAngles.z);
    }

    private float UpdateRotation(float currentRotation, float input, float min, float max, bool isXAxis)
    {
        currentRotation += isXAxis ? -input : input;
        return Mathf.Clamp(currentRotation, min, max);
    }
    private float GetMouseInput(string axis)
    {
        return Input.GetAxis(axis) * rotationSpeed * Time.deltaTime;
    }
}
