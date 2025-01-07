using System.Collections;
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
    [SerializeField]
    private int visibleItems = 12;      // Кількість видимих елементів одночасно

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        // Обчислюємо потрібну ширину одного елемента
        itemWidth = viewport.rect.width / visibleItems;

        // Обчислюємо максимальну і мінімальну позицію прокручування
        maxScrollPosition = 0;
        minScrollPosition = -content.rect.width + viewport.rect.width;

        // Центруємо контент, якщо його ширина менша за ширину viewport
        if (content.rect.width <= viewport.rect.width)
        {
            StartCoroutine(CenterContent());
        }

        // Підключаємо метод прокрутки до кнопок
        scrollLeftButton.onClick.AddListener(() => Scroll(-1));
        scrollRightButton.onClick.AddListener(() => Scroll(1));

        UpdateButtons();  // Оновлення стану кнопок
    }

    private IEnumerator CenterContent()
    {
        yield return new WaitForEndOfFrame();
        // Центруємо content всередині viewport
        float centeredPositionX = (viewport.rect.width - content.rect.width) / 2;
        content.anchoredPosition = new Vector2(centeredPositionX, content.anchoredPosition.y);
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