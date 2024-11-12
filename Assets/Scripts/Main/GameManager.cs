using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Глобальний доступ до екземпляра GameManager

    public GameObject shopPrefab;        // Префаб магазину

    private Shop shopInstance;           // Поточний магазин
    private Location currentLocation;    // Поточна локація

    private void Awake()
    {
        // Перевіряємо, чи існує вже екземпляр GameManager
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Дублікат GameManager виявлено та знищено.");
            Destroy(gameObject); // Знищуємо дублікати
            return;
        }

        Instance = this; // Призначаємо цей екземпляр як активний
        DontDestroyOnLoad(gameObject); // Робимо GameManager постійним між сценами
    }

    // Метод початку гри
    public void StartGame()
    {
        Player.Instance.Initialize();  // Ініціалізація гравця зі значеннями за замовчуванням
        LoadShop();
        LoadRandomLocation();
    }

    // Метод для створення нового гравця
    private void CreatePlayer()
    {
        Player.Instance.coins = 20;
        Debug.Log("Гравець створений або вже існує.");
    }

    // Метод для завантаження магазину
    private void LoadShop()
    {
        if (shopInstance == null)
        {
            GameObject shopObject = Instantiate(shopPrefab);
            shopInstance = shopObject.GetComponent<Shop>();

            if (shopInstance == null)
            {
                Debug.LogError("Не вдалося знайти компонент Shop на префабі.");
            }
            else
            {
                Debug.Log("Магазин завантажено.");
            }
        }
    }

    // Метод для завантаження випадкової локації з папки Resources/Locations
    private void LoadRandomLocation()
    {
        Location[] locations = Resources.LoadAll<Location>("Locations");

        if (locations.Length > 0)
        {
            currentLocation = locations[Random.Range(0, locations.Length)];
            Debug.Log("Випадкова локація завантажена: " + currentLocation.locationName);
        }
        else
        {
            Debug.LogWarning("Не вдалося знайти жодної локації в Resources/Locations.");
        }
    }

    // Публічний метод для отримання поточного магазину
    public Shop GetCurrentShop()
    {
        if (shopInstance == null)
        {
            Debug.LogWarning("Магазин ще не завантажений.");
        }
        return shopInstance;
    }

    // Публічний метод для отримання поточної локації
    public Location GetCurrentLocation()
    {
        if (currentLocation == null)
        {
            Debug.LogWarning("Локація ще не завантажена.");
        }
        return currentLocation;
    }

    // Метод для перезапуску гри
    public void RestartGame()
    {
        // Видалення магазину
        if (shopInstance != null)
            Destroy(shopInstance.gameObject);

        StartGame(); // Запуск гри заново
    }
}