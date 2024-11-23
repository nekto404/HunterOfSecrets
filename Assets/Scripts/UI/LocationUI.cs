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
    [SerializeField] private Button trackerRunUI;
    [SerializeField] private Button firstTileRunUI;
    [SerializeField] private Button secondTileRunUI;
    [SerializeField] private Button thirdTileRunUI;

    public void ShowQuestionUI(List<UnityEvent> yesActions, List<UnityEvent> noActions, string message)
    {
        if (messageTextQuestionUI != null)
        {
            messageTextQuestionUI.text = message;
        }

        gameObject.SetActive(true);

        firstEventsQuestionUI = yesActions;
        secondEventsQuestionUI = noActions;
    }


    public void ShowTextResultUI(List<UnityEvent> skipActions, string message)
    {
        if (messageTextQuestionUI != null)
        {
            messageTextQuestionUI.text = message;
        }

        gameObject.SetActive(true);

        firstEventsQuestionUI = skipActions;
    }

    public void ShowPathUI(int[] PathOne, int[] PathTwo, List<UnityEvent> firstActions, List<UnityEvent> secondActions)
    {
        foreach (var tile in PathOne)
        {

                // Створюємо новий UI-елемент для предмета
                GameObject newItemUI = Instantiate(tilePrefabPathUI, scrollControllerFirstPathUI.content);
                // Встановлюємо спрайт предмета
                var itemImage = newItemUI.transform.Find("ItemImage").GetComponent<Image>();
                if (itemImage != null)
                {
                    itemImage.sprite = TileManager.Instance.GetTileById(tile).sprite; // Встановлюємо спрайт з даних предмета
                }
                else
                {
                    Debug.LogWarning("UI-елемент не містить об'єкт ItemImage або компонент Image.");
                }
        }
        foreach (var tile in PathTwo)
        {

            // Створюємо новий UI-елемент для предмета
            GameObject newItemUI = Instantiate(tilePrefabPathUI, scrollControllerSecondPathUI.content);
            // Встановлюємо спрайт предмета
            var itemImage = newItemUI.transform.Find("ItemImage").GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = TileManager.Instance.GetTileById(tile).sprite; // Встановлюємо спрайт з даних предмета
            }
            else
            {
                Debug.LogWarning("UI-елемент не містить об'єкт ItemImage або компонент Image.");
            }
        }
        gameObject.SetActive(true);


        firstEventsQuestionUI = firstActions;
        secondEventsQuestionUI = secondActions;
    }

    public void ShowItemRewardUI(Item item)
    {
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
        ItemRewardUI.SetActive(true);
    }

    public void ShowActionChoseUI(LocationEvent locationEvent, List<UnityEvent> firstActions, List<UnityEvent> secondActions)
    {
        gameObject.SetActive(true);

        firstImageChoseUI.sprite = locationEvent.spriteOriginal;
        secondImageChoseUI.sprite = locationEvent.spriteAlter;

        firstEventsQuestionUI= firstActions;
        secondEventsQuestionUI = secondActions;
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
        if (Player.Instance.backpack.CanFitItem(rewardItem)) // Перевіряємо, чи є місце в рюкзаку
        {
            Player.Instance.backpack.AddItem(rewardItem); // Додаємо предмет до рюкзака
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
