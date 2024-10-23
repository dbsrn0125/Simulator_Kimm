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
        // 자동차 충돌 이벤트를 구독
        CarController.OnCarCollision += ShowCollisionUI;
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제
        CarController.OnCarCollision -= ShowCollisionUI;
    }

    private void Start()
    {
        // 초기 상태로 UI 비활성화
        redOverlayCanvasGroup.alpha = 0;
        collisionMessageText.gameObject.SetActive(false);
        carController = FindObjectOfType<CarController>();
    }

    private void ShowCollisionUI()
    {
        // UI 표시 코루틴 실행
        StartCoroutine(HandleCollisionUI());
    }

    private IEnumerator HandleCollisionUI()
    {
        // 메시지 활성화
        collisionMessageText.gameObject.SetActive(true);
        collisionMessageText.text = "Collision Detected!!!";

        // 페이드 인
        yield return StartCoroutine(FadeIn());

        // 지정된 시간 동안 UI 유지
        yield return new WaitForSeconds(displayDuration);

        // 페이드 아웃
        yield return StartCoroutine(FadeOut());

        // 메시지 비활성화
        collisionMessageText.gameObject.SetActive(false);

        // UI 처리가 끝난 후에 자동차 초기 위치로 리셋
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
