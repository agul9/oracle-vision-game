using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class GameStartManager : MonoBehaviour
{
    public CanvasGroup introPanel;
    public CanvasGroup instructionPanel;
    public GameObject staticBackground;
    public CanvasGroup introVideo; // control for the hide/show of ui
    public VideoPlayer videoPlayer; // controls the video controls like play

    private int screenState = 0;
    public float fadeSpeed = 1.0f;
    public GameObject playerController;

    void Start()
    {
        introPanel.alpha = 1;
        instructionPanel.alpha = 0;
        instructionPanel.gameObject.SetActive(false);

        introVideo.alpha = 0;
        introVideo.interactable = false;
        introVideo.blocksRaycasts = false; 

        Time.timeScale = 0; // pause game
        Cursor.lockState = CursorLockMode.None; // shows the mouse

        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.loopPointReached += OnVideoFinished; // advance when video ends
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
        vp.Pause();
    }
    // Update is called once per frame
    void Update()
    {
        if (screenState == 0 && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(FadeOutAndIn(introPanel, introVideo));
            screenState++;
        }
    }
    IEnumerator FadeOutAndIn(CanvasGroup outGroup, CanvasGroup inGroup)
    {
        while (outGroup.alpha > 0)
        {
            outGroup.alpha -= Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }
        outGroup.gameObject.SetActive(false);

        while (inGroup.alpha < 1)
        {
            inGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        if (videoPlayer != null)
        {
            if (!videoPlayer.isPrepared)
            {
                yield return new WaitUntil(() => videoPlayer.isPrepared);
            }
            videoPlayer.Play();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(FinalFadeOut(introVideo));
    }

    IEnumerator FinalFadeOut(CanvasGroup group)
    {
        while (group.alpha > 0)
        {
            group.alpha -= Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        if (staticBackground != null) 
        {
            staticBackground.SetActive(false);
        }
        
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        group.gameObject.SetActive(false);
        this.enabled = false;
    }
}
