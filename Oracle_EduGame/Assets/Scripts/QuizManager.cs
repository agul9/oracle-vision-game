using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class RoomData { 
    public Sprite[] choiceSprites; // All available images for this room
    public Sprite[] blankSprites;  // The specific "Oracle Bone" images that float above the blanks
    public int[] correctAnswers;
}

public class QuizManager : MonoBehaviour
{
    public Sprite selectedSprite;
    public GameObject selectedButton;
    public int selectedID = -1;

    public RoomData[] rooms; // Set size to 2 in the Inspector
    public Image[] slotCharacterImages; // Drag the Image components that sit ABOVE the blank buttons here
    public GameObject[] choiceButtons; // Drag your 16 choice buttons here
    public GameObject[] blankSlots;    // Drag your 4 blank buttons here
    int currentRoom;
    int[] playerAnswers = new int[4];
    public bool isRoomComplete = false;
    public TMP_Text instructionText;
    public Image cursor;

    // lock quiz variables
    public CanvasGroup quizCanvasGroup; // so we can disable it when locked
    public int failedAttempts = 0;
    public GameObject lockOutUI;
    public float lockoutTime;
    public bool isLockedOut = false;
    public float timerEndTime; // records when the timer should end, so that when they close the quiz and the timer ends, it still knows when to unlock



    void Start()
    {
    }

    void Update()
    {
        // If we are currently in a lockout state
        if (isLockedOut)
        {
            // 1. Check if the time has passed
            if (Time.unscaledTime >= timerEndTime)
            {
                EndLockout();
            }
            else
            {
                // 2. Keep the UI active and show the countdown
                // This runs every frame the panel is OPEN
                if (!lockOutUI.activeSelf) lockOutUI.SetActive(true);
                
                // If you have a countdown text:
                // timerText.text = Mathf.Ceil(timerEndTime - Time.unscaledTime) + "s";
            }
        }
    }

    public void SelectChoice(GameObject clickedBtn)
    {
        // Reset old button opacity
        if (selectedButton != null) SetOpacity(selectedButton, 1.0f);

        selectedButton = clickedBtn;
        
        // TRICK: Get the ID from the name (e.g., "Choice_3" becomes 3)
        string[] nameParts = clickedBtn.name.Split('_');
        selectedID = int.Parse(nameParts[1]);

        selectedSprite = clickedBtn.GetComponent<Image>().sprite;

        // Visual feedback
        SetOpacity(selectedButton, 0.5f); 

        // Update the "Chameleon"
        cursor.sprite = selectedSprite; // Swap the image to the one we just picked
        cursor.gameObject.SetActive(true); // Show it!
    }

    public void PlaceInBlank(GameObject blankSlot)
    {
        string[] nameParts = blankSlot.name.Split('_');
        int slotID = int.Parse(nameParts[1]);

        // 1. GET THE BUTTON CHILD
        Transform buttonTransform = blankSlot.transform.Find("Button");
        Image slotImage = buttonTransform.GetComponent<Image>();

        // 2. DESELECT LOGIC: If player clicks a full slot without holding a new choice
        if (selectedButton == null && slotImage.sprite != null)
        {
            ReturnArtifactToPool(slotID, slotImage);
            return;
        }

        // 3. PLACEMENT LOGIC: (Your existing code)
        if (selectedButton != null)
        {
            // If the slot is ALREADY full, return the old one first!
            if (slotImage.sprite != null) {
                ReturnArtifactToPool(slotID, slotImage);
            }

            // Place the new one
            slotImage.sprite = selectedSprite;
            slotImage.color = Color.white; 

            playerAnswers[slotID] = selectedID;

            SetOpacity(selectedButton, 0.5f); // Updated Opacity
            selectedButton.GetComponent<Button>().interactable = false;

            selectedButton = null;
            selectedID = -1;

            // reset cursor
            cursor.gameObject.SetActive(false); // Hide it
            Cursor.visible = true; // Bring back the regular mouse
        }
    }

    // New helper function to keep things clean
    void ReturnArtifactToPool(int slotID, Image slotImage)
    {
        int choiceID = playerAnswers[slotID];
        
        // Find the original button and reset it
        GameObject originalBtn = choiceButtons[choiceID];
        originalBtn.GetComponent<Button>().interactable = true;
        SetOpacity(originalBtn, 1.0f);

        // Clear the blank slot
        slotImage.sprite = null;
        slotImage.color = new Color(1, 1, 1, 0.1f);
        playerAnswers[slotID] = -1; // Reset answer tracking
    }

