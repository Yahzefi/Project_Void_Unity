using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DialogueUI", menuName = "Dialogue UI", order = 0)]
public class DialogueUI : ScriptableObject
{
    public GameObject dialogueContainer;
    public GameObject textBox;
    public GameObject speakerBox;
    public GameObject nextPrompt;
    public GameObject imageOutline;
    public GameObject characterImage;

    public bool isInstantiated;
}
