using System.Runtime.CompilerServices;
using UnityEngine;

public class GameRules : MonoBehaviour
{
    [Header("Fizik Ayarları")]
    [SerializeField] private float forcePower = 5f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float windForce = 3f;
    [SerializeField] private float coinAttractionForce = 4f;
    [SerializeField] private GameObject coinPrefab;

    private Rigidbody2D playerRb;
    private Camera mainCamera;

    private int currentMode = 0; // 0: Normal, 1: Rüzgar, 2: Para Çekimi

    private GameObject coin;
    
    
    void Start()
    {
        // Oyuncunun Rigidbody2D bileşenini al
        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        
        // Rigidbody2D ayarlarını yap
        if (playerRb != null)
        {
            playerRb.gravityScale = 1f;
            playerRb.drag = 0.5f; // Hava direnci
        }
    }

    void Update()
    {
        if (playerRb == null) return;

        // Mod değiştirme
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentMode = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentMode = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentMode = 2;

        HandleModes();
    }

    void HandleModes()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        switch (currentMode)
        {
            case 0: // Normal mod
                if (Input.GetMouseButton(0))
                {
                    Vector2 distance = mousePosition - (Vector2)playerRb.transform.position;    
                    Vector2 direction = distance.normalized;
                    ApplyForce(direction, distance);
                }
                break;

            case 1: // Rüzgar modu
                if (Input.GetMouseButton(0))
                {
                    Vector2 windDirection = (mousePosition - (Vector2)playerRb.transform.position).normalized;
                    ApplyWind(windDirection);
                }
                break;

            case 2: // Para modu

                if(coin != null)
                {
                    MoveToCoin(coin);
                }
                else if(Input.GetMouseButtonDown(0))
                {
                    SpawnCoin(mousePosition);
                }
                if(Input.GetMouseButtonDown(1))
                {
                    if(coin != null)
                    {
                        coin.GetComponent<Coin>().DestroyCoin();
                    }
                }
                break;
        }
    }

    private void ApplyForce(Vector2 direction, Vector2 distance)
    {
        float distanceMultiplier = -1/distance.magnitude;
        // Hız sınırlaması kontrol et
        if (playerRb.velocity.magnitude < maxSpeed)
        {
            playerRb.AddForce(direction * forcePower * distanceMultiplier, ForceMode2D.Force);
        }
    }

    private void ApplyWind(Vector2 direction)
    {
        playerRb.AddForce(direction * windForce, ForceMode2D.Force);
    }

    private void SpawnCoin(Vector2 position)
    {

        // Eğer coin yoksa, yeni bir tane oluştur
            GameObject newCoin = Instantiate(coinPrefab, position, Quaternion.identity);
            newCoin.tag = "Coin"; // Coin tag'ini eklemeyi unutmayın
            coin = newCoin;
        
        
    }

    private void MoveToCoin(GameObject coin)
    {
        if( Vector2.Distance(playerRb.transform.position, coin.transform.position) < 20)
        {
            playerRb.transform.position = Vector2.MoveTowards(playerRb.transform.position, coin.transform.position, coinAttractionForce * Time.deltaTime);
        }
    }
}



