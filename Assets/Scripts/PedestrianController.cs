using UnityEngine;
using UnityEngine.AI;
using System.Collections; // �ڷ�ƾ ����� ���� �߰�

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class PedestrianController : MonoBehaviour
{
    public Transform[] waypoints;
    public string speedParameterName = "Speed";
    public float waitTimeAtWaypoint = 1.5f; // �� �������� ����� �ð� (��)

    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;
    private Animator animator;
    private bool isWaiting = false; // ���� ��� ������ ���¸� ������ ����

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.updateRotation = false;
        if (waypoints == null || waypoints.Length < 2) // �պ��Ϸ��� �ּ� 2�� �ʿ�
        {
            Debug.LogError("Waypoints�� 2�� �̻� �������� �ʾҽ��ϴ�!");
            enabled = false;
            return;
        }

        // ó������ ù ��° waypoint�� ���� ��� �̵� ���� (�ε��� 0)
        agent.SetDestination(waypoints[0].position);
        // currentWaypointIndex�� ������ ���� �� ���� ����Ű���� 0���� �ʱ�ȭ (������ 1�� ������)
        currentWaypointIndex = 0;
    }

    void Update()
    {
        // ��� ���� �ƴ� ���� ���� ���� Ȯ��
        if (!isWaiting)
        {
            // ��ǥ ������ ����������� Ȯ��
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // ���������Ƿ� ��� �� ���� ��� ���� �ڷ�ƾ ����
                StartCoroutine(WaitAndGoToNext());
            }
        }

        // �ִϸ��̼� ������Ʈ�� ��� ����
        float currentSpeed = agent.velocity.magnitude;
        animator.SetFloat(speedParameterName, currentSpeed);
    }

    // ��� �� ���� ��ǥ �������� �̵��ϴ� �ڷ�ƾ
    IEnumerator WaitAndGoToNext()
    {
        isWaiting = true;
        agent.destination = transform.position; // ���ڸ� ����

        // ���� ��ǥ ���� ��� (���� �̵� ����� �ƴ�)
        int nextWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        Vector3 nextPosition = waypoints[nextWaypointIndex].position;

        // ���� ��ǥ ������ �ٶ󺸴� ���� ���
        Vector3 directionToNext = (nextPosition - transform.position).normalized;
        directionToNext.y = 0; // y�� ��ȭ�� ���� (���� ȸ����)

        Quaternion targetRotation = Quaternion.identity; // �⺻�� �ʱ�ȭ
        if (directionToNext != Vector3.zero) // ������ 0���Ͱ� �ƴ� ���� ���
        {
            targetRotation = Quaternion.LookRotation(directionToNext);
        }


        // ������ ��� �ð� ���� �ε巴�� ȸ�� + ���
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < waitTimeAtWaypoint)
        {
            // ���� �����ӿ��� ȸ�� (Slerp: ���� ���� ����)
            // ������ ����(elapsedTime / waitTimeAtWaypoint)�� 0���� 1�� �����ϸ� ȸ�� ����� ����
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / waitTimeAtWaypoint);

            elapsedTime += Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        // ȸ���� ���� �Ϸ�ǵ��� ���� ���� (����)
        transform.rotation = targetRotation;

        // ��Ⱑ �������� ���� ���� ��������Ʈ �ε��� ������Ʈ �� �̵� ���
        currentWaypointIndex = nextWaypointIndex;
        agent.SetDestination(nextPosition);

        isWaiting = false;
    }

}