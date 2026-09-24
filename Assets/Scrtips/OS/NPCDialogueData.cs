using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Dialogue", menuName = "MyHouse/NPC Dialogue")]
public class NPCDialogueData : ScriptableObject
{
    [Header("NPC Info")]
    public string npcName;

    public Sprite npcPortrait;

    [Header("Dialogue")]
    public DialogueLine[] dialogueLines;
}

[Serializable]
public class DialogueLine
{
    public Speaker speaker;

    [TextArea(2, 5)]
    public string text;
}

public enum Speaker
{
    NPC,
    Player
}
