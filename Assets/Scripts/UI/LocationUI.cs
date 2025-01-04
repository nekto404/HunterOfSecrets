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

        // �������� ������ �������� � ������� �����
        foreach (Transform child in scrollControllerFirstPathUI.content)
        {
            Destroy(child.gameObject);
        }

        // �������� ������ �������� � ������� �����
        foreach (Transform child in scrollControllerSecondPathUI.content)
        {
            Destroy(child.gameObject);
        }

        // ��������� ����� �������� ��� ������� �����
        foreach (var tile in PathOne)
        {
            // ��������� ����� UI-������� ��� ��������
            GameObject newItemUI = Instantiate(tilePrefabPathUI, scrollControllerFirstPathUI.content);
            // ������������ ������ ��������
            var itemImage = newItemUI.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = TileManager.Instance.GetTileById(tile).sprite; // ������������ ������ � ����� ��������
            }
            else
            {
                Debug.LogWarning("UI-������� �� ������ ��'��� ItemImage ��� ��������� Image.");
            }
        }

        // ��������� ����� �������� ��� ������� �����
        foreach (var tile in PathTwo)
        {
            // ��������� ����� UI-������� ��� ��������
            GameObject newItemUI = Instantiate(tilePrefabPathUI, scrollControllerSecondPathUI.content);
            // ������������ ������ ��������
            var itemImage = newItemUI.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.sprite = TileManager.Instance.GetTileById(tile).sprite; // ������������ ������ � ����� ��������
            }
            else
            {
                Debug.LogWarning("UI-������� �� ������ ��'��� ItemImage ��� ��������� Image.");
            }
        }

        // �������� ������ ����
        firstEventsQuestionUI = firstActions;
        secondEventsQuestionUI = secondActions;
    }

    public void ShowItemRewardUI(Item item)
    {
        ItemRewardUI.SetActive(true);
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
        // ����������� ������� ��� �����
        firstTileRunUI.sprite = firstTile == null ? emptySprite : firstTile;
        secondTileRunUI.sprite = secondTile == null ? emptySprite : secondTile; 
        thirdTileRunUI.sprite = thirdTile == null ? emptySprite : thirdTile; 

        // ���������� ������� �������
        RectTransform tileRect = secondTileRunUI.GetComponent<RectTransform>();
        RectTransform trackerRect = trackerRunUI.GetComponent<RectTransform>();

        float tileWidth = tileRect.rect.width; // ������ �����
        float startX = tileRect.anchoredPosition.x; //- (tileWidth / 2); // ˳�� ����
        float endX = tileRect.anchoredPosition.x + tileWidth;// (tileWidth / 2); // ����� ����

        // ���������� X-������� �������
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
        // ����'����� ������ �� ������
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
        if (Player.Instance.Backpack.CanFitItem(rewardItem)) // ����������, �� � ���� � �������
        {
            Player.Instance.Backpack.AddItem(rewardItem); // ������ ������� �� �������
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
