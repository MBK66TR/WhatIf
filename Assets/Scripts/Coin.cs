using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.5f;
    
    private Vector3 startPosition;
    private float bobTime;

    private void Start()
    {
        startPosition = transform.position;
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
            // You can add score or other effects here
            DestroyCoin();
        }
    }

    public void DestroyCoin()
    {
        Destroy(gameObject);
    }
} 