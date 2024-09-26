using UnityEngine;

public class Portal : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.WinGame(); // Якщо гравець заходить у портал, викликаємо перемогу
        }
    }
}
