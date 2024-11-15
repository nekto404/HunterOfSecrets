using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationUI : MonoBehaviour
{
    [SerializeField] private Button yesButton;    // Кнопка "Так" (встановлюється в інспекторі)
    [SerializeField] private Button noButton;     // Кнопка "Ні" (встановлюється в інспекторі)
    [SerializeField] private TMP_Text messageText;    // Поле тексту для повідомлення (встановлюється в інспекторі)

    private List<UnityEvent> yesEvents;           // Список івентів для кнопки "Так"
    private List<UnityEvent> noEvents;            // Список івентів для кнопки "Ні"

    private void Awake()
    {
        // Прив'язуємо методи до кнопок
        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);

        // Ініціалізуємо списки подій
        yesEvents = new List<UnityEvent>();
        noEvents = new List<UnityEvent>();
    }

    /// <summary>
    /// Метод для ініціалізації ConfirmationUI.
    /// </summary>
    /// <param name="yesActions">Список UnityEvent для кнопки "Так".</param>
    /// <param name="noActions">Список UnityEvent для кнопки "Ні".</param>
    /// <param name="message">Текст повідомлення, який буде відображено.</param>
    public void Initialize(List<UnityEvent> yesActions, List<UnityEvent> noActions, string message)
    {
        // Зберігаємо списки подій
        yesEvents = yesActions;
        noEvents = noActions;

        // Встановлюємо текст повідомлення
        if (messageText != null)
        {
            messageText.text = message;
        }

        // Робимо UI активним
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Виконується при натисканні кнопки "Так".
    /// </summary>
    private void OnYesClicked()
    {
        // Виконуємо всі івенти зі списку yesEvents
        foreach (var unityEvent in yesEvents)
        {
            unityEvent?.Invoke();
        }

        // Ховаємо UI
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Виконується при натисканні кнопки "Ні".
    /// </summary>
    private void OnNoClicked()
    {
        // Виконуємо всі івенти зі списку noEvents
        foreach (var unityEvent in noEvents)
        {
            unityEvent?.Invoke();
        }

        // Ховаємо UI
        gameObject.SetActive(false);
    }
}