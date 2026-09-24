using UnityEngine;

[CreateAssetMenu(menuName = "MyHouse/Object Info")]
public class ObjectInfoSO : ScriptableObject
{
    [Header("Info")]
    public string objectName;

    [TextArea]
    public string[] descriptions;
}
