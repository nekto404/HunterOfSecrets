using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    public RectTransform content;       // Контейнер з елементами
    public RectTransform viewport;      // Видима область (Viewport)
    public ScrollRect scrollRect;       // Компонент Scroll Rect
    public Button scrollLeftButton;     // Кнопка для прокручування ліворуч
    public Button scrollRightButton;    // Кнопка для прокручування праворуч

    private float itemWidth;            // Ширина одного елемента
    private float maxScrollPosition;    // Максимальна прокрутка
    private float minScrollPosition;    // Мінімальна прокрутка
    private int visibleItems = 18;      // Кількість видимих елементів одночасно

    void Start()
    {
        // Обчислюємо потрібну ширину одного елемента
        itemWidth = viewport.rect.width / visibleItems;

        // Обчислюємо максимальну і мінімальну позицію прокручування
        maxScrollPosition = 0;
        minScrollPosition = - content.rect.width + viewport.rect.width;
        // Підключаємо метод прокрутки до кнопок
        scrollLeftButton.onClick.AddListener(() => Scroll(-1));
        scrollRightButton.onClick.AddListener(() => Scroll(1));

        UpdateButtons();  // Оновлення стану кнопок
    }

    private void Scroll(int direction)
    {
        // Обчислюємо нову позицію
        float newPosition = content.anchoredPosition.x + direction * itemWidth;
        newPosition = Mathf.Clamp(newPosition, minScrollPosition, maxScrollPosition);

        // Оновлюємо позицію контейнера
        content.anchoredPosition = new Vector2(newPosition, content.anchoredPosition.y);

        UpdateButtons();  // Оновлюємо стан кнопок
    }

    private void UpdateButtons()
    {
        // Перевірка, чи можна прокручувати вліво або вправо
        scrollLeftButton.gameObject.SetActive(content.anchoredPosition.x > minScrollPosition);
        scrollRightButton.gameObject.SetActive(content.anchoredPosition.x < maxScrollPosition);
    }
}