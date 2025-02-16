using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.5f;
    
    private Vector3 startPosition;
    private float bobTime;
    private PlayerAnimationController playerAnim;

    private void Start()
    {
        startPosition = transform.position;
        FindPlayer();
    }

    private void FindPlayer()
    {
        if (playerAnim != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerAnim = player.GetComponent<PlayerAnimationController>();
            if (playerAnim != null)
            {
                playerAnim.Initialize();
            }
        }
    }

    private void Update()
    {
        // Rotate the coin
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        
        // Bob up and down
        bobTime += Time.deltaTime;
        float newY = startPosition.y + Mathf.Sin(bobTime * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySound("CoinEffect");
            if (playerAnim != null)
            {
                playerAnim.StopCoinRunAnimation(); // Para toplanınca koşma animasyonunu durdur
            }
            DestroyCoin();
        }
    }

    public void DestroyCoin()
    {
        if (playerAnim != null)
        {
            playerAnim.StopCoinRunAnimation(); // Para yok edildiğinde de animasyonu durdur
        }
        Destroy(gameObject);
    }
} 