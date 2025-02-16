using UnityEngine;
using System.Collections;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float minSpeedForMoving = 0.1f;
    
    [Header("Auto Run Settings")]
    [SerializeField] private float autoRunSpeed = 5f;
    [SerializeField] private float directionChangeInterval = 2f;
    private bool isAutoRunning = false;
    private float directionTimer = 0f;
    private int currentDirection = 1; // 1: sağ, -1: sol

    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isCoinRunning = false;
    private Transform targetCoin; // Hedef coin'in referansını tutacak
    [SerializeField] private float jumpCheckThreshold = 0.9f;

    private bool isInitialized = false;
    private bool isFinished = false; // Finish durumunu kontrol etmek için

    // Animator parameter names
    private readonly string IS_RUNNING = "IsRunning";    // Normal koşma
    private readonly string IS_COIN_RUN = "IsCoinRun";  // Para görünce koşma
    private readonly string TRIGGER_FINISH = "Finish";   // Bitiş animasyonu
    private readonly string IS_AIR = "IsAir"; // Havada olma

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (isInitialized) return;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Animator'ı daha güvenli bir şekilde başlat
        if (TryGetComponent(out Animator anim))
        {
            animator = anim;
            animator.enabled = true;
            
            // Parametreleri kontrol et ve oluştur
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == IS_RUNNING || param.name == IS_COIN_RUN || param.name == TRIGGER_FINISH || param.name == IS_AIR)
                {
                    continue;
                }
            }
        }
        else
        {
            Debug.LogError($"Animator component bulunamadı: {gameObject.name}");
        }

        isInitialized = true;
    }

    private void OnEnable()
    {
        if (animator != null && !animator.enabled)
        {
            animator.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    private void Update()
    {
        if (!isInitialized) return;
        UpdateMovementAnimation();
        UpdateDirection();
    }

    private void UpdateMovementAnimation()
    {
        if (isFinished) return; // Finish olduysa diğer animasyonları güncelleme
        if (IsGrounded())
        {
            StopAirAnimation();
            if (!isCoinRunning)
            {
                bool isMoving = Mathf.Abs(rb.velocity.magnitude) > minSpeedForMoving;
                animator.SetBool(IS_RUNNING, isMoving);
            }
        }
        else
        {
            animator.SetBool(IS_RUNNING, false);
            animator.SetBool(IS_COIN_RUN, false);
            StartAirAnimation();
        }
    }

    private void UpdateDirection()
    {
        if (isFinished) return; // Finish olduysa yön güncellemeyi durdur
        
        if (isCoinRunning && targetCoin != null)
        {
            Vector2 directionToCoin = targetCoin.position - transform.position;
            if (Mathf.Abs(directionToCoin.x) > 0.1f)
            {
                spriteRenderer.flipX = directionToCoin.x < 0;
            }
        }
        else if (Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            spriteRenderer.flipX = rb.velocity.x < 0;
        }
    }

    // Para görünce koşma animasyonunu başlat
    public void StartCoinRunAnimation(Transform coin)
    {
        if (animator == null || isFinished) return; // Finish olduysa yeni animasyon başlatma
        
        isCoinRunning = true;
        targetCoin = coin;
        animator.SetBool(IS_COIN_RUN, true);
        animator.SetBool(IS_RUNNING, false);
    }

    // Para koşma animasyonunu durdur
    public void StopCoinRunAnimation()
    {
        if (animator == null || isFinished) return;
        
        isCoinRunning = false;
        targetCoin = null;
        animator.SetBool(IS_COIN_RUN, false);
    }

    // Bitiş animasyonunu tetikle
    public void TriggerFinishAnimation()
    {
        if (animator == null) return;
        
        isFinished = true;
        isCoinRunning = false;
        targetCoin = null;
        
        // Önce diğer animasyonları durdur
        animator.SetBool(IS_RUNNING, false);
        animator.SetBool(IS_COIN_RUN, false);
        
        // Kısa bir gecikme ile Finish trigger'ını tetikle
        StartCoroutine(TriggerFinishAfterDelay());
    }

    private IEnumerator TriggerFinishAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetTrigger(TRIGGER_FINISH);
        
        // Debug log ekleyelim
        Debug.Log("Finish animation triggered!");
    }

    private void StartAirAnimation()
    {
        animator.SetBool(IS_AIR, true);
    }

    private void StopAirAnimation()
    {
        animator.SetBool(IS_AIR, false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.down * jumpCheckThreshold);
    }

    private bool IsGrounded()
    {
        // Karakterin altında küçük bir raycast ile yere değip değmediğini kontrol et
        RaycastHit2D hit = Physics2D.Raycast(
            new Vector2(transform.position.x, (transform.position.y + jumpCheckThreshold) * -1), 
            Vector2.down, 
            jumpCheckThreshold, 
            LayerMask.GetMask("Ground")
        );
        return hit.collider != null;
    }

    public void StartAutoRun()
    {
        if (animator == null || isFinished) return;
        
        isAutoRunning = true;
        animator.SetBool(IS_RUNNING, true);
        StartCoroutine(AutoRunRoutine());
    }

    private IEnumerator AutoRunRoutine()
    {
        while (isAutoRunning && !isFinished)
        {
            // Rastgele yön değiştirme zamanı
            directionTimer += Time.deltaTime;
            if (directionTimer >= Random.Range(directionChangeInterval * 0.5f, directionChangeInterval * 1.5f))
            {
                currentDirection *= -1; // Yönü tersine çevir
                directionTimer = 0f;
                spriteRenderer.flipX = currentDirection < 0;
            }

            // Karakteri hareket ettir
            Vector2 movement = new Vector2(currentDirection * autoRunSpeed, rb.velocity.y);
            rb.velocity = movement;

            // CoinRun animasyonunu kullan
            animator.SetBool(IS_COIN_RUN, true);
            animator.SetBool(IS_RUNNING, false);

            yield return null;
        }
    }

    public void StopAutoRun()
    {
        isAutoRunning = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
        animator.SetBool(IS_COIN_RUN, false);
        animator.SetBool(IS_RUNNING, false);
    }
} 