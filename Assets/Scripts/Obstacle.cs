using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public int damageAmount = 20; // Кількість шкоди, яку завдає перешкода
    public float damageInterval = 1f; // Інтервал між завданням шкоди (1 секунда)
    public AudioClip trapActivationSound; // Звуковий ефект активації пастки
    public AudioClip collisionSound; // Звуковий ефект зіткнення з ворогом
    private AudioSource audioSource; // Джерело звуку
    private bool isPlayerInRange = false;
    private PlayerHealth playerHealth;

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Отримуємо компонент AudioSource
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                PlayTrapActivationSound(); // Програємо звук активації пастки
                InvokeRepeating("DealDamage", 0f, damageInterval);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            CancelInvoke("DealDamage");
        }
    }

    private void DealDamage()
    {
        if (isPlayerInRange && playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
            PlayCollisionSound(); // Програємо звук зіткнення з ворогом
            Debug.Log("Player took damage from obstacle: " + damageAmount);
        }
    }

    // Цей метод викликається через Animation Event для відтворення звуку активації пастки
    public void PlayTrapActivationSound()
    {
        if (audioSource != null && trapActivationSound != null)
        {
            audioSource.PlayOneShot(trapActivationSound); // Програємо звуковий ефект активації пастки
        }
    }

    private void PlayCollisionSound()
    {
        if (audioSource != null && collisionSound != null)
        {
            audioSource.PlayOneShot(collisionSound); // Програємо звуковий ефект зіткнення
        }
    }
}
