using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ArtifactInteraction : MonoBehaviour
{
    public bool needsOracleBoneFirst = true; // false for room 0 bc we don't need it then
    public static bool touchedOracleBone = false; // meaning, touched the oracle bone in room 1
    public static int artifactsDecoded = 0;
    private bool hasBeenCounted = false;
    public CanvasGroup muralGroup;
    public GameObject interactPrompt; // 'press E'
    public GameObject pressFInteractPrompt; // 'press f'
    public GameObject pressFToClosePrompt; // 'Press F to close' when overlay is open
    public float fadeSpeed = 1.5f;
    private bool isPlayerInRange;
    private bool isMuralVisible = false;
    public Image charUI; // the image of the character
    public GameObject guideBeamParticles; // the light from the artifact to the mural
    public GameObject muralOverlay;
    public bool muralIsOverlayed;
    public Image sourceMuralImage;
    public AudioSource muralAppearSound;


    void Start()
    {
        muralGroup.alpha = 0;
        muralGroup.gameObject.SetActive(false);
    }

    void Update()
    {
        if ((!needsOracleBoneFirst || touchedOracleBone) && isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Artifact Script is firing!");
            interactPrompt.SetActive(false);
            if (!isMuralVisible)
            {
                StopAllCoroutines(); // in case they spam E
                StartCoroutine(FadeMural(0, 1));
                isMuralVisible = true;
                muralAppearSound.Play();
                charUI.gameObject.SetActive(true);
                guideBeamParticles.SetActive(true);
                pressFInteractPrompt.SetActive(true);

                if (!hasBeenCounted) 
                {
                    artifactsDecoded++;
                    hasBeenCounted = true; 
                    //Debug.Log("Artifacts found: " + artifactsDecoded);
                }
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(FadeMural(1, 0));
                isMuralVisible = false;
                charUI.gameObject.SetActive(false);
                guideBeamParticles.SetActive(false);
            }
        }

        if (isMuralVisible && isPlayerInRange && Input.GetKeyDown(KeyCode.F)) {
            muralIsOverlayed = !muralIsOverlayed;
            if (muralIsOverlayed)
            {
                muralOverlay.transform.Find("Image").GetComponent<Image>().sprite = sourceMuralImage.sprite;                DoorInteraction.LockPlayer(true);
                muralOverlay.SetActive(true);
                DoorInteraction.LockPlayer(true);
                pressFInteractPrompt.SetActive(false);
                if (pressFToClosePrompt != null) pressFToClosePrompt.SetActive(true);
            } else
            {
                muralOverlay.SetActive(false);
                DoorInteraction.LockPlayer(false);
                if (pressFToClosePrompt != null) pressFToClosePrompt.SetActive(false);
                pressFInteractPrompt.SetActive(true);
            }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (!needsOracleBoneFirst || touchedOracleBone)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            interactPrompt.SetActive(false);
            charUI.gameObject.SetActive(false);
            guideBeamParticles.SetActive(false);
            pressFInteractPrompt.SetActive(false);
            if (pressFToClosePrompt != null) pressFToClosePrompt.SetActive(false);
            
            // hide mural if they walk away
            if (isMuralVisible)
            {
                StartCoroutine(FadeMural(1, 0));
                isMuralVisible = false;
            }
        }
    }
}