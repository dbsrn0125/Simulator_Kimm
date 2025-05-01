using UnityEngine;
using UnityEngine.AI;
using System.Collections; // 코루틴 사용을 위해 추가

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class PedestrianController : MonoBehaviour
{
    public Transform[] waypoints;
    public string speedParameterName = "Speed";
    public float waitTimeAtWaypoint = 1.5f; // 각 지점에서 대기할 시간 (초)

    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private Animator animator;
    private bool isWaiting = false; // 현재 대기 중인지 상태를 저장할 변수

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.updateRotation = false;
        if (waypoints == null || waypoints.Length < 2) // 왕복하려면 최소 2개 필요
        {
            Debug.LogError("Waypoints가 2개 이상 설정되지 않았습니다!");
            enabled = false;
            return;
        }

        // 처음에는 첫 번째 waypoint를 향해 즉시 이동 시작 (인덱스 0)
        agent.SetDestination(waypoints[0].position);
        // currentWaypointIndex는 다음에 가야 할 곳을 가리키도록 0으로 초기화 (다음에 1로 증가됨)
        currentWaypointIndex = 0;
    }

    void Update()
    {
        // 대기 중이 아닐 때만 도착 여부 확인
        if (!isWaiting)
        {
            // 목표 지점에 가까워졌는지 확인
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // 도착했으므로 대기 및 다음 경로 설정 코루틴 시작
                StartCoroutine(WaitAndGoToNext());
            }
        }

        // 애니메이션 업데이트는 계속 수행
        float currentSpeed = agent.velocity.magnitude;
        animator.SetFloat(speedParameterName, currentSpeed);
    }

    // 대기 후 다음 목표 지점으로 이동하는 코루틴
    IEnumerator WaitAndGoToNext()
    {
        isWaiting = true;
        agent.destination = transform.position; // 제자리 멈춤

        // 다음 목표 지점 계산 (아직 이동 명령은 아님)
        int nextWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        Vector3 nextPosition = waypoints[nextWaypointIndex].position;

        // 다음 목표 지점을 바라보는 방향 계산
        Vector3 directionToNext = (nextPosition - transform.position).normalized;
        directionToNext.y = 0; // y축 변화는 무시 (수평 회전만)

        Quaternion targetRotation = Quaternion.identity; // 기본값 초기화
        if (directionToNext != Vector3.zero) // 방향이 0벡터가 아닐 때만 계산
        {
            targetRotation = Quaternion.LookRotation(directionToNext);
        }


        // 설정된 대기 시간 동안 부드럽게 회전 + 대기
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < waitTimeAtWaypoint)
        {
            // 현재 프레임에서 회전 (Slerp: 구면 선형 보간)
            // 마지막 인자(elapsedTime / waitTimeAtWaypoint)는 0에서 1로 증가하며 회전 진행률 결정
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / waitTimeAtWaypoint);

            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 회전이 거의 완료되도록 최종 설정 (보정)
        transform.rotation = targetRotation;

        // 대기가 끝났으니 실제 다음 웨이포인트 인덱스 업데이트 및 이동 명령
        currentWaypointIndex = nextWaypointIndex;
        agent.SetDestination(nextPosition);

        isWaiting = false;
    }

}