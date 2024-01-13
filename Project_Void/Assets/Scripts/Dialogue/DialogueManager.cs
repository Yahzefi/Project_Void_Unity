using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    // LOGIC COMPONENTS
    public DialogueEvent dialogueEvent;

    private Queue<string> conversation;

    // EVENTS
    //

    // VISUAL COMPONENTS
    public GameObject dialogueBox;
    public GameObject textBox;
    public GameObject speakerBox;
    public Image imageOutline;
    public Image characterImage;
    public Text speakerText;
    public Text dialogueText;
    public Image nextPrompt;

    private Animator animator;

    // DATA/VARIABLES
        // SERIALIZED VARIABLES
    //
        // HIDDEN PUBLIC VARIABLES
    [HideInInspector] public int currentNumber; // for cycling through lines
    [HideInInspector] public bool isRunning; // determines whether StartDialogue() or NextLine() is called
    [HideInInspector] public bool isAnimating; // spam (mashing space/interact) guard
    // PRIVATE VARIABLES
    private bool triggerisDetected;
    private bool isTyping;
    private bool isFlashing;

    // animator

    private void Start()
    {
        animator = dialogueBox.GetComponent<Animator>();
        imageOutline.enabled = false;
        characterImage.enabled = false;
        speakerBox.SetActive(false);
        textBox.SetActive(false);
        dialogueBox.SetActive(false);
        // instantiate queue
        conversation = new Queue<string>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && triggerisDetected)
        {
            // if the input type is interact & the dialogue manager isn't currently animating anything:
            if (isAnimating) return;
            // if the dialogue manager isn't running: start it | otherwise: proceed to next line OR skip text
            if (!isRunning) StartDialogue();
            else NextLine();
        }
    }

    public void StartDialogue()
    {
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

        // if dialogue event has reached the final line: end event
        if (conversation.Count == 0)
        {
            currentNumber++;
            EndDialogue();
            return;
        }

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

            //textBoxRec.offsetMin --> x (left), y (bottom) [+]
            //textBoxRec.offsetMax --> x (right), y (top) [-]

            //RectTransform textBoxRec = textBox.GetComponent<RectTransform>();
            //Debug.Log(textBoxRec.offsetMin);
            //Debug.Log(textBoxRec.offsetMax);

            //textBoxRec.offsetMin = new Vector2(characterImage.sprite == null ? 25.0f : 162.475f, textBoxRec.offsetMin.y); // 25 --> no character image | 162.475 --> character image inserted

            StartCoroutine(TypeText(line)); // start typing text
        }
    }

    public void EndDialogue()
    {

        Dialogue[] dialogues = dialogueEvent.hasEnded ? dialogueEvent.endedDialogue : dialogueEvent.dialogue;

        speakerText.text = "";
        dialogueText.text = "";
        conversation.Clear();

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

    IEnumerator Animate (string type)
    {
        isAnimating = true;

        switch (type)
        {
            case "OPEN":
                yield return new WaitUntil(() => !dialogueBox.activeInHierarchy);
                dialogueBox.SetActive(true);
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
                yield return new WaitUntil(() => dialogueBox.activeInHierarchy);
                nextPrompt.enabled = false;
                characterImage.enabled = false;
                imageOutline.enabled = false;
                speakerBox.SetActive(false);
                textBox.SetActive(false);
                animator.SetBool("isOpen", false);
                yield return new WaitForSeconds(0.5f);
                dialogueBox.SetActive(false);
                isAnimating = false;
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
