using UnityEngine;

[CreateAssetMenu(fileName = "NewTile", menuName = "Tile")]
public class Tile : ScriptableObject
{
    public int id;                          // ��������� ������������� �����
    public string tileName;                 // ����� �����
    public Sprite sprite;                   // ������ ��� �����������
    public float travelTime;                  // ��� ��� ����������� ����� �����
    public int negativeEffect;              // �������� ����������� ������ �����
}