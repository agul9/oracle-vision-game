using UnityEngine;
using StarterAssets;
using System.Collections;

public class OracleBoneInteraction : MonoBehaviour
{
    //public MonoBehaviour playerController;
    private bool isPlayerInRange;
    public GameObject firstPrompt; // this is just the one that says press E
    public GameObject interactPrompt; // this is i think either the instructions or the quiz
    public CanvasGroup muralGroup;
    public float fadeSpeed = 1.5f;
    public GameObject glowEffect;
    private bool isOracleInfoVisible = false;
    
    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ArtifactInteraction.touchedOracleBone = true; // sets this to true the first time they interact with it
            firstPrompt.SetActive(false);
            ToggleUI();
        }
    }

private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (!isOracleInfoVisible) 
            {
                firstPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            firstPrompt.SetActive(false);
            
            if (isOracleInfoVisible)
            {
                isOracleInfoVisible = false;
                interactPrompt.SetActive(false);
                StopAllCoroutines();
                StartCoroutine(FadeMural(muralGroup.alpha, 0));
            }
        }
    }

    private void ToggleUI ()
    {
        StopAllCoroutines();
        isOracleInfoVisible = !isOracleInfoVisible;

        if (isOracleInfoVisible)
        {
            StartCoroutine(FadeMural(muralGroup.alpha, 1));
            interactPrompt.SetActive(true);
            firstPrompt.SetActive(false);
            glowEffect.SetActive(false);
            //playerController.enabled = true;
        } else
        {
            StartCoroutine(FadeMural(muralGroup.alpha, 0));
            interactPrompt.SetActive(false);
            glowEffect.SetActive(false);
            //playerController.enabled = false;
        }
    }

    IEnumerator FadeMural(float startAlpha, float endAlpha)
    {
        if (endAlpha > 0) muralGroup.gameObject.SetActive(true);

        float timer = 0;
        while (timer < 1.0f)
        {
            timer += Time.deltaTime * fadeSpeed;
            muralGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timer);
            yield return null;
        }

        if (endAlpha <= 0) muralGroup.gameObject.SetActive(false);
    }
}
