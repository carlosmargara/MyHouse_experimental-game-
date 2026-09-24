using UnityEngine;

[CreateAssetMenu(menuName = "MyHouse/Inspection Data")]
public class InspectionData : ScriptableObject
{
    [Header("Info")]
    public string inspectionTitle;

    [Header("Visual")]
    public Sprite inspectionImage;

    [Header("Text")]
    [TextArea(2, 5)]
    public string[] messages;

    [Header("Choice")]
    public bool hasChoice;
    public string yesText = "Yes";
    public string noText = "No";
}