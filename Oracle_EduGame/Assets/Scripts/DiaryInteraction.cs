using UnityEngine;
using UnityEngine.UI;

public class DiaryInteraction : MonoBehaviour
{
    public GameObject pressEPrompt;
    public Button rightBtn;
    public Button leftBtn;
    public Button closeBtn;
    public GameObject diaryOverlay;
    public Sprite diaryPage1;
    public Sprite diaryPage2;
    public int currentPage = 1; // 1 is first page, 2 is next page
    public bool diaryOpen;
    private bool isPlayerInRange;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            diaryOpen = !diaryOpen;
            if (diaryOpen)
            {
                rightBtn.onClick.RemoveAllListeners();
                leftBtn.onClick.RemoveAllListeners();
                closeBtn.onClick.RemoveAllListeners();

                rightBtn.onClick.AddListener(RightBtnClicked);
                leftBtn.onClick.AddListener(LeftBtnClicked);
                closeBtn.onClick.AddListener(CloseBtnClicked);


                ShowPage();
                // lock camera, show diary overlay
                DoorInteraction.LockPlayer(true);
                diaryOverlay.SetActive(true);
                pressEPrompt.SetActive(false);
            } else
            {
                diaryOverlay.SetActive(false);
                //pressEPrompt.SetActive(true);
                DoorInteraction.LockPlayer(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            pressEPrompt.SetActive(true);
        }
    }

    void ShowPage()
    {
        if (currentPage == 1)
        {
            Image display = diaryOverlay.transform.Find("Image").GetComponent<Image>();
            if (display != null) {
                display.sprite = diaryPage1;
            }
            rightBtn.gameObject.SetActive(true);
            leftBtn.gameObject.SetActive(false);   
        } else if (currentPage == 2)
        {
            Image display = diaryOverlay.transform.Find("Image").GetComponent<Image>();
            if (display != null) {
                display.sprite = diaryPage2;
            }
            rightBtn.gameObject.SetActive(false);
            leftBtn.gameObject.SetActive(true); 
        }
    }

    public void RightBtnClicked ()
    {
        if (currentPage == 1)
        {
            currentPage = 2;
            ShowPage();
        }
    }

    public void LeftBtnClicked ()
    {
        if (currentPage == 2)
        {
            currentPage = 1;
            ShowPage();
        }
    }

    public void CloseBtnClicked()
    {
        diaryOverlay.SetActive(false);
        diaryOpen = false;
        DoorInteraction.LockPlayer(false);
    }

    // locked the player so this shouldnt happen but just in case
    void OnTriggerExit (Collider other)
    {
        pressEPrompt.SetActive(false);
        diaryOpen = false;
        isPlayerInRange = false;
    }
}
