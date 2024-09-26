using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageTextComponent;

    public void SetValue(float damageAmount)
    {
        damageTextComponent.text = $"-{damageAmount}".ToString(); // Встановлюємо значення тексту
    }

    // Повертаємо об'єкт у пул через певний час (наприклад, після анімації)
    public void ReturnToPool(DamageTextSpawner spawner)
    {
        // Логіка завершення анімації
        spawner.ReturnToPool(this);
    }
}
