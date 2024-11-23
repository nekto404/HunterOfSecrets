using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; } // ���������� ������ �� TileManager

    private Dictionary<int, Tile> tileDictionary; // ������� ��� ��������� ����� �� �� ����������������

    private void Awake()
    {
        // ����������, �� ��� ���� ��������� TileManager
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("������� TileManager �������� � �������.");
            Destroy(gameObject);
            return;
        }

        Instance = this; // ���������� ��� ��������� �� ��������
        DontDestroyOnLoad(gameObject); // �������� �� �������

        LoadAllTiles(); // ����������� �����
    }

    // ����� ��� ������������ ��� �����
    private void LoadAllTiles()
    {
        tileDictionary = new Dictionary<int, Tile>();
        Tile[] tiles = Resources.LoadAll<Tile>("Tiles");

        foreach (Tile tile in tiles)
        {
            if (tileDictionary.ContainsKey(tile.id))
            {
                Debug.LogWarning($"���������� ID �����: {tile.id}. ���������.");
                continue;
            }

            tileDictionary.Add(tile.id, tile);
        }

        Debug.Log($"����������� {tileDictionary.Count} �����.");
    }

    // �������� ����� ��� ��������� ����� �� ID
    public Tile GetTileById(int id)
    {
        if (tileDictionary.TryGetValue(id, out Tile tile))
        {
            return tile;
        }
        else
        {
            Debug.LogWarning($"���� � ID {id} �� ��������.");
            return null;
        }
    }
}