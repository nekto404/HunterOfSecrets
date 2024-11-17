using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // ���������� ������ �� ���������� GameManager

    public GameObject shopPrefab;        // ������ ��������
    public MenuController menuController; // ��������� �� MenuController (�������������� � ���������)

    private Shop shopInstance;           // �������� �������
    private Location currentLocation;    // ������� �������

    private void Awake()
    {
        // ����������, �� ���� ��� ��������� GameManager
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("������� GameManager �������� �� �������.");
            Destroy(gameObject); // ������� ��������
            return;
        }

        Instance = this; // ���������� ��� ��������� �� ��������
        DontDestroyOnLoad(gameObject); // ������ GameManager �������� �� �������
    }

    // ����� ������� ���
    public void StartGame()
    {
        Player.Instance.Initialize();  // ����������� ������ � ���������� �� �������������
        LoadShop();
        LoadRandomLocation();
    }

    // ����� ��� ��������� ������ ������
    private void CreatePlayer()
    {
        Player.Instance.coins = 20;
        Debug.Log("������� ��������� ��� ��� ����.");
    }

    // ����� ��� ������������ ��������
    private void LoadShop()
    {
        if (shopInstance == null)
        {
            GameObject shopObject = Instantiate(shopPrefab);
            shopInstance = shopObject.GetComponent<Shop>();

            if (shopInstance == null)
            {
                Debug.LogError("�� ������� ������ ��������� Shop �� ������.");
            }
            else
            {
                Debug.Log("������� �����������.");
            }
        }
    }

    // ����� ��� ������������ ��������� ������� � ����� Resources/Locations
    private void LoadRandomLocation()
    {
        Location[] locations = Resources.LoadAll<Location>("Locations");

        if (locations.Length > 0)
        {
            currentLocation = locations[Random.Range(0, locations.Length)];
            Debug.Log("��������� ������� �����������: " + currentLocation.locationName);
        }
        else
        {
            Debug.LogWarning("�� ������� ������ ����� ������� � Resources/Locations.");
        }
    }

    // �������� ����� ��� ��������� ��������� ��������
    public Shop GetCurrentShop()
    {
        if (shopInstance == null)
        {
            Debug.LogWarning("������� �� �� ������������.");
        }
        return shopInstance;
    }

    // �������� ����� ��� ��������� ������� �������
    public Location GetCurrentLocation()
    {
        if (currentLocation == null)
        {
            Debug.LogWarning("������� �� �� �����������.");
        }
        return currentLocation;
    }

    // ����� ��� ����������� ���
    public void RestartGame()
    {
        // ��������� ��������
        if (shopInstance != null)
            Destroy(shopInstance.gameObject);

        StartGame(); // ������ ��� ������
    }

    public void StartRound()
    {

    }

    // ����� ��� ������ ConfirmationUI
    public void ShowConfirmationToStartExploration()
    {
        if (menuController == null)
        {
            Debug.LogError("MenuController �� ����������� � GameManager.");
            return;
        }

        // ��������� UnityEvent ��� ������ "���"
        UnityEngine.Events.UnityEvent onYesEvent = new UnityEngine.Events.UnityEvent();
        onYesEvent.AddListener(StartRound);

        // ��������� ����� MenuController ��� ����������� ConfirmationUI
        menuController.ShowConfirmation(
            new List<UnityEngine.Events.UnityEvent> { onYesEvent },
            new List<UnityEngine.Events.UnityEvent>(), // ������ ������ ��� ������ "ͳ"
            "Are you ready to start the exploration?"
        );
    }
}