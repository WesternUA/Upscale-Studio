using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100; // Максимальне здоров'я гравця
    private int currentHealth;  // Поточне здоров'я гравця
    [SerializeField] private TextMeshProUGUI keyCountText; // TMPro текст для відображення кількості ключів
    [SerializeField] private DamageTextSpawner damageTextSpawner; // Посилання на спаунер тексту дамага

    private int keysCollected = 0; // Лічильник зібраних ключів
    [SerializeField] private int totalKeys = 5; // Загальна кількість ключів для перемоги

    void Start()
    {
        currentHealth = maxHealth; // Встановлюємо поточне здоров'я на максимальне при старті гри
        UpdateKeyCountUI();
    }

    // Функція для отримання шкоди
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        // Спаунимо текст дамага
        if (damageTextSpawner != null)
        {
            damageTextSpawner.Spawn(damageAmount); // Відображаємо кількість шкоди
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Функція для збирання ключа
    public void CollectKey()
    {
        keysCollected++;
        UpdateKeyCountUI();
    }

    private void Die()
    {
        Debug.Log("Player is dead!");
        GameManager.Instance.LoseGame(); // Викликаємо функцію поразки через GameManager
        
    }

    // Оновлення UI для ключів
    private void UpdateKeyCountUI()
    {
        keyCountText.text = "" + keysCollected + "/" + totalKeys;
    }

    // Лікування гравця
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    // Повернення поточного здоров'я
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // Повернення максимального здоров'я
    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
