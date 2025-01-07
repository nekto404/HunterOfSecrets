using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    public RectTransform content;       // ��������� � ����������
    public RectTransform viewport;      // ������ ������� (Viewport)
    public ScrollRect scrollRect;       // ��������� Scroll Rect
    public Button scrollLeftButton;     // ������ ��� ������������� ������
    public Button scrollRightButton;    // ������ ��� ������������� ��������

    private float itemWidth;            // ������ ������ ��������
    private float maxScrollPosition;    // ����������� ���������
    private float minScrollPosition;    // ̳������� ���������
    [SerializeField]
    private int visibleItems = 12;      // ʳ������ ������� �������� ���������

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        // ���������� ������� ������ ������ ��������
        itemWidth = viewport.rect.width / visibleItems;

        // ���������� ����������� � �������� ������� �������������
        maxScrollPosition = 0;
        minScrollPosition = -content.rect.width + viewport.rect.width;

        // �������� �������, ���� ���� ������ ����� �� ������ viewport
        if (content.rect.width <= viewport.rect.width)
        {
            StartCoroutine(CenterContent());
        }

        // ϳ�������� ����� ��������� �� ������
        scrollLeftButton.onClick.AddListener(() => Scroll(-1));
        scrollRightButton.onClick.AddListener(() => Scroll(1));

        UpdateButtons();  // ��������� ����� ������
    }

    private IEnumerator CenterContent()
    {
        yield return new WaitForEndOfFrame();
        // �������� content �������� viewport
        float centeredPositionX = (viewport.rect.width - content.rect.width) / 2;
        content.anchoredPosition = new Vector2(centeredPositionX, content.anchoredPosition.y);
    }

    private void Scroll(int direction)
    {
        // ���������� ���� �������
        float newPosition = content.anchoredPosition.x + direction * itemWidth;
        newPosition = Mathf.Clamp(newPosition, minScrollPosition, maxScrollPosition);

        // ��������� ������� ����������
        content.anchoredPosition = new Vector2(newPosition, content.anchoredPosition.y);

        UpdateButtons();  // ��������� ���� ������
    }

    private void UpdateButtons()
    {
        // ��������, �� ����� ������������ ���� ��� ������
        scrollLeftButton.gameObject.SetActive(content.anchoredPosition.x > minScrollPosition);
        scrollRightButton.gameObject.SetActive(content.anchoredPosition.x < maxScrollPosition);
    }
}