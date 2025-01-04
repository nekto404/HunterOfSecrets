using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject shopPrefab;
    public MenuController menuController;

    [SerializeField]
    private Shop shopInstance;
    [SerializeField]
    private Location currentLocation;

    [SerializeField]
    private float timeRemaining; // Залишок часу
    [SerializeField]
    private bool isRoundActive = false;
    [SerializeField]
    private List<int> fullPath = new List<int>();
    [SerializeField]
    private Coroutine pathTraversalCoroutine;

    public int secretsFound = 0; // Кількість знайдених секретів
    public int secretsRequiredForEvacuation = 3;
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
        if (shopInstance != null)
        {
            Destroy(shopInstance.gameObject);
            shopInstance = null;
        }

        if (Player.Instance != null)
        {
            Player.Instance.ClearInstance();
        }

        secretsFound = 0;
        isRoundActive = false;
        fullPath = new List<int>();
        menuController.ClearLastGameUI();

        Player.Instance.Initialize();
        LoadShop();
        
        LoadRandomLocation();
        menuController.GameShop.SetActive(true);
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
                shopInstance.UpdateAvailableItems();
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
        int playerLevel = Mathf.Clamp(Player.Instance.Level - 1, 0, currentLocation.travelTimes.Length - 1);

        // Встановлюємо тривалість раунду з `travelTimes`
        timeRemaining = currentLocation.travelTimes[playerLevel];

        isRoundActive = true;
        menuController.GameShop.SetActive(false);
        menuController.GameLocation.SetActive(true);

        // Обчислюємо навички, що активуються при вході на тайл
        Player.Instance.CalculateTileEnterSkills();

        // Обчислюємо навички, що активуються при отриманні стека ефекту
        Player.Instance.CalculateGetPlayerEffectStacksSkills();

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
        menuController.LocationUI.HideAll();
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

            // Активація навичок предметів при вході на тайл
            foreach (var skill in Player.Instance.GetTileEnterSkills())
            {
                ActivateSkill(skill);
            }

            // Отримуємо спрайти для трьох тайлів
            Sprite previousTileSprite = i > 0 ? TileManager.Instance.GetTileById(pathStepIds[i - 1]).sprite : null;
            Sprite currentTileSprite = currentTile.sprite;
            Sprite nextTileSprite = i < pathStepIds.Length - 1 ? TileManager.Instance.GetTileById(pathStepIds[i + 1]).sprite : null;

            float timeSpent = 0f;
            float updateInterval = 0.1f; // Інтервал оновлення (10 разів на секунду)
            float originalTravelTime = currentTile.travelTime; // Зберігаємо оригінальний час проходження

            // Відображаємо початковий стан прогресу
            menuController.LocationUI.ShowRunUI(previousTileSprite, currentTileSprite, nextTileSprite, 0f);

            // Починаємо проходження тайла
            while (timeSpent < originalTravelTime)
            {
                // Перевіряємо, чи гравець повинен зупинитися
                if (Player.Instance.ShouldStop())
                {
                    Debug.Log("Гравець зупинений на 3 секунди.");
                    yield return new WaitForSeconds(3f); // Зупиняємо гравця на 3 секунди
                }

                yield return new WaitForSeconds(updateInterval); // Чекаємо 0.1 секунди
                timeSpent += updateInterval * Player.Instance.GetSpeedModifier(); // Застосовуємо модифікатор швидкості

                // Оновлюємо прогрес у LocationUI
                float progress = Mathf.Clamp01(timeSpent / originalTravelTime);
                menuController.LocationUI.ShowRunUI(previousTileSprite, currentTileSprite, nextTileSprite, progress);

                // Додаємо негативний ефект, якщо він є, та інтервал проходження збігається
                if (currentTile.negativeEffect > 0)
                {
                    int effectInterval = Mathf.FloorToInt(timeSpent / originalTravelTime);
                    if (effectInterval > 0 && Mathf.FloorToInt((timeSpent - updateInterval) / originalTravelTime) < effectInterval)
                    {
                        ApplyNegativeEffect(currentTile.negativeEffect);
                    }
                }

                Debug.Log($"Прогрес на тайлі {currentTile.tileName}: {progress * 100}%");
                Debug.Log($"Поточні ефекти: {Player.Instance.GetActiveStatuses()}");
            }

            Debug.Log($"Гравець завершив тайл {currentTile.tileName} (ID: {pathStepIds[i]})");
        }

        // Закриваємо RunUI після завершення шляху
        menuController.LocationUI.HideAll();

        Debug.Log("Гравець завершив обраний шлях.");

        // Вибір випадкової події з поточної локації
        if (currentLocation.events.Count > 0)
        {
            LocationEvent randomEvent = currentLocation.events[UnityEngine.Random.Range(0, currentLocation.events.Count)];
            ShowLocationEvent(randomEvent);
        }
        else
        {
            Debug.LogWarning("У поточній локації немає доступних подій.");
        }
        // Логіка після завершення проходження шляху
    }

    private void ActivateSkill(Skill skill)
    {
        switch (skill.Effect)
        {
            case Effect.RemoveStackWithChance:
                if (UnityEngine.Random.Range(0, 100) < skill.EffectValue)
                {
                    Player.Instance.RemoveStatus(skill.EffectValueAdditional);
                    Debug.Log($"Ефект {skill.EffectDescription} активовано: видалено стек ефекту {skill.EffectValueAdditional}");
                }
                break;

            case Effect.AbbreviatedPassageOfTile:
                // Скорочення часу проходження тайла
                // Логіка для скорочення часу проходження тайла
                break;

            case Effect.AddStackWithChance:
                if (UnityEngine.Random.Range(0, 100) < skill.EffectValue)
                {
                    Player.Instance.ApplyStatus(skill.EffectValueAdditional);
                    Debug.Log($"Ефект {skill.EffectDescription} активовано: додано стек ефекту {skill.EffectValueAdditional}");
                }
                break;
        }

        if (skill.OneTimeUse)
        {
            // Логіка для видалення одноразової навички
        }
    }

    private void ShowLocationEvent(LocationEvent locationEvent)
    {
        // Формуємо списки дій для події
        List<UnityEvent> firstActions = new List<UnityEvent>();
        List<UnityEvent> secondActions = new List<UnityEvent>();

        // Перший варіант: Гравець бере участь у події
        UnityEvent participateAction = new UnityEvent();
        participateAction.AddListener(() =>
        {
            if (UnityEngine.Random.Range(0, 100) < locationEvent.successChance)
            {
                // Успіх: отримати нагороду
                EventOutcome reward = locationEvent.GetRandomReward();
                ProcessEventOutcome(reward);
                Debug.Log($"Гравець успішно виконав подію '{locationEvent.eventName}': {reward.description}");
            }
            else
            {
                // Провал: отримати покарання
                EventOutcome penalty = locationEvent.GetRandomPenalty();
                ProcessEventOutcome(penalty);
                Debug.Log($"Гравець провалив подію '{locationEvent.eventName}': {penalty.description}");
            }
        });
        firstActions.Add(participateAction);

        // Другий варіант: Гравець пропускає подію
        UnityEvent skipAction = new UnityEvent();
        skipAction.AddListener(() =>
        {
            Debug.Log($"Гравець пропустив подію '{locationEvent.eventName}'.");
            ShowPathSelection();
        });
        secondActions.Add(skipAction);

        // Відображаємо UI з подією
        menuController.LocationUI.ShowActionChoseUI(locationEvent, firstActions, secondActions);

        // Логи для відладки
        Debug.Log($"Подія '{locationEvent.eventName}' показана гравцеві.");
        Debug.Log($"Опис події: {locationEvent.description}");
    }

    public void CheckEvacuationAvailability()
    {
        if (secretsFound >= secretsRequiredForEvacuation)
        {
            Debug.Log("Відкрито можливість евакуації!");
            EndRound();
        }
    }

    public void ProcessEventOutcome(EventOutcome outcome)
    {
        // List of actions to execute after showing the result
        List<UnityEvent> skipActions = new List<UnityEvent>();
        UnityEvent skipAction = new UnityEvent();
        skipAction.AddListener(() =>
        {
            Debug.Log("The player finished viewing the event result.");
            // Return to path selection

            ShowPathSelection();
        });
        skipActions.Add(skipAction);

        string message = string.Empty;

        switch (outcome.outcomeType)
        {
            case EventOutcome.OutcomeType.Coins:
                Player.Instance.AddCoins(outcome.value);
                message = outcome.value > 0
                    ? $"You gained {Mathf.Abs(outcome.value)} coins!"
                    : $"You lost {Mathf.Abs(outcome.value)} coins!";
                break;

            case EventOutcome.OutcomeType.StatusEffect:
                if (outcome.effectType >= 0 && outcome.effectType < Player.Instance.CurrentStatuses.Length)
                {
                    for (int i = 0; i < outcome.value; i++)
                    {
                        Player.Instance.ApplyStatus(outcome.effectType);
                    }
                    message = $"You are affected by: {outcome.description}.";
                }
                break;

            case EventOutcome.OutcomeType.TimeLose:
                timeRemaining -= outcome.value;
                message = $"You lost {outcome.value} seconds.";
                break;

            case EventOutcome.OutcomeType.SecretFounded:
                secretsFound++;
                message = "You found a secret!";
                CheckEvacuationAvailability();
                break;

            case EventOutcome.OutcomeType.ItemReward:
                // Placeholder for item reward handling
                Debug.Log($"Item found: {outcome.itemReward.Name}. UI implementation pending.");
                ShowItemRewardPlaceholder(outcome.itemReward);
                return; // Exit to avoid calling ShowTextResultUI
        }

        if (!string.IsNullOrEmpty(message))
        {
            // Display result as text
            Debug.Log("Send text " + message);
            menuController.LocationUI.HideAll();
            menuController.LocationUI.ShowTextResultUI(skipActions, message);
        }
    }

    // Метод-заглушка для предмета
    private void ShowItemRewardPlaceholder(Item itemReward)
    {
        Debug.Log($"Тут буде показ UI для предмета: {itemReward.Name}.");

        menuController.LocationUI.HideAll();
        ShowPathSelection(); // Повернення до вибору шляху
    }

    private void ApplyNegativeEffect(int effectValue)
    {
        Debug.Log($"Гравець отримує негативний ефект: {effectValue}");
        Player.Instance.ApplyStatus(effectValue); // Логіка в класі `Player`
        Debug.Log($"Поточні ефекти: {Player.Instance.GetActiveStatuses()}");
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

        if (secretsFound >= secretsRequiredForEvacuation)
        {
            StopCoroutine(pathTraversalCoroutine);
            menuController.ShowWinScreen();
            Debug.Log("Гра завершена! Ви знайшли всі секрети.");
            // Тут можна додати логіку відкриття евакуації
        }
        else if (timeRemaining <= 0)
        {
            StopCoroutine(pathTraversalCoroutine);
            menuController.ShowLoseScreen();
            Debug.Log("Час вийшов! Гру завершено.");
            // Тут можна додати логіку програшу або іншого фіналу
        }

        // Зупинка гри або перехід до іншого стану
  
    }
}