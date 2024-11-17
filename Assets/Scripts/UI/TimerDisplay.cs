using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [Header("Посилання на TMP_Text")]
    public TMP_Text timerText; // Посилання на TMP_Text компонент

    [Header("Налаштування кольорів")]
    public Color moreThanOneMinuteColor = Color.white; // Колір для часу більше хвилини
    public Color lessThanOneMinuteColor = Color.red;   // Колір для часу менше хвилини

    private void Start()
    {
        // Перевірка, чи призначено TMP_Text
        if (timerText == null)
        {
            Debug.LogError("Не призначено TMP_Text компонент у TimerDisplay.");
        }
    }

    private void Update()
    {
        if (timerText == null || GameManager.Instance == null)
            return;

        // Отримуємо залишок часу через GameManager
        float timeRemaining = GameManager.Instance.GetTimeRemaining();

        // Форматуємо час у вигляді "mm:ss:ms"
        string formattedTime = FormatTime(timeRemaining);

        // Змінюємо колір залежно від часу
        UpdateTextColor(timeRemaining);

        // Оновлюємо текст у TMP_Text
        timerText.text = formattedTime;
    }

    private string FormatTime(float time)
    {
        if (time < 0)
        {
            return "00:00:00"; // Якщо час вичерпано, показуємо нулі
        }

        int minutes = Mathf.FloorToInt(time / 60); // Хвилини
        int seconds = Mathf.FloorToInt(time % 60); // Секунди
        int milliseconds = Mathf.FloorToInt((time * 100) % 100); // Мілісекунди (2 цифри)

        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    private void UpdateTextColor(float timeRemaining)
    {
        if (timeRemaining > 60f)
        {
            timerText.color = moreThanOneMinuteColor; // Колір для часу більше хвилини
        }
        else
        {
            timerText.color = lessThanOneMinuteColor; // Колір для часу менше хвилини
        }
    }
}