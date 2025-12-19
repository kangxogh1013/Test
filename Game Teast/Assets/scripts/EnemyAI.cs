using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Detection Settings")]
    public float detectionRange = 15f;  // 플레이어를 감지하는 거리
    public float stopRange = 20f;       // 이 거리 이상이면 멈춤

    [Header("Movement Settings")]
    public float moveSpeed = 4f;        // 이동 속도
    public float stopDistance = 1f;     // 플레이어에 이 거리까지 접근

    [Header("Gravity Settings")]
    public float gravity = -9.81f;      // 중력

    private Rigidbody rb;
    private bool isChasing = false;
    private float distanceToPlayer;
    private Vector3 velocity = Vector3.zero;  // 수직 속도 저장

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Rigidbody 설정 - 물리 상호작용 최소화
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;  // 중력 비활성화 (수동으로 처리)
        rb.isKinematic = false; // Kinematic 아님

        // 플레이어를 자동으로 찾기
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;

            // 태그가 없으면 수동으로 찾기
            if (player == null)
            {
                player = GameObject.Find("Player")?.transform;
            }
        }

        if (player == null)
        {
            Debug.LogError("플레이어를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        if (player == null) return;

        // 플레이어까지의 거리 계산
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 추적 범위 체크
        if (distanceToPlayer < detectionRange)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > stopRange)
        {
            isChasing = false;
        }

        // 추적 또는 정지
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            StopMovement();
        }

        // 중력 적용
        ApplyGravity();
    }

    void ChasePlayer()
    {
        // 플레이어 방향 계산 (Y축 제외)
        Vector3 directionToPlayer = new Vector3(player.position.x - transform.position.x,
                                               0,
                                               player.position.z - transform.position.z).normalized;

        // 플레이어를 바라보도록 회전 (Y축만)
        if (directionToPlayer.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }

        // 플레이어와의 거리가 stopDistance보다 크면 이동
        if (distanceToPlayer > stopDistance)
        {
            // 수평 이동만 (중력은 따로 처리)
            Vector3 horizontalMovement = directionToPlayer * moveSpeed * Time.deltaTime;
            transform.position += horizontalMovement;
        }
    }

    void StopMovement()
    {
        // 정지 상태 (중력은 계속 적용됨)
    }

    void ApplyGravity()
    {
        // 땅에 닿았는지 체크 (Raycast 사용)
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.6f);

        if (isGrounded)
        {
            // 땅에 닿으면 수직 속도 초기화
            velocity.y = 0f;
        }
        else
        {
            // 공중에 있으면 중력 적용
            velocity.y += gravity * Time.deltaTime;
        }

        // 수직 이동 적용
        transform.position += new Vector3(0, velocity.y * Time.deltaTime, 0);
    }

    // 디버그: 감지 범위를 Scene에 표시
    void OnDrawGizmosSelected()
    {
        // 감지 범위 (파란색)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 중단 범위 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);

        // Raycast 표시 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 0.6f);
    }
}
