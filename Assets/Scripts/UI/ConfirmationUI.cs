using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationUI : MonoBehaviour
{
    [SerializeField] private Button yesButton;    // ������ "���" (�������������� � ���������)
    [SerializeField] private Button noButton;     // ������ "ͳ" (�������������� � ���������)
    [SerializeField] private TMP_Text messageText;    // ���� ������ ��� ����������� (�������������� � ���������)

    private List<UnityEvent> yesEvents;           // ������ ������ ��� ������ "���"
    private List<UnityEvent> noEvents;            // ������ ������ ��� ������ "ͳ"

    private void Awake()
    {
        // ����'����� ������ �� ������
        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);

        // ���������� ������ ����
        yesEvents = new List<UnityEvent>();
        noEvents = new List<UnityEvent>();
    }

    /// <summary>
    /// ����� ��� ����������� ConfirmationUI.
    /// </summary>
    /// <param name="yesActions">������ UnityEvent ��� ������ "���".</param>
    /// <param name="noActions">������ UnityEvent ��� ������ "ͳ".</param>
    /// <param name="message">����� �����������, ���� ���� ����������.</param>
    public void Initialize(List<UnityEvent> yesActions, List<UnityEvent> noActions, string message)
    {
        // �������� ������ ����
        yesEvents = yesActions;
        noEvents = noActions;

        // ������������ ����� �����������
        if (messageText != null)
        {
            messageText.text = message;
        }

        // ������ UI ��������
        gameObject.SetActive(true);
    }

    /// <summary>
    /// ���������� ��� ��������� ������ "���".
    /// </summary>
    private void OnYesClicked()
    {
        // �������� �� ������ � ������ yesEvents
        foreach (var unityEvent in yesEvents)
        {
            unityEvent?.Invoke();
        }

        // ������ UI
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ���������� ��� ��������� ������ "ͳ".
    /// </summary>
    private void OnNoClicked()
    {
        // �������� �� ������ � ������ noEvents
        foreach (var unityEvent in noEvents)
        {
            unityEvent?.Invoke();
        }

        // ������ UI
        gameObject.SetActive(false);
    }
}