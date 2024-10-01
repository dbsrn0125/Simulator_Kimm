using UnityEngine;
using System.Collections;

public class UIToggle : MonoBehaviour
{
    public GameObject uiPanel; // UI ������Ʈ
    public float slideDuration = 0.5f; // �����̵� �ִϸ��̼� �ð�
    private bool isUIActive = false; // UI Ȱ��ȭ ����

    private RectTransform uiRectTransform;

    void Start()
    {
        uiRectTransform = uiPanel.GetComponent<RectTransform>();
        // �ʱ� ��ġ�� ȭ�� ������ ����
        uiRectTransform.anchoredPosition = new Vector2(-uiRectTransform.rect.width, 0);
        uiPanel.SetActive(true); // UI�� Ȱ��ȭ
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

        uiRectTransform.anchoredPosition = endPosition; // ���� ��ġ ����
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

        uiRectTransform.anchoredPosition = endPosition; // ���� ��ġ ����
    }
}
