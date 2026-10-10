using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviour
{
    public CanvasGroup introPanel;
    //public CanvasGroup instructionPanel;
    public GameObject staticBackground;
    public CanvasGroup introVideo; // control for the hide/show of ui
    public VideoPlayer introVideoPlayer; // controls the video controls like play

    public CanvasGroup creditsVideo;
    public VideoPlayer creditsVideoPlayer;

    // start screen buttons
    public Button newGameBtn;
    public Button creditsBtn;
    public Button exitBtn;

    private bool isTransitioning = false;
    public float fadeSpeed = 1.0f;
    public GameObject playerController;

    void Start()
    {

        newGameBtn.onClick.AddListener(NewGameClicked);
        creditsBtn.onClick.AddListener(CreditsBtnClicked);
        exitBtn.onClick.AddListener(ExitBtnClicked);

        introPanel.alpha = 1;
        // instructionPanel.alpha = 0;
        // instructionPanel.gameObject.SetActive(false);

        introVideo.alpha = 0;
        introVideo.interactable = false;
        introVideo.blocksRaycasts = false;

        creditsVideo.alpha = 0;
        creditsVideo.interactable = false;
        creditsVideo.blocksRaycasts = false;

        Time.timeScale = 0; // pause game
        Cursor.lockState = CursorLockMode.None; // shows the mouse

        if (introVideoPlayer != null)
        {
            introVideoPlayer.playOnAwake = false;
            introVideoPlayer.loopPointReached += OnVideoFinished; // advance when video ends
            introVideoPlayer.Prepare();
            introVideoPlayer.prepareCompleted += OnVideoPrepared;
        }

        if (creditsVideoPlayer != null)
        {
            creditsVideoPlayer.playOnAwake = false;
            creditsVideoPlayer.loopPointReached += OnCreditsVideoFinished; // advance when video ends
            creditsVideoPlayer.Prepare();
            creditsVideoPlayer.prepareCompleted += OnVideoPrepared;
        }
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
        vp.Pause();
    }
    // Update is called once per frame
    public void NewGameClicked ()
    {
        if (!isTransitioning)
        {
            StartCoroutine(NewGameSequence());
        }
    }

    public void CreditsBtnClicked ()
    {
        if (!isTransitioning)
        {
            StartCoroutine(CreditsSequence());
        }
    }

    public void ExitBtnClicked ()
    {
        Application.Quit();
    }

    void Update()
    {
       if (!isTransitioning && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(NewGameSequence());
        }
    }

    IEnumerator FadeTo(CanvasGroup group, float targetAlpha)
    {
        while (!Mathf.Approximately(group.alpha, targetAlpha))
        {
            group.alpha = Mathf.MoveTowards(group.alpha, targetAlpha, Time.unscaledDeltaTime * fadeSpeed);
            yield return null;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(FinalFadeOut(introVideo));
    }

    void OnCreditsVideoFinished (VideoPlayer vp)
    {
        StartCoroutine(ReturnToMenuSequence());
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

    IEnumerator PlayWhenReady(VideoPlayer player)
    {
        if (!player.isPrepared)
        {
            yield return new WaitUntil(() => player.isPrepared);
        }
        player.Play();
    }

    IEnumerator NewGameSequence()
    {
        isTransitioning = true;
        yield return FadeTo(introPanel, 0);
        introPanel.gameObject.SetActive(false);
        yield return FadeTo(introVideo, 1);
        yield return PlayWhenReady(introVideoPlayer);
        // OnVideoFinished takes over from here
    }

    IEnumerator CreditsSequence()
    {
        isTransitioning = true;
        yield return FadeTo(introPanel, 0);
        introPanel.gameObject.SetActive(false);
        yield return FadeTo(creditsVideo, 1);
        yield return PlayWhenReady(creditsVideoPlayer);
    }   

    IEnumerator ReturnToMenuSequence()
    {
        yield return FadeTo(creditsVideo, 0);

        creditsVideoPlayer.Stop();     // reset so it can play again
        creditsVideoPlayer.Prepare();  // warm it up again (your OnVideoPrepared handles the rest)

        introPanel.gameObject.SetActive(true);  // turn the menu back on BEFORE fading it in
        yield return FadeTo(introPanel, 1);
        isTransitioning = false;                // menu is fully back, buttons are allowed again
    }
}
