using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueEvent", menuName = "Dialogue Event")]
public class DialogueEvent : ScriptableObject
{
    public Dialogue[] dialogue;
    public Dialogue[] endedDialogue;
    [HideInInspector] public bool hasEnded;
}
