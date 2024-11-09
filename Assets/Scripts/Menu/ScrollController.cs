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
    private int visibleItems = 18;      // ʳ������ ������� �������� ���������

    void Start()
    {
        // ���������� ������� ������ ������ ��������
        itemWidth = viewport.rect.width / visibleItems;

        // ���������� ����������� � �������� ������� �������������
        maxScrollPosition = 0;
        minScrollPosition = - content.rect.width + viewport.rect.width;
        // ϳ�������� ����� ��������� �� ������
        scrollLeftButton.onClick.AddListener(() => Scroll(-1));
        scrollRightButton.onClick.AddListener(() => Scroll(1));

        UpdateButtons();  // ��������� ����� ������
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