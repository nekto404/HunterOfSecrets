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
    private float timeRemaining; // ������� ����
    [SerializeField]
    private bool isRoundActive = false;
    [SerializeField]
    private List<int> fullPath = new List<int>();
    [SerializeField]
    private Coroutine pathTraversalCoroutine;

    public int secretsFound = 0; // ʳ������ ��������� �������
    public int secretsRequiredForEvacuation = 3;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("������� GameManager �������� �� �������.");
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
                Debug.LogError("�� ������� ������ ��������� Shop �� ������.");
            }
            else
            {
                Debug.Log("������� �����������.");
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
            Debug.Log("��������� ������� �����������: " + currentLocation.locationName);
        }
        else
        {
            Debug.LogWarning("�� ������� ������ ����� ������� � Resources/Locations.");
        }
    }

    public Location GetCurrentLocation()
    {
        if (currentLocation == null)
        {
            Debug.LogWarning("������� �� �� �����������.");
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
            Debug.LogError("MenuController �� ����������� � GameManager.");
            return;
        }

        // ��������� ��� ������� UnityEvent
        UnityEvent onYesEvent = new UnityEvent();
        onYesEvent.RemoveAllListeners(); // ������� �������� ����� ���������� �����

        // ������ ������� ��� ������� ������
        onYesEvent.AddListener(StartRound);

        // ��������� ����� MenuController ��� ����������� ConfirmationUI
        menuController.ShowConfirmation(
            new List<UnityEvent> { onYesEvent },
            new List<UnityEvent>(), // ������ ������ ��� ������ "ͳ"
            "Are you ready to start the exploration?"
        );
    }

    public void StartRound()
    {
        Debug.Log("����� ���������!");
        if (currentLocation == null)
        {
            Debug.LogError("��������� ��������� �����: ������� �� �����������.");
            return;
        }

        // ��������� ����� ������ (�� ������������� 0, ���� ������ ���� ����)
        int playerLevel = Mathf.Clamp(Player.Instance.Level - 1, 0, currentLocation.travelTimes.Length - 1);

        // ������������ ��������� ������ � `travelTimes`
        timeRemaining = currentLocation.travelTimes[playerLevel];

        isRoundActive = true;
        menuController.GameShop.SetActive(false);
        menuController.GameLocation.SetActive(true);

        // ���������� �������, �� ����������� ��� ���� �� ����
        Player.Instance.CalculateTileEnterSkills();

        // ���������� �������, �� ����������� ��� �������� ����� ������
        Player.Instance.CalculateGetPlayerEffectStacksSkills();

        // ��������� ���� ����� ����� ShowPathSelection
        ShowPathSelection();
    }

    private void ShowPathSelection()
    {
        // �������� ��� ���������� ����� ����� ����� � Location
        var randomPaths = currentLocation.GetTwoRandomPaths();

        if (randomPaths == null || randomPaths.Count < 2)
        {
            Debug.LogError("�� ������� �������� ��� ���������� �����.");
            return;
        }

        // ����������� 䳿 ��� ������� �����
        var firstPathActions = new List<UnityEngine.Events.UnityEvent>();
        var firstAction = new UnityEngine.Events.UnityEvent();
        firstAction.AddListener(() => OnPathChosen(randomPaths[0])); // �������� ������ ����
        firstPathActions.Add(firstAction);

        // ����������� 䳿 ��� ������� �����
        var secondPathActions = new List<UnityEngine.Events.UnityEvent>();
        var secondAction = new UnityEngine.Events.UnityEvent();
        secondAction.AddListener(() => OnPathChosen(randomPaths[1])); // �������� ������ ����
        secondPathActions.Add(secondAction);
        menuController.LocationUI.HideAll();
        // �������� UI ��� ������ ������
        menuController.LocationUI.ShowPathUI(randomPaths[0].pathSteps, randomPaths[1].pathSteps, firstPathActions, secondPathActions);
    }


    private void OnPathChosen(Path chosenPath)
    {

        // ������ ������� ���� �� ���������� ��������
        fullPath.AddRange(chosenPath.pathSteps);

        // ��������� UI
        menuController.LocationUI.HideAll();

        // ����������� ����������� �������� �����
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
            // �������� �������� ���� �� ���� ID
            Tile currentTile = TileManager.Instance.GetTileById(pathStepIds[i]);
            if (currentTile == null)
            {
                Debug.LogWarning($"���� �� ID {pathStepIds[i]} �� ��������. ���������.");
                continue;
            }

            // ��������� ������� �������� ��� ���� �� ����
            foreach (var skill in Player.Instance.GetTileEnterSkills())
            {
                ActivateSkill(skill);
            }

            // �������� ������� ��� ����� �����
            Sprite previousTileSprite = i > 0 ? TileManager.Instance.GetTileById(pathStepIds[i - 1]).sprite : null;
            Sprite currentTileSprite = currentTile.sprite;
            Sprite nextTileSprite = i < pathStepIds.Length - 1 ? TileManager.Instance.GetTileById(pathStepIds[i + 1]).sprite : null;

            float timeSpent = 0f;
            float updateInterval = 0.1f; // �������� ��������� (10 ���� �� �������)
            float originalTravelTime = currentTile.travelTime; // �������� ����������� ��� �����������

            // ³��������� ���������� ���� ��������
            menuController.LocationUI.ShowRunUI(previousTileSprite, currentTileSprite, nextTileSprite, 0f);

            // �������� ����������� �����
            while (timeSpent < originalTravelTime)
            {
                // ����������, �� ������� ������� ����������
                if (Player.Instance.ShouldStop())
                {
                    Debug.Log("������� ��������� �� 3 �������.");
                    yield return new WaitForSeconds(3f); // ��������� ������ �� 3 �������
                }

                yield return new WaitForSeconds(updateInterval); // ������ 0.1 �������
                timeSpent += updateInterval * Player.Instance.GetSpeedModifier(); // ����������� ����������� ��������

                // ��������� ������� � LocationUI
                float progress = Mathf.Clamp01(timeSpent / originalTravelTime);
                menuController.LocationUI.ShowRunUI(previousTileSprite, currentTileSprite, nextTileSprite, progress);

                // ������ ���������� �����, ���� �� �, �� �������� ����������� ��������
                if (currentTile.negativeEffect > 0)
                {
                    int effectInterval = Mathf.FloorToInt(timeSpent / originalTravelTime);
                    if (effectInterval > 0 && Mathf.FloorToInt((timeSpent - updateInterval) / originalTravelTime) < effectInterval)
                    {
                        ApplyNegativeEffect(currentTile.negativeEffect);
                    }
                }

                Debug.Log($"������� �� ���� {currentTile.tileName}: {progress * 100}%");
                Debug.Log($"������ ������: {Player.Instance.GetActiveStatuses()}");
            }

            Debug.Log($"������� �������� ���� {currentTile.tileName} (ID: {pathStepIds[i]})");
        }

        // ��������� RunUI ���� ���������� �����
        menuController.LocationUI.HideAll();

        Debug.Log("������� �������� ������� ����.");

        // ���� ��������� ��䳿 � ������� �������
        if (currentLocation.events.Count > 0)
        {
            LocationEvent randomEvent = currentLocation.events[UnityEngine.Random.Range(0, currentLocation.events.Count)];
            ShowLocationEvent(randomEvent);
        }
        else
        {
            Debug.LogWarning("� ������� ������� ���� ��������� ����.");
        }
        // ����� ���� ���������� ����������� �����
    }

    private void ActivateSkill(Skill skill)
    {
        switch (skill.Effect)
        {
            case Effect.RemoveStackWithChance:
                if (UnityEngine.Random.Range(0, 100) < skill.EffectValue)
                {
                    Player.Instance.RemoveStatus(skill.EffectValueAdditional);
                    Debug.Log($"����� {skill.EffectDescription} ����������: �������� ���� ������ {skill.EffectValueAdditional}");
                }
                break;

            case Effect.AbbreviatedPassageOfTile:
                // ���������� ���� ����������� �����
                // ����� ��� ���������� ���� ����������� �����
                break;

            case Effect.AddStackWithChance:
                if (UnityEngine.Random.Range(0, 100) < skill.EffectValue)
                {
                    Player.Instance.ApplyStatus(skill.EffectValueAdditional);
                    Debug.Log($"����� {skill.EffectDescription} ����������: ������ ���� ������ {skill.EffectValueAdditional}");
                }
                break;
        }

        if (skill.OneTimeUse)
        {
            // ����� ��� ��������� ���������� �������
        }
    }

    private void ShowLocationEvent(LocationEvent locationEvent)
    {
        // ������� ������ �� ��� ��䳿
        List<UnityEvent> firstActions = new List<UnityEvent>();
        List<UnityEvent> secondActions = new List<UnityEvent>();

        // ������ ������: ������� ���� ������ � ��䳿
        UnityEvent participateAction = new UnityEvent();
        participateAction.AddListener(() =>
        {
            if (UnityEngine.Random.Range(0, 100) < locationEvent.successChance)
            {
                // ����: �������� ��������
                EventOutcome reward = locationEvent.GetRandomReward();
                ProcessEventOutcome(reward);
                Debug.Log($"������� ������ ������� ���� '{locationEvent.eventName}': {reward.description}");
            }
            else
            {
                // ������: �������� ���������
                EventOutcome penalty = locationEvent.GetRandomPenalty();
                ProcessEventOutcome(penalty);
                Debug.Log($"������� �������� ���� '{locationEvent.eventName}': {penalty.description}");
            }
        });
        firstActions.Add(participateAction);

        // ������ ������: ������� �������� ����
        UnityEvent skipAction = new UnityEvent();
        skipAction.AddListener(() =>
        {
            Debug.Log($"������� ��������� ���� '{locationEvent.eventName}'.");
            ShowPathSelection();
        });
        secondActions.Add(skipAction);

        // ³��������� UI � ��䳺�
        menuController.LocationUI.ShowActionChoseUI(locationEvent, firstActions, secondActions);

        // ���� ��� �������
        Debug.Log($"���� '{locationEvent.eventName}' �������� �������.");
        Debug.Log($"���� ��䳿: {locationEvent.description}");
    }

    public void CheckEvacuationAvailability()
    {
        if (secretsFound >= secretsRequiredForEvacuation)
        {
            Debug.Log("³������ ��������� ���������!");
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

    // �����-�������� ��� ��������
    private void ShowItemRewardPlaceholder(Item itemReward)
    {
        Debug.Log($"��� ���� ����� UI ��� ��������: {itemReward.Name}.");

        menuController.LocationUI.HideAll();
        ShowPathSelection(); // ���������� �� ������ �����
    }

    private void ApplyNegativeEffect(int effectValue)
    {
        Debug.Log($"������� ������ ���������� �����: {effectValue}");
        Player.Instance.ApplyStatus(effectValue); // ����� � ���� `Player`
        Debug.Log($"������ ������: {Player.Instance.GetActiveStatuses()}");
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
        Debug.Log("����� ���������!");
        isRoundActive = false;

        if (secretsFound >= secretsRequiredForEvacuation)
        {
            StopCoroutine(pathTraversalCoroutine);
            menuController.ShowWinScreen();
            Debug.Log("��� ���������! �� ������� �� �������.");
            // ��� ����� ������ ����� �������� ���������
        }
        else if (timeRemaining <= 0)
        {
            StopCoroutine(pathTraversalCoroutine);
            menuController.ShowLoseScreen();
            Debug.Log("��� ������! ��� ���������.");
            // ��� ����� ������ ����� �������� ��� ������ ������
        }

        // ������� ��� ��� ������� �� ������ �����
  
    }
}