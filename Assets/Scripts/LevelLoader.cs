using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject soundManagerPrefab;

    private void Awake()
    {
        if (SoundManager.Instance == null)
        {
            Instantiate(soundManagerPrefab);
        }
    }

    private void Start()
    {
        // Level başladığında rastgele müzik çal
        SoundManager.Instance.PlayRandomMusic();
    }
} 