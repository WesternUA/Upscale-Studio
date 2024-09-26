using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Для використання TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Створюємо статичний екземпляр GameManager
    public GameObject pauseMenuUI; // UI паузи
    public GameObject winMenuUI; // UI перемоги
    public GameObject loseMenuUI; // UI поразки
    public GameObject playerGameObject; // Додаємо посилання на об'єкт гравця

    public TextMeshProUGUI timerText; // Посилання на UI текст для відображення часу

    // Звукові ефекти
    public AudioClip gameMusic; // Фонова музика для гри
    public AudioClip pauseMusic; // Музика паузи
    public AudioClip winMusic; // Музика перемоги
    public AudioClip loseMusic; // Музика поразки
    private AudioSource audioSource; // Джерело звуку для музики

    private bool isPaused = false; // Чи гра на паузі
    private bool isGameOver = false; // Чи завершена гра (перемога або поразка)

    private float elapsedTime = 0f; // Лічильник часу

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>(); // Отримуємо компонент AudioSource
            PlayGameMusic(); // Вмикаємо фонову музику для гри
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!isPaused && !isGameOver) // Оновлюємо час тільки, якщо гра не на паузі і не завершена
        {
            elapsedTime += Time.deltaTime; // Додаємо час з кожним кадром
            UpdateTimerUI(); // Оновлюємо відображення часу
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver) // Додаємо перевірку, чи гра завершена
        {
            if (isPaused)
            {
                ResumeGame(); // Якщо гра на паузі, відновлюємо гру
            }
            else
            {
                PauseGame(); // Якщо гра не на паузі, ставимо її на паузу
            }
        }
    }

    // Оновлення тексту таймера в UI
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60F);
            int seconds = Mathf.FloorToInt(elapsedTime % 60F);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 100F) % 100F);
            timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}"; // Формат: хвилини:секунди:мілісекунди
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Зупиняємо час
        pauseMenuUI.SetActive(true); // Відображаємо меню паузи
        PlayPauseMusic(); // Вмикаємо музику паузи
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Відновлюємо час
        pauseMenuUI.SetActive(false); // Приховуємо меню паузи
        PlayGameMusic(); // Повертаємо музику гри
    }

    public void WinGame()
    {
        isGameOver = true; // Гра завершена перемогою
        Time.timeScale = 0f;
        winMenuUI.SetActive(true); // Відображаємо UI перемоги
        PlayWinMusic(); // Вмикаємо музику перемоги
    }

    public void LoseGame()
    {
        if (playerGameObject != null)
        {
            ShowLoseScreen(); // Відображаємо екран поразки
        }
        else
        {
            Debug.LogError("Player GameObject has been destroyed!");
        }
    }

    private void ShowLoseScreen()
    {
        isGameOver = true; // Встановлюємо, що гра завершена
        Time.timeScale = 0f; // Зупиняємо час
        loseMenuUI.SetActive(true); // Відображаємо UI поразки
        PlayLoseMusic(); // Вмикаємо музику поразки
    }

    public void RestartGame()
    {
        isGameOver = false; // Скидаємо стан гри
        Time.timeScale = 1f;
         KeyManager.ResetCollectedKeys(); // Скидаємо кількість зібраних ключів
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Перезавантажуємо активну сцену
        elapsedTime = 0f; // Скидаємо лічильник часу
        PlayGameMusic(); // Повертаємо фонову музику гри
    }

    public void QuitGame()
    {
        Application.Quit(); // Вихід з гри
    }

    // Додаємо метод для перевірки, чи гра завершена
    public bool IsGameOver()
    {
        return isGameOver;
    }

    // Додаємо метод для перевірки, чи гра на паузі
    public bool IsPaused()
    {
        return isPaused;
    }

    // Фонові звуки
    private void PlayGameMusic()
    {
        if (audioSource != null && gameMusic != null)
        {
            audioSource.clip = gameMusic;
            audioSource.Play();
        }
    }

    private void PlayPauseMusic()
    {
        if (audioSource != null && pauseMusic != null)
        {
            audioSource.clip = pauseMusic;
            audioSource.Play();
        }
    }

    private void PlayWinMusic()
    {
        if (audioSource != null && winMusic != null)
        {
            audioSource.clip = winMusic;
            audioSource.Play();
        }
    }

    private void PlayLoseMusic()
    {
        if (audioSource != null && loseMusic != null)
        {
            audioSource.clip = loseMusic;
            audioSource.Play();
        }
    }
}
