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
    [SerializeField] private Button firstButtonQuestionUI;   
    [SerializeField] private Button secondButtonQuestionUI;

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

    public void ShowPathUI(int[] PathOne, int[] PathTwo)
    {

    }

    public void ShowItemRewardUI(Item item)
    {
        rewardItem = item;

        // ��������� �������� ������������� ��������
        GameObject itemUI = Instantiate(itemPrefabRewardUI, placeForItemRewardUI);
        var itemImage = itemUI.transform.Find("ItemImage").GetComponent<Image>();
        if (itemImage != null)
        {
            itemImage.sprite = item.Sprite;  // ������������ ������ � ����� ��������
        }
        else
        {
            Debug.LogWarning("UI-������� �� ������ ��'��� ItemImage ��� ��������� Image.");
        }

        Button rewardButton = itemUI.GetComponentInChildren<Button>();
        if (rewardButton != null)
        {
            rewardButton.onClick.RemoveAllListeners();
            rewardButton.onClick.AddListener(() => AddItemToBackpack());
        }

        // �������� ������ ��������
        ItemRewardUI.SetActive(true);
    }



    private void Awake()
    {
        // ����'����� ������ �� ������
        yesButtonQuestionUI.onClick.AddListener(OnYesClicked);
        noButtonQuestionUI.onClick.AddListener(OnNoClicked);
        skipButtonTextResultUI.onClick.AddListener(OnYesClicked);

    }

    private void OnYesClicked()
    {
        // �������� �� ������ � ������ yesEvents
        foreach (var unityEvent in firstEventsQuestionUI)
        {
            unityEvent?.Invoke();
        }
    }

    /// <summary>
    /// ���������� ��� ��������� ������ "ͳ".
    /// </summary>
    private void OnNoClicked()
    {
        // �������� �� ������ � ������ noEvents
        foreach (var unityEvent in secondEventsQuestionUI)
        {
            unityEvent?.Invoke();
        }
    }

    // ����� ��� ��������� �������� �� �������
    private void AddItemToBackpack()
    {
        if (Player.Instance.backpack.CanFitItem(rewardItem)) // ����������, �� � ���� � �������
        {
            Player.Instance.backpack.AddItem(rewardItem); // ������ ������� �� �������
            Debug.Log($"������� {rewardItem.Name} ������ �� �������.");

            // ��������� UI ��������
            CloseItemRewardUI();
        }
        else
        {
            Debug.LogWarning("����������� ���� � ������� ��� ����� ��������.");
        }
    }

    // ����� ��� �������� UI ��������
    private void CloseItemRewardUI()
    {
        foreach (Transform child in placeForItemRewardUI) // ������� ���� ��� ��������
        {
            Destroy(child.gameObject);
        }

        ItemRewardUI.SetActive(false); // ���������� ������
    }
}
