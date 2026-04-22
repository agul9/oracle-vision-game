using UnityEngine;
using StarterAssets;
using System.Collections;
using UnityEngine.UI;

public class OracleBoneInteraction : MonoBehaviour
{
    //public MonoBehaviour playerController;
    private bool isPlayerInRange;
    public GameObject firstPrompt; // this is just the one that says press E
    public GameObject interactPrompt; // this is i think either the instructions or the quiz
    public GameObject pressFInteractPrompt; // "Press F" prompt when oracle info is open
    public GameObject pressFToClosePrompt; // "Press F to close" prompt when overlay is open
    public CanvasGroup muralGroup;
    public float fadeSpeed = 1.5f;
    public Image charUI; // the image of the character/sentence
    public GameObject glowEffect;
    private bool isOracleInfoVisible = false;

    // varaibles for play ui test
    public GameObject fillInTheBlankPanel; 
    public Image displayImage; 
    public Sprite[] figmaScreenshots;
    public GameObject guideBeamParticles; // the light from the artifact to the mural

    public GameObject muralOverlay;
    public bool muralIsOverlayed;
    public Image sourceMuralImage;
    public AudioSource oracleMuralAppearSound;
    
    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (ArtifactInteraction.touchedOracleBone && ArtifactInteraction.artifactsDecoded > 0)
            {
                ToggleUI();
            } else
            {
                ArtifactInteraction.touchedOracleBone = true; // sets this to true the first time they interact with it
                firstPrompt.SetActive(false);
                ToggleUI();
            }
        }

        if (isOracleInfoVisible && isPlayerInRange && Input.GetKeyDown(KeyCode.F)) {
            muralIsOverlayed = !muralIsOverlayed;
            if (muralIsOverlayed)
            {
                muralOverlay.transform.Find("Image").GetComponent<Image>().sprite = sourceMuralImage.sprite;                DoorInteraction.LockPlayer(true);
                muralOverlay.SetActive(true);
                DoorInteraction.LockPlayer(true);
                if (pressFInteractPrompt != null) pressFInteractPrompt.SetActive(false);
                if (pressFToClosePrompt != null) pressFToClosePrompt.SetActive(true);
            } else
            {
                muralOverlay.SetActive(false);
                DoorInteraction.LockPlayer(false);
                if (pressFToClosePrompt != null) pressFToClosePrompt.SetActive(false);
                if (pressFInteractPrompt != null) pressFInteractPrompt.SetActive(true);
            }
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
                if (pressFInteractPrompt != null) pressFInteractPrompt.SetActive(false);
                if (pressFToClosePrompt != null) pressFToClosePrompt.SetActive(false);
                charUI.gameObject.SetActive(false);
                guideBeamParticles.SetActive(false);
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
            if (pressFInteractPrompt != null) pressFInteractPrompt.SetActive(true);
            if (pressFToClosePrompt != null) pressFToClosePrompt.SetActive(false);
            charUI.gameObject.SetActive(true);
            firstPrompt.SetActive(false);
            glowEffect.SetActive(false);
            guideBeamParticles.SetActive(true);
            oracleMuralAppearSound.Play();
            //playerController.enabled = true;
        } else
        {
            StartCoroutine(FadeMural(muralGroup.alpha, 0));
            interactPrompt.SetActive(false);
            if (pressFInteractPrompt != null) pressFInteractPrompt.SetActive(false);
            if (pressFToClosePrompt != null) pressFToClosePrompt.SetActive(false);
            charUI.gameObject.SetActive(false);
            glowEffect.SetActive(false);
            guideBeamParticles.SetActive(false);
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

    // void ShowFillInTheBlank() 
    // {
    //     bool isCurrentlyOpen = fillInTheBlankPanel.activeSelf;
    //     fillInTheBlankPanel.SetActive(!isCurrentlyOpen);

    //     if (!isCurrentlyOpen)
    //     {
    //         int count = ArtifactInteraction.artifactsDecoded; 
    //         if (count > 0) 
    //         {
    //             int index = Mathf.Clamp(count - 1, 0, figmaScreenshots.Length - 1);
    //             displayImage.sprite = figmaScreenshots[index];
    //         }
    //     }
    // }
}
