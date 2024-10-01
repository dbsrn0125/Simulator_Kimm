using UnityEngine;
using System.Collections;

public class UIToggle : MonoBehaviour
{
    public GameObject uiPanel; // UI 오브젝트
    public float slideDuration = 0.5f; // 슬라이드 애니메이션 시간
    private bool isUIActive = false; // UI 활성화 상태

    private RectTransform uiRectTransform;

    void Start()
    {
        uiRectTransform = uiPanel.GetComponent<RectTransform>();
        // 초기 위치를 화면 밖으로 설정
        uiRectTransform.anchoredPosition = new Vector2(-uiRectTransform.rect.width, 0);
        uiPanel.SetActive(true); // UI를 활성화
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isUIActive)
            {
                StartCoroutine(SlideOut());
            }
            else
            {
                StartCoroutine(SlideIn());
            }
            isUIActive = !isUIActive;
        }
    }

    private IEnumerator SlideIn()
    {
        float elapsedTime = 0;
        Vector2 startPosition = new Vector2(-uiRectTransform.rect.width, 0);
        Vector2 endPosition = Vector2.zero;

        while (elapsedTime < slideDuration)
        {
            uiRectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, (elapsedTime / slideDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        uiRectTransform.anchoredPosition = endPosition; // 최종 위치 설정
    }

    private IEnumerator SlideOut()
    {
        float elapsedTime = 0;
        Vector2 startPosition = Vector2.zero;
        Vector2 endPosition = new Vector2(-uiRectTransform.rect.width, 0);

        while (elapsedTime < slideDuration)
        {
            uiRectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, (elapsedTime / slideDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        uiRectTransform.anchoredPosition = endPosition; // 최종 위치 설정
    }
}
