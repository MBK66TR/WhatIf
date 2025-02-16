using UnityEngine;
using System.Collections;

public class UITimer : MonoBehaviour
{
    [System.Serializable]
    public class UITimerData
    {
        public GameObject uiElement;
        public float delayBeforeShow = 2f;
        public float displayDuration = 3f;
    }

    [SerializeField] private UITimerData[] uiElements; // Inspector'da birden fazla UI elementi ve süreleri ayarlanabilir

    private void Start()
    {
        foreach (var uiData in uiElements)
        {
            if (uiData.uiElement != null)
            {
                StartCoroutine(HandleUIVisibility(uiData));
            }
        }
    }

    private IEnumerator HandleUIVisibility(UITimerData data)
    {
        data.uiElement.SetActive(false);
        
        // Level yüklendikten sonra belirlenen süre kadar bekle
        yield return new WaitForSeconds(data.delayBeforeShow);
        
        // UI'ı göster
        data.uiElement.SetActive(true);
        
        // Belirlenen süre kadar görünür kal
        yield return new WaitForSeconds(data.displayDuration);
        
        // UI'ı gizle
        data.uiElement.SetActive(false);
    }
}
