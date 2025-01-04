using TMPro;
using UnityEngine;

public class CoinsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText; // Посилання на текстове поле для монет

    private void OnEnable()
    {
        // Підписуємося на подію оновлення монет
        Player.Instance.OnCoinsChanged += UpdateCoinsUI;
        // Початкове оновлення тексту
        UpdateCoinsUI(Player.Instance.Coins);
    }

    private void OnDisable()
    {
        // Відписуємося від події
        Player.Instance.OnCoinsChanged -= UpdateCoinsUI;
    }

    private void UpdateCoinsUI(int coins)
    {
        // Оновлення текстового поля
        coinsText.text = ""+coins;
    }
}