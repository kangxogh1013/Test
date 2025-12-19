using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    public Transform firstPersonCamera;
    public Transform thirdPersonCamera;
    public float mouseSensitivity = 2f;
    public float thirdPersonDistance = 5f;
    public float thirdPersonHeight = 2f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isFirstPerson = false;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 커서 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 초기 카메라 설정 (3인칭)
        SetCameraMode(false);
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleCameraSwitch();
    }

    void HandleMovement()
    {
        // 땅에 닿았는지 체크
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // WASD 입력
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 이동 방향 계산
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // 달리기 (Shift 키)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        // 점프 (Space 키)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        // 마우스 입력
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 좌우 회전 (Y축)
        yRotation += mouseX;
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

        // 상하 회전 (X축) - 카메라만
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (isFirstPerson)
        {
            firstPersonCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        else
        {
            thirdPersonCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    void HandleCameraSwitch()
    {
        // T 키로 카메라 전환
        if (Input.GetKeyDown(KeyCode.T))
        {
            isFirstPerson = !isFirstPerson;
            SetCameraMode(isFirstPerson);
        }
    }

    void SetCameraMode(bool firstPerson)
    {
        if (firstPerson)
        {
            // 1인칭 모드
            firstPersonCamera.gameObject.SetActive(true);
            thirdPersonCamera.gameObject.SetActive(false);
        }
        else
        {
            // 3인칭 모드
            firstPersonCamera.gameObject.SetActive(false);
            thirdPersonCamera.gameObject.SetActive(true);
        }
    }
}
