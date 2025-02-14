using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    [Header("Ayarlar")]
    [SerializeField] private string nextLevel = "Level2"; // Sonraki level'ın sahne adı
    [SerializeField] private float transitionDelay = 1f; // Geçiş için beklenecek süre
    [SerializeField] private ParticleSystem completionEffect; // Opsiyonel parçacık efekti
    
    private bool levelCompleted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (levelCompleted) return;
        
        // Eğer çarpışan obje Player ise
        if (other.CompareTag("Player"))
        {
            LevelCompleted();
        }
    }

    private void LevelCompleted()
    {
        levelCompleted = true;
        
        // Parçacık efektini oynat (eğer varsa)
        if (completionEffect != null)
        {
            completionEffect.Play();
        }
        
        // Sonraki levele geçiş
        Invoke("NextLevelTransition", transitionDelay);
    }

    private void NextLevelTransition()
    {
        // Sonraki sahneyi yükle
        SceneManager.LoadScene(nextLevel);
    }
} 