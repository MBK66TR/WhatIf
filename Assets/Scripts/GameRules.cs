using System.Runtime.CompilerServices;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Sadece UnityEngine.UI kullanacağız

public class GameRules : MonoBehaviour
{
    [Header("Fizik Ayarları")]
    [SerializeField] private float forcePower = 5f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float coinAttractionForce = 4f;

/*
    [Header("Güç Ayarları")]
    [SerializeField] private float gucSarjSuresi = 2f;
    [SerializeField] private float gucCarpani = 2f;
    [SerializeField] private Color gucRengi = Color.red;
*/

    [SerializeField] private GameObject coinPrefab;

    private Rigidbody2D playerRb;
    private Camera mainCamera;
   // private float chargeTime;
    private Color baslangicRengi;
    private SpriteRenderer playerSprite;
    private bool RandomMoveBool = false;
    private float RandomMoveTime = 0f;

    private int currentMode = 0; // 0: Normal, 1: Rüzgar, 2: Para Çekimi

    private GameObject coin;
    
    private PlayerAnimationController animController;
    private bool isInitialized = false;

    [Header("Rüzgar Modu Ayarları")]
    [SerializeField] private float maxWindTime = 3f;
    [SerializeField] private float windRechargeTime = 5f;
    [SerializeField] private Image windBarFill;
    [SerializeField] private GameObject windBarContainer;
    
    private float currentWindTime; // Mevcut rüzgar süresi
    private bool canUseWind = true; // Rüzgar kullanılabilir mi
    private bool isWindActive = false; // Rüzgar aktif mi

    [Header("Para Modu Ayarları")]
    [SerializeField] private float maxCoinTime = 5f; // Para bekletme süresi
    [SerializeField] private Image coinBarFill; // Para bar'ı
    [SerializeField] private GameObject coinBarContainer;
    private float currentCoinTime;
    private bool canPlaceCoin = true;

    void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (isInitialized) return;

        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            animController = playerRb.GetComponent<PlayerAnimationController>();
            if (animController != null)
            {
                animController.Initialize();
            }
        }

        mainCamera = Camera.main;
        
        if (playerRb != null)
        {
            playerSprite = playerRb.GetComponent<SpriteRenderer>();
            if (playerSprite != null)
            {
                baslangicRengi = playerSprite.color;
            }

            playerRb.gravityScale = 1f;
            playerRb.drag = 0.5f;
        }

        isInitialized = true;
        currentWindTime = maxWindTime;
        UpdateWindBar();
        currentCoinTime = maxCoinTime;
        UpdateCoinBar();
    }

    void Update()
    {
        if (!isInitialized || playerRb == null) return;
        
        #region Random Move
        if(RandomMoveBool)  
        {
            RandomMoveBool = false;
            RandomMoveTime = 0f;
            calculateRandomMovement();
        }
        RandomMoveTime += Time.deltaTime;
        if(RandomMoveTime > 3f)
        {
            RandomMoveBool = true;
            RandomMoveTime = 0f;
        }
        #endregion

        // Mod değiştirme
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentMode = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentMode = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentMode = 2;

        HandleModes();
        HandleWindTimer();
    }

    void HandleModes()
    {
        // Bar'ların görünürlüğünü ayarla
        if (windBarContainer != null)
        {
            windBarContainer.SetActive(currentMode == 0);
        }
        if (coinBarContainer != null)
        {
            coinBarContainer.SetActive(currentMode == 2);
        }

        if(coin != null)
        {
            MoveToCoin(coin);
        }

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        switch (currentMode)
        {
            case 0: // Rüzgar mod
                if (Input.GetMouseButton(0) && canUseWind)
                {
                    isWindActive = true;
                    Vector2 distance = mousePosition - (Vector2)playerRb.transform.position;    
                    Vector2 direction = distance.normalized;
                    ApplyForce(direction, distance);
                }
                else
                {
                    isWindActive = false;
                }
                break;

            case 1: // Para modu

                if(Input.GetMouseButtonDown(0))
                {
                    SpawnCoin(mousePosition);
                }
                if(Input.GetMouseButtonDown(1))
                {
                    if(coin != null)
                    {
                        coin.GetComponent<Coin>().DestroyCoin();
                        coin = null;
                    }
                }
                break;

            default:
                windBarContainer?.SetActive(false); // Diğer modlarda bar'ı gizle
                break;
        }
    }

    private void ApplyForce(Vector2 direction, Vector2 distance)
    {
        float distanceMultiplier = -1;
        if(distance.magnitude != 0)
            {
                distanceMultiplier = -1/distance.magnitude;
            }

        // Hız sınırlaması kontrol et
        if (playerRb.velocity.magnitude < maxSpeed)
        {
            playerRb.AddForce(direction * forcePower * distanceMultiplier, ForceMode2D.Force);
        }
    }



    private void SpawnCoin(Vector2 position)
    {
        if (!canPlaceCoin) return;

        if (coin != null)
        {
            coin.GetComponent<Coin>().DestroyCoin();
        }

        GameObject newCoin = Instantiate(coinPrefab, position, Quaternion.identity);
        newCoin.tag = "Coin";
        coin = newCoin;

        canPlaceCoin = false;
        currentCoinTime = 0;
        StartCoroutine(RechargeCoin());
    }

    private void MoveToCoin(GameObject coin)
    {
        if (coin == null || animController == null) return;

        if(Vector2.Distance(playerRb.transform.position, coin.transform.position) < 20)
        {
            animController.StartCoinRunAnimation(coin.transform);
            playerRb.transform.position = Vector2.MoveTowards(playerRb.transform.position, 
                coin.transform.position, 
                coinAttractionForce * Time.deltaTime);
        }
        else
        {
            animController.StopCoinRunAnimation();
        }
    }
