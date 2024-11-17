using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [Header("��������� �� TMP_Text")]
    public TMP_Text timerText; // ��������� �� TMP_Text ���������

    [Header("������������ �������")]
    public Color moreThanOneMinuteColor = Color.white; // ���� ��� ���� ����� �������
    public Color lessThanOneMinuteColor = Color.red;   // ���� ��� ���� ����� �������

    private void Start()
    {
        // ��������, �� ���������� TMP_Text
        if (timerText == null)
        {
            Debug.LogError("�� ���������� TMP_Text ��������� � TimerDisplay.");
        }
    }

    private void Update()
    {
        if (timerText == null || GameManager.Instance == null)
            return;

        // �������� ������� ���� ����� GameManager
        float timeRemaining = GameManager.Instance.GetTimeRemaining();

        // ��������� ��� � ������ "mm:ss:ms"
        string formattedTime = FormatTime(timeRemaining);

        // ������� ���� ������� �� ����
        UpdateTextColor(timeRemaining);

        // ��������� ����� � TMP_Text
        timerText.text = formattedTime;
    }

    private string FormatTime(float time)
    {
        if (time < 0)
        {
            return "00:00:00"; // ���� ��� ���������, �������� ���
        }

        int minutes = Mathf.FloorToInt(time / 60); // �������
        int seconds = Mathf.FloorToInt(time % 60); // �������
        int milliseconds = Mathf.FloorToInt((time * 100) % 100); // ̳�������� (2 �����)

        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    private void UpdateTextColor(float timeRemaining)
    {
        if (timeRemaining > 60f)
        {
            timerText.color = moreThanOneMinuteColor; // ���� ��� ���� ����� �������
        }
        else
        {
            timerText.color = lessThanOneMinuteColor; // ���� ��� ���� ����� �������
        }
    }
}