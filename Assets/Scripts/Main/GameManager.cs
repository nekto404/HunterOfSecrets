using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;      // ������ ������
    public GameObject shopPrefab;        // ������ ��������

    private Shop shopInstance;           // �������� �������
    private Location currentLocation;    // ������� �������

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

    // ����� ��� ����������� ���
    public void RestartGame()
    {
        // ��������� ��������
        if (shopInstance != null)
            Destroy(shopInstance.gameObject);

        StartGame(); // ������ ��� ������
    }
}
