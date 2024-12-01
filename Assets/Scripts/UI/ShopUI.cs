using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Shop UI Points")]
    public Transform[] shopPoints = new Transform[5]; // �'��� ����� �� ������

    [Header("Prefabs")]
    public GameObject itemUIPrefab;        // ������ UI-�������� ��������
    public GameObject refreshUIPrefab;     // ������ ��� ������ �������
    public GameObject upgradeUIPrefab;     // ������ ��� ������ ���������� �������

    [Header("Static Images")]
    public Sprite refreshIcon;             // ������ ��� ������ �������
    public Sprite upgradeIcon;             // ������ ��� ������ ���������� �������

    [SerializeField]
    private Shop shop;

    private void Start()
    {
        shop = GameManager.Instance.GetCurrentShop();
        if (shop == null)
        {
            Debug.LogWarning("Shop instance not found. ShopUI will not function correctly.");
            return;
        }

        UpdateShopUI();
    }

    private void OnEnable()
    {
        shop = GameManager.Instance.GetCurrentShop();
        UpdateShopUI();
    }

    public void UpdateShopUI()
    {
        // ���������� �������� �������� ����� ���������� UI
        if (shop == null)
        {
            Debug.LogWarning("Shop instance not found. Cannot update Shop UI.");
            return;
        }

        ClearShopUI();

        // ����������� ������ �������
        GameObject refreshUI = Instantiate(refreshUIPrefab, shopPoints[0]);
        Image refreshImage = refreshUI.GetComponentInChildren<Image>();
        refreshImage.sprite = refreshIcon;
        Button refreshButton = refreshUI.GetComponentInChildren<Button>();
        refreshButton.onClick.AddListener(() => RefreshShop());

        // ����������� UI-�������� ��� �������� ��������
        for (int i = 0; i < 3; i++)
        {
            if (i < shop.availableItems.Count)
            {
                Item item = shop.availableItems[i];
                GameObject itemUI = Instantiate(itemUIPrefab, shopPoints[i + 1]);

                // ������������� ������ ��'���� �� ������� ��������
                RectTransform itemRect = itemUI.GetComponent<RectTransform>();
                float initialWidth = itemRect.sizeDelta.x;
                itemRect.sizeDelta = new Vector2(initialWidth * item.Size, itemRect.sizeDelta.y);

                Image itemImage = itemUI.GetComponentInChildren<Image>();
                itemImage.sprite = item.Sprite; // ������������ �������� � ��������

                Button buyButton = itemUI.GetComponentInChildren<Button>();
                buyButton.onClick.AddListener(() => BuyItem(item));
            }
        }

        // ����������� ������ ���������� �������
        GameObject upgradeUI = Instantiate(upgradeUIPrefab, shopPoints[4]);
        Image upgradeImage = upgradeUI.GetComponentInChildren<Image>();
        upgradeImage.sprite = upgradeIcon;
        Button upgradeButton = upgradeUI.GetComponentInChildren<Button>();
        upgradeButton.onClick.AddListener(() => UpgradeBackpack());
    }

    private void ClearShopUI()
    {
        foreach (Transform point in shopPoints)
        {
            if (point.childCount > 0)
            {
                foreach (Transform child in point)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    private void RefreshShop()
    {
        if (shop != null && shop.RefreshShop())
        {
            UpdateShopUI();
        }
        else
        {
            Debug.LogWarning("Failed to refresh shop: Shop instance not found or insufficient coins.");
        }
    }

    private void BuyItem(Item item)
    {
        if (shop != null && shop.BuyItem(item))
        {
            UpdateShopUI();
        }
        else
        {
            Debug.LogWarning("Failed to buy item: Shop instance not found or insufficient resources.");
        }
    }

    private void UpgradeBackpack()
    {
        if (shop != null && shop.UpgradeBackpack())
        {
            UpdateShopUI();
        }
        else
        {
            Debug.LogWarning("Failed to upgrade backpack: Shop instance not found or insufficient coins.");
        }
    }
}