using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform foreground = null; // Смуга здоров'я
    [SerializeField] private PlayerHealth player = null; // Прив'язка до компонента PlayerHealth

    private void Update()
    {
        // Отримання відношення поточного здоров'я гравця до максимального здоров'я
        float healthFraction = (float)player.GetCurrentHealth() / player.GetMaxHealth();

        // Переконаємося, що значення між 0 і 1
        healthFraction = Mathf.Clamp(healthFraction, 0f, 1f);

        // Оновлюємо ширину смуги здоров'я (foreground) залежно від відсотка здоров'я
        foreground.localScale = new Vector3(healthFraction, 1, 1);
    }
}
