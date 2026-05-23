using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public Sprite portrait;
    public string characterName;

    [TextArea(3, 5)]
    public string dialogue;
}
