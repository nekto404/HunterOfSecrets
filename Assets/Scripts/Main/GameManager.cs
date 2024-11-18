using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject shopPrefab;
    public MenuController menuController;

    private Shop shopInstance;
    private Location currentLocation;

    private float timeRemaining; // Залишок часу
    private bool isRoundActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Дублікат GameManager виявлено та знищено.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame()
    {
        Player.Instance.Initialize();
        LoadShop();
        LoadRandomLocation();
    }

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

    public Location GetCurrentLocation()
    {
        if (currentLocation == null)
        {
            Debug.LogWarning("Локація ще не завантажена.");
        }
        return currentLocation;
    }

    public void RestartGame()
    {
        if (shopInstance != null)
            Destroy(shopInstance.gameObject);

        StartGame();
    }

    public void ShowConfirmationToStartExploration()
    {
        if (menuController == null)
        {
            Debug.LogError("MenuController не призначений у GameManager.");
            return;
        }

        // Створюємо або очищаємо UnityEvent
        UnityEvent onYesEvent = new UnityEvent();
        onYesEvent.RemoveAllListeners(); // Очищаємо слухачів перед додаванням нових

        // Додаємо слухача для початку раунду
        onYesEvent.AddListener(StartRound);

        // Викликаємо метод MenuController для ініціалізації ConfirmationUI
        menuController.ShowConfirmation(
            new List<UnityEvent> { onYesEvent },
            new List<UnityEvent>(), // Пустий список для кнопки "Ні"
            "Are you ready to start the exploration?"
        );
    }

    public void StartRound()
    {
        Debug.Log("Раунд розпочато!");
        if (currentLocation == null)
        {
            Debug.LogError("Неможливо розпочати раунд: локація не завантажена.");
            return;
        }

        // Визначаємо рівень гравця (за замовчуванням 0, якщо такого поля немає)
        int playerLevel = Mathf.Clamp(Player.Instance.level - 1, 0, currentLocation.travelTimes.Length - 1);

        // Встановлюємо тривалість раунду з `travelTimes`
        timeRemaining = currentLocation.travelTimes[playerLevel];

        isRoundActive = true;
        menuController.GameShop.SetActive(false);
        menuController.GameLocation.SetActive(true);
    }

    private void Update()
    {
        if (isRoundActive)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                EndRound();
            }
        }
    }

    public float GetTimeRemaining()
    {
        return isRoundActive ? timeRemaining : 0f;
    }

    public Shop GetCurrentShop()
    {
        return shopInstance;
    }

    private void EndRound()
    {
        Debug.Log("Раунд завершено!");
        isRoundActive = false;

        // Додайте тут логіку завершення раунду
    }
}