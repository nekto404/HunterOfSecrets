using TMPro;
using UnityEngine;

public class CoinsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText; // ��������� �� �������� ���� ��� �����

    private void OnEnable()
    {
        // ϳ��������� �� ���� ��������� �����
        Player.Instance.OnCoinsChanged += UpdateCoinsUI;
        // ��������� ��������� ������
        UpdateCoinsUI(Player.Instance.Coins);
    }

    private void OnDisable()
    {
        // ³��������� �� ��䳿
        Player.Instance.OnCoinsChanged -= UpdateCoinsUI;
    }

    private void UpdateCoinsUI(int coins)
    {
        // ��������� ���������� ����
        coinsText.text = ""+coins;
    }
}