using UnityEngine;
using System.Collections;

public class GameStartManager : MonoBehaviour
{
    public CanvasGroup introPanel;
    public CanvasGroup instructionPanel;
    public GameObject staticBackground;
    private int screenState = 0;
    public float fadeSpeed = 1.0f;
    public GameObject playerController;

    void Start()
    {
        introPanel.alpha = 1;
        instructionPanel.alpha = 0;
        instructionPanel.gameObject.SetActive(false);
        Time.timeScale = 0; // pause game
        Cursor.lockState = CursorLockMode.None; // shows the mouse
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (screenState == 0)
            {
                StartCoroutine(FadeOutAndIn(introPanel, instructionPanel));
                screenState++;
            } else
            {
                StartCoroutine(FinalFadeOut(instructionPanel));
            }
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

        inGroup.gameObject.SetActive(true);
        while (inGroup.alpha < 1)
        {
            inGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }
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
