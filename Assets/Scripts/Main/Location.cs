using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Path
{
    public int[] pathSteps = new int[5]; // Масив з 5 елементів для зберігання шляху
}

[CreateAssetMenu(fileName = "NewLocation", menuName = "Game/Location")]
public class Location : ScriptableObject
{
    [Header("Основні параметри локації")]
    public string locationName;                // Назва локації
    [TextArea]
    public string description;                 // Опис локації
    public Sprite locationSprite;              // Спрайт локації

    [Header("Налаштування проходження")]
    [Tooltip("Час на проходження для кожного рівня гравця від 1 до 5")]
    public int[] travelTimes = new int[5];     // Час проходження для кожного рівня гравця

    [Header("Події та шляхи")]
    public List<LocationEvent> events;         // Перелік доступних подій
    public List<Path> paths = new List<Path>(); // Список шляхів, де кожен шлях є екземпляром класу Path

    // Метод для отримання двох випадкових шляхів
    public List<Path> GetTwoRandomPaths()
    {
        if (paths.Count < 2)
        {
            Debug.LogWarning("Недостатньо шляхів для вибору двох унікальних.");
            return paths;
        }

        // Вибір двох випадкових шляхів
        int firstPathIndex = Random.Range(0, paths.Count);
        int secondPathIndex;
        do
        {
            secondPathIndex = Random.Range(0, paths.Count);
        } while (secondPathIndex == firstPathIndex);

        return new List<Path> { paths[firstPathIndex], paths[secondPathIndex] };
    }
}