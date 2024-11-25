using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject shopPrefab;
    public MenuController menuController;

    private Shop shopInstance;
    private Location currentLocation;

    private float timeRemaining; // Залишок часу
    private bool isRoundActive = false;

    private List<int> fullPath = new List<int>();
    private Coroutine pathTraversalCoroutine;

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

        // Викликаємо вибір шляху через ShowPathSelection
        ShowPathSelection();
    }

    private void ShowPathSelection()
    {
        // Отримуємо два випадкових шляхи через метод у Location
        var randomPaths = currentLocation.GetTwoRandomPaths();

        if (randomPaths == null || randomPaths.Count < 2)
        {
            Debug.LogError("Не вдалося отримати два випадкових шляхи.");
            return;
        }

        // Налаштовуємо дії для першого шляху
        var firstPathActions = new List<UnityEngine.Events.UnityEvent>();
        var firstAction = new UnityEngine.Events.UnityEvent();
        firstAction.AddListener(() => OnPathChosen(randomPaths[0])); // Передаємо перший шлях
        firstPathActions.Add(firstAction);

        // Налаштовуємо дії для другого шляху
        var secondPathActions = new List<UnityEngine.Events.UnityEvent>();
        var secondAction = new UnityEngine.Events.UnityEvent();
        secondAction.AddListener(() => OnPathChosen(randomPaths[1])); // Передаємо другий шлях
        secondPathActions.Add(secondAction);

        // Показуємо UI для вибору шляхів
        menuController.LocationUI.ShowPathUI(randomPaths[0].pathSteps, randomPaths[1].pathSteps, firstPathActions, secondPathActions);
    }


    private void OnPathChosen(Path chosenPath)
    {

        // Додаємо обраний шлях до загального маршруту
        fullPath.AddRange(chosenPath.pathSteps);

        // Закриваємо UI
        menuController.LocationUI.HideAll();

        // Розпочинаємо проходження обраного шляху
        if (pathTraversalCoroutine != null)
        {
            StopCoroutine(pathTraversalCoroutine);
        }
        pathTraversalCoroutine = StartCoroutine(TraversePath(chosenPath.pathSteps));
    }

    private IEnumerator TraversePath(int[] pathStepIds)
    {
        for (int i = 0; i < pathStepIds.Length; i++)
        {
            // Отримуємо поточний тайл за його ID
            Tile currentTile = TileManager.Instance.GetTileById(pathStepIds[i]);
            if (currentTile == null)
            {
                Debug.LogWarning($"Тайл із ID {pathStepIds[i]} не знайдено. Пропущено.");
                continue;
            }

            // Отримуємо спрайти для трьох тайлів
            Sprite previousTileSprite = i > 0 ? TileManager.Instance.GetTileById(pathStepIds[i - 1]).sprite : null;
            Sprite currentTileSprite = currentTile.sprite;
            Sprite nextTileSprite = i < pathStepIds.Length - 1 ? TileManager.Instance.GetTileById(pathStepIds[i + 1]).sprite : null;

            float timeSpent = 0f;
            float updateInterval = 0.1f; // Інтервал оновлення (10 разів на секунду)

            // Відображаємо початковий стан прогресу
            menuController.LocationUI.ShowRunUI(previousTileSprite, currentTileSprite, nextTileSprite, 0f);

            // Починаємо проходження тайла
            while (timeSpent < currentTile.travelTime)
            {
                yield return new WaitForSeconds(updateInterval); // Чекаємо 0.1 секунди
                timeSpent += updateInterval;

                // Оновлюємо прогрес у LocationUI
                float progress = Mathf.Clamp01(timeSpent / currentTile.travelTime);
                menuController.LocationUI.ShowRunUI(previousTileSprite, currentTileSprite, nextTileSprite, progress);

                // Додаємо негативний ефект, якщо він є, та інтервал проходження збігається
                if (currentTile.negativeEffect > 0 && Mathf.FloorToInt(timeSpent / updateInterval) > Mathf.FloorToInt((timeSpent - updateInterval) / updateInterval))
                {
                    ApplyNegativeEffect(currentTile.negativeEffect);
                }

                Debug.Log($"Прогрес на тайлі {currentTile.tileName}: {progress * 100}%");
            }

            Debug.Log($"Гравець завершив тайл {currentTile.tileName} (ID: {pathStepIds[i]})");
        }

        // Закриваємо RunUI після завершення шляху
        menuController.LocationUI.HideAll();

        Debug.Log("Гравець завершив обраний шлях.");
        // Логіка після завершення проходження шляху
    }



    private void ApplyNegativeEffect(int effectValue)
    {
        Debug.Log($"Гравець отримує негативний ефект: {effectValue}");
        Player.Instance.ApplyStatus(effectValue); // Логіка в класі `Player`
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