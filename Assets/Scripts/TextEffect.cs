using UnityEngine;
using TMPro;

public class TextEffect : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [SerializeField] private float bobSpeed = 2f; // Yukarı aşağı hareket hızı
    [SerializeField] private float bobAmount = 0.2f; // Yukarı aşağı hareket miktarı
    [SerializeField] private bool enableBob = true; // Yukarı aşağı hareketi aktif/pasif

    [Header("Ölçek Ayarları")]
    [SerializeField] private float scaleSpeed = 1f; // Büyüyüp küçülme hızı
    [SerializeField] private float scaleAmount = 0.1f; // Büyüyüp küçülme miktarı
    [SerializeField] private bool enableScale = true; // Ölçek değişimini aktif/pasif

    [Header("Döndürme Ayarları")]
    [SerializeField] private float rotationSpeed = 30f; // Dönme hızı
    [SerializeField] private float maxRotation = 5f; // Maksimum dönme açısı
    [SerializeField] private bool enableRotation = true; // Dönme hareketini aktif/pasif

    private Vector3 startPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private float time;

    private void Start()
    {
        startPosition = transform.position;
        originalScale = transform.localScale;
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        time += Time.deltaTime;

        // Yukarı aşağı hareket
        if (enableBob)
        {
            float newY = startPosition.y + Mathf.Sin(time * bobSpeed) * bobAmount;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        // Büyüyüp küçülme
        if (enableScale)
        {
            float scale = 1f + Mathf.Sin(time * scaleSpeed) * scaleAmount;
            transform.localScale = originalScale * scale;
        }

        // Sağa sola dönme
        if (enableRotation)
        {
            float rotation = Mathf.Sin(time * rotationSpeed) * maxRotation;
            transform.rotation = originalRotation * Quaternion.Euler(0, 0, rotation);
        }
    }

    // Efektleri sıfırlama metodu
    public void ResetEffects()
    {
        transform.position = startPosition;
        transform.localScale = originalScale;
        transform.rotation = originalRotation;
        time = 0f;
    }

    // Efektleri açıp kapatma metodları
    public void ToggleBob(bool state) => enableBob = state;
    public void ToggleScale(bool state) => enableScale = state;
    public void ToggleRotation(bool state) => enableRotation = state;
} 