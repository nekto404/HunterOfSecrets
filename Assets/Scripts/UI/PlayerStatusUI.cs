using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    public ScrollController scrollController;    // Компонент ScrollController для керування прокруткою
    public GameObject itemPrefab;                // Префаб UI-елемента для предмета

    private Dictionary<int, GameObject> statusUIElements = new Dictionary<int, GameObject>();
    public Sprite[] Sprites;

    

    public void UpdateStatusUI()
    {
        int[] statuses = Player.Instance.CurrentStatuses;  // Отримуємо всі активні статуси гравця

        // Оновлюємо або додаємо нові елементи статусів
        for (int i = 0; i < statuses.Length; i++)
        {
            int status = statuses[i];
            if (status != 0)
            {
                if (!statusUIElements.ContainsKey(i))
                {
                    // Створюємо новий UI-елемент для статусу
                    GameObject newStatusUI = Instantiate(itemPrefab, scrollController.content);

                    // Встановлюємо спрайт статусу
                    var statusImage = newStatusUI.transform.Find("Image").GetComponent<Image>();
                    if (statusImage != null)
                    {
                        statusImage.sprite = Sprites[i];  // Встановлюємо спрайт з даних статусу
                    }
                    else
                    {
                        Debug.LogWarning("UI-елемент не містить об'єкт StatusImage або компонент Image.");
                    }

                    // Встановлюємо значення статусу
                    var statusText = newStatusUI.transform.Find("Text").GetComponent<TMP_Text>();
                    if (statusText != null)
                    {
                        statusText.text = status.ToString();  // Встановлюємо значення статусу
                    }
                    else
                    {
                        Debug.LogWarning("UI-елемент не містить об'єкт StatusText або компонент Text.");
                    }

                    statusUIElements[i] = newStatusUI;  // Додаємо елемент до словника
                }
                else
                {
                    // Оновлюємо значення статусу
                    var statusText = statusUIElements[i].transform.Find("Text").GetComponent<TMP_Text>();
                    if (statusText != null)
                    {
                        statusText.text = status.ToString();  // Оновлюємо значення статусу
                    }
                }
            }
        }

        // Видаляємо елементи статусів, які більше не активні
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

        // Ініціалізуємо прокрутку канваса після оновлення статусів
        scrollController.Initialize();
    }
}