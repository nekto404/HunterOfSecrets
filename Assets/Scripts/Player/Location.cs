using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Path
{
    public int[] pathSteps = new int[5]; // ����� � 5 �������� ��� ��������� �����
}

[CreateAssetMenu(fileName = "NewLocation", menuName = "Game/Location")]
public class Location : ScriptableObject
{
    [Header("������ ��������� �������")]
    public string locationName;                // ����� �������
    [TextArea]
    public string description;                 // ���� �������
    public Sprite locationSprite;              // ������ �������

    [Header("������������ �����������")]
    [Tooltip("��� �� ����������� ��� ������� ���� ������ �� 1 �� 5")]
    public int[] travelTimes = new int[5];     // ��� ����������� ��� ������� ���� ������

    [Header("��䳿 �� �����")]
    public List<LocationEvent> events;         // ������ ��������� ����
    public List<Path> paths = new List<Path>(); // ������ ������, �� ����� ���� � ����������� ����� Path

    // ����� ��� ��������� ���� ���������� ������
    public List<Path> GetTwoRandomPaths()
    {
        if (paths.Count < 2)
        {
            Debug.LogWarning("����������� ������ ��� ������ ���� ���������.");
            return paths;
        }

        // ���� ���� ���������� ������
        int firstPathIndex = Random.Range(0, paths.Count);
        int secondPathIndex;
        do
        {
            secondPathIndex = Random.Range(0, paths.Count);
        } while (secondPathIndex == firstPathIndex);

        return new List<Path> { paths[firstPathIndex], paths[secondPathIndex] };
    }
}