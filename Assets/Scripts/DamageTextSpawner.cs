using System.Collections.Generic;
using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    [SerializeField] private DamageText damageTextPrefab = null; // Префаб тексту дамага
    [SerializeField] private int poolSize = 10; // Розмір пулу
    [SerializeField] private Transform parentTransform = null; // Батьківський об'єкт для нових текстів
    private Queue<DamageText> pool; // Пул об'єктів

    private void Start()
    {
        pool = new Queue<DamageText>();

        // Ініціалізація пулу
        for (int i = 0; i < poolSize; i++)
        {
            DamageText instance = Instantiate(damageTextPrefab, parentTransform); // Створюємо об'єкт під батьком
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
    }

    // Спавн тексту дамага
    public void Spawn(float damageAmount)
    {
        DamageText instance;
        if (pool.Count > 0)
        {
            instance = pool.Dequeue();
        }
        else
        {
            instance = Instantiate(damageTextPrefab, parentTransform); // Створюємо новий об'єкт, якщо пул порожній
        }

        instance.transform.SetParent(parentTransform); // Встановлюємо батьківський об'єкт
        instance.gameObject.SetActive(true); // Активуємо об'єкт
        instance.SetValue(damageAmount); // Встановлюємо значення

        // Повертаємо об'єкт у пул після 1 секунди
        StartCoroutine(DeactivateAfterDelay(instance, 1f));
    }

    // Повертаємо об'єкт у пул
    public void ReturnToPool(DamageText damageText)
    {
        damageText.gameObject.SetActive(false);
        pool.Enqueue(damageText); // Додаємо об'єкт назад у пул
        Debug.Log("Returning DamageText to pool.");
    }

    private System.Collections.IEnumerator DeactivateAfterDelay(DamageText damageText, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(damageText); // Деактивуємо і повертаємо в пул
    }
}
