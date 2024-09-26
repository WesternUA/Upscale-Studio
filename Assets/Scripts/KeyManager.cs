using UnityEngine;
using System.Collections;

public class KeyManager : MonoBehaviour
{
    public static int collectedKeys = 0;
    private int totalKeys = 5;
    public PlayerHealth player;
    public Animator doorAnimator;
    public AudioClip keyPickupSound;
    private AudioSource audioSource;

    private float scaleDuration = 1f;
    private Vector3 maxScale = new Vector3(1.1f, 1.1f, 1.1f);
    private bool raz = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (raz == true)
        {
            if (other.CompareTag("Player"))
            {
                raz = false;
                player.CollectKey();
                CollectKey();
                PlayKeyPickupSound();
                StartCoroutine(ScaleAndDeactivate());
            }
        }
    }

    public void CollectKey()
    {
        collectedKeys++;
        CheckForUnlock();
    }

    private void CheckForUnlock()
    {
        Debug.Log("Collected Keys: " + collectedKeys);

        if (collectedKeys >= totalKeys)
        {
            UnlockExit();
        }
    }

    private void UnlockExit()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("isOpened", true);
        }
        else
        {
            Debug.LogError("Door Animator is not assigned!");
        }
    }

    private IEnumerator ScaleAndDeactivate()
    {
        Vector3 originalScale = transform.localScale;
        float timeElapsed = 0;

        while (timeElapsed < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, maxScale, timeElapsed / scaleDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        timeElapsed = 0;
        while (timeElapsed < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(maxScale, Vector3.zero, timeElapsed / scaleDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void PlayKeyPickupSound()
    {
        if (audioSource != null && keyPickupSound != null)
        {
            audioSource.PlayOneShot(keyPickupSound);
        }
    }

    // Додаємо метод для скидання кількості зібраних ключів
    public static void ResetCollectedKeys()
    {
        collectedKeys = 0;
    }
}
