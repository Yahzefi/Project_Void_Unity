using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//textBoxRec.offsetMin --> x (left), y (bottom) [+]
//textBoxRec.offsetMax --> x (right), y (top) [-]

//RectTransform textBoxRec = textBox.GetComponent<RectTransform>();
//Debug.Log(textBoxRec.offsetMin);
//Debug.Log(textBoxRec.offsetMax);

public class DialogueManager : MonoBehaviour
{
    // LOGIC COMPONENTS
    public DialogueEvent dialogueEvent;

    private Queue<string> conversation;

    // EVENTS
    //

    // VISUAL COMPONENTS
    public DialogueUI dialogueUI;

    private GameObject dialogueContainer;
    private GameObject textBox;
    private GameObject speakerBox;
    private Image imageOutline;
    private Image characterImage;
    private Image nextPrompt;
    private Text speakerText;
    private Text dialogueText;

    private Animator animator;

    // DATA/VARIABLES
        // SERIALIZED VARIABLES
    //
        // HIDDEN PUBLIC VARIABLES
    [HideInInspector] public bool isRunning; // determines whether StartDialogue() or NextLine() is called
    // PRIVATE VARIABLES
    private int currentNumber; // for cycling through lines
    private bool isAnimating; // spam (mashing space/interact) guard
    private bool triggerisDetected;
    private bool isTyping;
    private bool isFlashing;

    // animator

