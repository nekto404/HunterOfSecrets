using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    public ScrollController scrollController;    // ��������� ScrollController ��� ��������� ����������
    public GameObject itemPrefab;                // ������ UI-�������� ��� ��������

    private Dictionary<int, GameObject> statusUIElements = new Dictionary<int, GameObject>();
    public Sprite[] Sprites;

    

    public void UpdateStatusUI()
    {
        int[] statuses = Player.Instance.CurrentStatuses;  // �������� �� ������ ������� ������

        // ��������� ��� ������ ��� �������� �������
        for (int i = 0; i < statuses.Length; i++)
        {
            int status = statuses[i];
            if (status != 0)
            {
                if (!statusUIElements.ContainsKey(i))
                {
                    // ��������� ����� UI-������� ��� �������
                    GameObject newStatusUI = Instantiate(itemPrefab, scrollController.content);

                    // ������������ ������ �������
                    var statusImage = newStatusUI.transform.Find("Image").GetComponent<Image>();
                    if (statusImage != null)
                    {
                        statusImage.sprite = Sprites[i];  // ������������ ������ � ����� �������
                    }
                    else
                    {
                        Debug.LogWarning("UI-������� �� ������ ��'��� StatusImage ��� ��������� Image.");
                    }

                    // ������������ �������� �������
                    var statusText = newStatusUI.transform.Find("Text").GetComponent<TMP_Text>();
                    if (statusText != null)
                    {
                        statusText.text = status.ToString();  // ������������ �������� �������
                    }
                    else
                    {
                        Debug.LogWarning("UI-������� �� ������ ��'��� StatusText ��� ��������� Text.");
                    }

                    statusUIElements[i] = newStatusUI;  // ������ ������� �� ��������
                }
                else
                {
                    // ��������� �������� �������
                    var statusText = statusUIElements[i].transform.Find("Text").GetComponent<TMP_Text>();
                    if (statusText != null)
                    {
                        statusText.text = status.ToString();  // ��������� �������� �������
                    }
                }
            }
        }

        // ��������� �������� �������, �� ����� �� ������
        List<int> keysToRemove = new List<int>();
        foreach (var key in statusUIElements.Keys)
        {
            if (statuses[key] == 0)
            {
                Destroy(statusUIElements[key]);
                keysToRemove.Add(key);
            }
        }

        foreach (var key in keysToRemove)
        {
            statusUIElements.Remove(key);
        }

        // ���������� ��������� ������� ���� ��������� �������
        scrollController.Initialize();
    }
}