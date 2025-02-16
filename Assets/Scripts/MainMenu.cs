using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject soundManagerPrefab;

    private void Start()
    {
        // Eğer SoundManager yoksa oluştur
        if (SoundManager.Instance == null)
        {
            Instantiate(soundManagerPrefab);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

}
