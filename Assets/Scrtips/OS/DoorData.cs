using System;
using UnityEngine;

public enum DoorAccessType
{
    AlwaysOpen,
    KeyRequired,
    LockedOnly
}

public enum DoorSoundType
{
    Wood,
    Metal
}

[CreateAssetMenu(fileName = "New Door Data", menuName = "Doors/Door Data")]
public class DoorData : ScriptableObject
{
    [Header("Info")]
    public string ID;

    [Header("Access")]
    public DoorAccessType accessType;
    public string requiredKeyID;

    [Header("Item Interaction")]
    public string requiredUseItemID;

    [Header("Texts")]
    [TextArea] public string lockedText;
    [TextArea] public string unlockedText;
    [TextArea] public string openText;

    [Header("Sound")]
    public DoorSoundType doorSoundType;
}
