using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LocationUI : MonoBehaviour
{
    private List<UnityEvent> firstEventsQuestionUI;
    private List<UnityEvent> secondEventsQuestionUI;

    [Header("Question")]
    [SerializeField] private GameObject QuestionUI;
    [SerializeField] private Button yesButtonQuestionUI;    
    [SerializeField] private Button noButtonQuestionUI;    
    [SerializeField] private TMP_Text messageTextQuestionUI;

    [Header("TextResult")]
    [SerializeField] private GameObject TextResultUI;
    [SerializeField] private Button skipButtonTextResultUI;
    [SerializeField] private TMP_Text messageTextResultUI;

    [Header("PathUI")]
    [SerializeField] private GameObject PathUI;
    [SerializeField] private ScrollController scrollControllerFirstPathUI;
    [SerializeField] private ScrollController scrollControllerSecondPathUI;
    [SerializeField] private GameObject tilePrefabPathUI;
    [SerializeField] private Button firstButtonPathUI;
    [SerializeField] private Button secondButtonPathUI;

    [Header("ItemRewardUI")]
    [SerializeField] private GameObject ItemRewardUI;
    [SerializeField] private Transform placeForItemRewardUI;
    [SerializeField] private  GameObject itemPrefabRewardUI;
    private Item rewardItem;

    [Header("ActionChoseUI")]
    [SerializeField] private GameObject ActionChoseUI;
    [SerializeField] private Button firstButtonChoseUI;   
    [SerializeField] private Button secondButtonChoseUI;
    [SerializeField] private Image firstImageChoseUI;
    [SerializeField] private Image secondImageChoseUI;

    [Header("RunUI")]
    [SerializeField] private GameObject RunUI;
    [SerializeField] private Image trackerRunUI;
    [SerializeField] private Image firstTileRunUI;
    [SerializeField] private Image secondTileRunUI;
    [SerializeField] private Image thirdTileRunUI;
    [SerializeField] private Sprite emptySprite;

    public void ShowQuestionUI(List<UnityEvent> yesActions, List<UnityEvent> noActions, string message)
    {
        if (messageTextQuestionUI != null)
        {
            messageTextQuestionUI.text = message;
        }

        QuestionUI.SetActive(true);

        firstEventsQuestionUI = yesActions;
        secondEventsQuestionUI = noActions;
    }


    public void ShowTextResultUI(List<UnityEvent> skipActions, string message)
    {
        if (messageTextResultUI != null)
        {
            messageTextResultUI.text = message;
        }
        else
        {
            Debug.Log("No field");
        }

        TextResultUI.SetActive(true);

        firstEventsQuestionUI = skipActions;
    }

    public void ShowPathUI(int[] PathOne, int[] PathTwo, List<UnityEvent> firstActions, List<UnityEvent> secondActions)
    {
        PathUI.SetActive(true);

        // Очищення старих елементів у першому шляху
        foreach (Transform child in scrollControllerFirstPathUI.content)
        {
            Destroy(child.gameObject);
        }

        // Очищення старих елементів у другому шляху
        foreach (Transform child in scrollControllerSecondPathUI.content)
        {
            Destroy(child.gameObject);
        }

        // Додавання нових елементів для першого шляху
        foreach (var tile in PathOne)
        {
            // Створюємо новий UI-елемент для предмета
            GameObject newItemUI = Instantiate(tilePrefabPathUI, scrollControllerFirstPathUI.content);
            // Встановлюємо спрайт предмета
            var itemImage = newItemUI.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = TileManager.Instance.GetTileById(tile).sprite; // Встановлюємо спрайт з даних предмета
            }
            else
            {
                Debug.LogWarning("UI-елемент не містить об'єкт ItemImage або компонент Image.");
            }
        }

        // Додавання нових елементів для другого шляху
        foreach (var tile in PathTwo)
        {
            // Створюємо новий UI-елемент для предмета
            GameObject newItemUI = Instantiate(tilePrefabPathUI, scrollControllerSecondPathUI.content);
            // Встановлюємо спрайт предмета
            var itemImage = newItemUI.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = TileManager.Instance.GetTileById(tile).sprite; // Встановлюємо спрайт з даних предмета
            }
            else
            {
                Debug.LogWarning("UI-елемент не містить об'єкт ItemImage або компонент Image.");
            }
        }

        // Зберігаємо списки подій
        firstEventsQuestionUI = firstActions;
        secondEventsQuestionUI = secondActions;
    }

    public void ShowItemRewardUI(Item item)
    {
        ItemRewardUI.SetActive(true);
        rewardItem = item;

        // Створюємо візуальне представлення предмета
        GameObject itemUI = Instantiate(itemPrefabRewardUI, placeForItemRewardUI);
        var itemImage = itemUI.transform.Find("ItemImage").GetComponent<Image>();
        if (itemImage != null)
        {
            itemImage.sprite = item.Sprite;  // Встановлюємо спрайт з даних предмета
        }
        else
        {
            Debug.LogWarning("UI-елемент не містить об'єкт ItemImage або компонент Image.");
        }

        Button rewardButton = itemUI.GetComponentInChildren<Button>();
        if (rewardButton != null)
        {
            rewardButton.onClick.RemoveAllListeners();
            rewardButton.onClick.AddListener(() => AddItemToBackpack());
        }

        // Активуємо панель нагороди
    
    }




    public void ShowActionChoseUI(LocationEvent locationEvent, List<UnityEvent> firstActions, List<UnityEvent> secondActions)
    {
        ActionChoseUI.SetActive(true);

        firstImageChoseUI.sprite = locationEvent.spriteOriginal;
        secondImageChoseUI.sprite = locationEvent.spriteAlter;

        firstEventsQuestionUI= firstActions;
        secondEventsQuestionUI = secondActions;
    }

    public void ShowRunUI(Sprite firstTile, Sprite secondTile, Sprite thirdTile, float progress)
    {
        RunUI.SetActive(true);
        // Призначення спрайтів для тайлів
        firstTileRunUI.sprite = firstTile == null ? emptySprite : firstTile;
        secondTileRunUI.sprite = secondTile == null ? emptySprite : secondTile; 
        thirdTileRunUI.sprite = thirdTile == null ? emptySprite : thirdTile; 

        // Розрахунок позиції трекера
        RectTransform tileRect = secondTileRunUI.GetComponent<RectTransform>();
        RectTransform trackerRect = trackerRunUI.GetComponent<RectTransform>();

        float tileWidth = tileRect.rect.width; // Ширина тайла
        float startX = tileRect.anchoredPosition.x; //- (tileWidth / 2); // Ліва межа
        float endX = tileRect.anchoredPosition.x + tileWidth;// (tileWidth / 2); // Права межа

        // Обчислення X-позиції трекера
        float trackerX = Mathf.Lerp(startX, endX, progress);
        trackerRect.anchoredPosition = new Vector2(trackerX, tileRect.anchoredPosition.y);
    }

    public void HideAll()
    {
        RunUI.SetActive(false);
        ActionChoseUI.SetActive(false);
        ItemRewardUI.SetActive(false);
        PathUI.SetActive(false);
        TextResultUI.SetActive(false);
        QuestionUI.SetActive(false);
    }

    private void Awake()
    {
        // Прив'язуємо методи до кнопок
        yesButtonQuestionUI.onClick.AddListener(OnYesClicked);
        noButtonQuestionUI.onClick.AddListener(OnNoClicked);
        skipButtonTextResultUI.onClick.AddListener(OnYesClicked);
        secondButtonPathUI.onClick.AddListener(OnNoClicked);
        firstButtonPathUI.onClick.AddListener(OnYesClicked);
        firstButtonChoseUI.onClick.AddListener(OnYesClicked);
        secondButtonChoseUI.onClick.AddListener(OnNoClicked);

    }

    private void OnYesClicked()
    {
        // Виконуємо всі івенти зі списку yesEvents
        foreach (var unityEvent in firstEventsQuestionUI)
        {
            unityEvent?.Invoke();
        }
    }

    /// <summary>
    /// Виконується при натисканні кнопки "Ні".
    /// </summary>
    private void OnNoClicked()
    {
        // Виконуємо всі івенти зі списку noEvents
        foreach (var unityEvent in secondEventsQuestionUI)
        {
            unityEvent?.Invoke();
        }
    }

    // Метод для додавання предмета до рюкзака
    private void AddItemToBackpack()
    {
        if (Player.Instance.Backpack.CanFitItem(rewardItem)) // Перевіряємо, чи є місце в рюкзаку
        {
            Player.Instance.Backpack.AddItem(rewardItem); // Додаємо предмет до рюкзака
            Debug.Log($"Предмет {rewardItem.Name} додано до рюкзака.");

            // Закриваємо UI нагороди
            CloseItemRewardUI();
        }
        else
        {
            Debug.LogWarning("Недостатньо місця в рюкзаку для цього предмета.");
        }
    }

    // Метод для закриття UI нагороди
    private void CloseItemRewardUI()
    {
        foreach (Transform child in placeForItemRewardUI) // Очищаємо місце для предмета
        {
            Destroy(child.gameObject);
        }

        ItemRewardUI.SetActive(false); // Деактивуємо панель
    }
}
