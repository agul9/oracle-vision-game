using UnityEngine;
using UnityEngine.UI;
using StarterAssets;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

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
    public GameObject endingUI;
    public GameObject endingVideoObject; // raw img object
    public VideoPlayer endingVideoPlayer; // video obj
    
    public AudioSource doorOpenSound;
    private bool waitingForRestart = false;
    void Start()
    {
        // load ending video and hide it
        if (endingVideoPlayer != null)
        {
            endingVideoPlayer.playOnAwake = false;
            endingVideoPlayer.loopPointReached += OnEndingVideoFinished;
            endingVideoPlayer.Prepare();
            endingVideoPlayer.prepareCompleted += OnEndingVideoPrepared;
        }

        if (endingVideoObject != null)
        {
            endingVideoObject.SetActive(false);
        }
    }

    //play it but then pause it right after
    void OnEndingVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
        vp.Pause();
    }

    void Update()
    {
        if (waitingForRestart && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

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
            // did the timer finish while we were walking around?
            if (quizScript.isLockedOut && Time.unscaledTime >= quizScript.timerEndTime)
            {
                // If yes, we force the lockout to end before opening the panel
                quizScript.isLockedOut = false;
                quizScript.failedAttempts = 0;
                quizScript.quizCanvasGroup.interactable = true;
                quizScript.quizCanvasGroup.blocksRaycasts = true;
            }

            // open the main panel
            fillInTheBlankPanel.SetActive(true);
            
            // decide if lockout screen should be visible
            quizScript.lockOutUI.SetActive(quizScript.isLockedOut);

            LockPlayer(true);
            quizScript.SetupRoom(roomNum); 
        } 
        else 
        {
            // Closing logic
            quizScript.CloseAllSuccessPopups();
            LockPlayer(false);
            fillInTheBlankPanel.SetActive(false);
            quizScript.lockOutUI.SetActive(false); // hide lock screen on exit
            
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
            ShowEndingUI();
        } else
        {
            ArtifactInteraction.artifactsDecoded = 0; // reset for next room
            ArtifactInteraction.touchedOracleBone = false;

            // Door gets disabled immediately, so OnTriggerExit may never fire.
            // Explicitly clear prompt/range state to avoid a stuck "Press E".
            isPlayerInRange = false;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        
            doorObject.SetActive(false);
            doorOpenSound.Play();
            if (doorLight != null) {
                doorLight.SetActive(false);
            }
        }
    }

    public static void LockPlayer(bool lockIt)
    {
        Cursor.lockState = lockIt ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = lockIt;
        
        var controller = FindFirstObjectByType<FirstPersonController>();
        if(controller != null) controller.enabled = !lockIt;
    }

    void ShowEndingUI()
    {
        StartCoroutine(FadeAndEnd());
    }

    IEnumerator FadeAndEnd()
    {
        // fade effect
        fadePanel.SetActive(true);
        Image panelImage = fadePanel.GetComponent<Image>();
        float alpha = 0;

        while (alpha < 1)
        {
            // Use unscaledDeltaTime in case you have time paused
            alpha += Time.unscaledDeltaTime; 
            panelImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // show + play the ending video
        if (endingVideoPlayer != null && endingVideoObject != null)
        {
            endingVideoObject.SetActive(true);

            if (!endingVideoPlayer.isPrepared)
            {
                yield return new WaitUntil(() => endingVideoPlayer.isPrepared);
            }
            endingVideoPlayer.Play();

            // fade the black panel out now so the video is actually visible underneath
            while (alpha > 0)
            {
                alpha -= Time.unscaledDeltaTime;
                panelImage.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            fadePanel.SetActive(false);

            // OnEndingVideoFinished() handles rest
            yield break;
        }

        // Fallback: if no video assigned, behave exactly like before
        if (endingUI != null)
        {
            endingUI.SetActive(true);
            waitingForRestart = true;
            LockPlayer(true);
        }

        while (alpha > 0)
        {
            alpha -= Time.unscaledDeltaTime;
            panelImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
        fadePanel.SetActive(false);
    }

        void OnEndingVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(FadeOutVideoThenShowEnding());
    }

    // NEW
    IEnumerator FadeOutVideoThenShowEnding()
    {
        // quick fade back to black before swapping to the acknowledgement screen,
        // so it's not a jarring hard-cut from video to UI
        fadePanel.SetActive(true);
        Image panelImage = fadePanel.GetComponent<Image>();
        float alpha = 0;

        while (alpha < 1)
        {
            alpha += Time.unscaledDeltaTime;
            panelImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        endingVideoObject.SetActive(false);
        yield return new WaitForSecondsRealtime(0.3f);

        if (endingUI != null)
        {
            endingUI.SetActive(true);
            waitingForRestart = true;
            LockPlayer(true);
        }

        while (alpha > 0)
        {
            alpha -= Time.unscaledDeltaTime;
            panelImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadePanel.SetActive(false);
    }
}
