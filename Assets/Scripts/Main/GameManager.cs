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

    private float timeRemaining; // ������� ����
    private bool isRoundActive = false;

    private List<int> fullPath = new List<int>();
    private Coroutine pathTraversalCoroutine;

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
                Debug.LogError("�� ������� ������ ��������� Shop �� ������.");
            }
            else
            {
                Debug.Log("������� �����������.");
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
        int playerLevel = Mathf.Clamp(Player.Instance.level - 1, 0, currentLocation.travelTimes.Length - 1);

        // ������������ ��������� ������ � `travelTimes`
        timeRemaining = currentLocation.travelTimes[playerLevel];

        isRoundActive = true;
        menuController.GameShop.SetActive(false);
        menuController.GameLocation.SetActive(true);

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

            // �������� ������� ��� ����� �����
            Sprite previousTileSprite = i > 0 ? TileManager.Instance.GetTileById(pathStepIds[i - 1]).sprite : null;
            Sprite currentTileSprite = currentTile.sprite;
            Sprite nextTileSprite = i < pathStepIds.Length - 1 ? TileManager.Instance.GetTileById(pathStepIds[i + 1]).sprite : null;

            float timeSpent = 0f;
            float updateInterval = 0.1f; // �������� ��������� (10 ���� �� �������)

            // ³��������� ���������� ���� ��������
            menuController.LocationUI.ShowRunUI(previousTileSprite, currentTileSprite, nextTileSprite, 0f);

            // �������� ����������� �����
            while (timeSpent < currentTile.travelTime)
            {
                yield return new WaitForSeconds(updateInterval); // ������ 0.1 �������
                timeSpent += updateInterval;

                // ��������� ������� � LocationUI
                float progress = Mathf.Clamp01(timeSpent / currentTile.travelTime);
                menuController.LocationUI.ShowRunUI(previousTileSprite, currentTileSprite, nextTileSprite, progress);

                // ������ ���������� �����, ���� �� �, �� �������� ����������� ��������
                if (currentTile.negativeEffect > 0 && Mathf.FloorToInt(timeSpent / updateInterval) > Mathf.FloorToInt((timeSpent - updateInterval) / updateInterval))
                {
                    ApplyNegativeEffect(currentTile.negativeEffect);
                }

                Debug.Log($"������� �� ���� {currentTile.tileName}: {progress * 100}%");
            }

            Debug.Log($"������� �������� ���� {currentTile.tileName} (ID: {pathStepIds[i]})");
        }

        // ��������� RunUI ���� ���������� �����
        menuController.LocationUI.HideAll();

        Debug.Log("������� �������� ������� ����.");
        // ����� ���� ���������� ����������� �����
    }



    private void ApplyNegativeEffect(int effectValue)
    {
        Debug.Log($"������� ������ ���������� �����: {effectValue}");
        Player.Instance.ApplyStatus(effectValue); // ����� � ���� `Player`
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

        // ������� ��� ����� ���������� ������
    }
}