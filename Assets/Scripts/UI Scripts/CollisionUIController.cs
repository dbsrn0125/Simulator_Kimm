using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CollisionUIController : MonoBehaviour
{
    public CanvasGroup redOverlayCanvasGroup;
    public TextMeshProUGUI collisionMessageText;
    public float fadeDuration = 1f;
    public float displayDuration = 2f;
    private CarController carController;

    private void OnEnable()
    {
        // �ڵ��� �浹 �̺�Ʈ�� ����
        CarController.OnCarCollision += ShowCollisionUI;
    }

    private void OnDisable()
    {
        // �̺�Ʈ ���� ����
        CarController.OnCarCollision -= ShowCollisionUI;
    }

    private void Start()
    {
        // �ʱ� ���·� UI ��Ȱ��ȭ
        redOverlayCanvasGroup.alpha = 0;
        collisionMessageText.gameObject.SetActive(false);
        carController = FindObjectOfType<CarController>();
    }

    private void ShowCollisionUI()
    {
        // UI ǥ�� �ڷ�ƾ ����
        StartCoroutine(HandleCollisionUI());
    }

    private IEnumerator HandleCollisionUI()
    {
        // �޽��� Ȱ��ȭ
        collisionMessageText.gameObject.SetActive(true);
        collisionMessageText.text = "Collision Detected!!!";

        // ���̵� ��
        yield return StartCoroutine(FadeIn());

        // ������ �ð� ���� UI ����
        yield return new WaitForSeconds(displayDuration);

        // ���̵� �ƿ�
        yield return StartCoroutine(FadeOut());

        // �޽��� ��Ȱ��ȭ
        collisionMessageText.gameObject.SetActive(false);

        // UI ó���� ���� �Ŀ� �ڵ��� �ʱ� ��ġ�� ����
        carController.CollisionResetCarPosition();
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            redOverlayCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            redOverlayCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }
    }
}