    void SetOpacity(GameObject obj, float alpha)
    {
        Image img = obj.GetComponent<Image>();
        if (img != null) {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }
    }

    public void SetupRoom(int roomIndex)
    {
        currentRoom = roomIndex;
        RoomData data = rooms[roomIndex];
        isRoomComplete = false;
        instructionText.text = "Instructions!";

        // 1. Wipe the player's old answers
        for (int i = 0; i < playerAnswers.Length; i++) {
            playerAnswers[i] = -1;
        }

        // 2. Clear the UI slots (so the old sprites aren't there)
        foreach (GameObject blank in blankSlots) {
            Transform btn = blank.transform.Find("Button");
            if (btn != null) {
                Image img = btn.GetComponent<Image>();
                img.sprite = null;
                // Set it to a faint placeholder color or transparent
                img.color = new Color(1, 1, 1, 0.1f); 
            }
        }

        // 1. Setup Choices
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < data.choiceSprites.Length)
            {
                choiceButtons[i].SetActive(true);
                choiceButtons[i].GetComponent<Image>().sprite = data.choiceSprites[i];
                choiceButtons[i].GetComponent<Button>().interactable = true;
                SetOpacity(choiceButtons[i], 1.0f); // Reset visibility
            }
            else
            {
                choiceButtons[i].SetActive(false);
            }
        }

        // 2. Setup Blanks & Floating Characters
        for (int i = 0; i < blankSlots.Length; i++)
        {
            if (blankSlots[i] == null) continue; // Safety skip

            if (i < data.blankSprites.Length)
            {
                blankSlots[i].SetActive(true);
                
                // Safety check for the character image frames
                if (i < slotCharacterImages.Length && slotCharacterImages[i] != null)
                {
                    slotCharacterImages[i].gameObject.SetActive(true);
                    slotCharacterImages[i].sprite = data.blankSprites[i];
                    slotCharacterImages[i].color = Color.white;
                }

                // Reset Blank Slot
                Image blankImg = blankSlots[i].GetComponent<Image>();
                if (blankImg != null) {
                    blankImg.sprite = null; 
                    blankImg.color = new Color(1, 1, 1, 0.1f);
                }
            }
            else
            {
                blankSlots[i].SetActive(false);
                if (i < slotCharacterImages.Length && slotCharacterImages[i] != null)
                    slotCharacterImages[i].gameObject.SetActive(false);
            }
        }
    }

    public void CheckAnswers()
    {
        RoomData data = rooms[currentRoom];
        int correctCount = 0;

        for (int i = 0; i < data.blankSprites.Length; i++)
        {
            // Compare what the player put in (playerSelections) 
            // to what you typed in the Inspector (correctAnswers)
            if (playerAnswers[i] == data.correctAnswers[i])
            {
                correctCount++;
            }
        }

        if (correctCount == data.blankSprites.Length) {
            //Debug.Log("SUCCESS: All " + correctCount + " symbols match!");
            isRoomComplete = true;
            instructionText.text = "SUCCESS: All " + correctCount + " symbols match!";
            // Trigger your win animation/sound here
        } else {
            isRoomComplete = false;
            instructionText.text = "FAIL: Only " + correctCount + " are correct.";
            failedAttempts++;
            if (failedAttempts >= 3)
            {
                //lockOutUI.SetActive(true);
                StartLockout();
            }
        }
    }

    public void StartLockout()
    {
        isLockedOut = true;
        timerEndTime = Time.unscaledTime + lockoutTime;
        
        // We turn on the UI immediately
        lockOutUI.SetActive(true);
        quizCanvasGroup.interactable = false;
        quizCanvasGroup.blocksRaycasts = false;
        
        Debug.Log("Lockout set to end at: " + timerEndTime);
    }

    void EndLockout()
    {
        isLockedOut = false;
        failedAttempts = 0;
        lockOutUI.SetActive(false);
        quizCanvasGroup.interactable = true;
        quizCanvasGroup.blocksRaycasts = true;
        Debug.Log("Lockout cleared by Update clock.");
    }
}