    private void Start()
    {
        //
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && triggerisDetected) // if the interact key is pressed & player is within trigger area
        {
            // if the dialogue manager is currently animating anything: return
            if (isAnimating) return;
            // if the dialogue manager isn't running: start it | otherwise: proceed to next line OR skip text
            if (!isRunning) StartDialogue();
            else NextLine();
        }
    }

    public void StartDialogue()
    {
        if (!dialogueUI.isInstantiated) InstantiateDialogueUI();

        animator = dialogueContainer.GetComponent<Animator>();

        imageOutline.enabled = false;
        characterImage.enabled = false;
        speakerBox.SetActive(false);
        textBox.SetActive(false);
        dialogueContainer.SetActive(false);

        conversation ??= new Queue<string>(); // instantiate queue

        // clear fields/values
        currentNumber = 0;
        speakerText.text = "";
        dialogueText.text = "";

        Dialogue[] dialogue = dialogueEvent.hasEnded ? dialogueEvent.endedDialogue : dialogueEvent.dialogue;

        foreach (string line in dialogue[0].lines) conversation.Enqueue(line);

        StartCoroutine(Animate("OPEN"));
        NextLine();
        isRunning = true; // dialogue event is initialized
    }

    public void NextLine()
    {
        Dialogue dialogue = dialogueEvent.hasEnded ? dialogueEvent.endedDialogue[currentNumber] : dialogueEvent.dialogue[currentNumber];

        // if dialogue event has reached the final line:
            // -> [1] increment "currentNumber" count
            // -> [2] call "EndDialogue()" (either proceeds to next section of dialogue or ends event)
        if (conversation.Count == 0)
        {
            currentNumber++;
            EndDialogue();
            return;
        }

        // if text is already typing: "line" will be set to the next string in queue (because the remaining text (that was typing) will be fully displayed)
        // otherwise: "line" will be set to the string in the queue's current position
        string line = isTyping ? conversation.Dequeue() : conversation.Peek();

        // if text is typing: stop the typing text coroutine and display the remaining text
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = line;
            isTyping = false;
            // after remaining text is displayed: start flashing next prompt
            isFlashing = true;
            StartCoroutine(FlashPrompt());
        }
        else
        {
            // if text isn't typing
                // if next prompt is already flashing: stop it & set to active (it otherwise would remain inactive from coroutine)
            if (isFlashing)
            {
                StopAllCoroutines();
                isFlashing = false;
                nextPrompt.enabled = true;
            }

            // add speaker text, character image & adjust rect transform of text box accordingly
            speakerText.text = dialogue.characterName;
            speakerText.color = new Color(dialogue.textColor.r, dialogue.textColor.g, dialogue.textColor.b);
            characterImage.sprite = dialogue.characterImage;

            StartCoroutine(TypeText(line)); // start typing text
        }
    }

    public void EndDialogue()
    {

        Dialogue[] dialogues = dialogueEvent.hasEnded ? dialogueEvent.endedDialogue : dialogueEvent.dialogue;

        // clear fields/values
        speakerText.text = "";
        dialogueText.text = "";
        conversation.Clear();

        // if the "currentNumber" value is LESS THAN the amount of events/speakers: refill the queue with new strings
        // otherwise("currentNumber" == length): end the dialogue event & close the dialogue box 
        if (currentNumber < dialogues.Length)
        {
            foreach (string line in dialogues[currentNumber].lines) conversation.Enqueue(line);
            NextLine();
            return;
        }
        else
        {
            isRunning = false;
            isFlashing = false;
            dialogueEvent.hasEnded = true;
            StartCoroutine(Animate("CLOSE"));
        }

    }

    private void InstantiateDialogueUI ()
    {
        GameObject GameUI = GameObject.Find("GameUI");

        // instantiate, define local ref & rename clone for: "dialogueContainer", "characterImage", "imageOutline", "nextPrompt", "speakerBox" & "textBox"
        // define local ref & rename: "speakerText"(child of "speakerBox") & "dialogueText"(child of "textBox")
        // update dialogue UI status bool
        Instantiate(dialogueUI.dialogueContainer, GameUI.transform);
        dialogueContainer = GameUI.transform.Find($"{dialogueUI.dialogueContainer.name}(Clone)").gameObject;
        dialogueContainer.name = dialogueUI.dialogueContainer.name;
        Instantiate(dialogueUI.characterImage, dialogueContainer.transform);
        characterImage = dialogueContainer.transform.Find($"{dialogueUI.characterImage.name}(Clone)").GetComponent<Image>();
        characterImage.name = dialogueUI.characterImage.name;
        Instantiate(dialogueUI.imageOutline, dialogueContainer.transform);
        imageOutline = dialogueContainer.transform.Find($"{dialogueUI.imageOutline.name}(Clone)").GetComponent<Image>();
        imageOutline.name = dialogueUI.imageOutline.name;
        Instantiate(dialogueUI.nextPrompt, dialogueContainer.transform);
        nextPrompt = dialogueContainer.transform.Find($"{dialogueUI.nextPrompt.name}(Clone)").GetComponent<Image>();
        nextPrompt.name = dialogueUI.nextPrompt.name;
        Instantiate(dialogueUI.speakerBox, dialogueContainer.transform);
        speakerBox = dialogueContainer.transform.Find($"{dialogueUI.speakerBox.name}(Clone)").gameObject;
        speakerBox.name = dialogueUI.speakerBox.name;
        speakerText = speakerBox.transform.GetChild(0).GetComponent<Text>();
        speakerText.name = "SpeakerText";
        Instantiate(dialogueUI.textBox, dialogueContainer.transform);
        textBox = dialogueContainer.transform.Find($"{dialogueUI.textBox.name}(Clone)").gameObject;
        textBox.name = dialogueUI.textBox.name;
        dialogueText = textBox.transform.GetChild(0).GetComponent<Text>();
        dialogueText.name = "DialogueText";

        dialogueUI.isInstantiated = true;
    }

    IEnumerator Animate (string type)
    {
        isAnimating = true; // prevents other coroutines from running until animation has finished (SEE: "this.Update()" & "PlayerEvent.Init_PlayerMovement")

        switch (type)
        {
            case "OPEN":
                // after confirming the dialogue box isn't already open/active:
                // -> [1] enable/display the "dialogueContainer" & "imageOutline"
                // -> [2] update animator's "isOpen" bool as true (starts open animation) & wait "x" seconds
                // -> [3] once animation has finished: enable/display the "speakerBox", "textBox", "nextPrompt" & "characterImage"
                // -> [4] update "isAnimating" bool as false (allows previously blocked inputs/actions)
                yield return new WaitUntil(() => !dialogueContainer.activeInHierarchy);
                dialogueContainer.SetActive(true);
                imageOutline.enabled = true;

                animator.SetBool("isOpen", true);
                yield return new WaitForSeconds(0.5f);

                speakerBox.SetActive(true);
                textBox.SetActive(true);
                nextPrompt.enabled = true;
                characterImage.enabled = true;

                isAnimating = false;
                break;
            case "CLOSE":
                // after confirming the dialogue box is currently open/active:
                // -> [1] disable/hide "nextPrompt", "characterImage", "imageOutline", "speakerBox" & "textBox"
                // -> [2] update animator's "isOpen" bool as false (starts close animation) & wait for "x" seconds
                /* -> [3] once animation has finished: disable/hide "dialogueContainer" & update "isAnimating" bool 
                as false (allows previously blocked inputs/actions) */
                // -> [4] remove the dialogue box UI from scene & update corresponding status bool
                yield return new WaitUntil(() => dialogueContainer.activeInHierarchy);
                nextPrompt.enabled = false;
                characterImage.enabled = false;
                imageOutline.enabled = false;
                speakerBox.SetActive(false);
                textBox.SetActive(false);

                animator.SetBool("isOpen", false);
                yield return new WaitForSeconds(0.5f);

                dialogueContainer.SetActive(false);
                isAnimating = false;

                Destroy(dialogueContainer);
                dialogueUI.isInstantiated = false;
                break;
            default:
                break;
        }
    }

    // type text method
    IEnumerator TypeText (string line)
    {
        yield return new WaitWhile(() => isAnimating);
        dialogueText.text = ""; // clear text box
        isTyping = true;
        foreach (char letter in line) // start typing text with fixed wait interval
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.075f);
        }
        isTyping = false;
        // before ending coroutine: start next prmopt flash coroutine
        isFlashing = true;
        StartCoroutine(FlashPrompt());
        yield return null;
    }

    IEnumerator FlashPrompt ()
    {
        while (isFlashing)
        {
            nextPrompt.enabled = !nextPrompt.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Trigger Enter Detected With GameObject: {collision.name}");
        triggerisDetected = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log($"Trigger Exit Detected With GameObject: {collision.name}");
        triggerisDetected = false;
    }

}
