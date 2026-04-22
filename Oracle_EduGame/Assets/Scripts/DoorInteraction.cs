using UnityEngine;
using UnityEngine.UI;
using StarterAssets;
using System.Collections;
using UnityEngine.SceneManagement;

public class DoorInteraction : MonoBehaviour
{
    public QuizManager quizScript;
    public int numOfArtifacts; // this will be 1 for room 0, and 5 for room 1
    public GameObject doorLight; // make door glow once they finish interacting with all objects
    public GameObject doorObject;
    public int roomNum; // 0 for room 0, 1 for room 1 - assign in inspector
    public GameObject fillInTheBlankPanel;
    public GameObject interactPrompt; // press E
    //public Image displayImage;
    public Sprite quizPreviewImage;
    private bool isPlayerInRange;
    private bool isDoorOpen = false;
    public GameObject fadePanel; 
    void Start()
    {
        
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isDoorOpen)
        {
            ShowQuiz();
        }

        if (ArtifactInteraction.artifactsDecoded >= numOfArtifacts)
        {
            if (doorLight != null) {
                doorLight.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            interactPrompt.SetActive(false);
        }
    }

    void ShowQuiz() 
    {
        bool openingNow = !fillInTheBlankPanel.activeSelf;

        if (openingNow)
        {
            // 1. THE SAFETY CHECK: Did the timer finish while we were walking around?
            if (quizScript.isLockedOut && Time.unscaledTime >= quizScript.timerEndTime)
            {
                // If yes, we force the lockout to end before opening the panel
                quizScript.isLockedOut = false;
                quizScript.failedAttempts = 0;
                quizScript.quizCanvasGroup.interactable = true;
                quizScript.quizCanvasGroup.blocksRaycasts = true;
            }

            // 2. Open the main panel
            fillInTheBlankPanel.SetActive(true);
            
            // 3. Decide if the RED lockout screen should be visible
            quizScript.lockOutUI.SetActive(quizScript.isLockedOut);

            LockPlayer(true);
            quizScript.SetupRoom(roomNum); 
        } 
        else 
        {
            // Closing logic
            LockPlayer(false);
            fillInTheBlankPanel.SetActive(false);
            quizScript.lockOutUI.SetActive(false); // Hide red screen on exit
            
            if (quizScript.isRoomComplete) 
            {
                OpenDoor();
            }
        }
    }

    void OpenDoor()
    {
        isDoorOpen = true;
        
        if (roomNum == 1)
        {
            StartCoroutine(FadeAndEnd());
        } else
        {
            ArtifactInteraction.artifactsDecoded = 0; // reset for next room
            ArtifactInteraction.touchedOracleBone = false;
        
            doorObject.SetActive(false); 
            if (doorLight != null) {
                doorLight.SetActive(false);
            }
        }
    }

    void LockPlayer(bool lockIt)
    {
        Cursor.lockState = lockIt ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = lockIt;
        
        var controller = FindFirstObjectByType<FirstPersonController>();
        if(controller != null) controller.enabled = !lockIt;
    }


 // Add this at the very top of your script!

IEnumerator FadeAndEnd()
{
    fadePanel.SetActive(true);
    Image panelImage = fadePanel.GetComponent<Image>();
    float alpha = 0;

    // 1. Fade to Black
    while (alpha < 1)
    {
        alpha += Time.unscaledDeltaTime; 
        panelImage.color = new Color(0, 0, 0, alpha);
        yield return null;
    }

    yield return new WaitForSecondsRealtime(1f);

    // 2. THE NUCLEAR OPTION: Reload the current scene
    // This resets everything to its original state perfectly.
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
}