/*
    private void Bomb()
    {
        // Mouse basılı tutulduğunda güç şarj olur

        if (Input.GetMouseButton(0))

        {

            chargeTime += Time.deltaTime;

            float chargeRatio = Mathf.Clamp01(chargeTime / gucSarjSuresi);

            

            // Şarj efekti

            if (playerSprite != null)

            {

                playerSprite.color = Color.Lerp(baslangicRengi, gucRengi, chargeRatio);

            }

            

            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector2 direction = ((Vector2)playerRb.transform.position - mousePosition).normalized;

            

            // Çizgi çizerek yönü göster

            Debug.DrawLine(playerRb.transform.position, 

                (Vector2)playerRb.transform.position + direction * 2f, 

                Color.red);


        }

        

        // Mouse bırakıldığında güç uygulanır

        if (Input.GetMouseButtonUp(0))

        {

            float chargeRatio = Mathf.Clamp01(chargeTime / gucSarjSuresi);

            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector2 direction = ((Vector2)playerRb.transform.position - mousePosition).normalized;

            

            ApplyForce(direction * (1f + chargeRatio * gucCarpani), Vector2.zero);

            

            // Değerleri sıfırla

            chargeTime = 0f;

            if (playerSprite != null)

            {
                playerSprite.color = baslangicRengi;
            }


        }
    }
*/
    private void calculateRandomMovement()
    {
        int randomPossibility = Random.Range(0, 100);
        Vector2 randomDirection = new Vector2(Random.Range(-5f, 5f), playerRb.transform.position.y);
        if(randomPossibility < 70)
        {
            AddMovement(randomDirection);
        }
    }

    private void AddMovement(Vector2 direction)
    {
        playerRb.AddForce(direction, ForceMode2D.Impulse);
    }

    private void HandleWindTimer()
    {
        if (currentMode != 0) return;

        if (isWindActive)
        {
            currentWindTime -= Time.deltaTime;
            if (currentWindTime <= 0)
            {
                currentWindTime = 0;
                canUseWind = false;
                isWindActive = false;
                StartCoroutine(RechargeWind());
            }
        }

        UpdateWindBar();
    }

    private IEnumerator RechargeWind()
    {
        float rechargeProgress = 0;
        
        while (rechargeProgress < windRechargeTime)
        {
            rechargeProgress += Time.deltaTime;
            currentWindTime = Mathf.Lerp(0, maxWindTime, rechargeProgress / windRechargeTime);
            UpdateWindBar();
            yield return null;
        }

        currentWindTime = maxWindTime;
        canUseWind = true;
        UpdateWindBar();
    }

    private void UpdateWindBar()
    {
        
        if (windBarFill != null)
        {
            float normalizedValue = Mathf.Clamp01(currentWindTime / maxWindTime);
            windBarFill.fillAmount = normalizedValue;
        }
    }

    private void UpdateCoinBar()
    {
        if (coinBarFill != null)
        {
            float normalizedValue = Mathf.Clamp01(currentCoinTime / maxCoinTime);
            coinBarFill.fillAmount = normalizedValue;
        }
    }

    private IEnumerator RechargeCoin()
    {
        float rechargeProgress = 0;
        
        while (rechargeProgress < maxCoinTime)
        {
            rechargeProgress += Time.deltaTime;
            currentCoinTime = Mathf.Lerp(0, maxCoinTime, rechargeProgress / maxCoinTime);
            UpdateCoinBar();
            yield return null;
        }

        currentCoinTime = maxCoinTime;
        canPlaceCoin = true;
        UpdateCoinBar();
    }
}



