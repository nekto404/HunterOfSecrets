using UnityEngine;

[CreateAssetMenu(fileName = "NewTile", menuName = "Tile")]
public class Tile : ScriptableObject
{
    public int id;                          // Унікальний ідентифікатор тайла
    public string tileName;                 // Назва тайла
    public Sprite sprite;                   // Спрайт для відображення
    public int travelTime;                  // Час для проходження цього тайла
    public int negativeEffect;              // Показник негативного ефекту тайла
}