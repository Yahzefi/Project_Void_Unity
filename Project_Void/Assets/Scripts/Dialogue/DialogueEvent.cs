using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "DialogueEvent", menuName = "Dialogue Event", order = 1)]
public class DialogueEvent : ScriptableObject
{
    public Dialogue[] dialogue;
    public Dialogue[] endedDialogue;

    public bool hasEnded;
}
