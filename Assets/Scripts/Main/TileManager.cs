using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; } // Глобальний доступ до TileManager

    private Dictionary<int, Tile> tileDictionary; // Словник для зберігання тайлів за їх ідентифікаторами

    private void Awake()
    {
        // Перевіряємо, чи вже існує екземпляр TileManager
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Дублікат TileManager виявлено і знищено.");
            Destroy(gameObject);
            return;
        }

        Instance = this; // Призначаємо цей екземпляр як активний
        DontDestroyOnLoad(gameObject); // Зберігаємо між сценами

        LoadAllTiles(); // Завантажуємо тайли
    }

    // Метод для завантаження всіх тайлів
    private void LoadAllTiles()
    {
        tileDictionary = new Dictionary<int, Tile>();
        Tile[] tiles = Resources.LoadAll<Tile>("Tiles");

        foreach (Tile tile in tiles)
        {
            if (tileDictionary.ContainsKey(tile.id))
            {
                Debug.LogWarning($"Дублюється ID тайла: {tile.id}. Пропущено.");
                continue;
            }

            tileDictionary.Add(tile.id, tile);
        }

        Debug.Log($"Завантажено {tileDictionary.Count} тайлів.");
    }

    // Публічний метод для отримання тайла за ID
    public Tile GetTileById(int id)
    {
        if (tileDictionary.TryGetValue(id, out Tile tile))
        {
            return tile;
        }
        else
        {
            Debug.LogWarning($"Тайл з ID {id} не знайдено.");
            return null;
        }
    }
}