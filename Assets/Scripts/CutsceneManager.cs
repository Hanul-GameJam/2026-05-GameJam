using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI")]
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text dialogText;

    [Header("Dialogue Data")]
    public DialogueData dialogueData;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public string nextSceneName;

    private int currentLineIndex = 0;

    private bool isTyping = false;
    private bool skipTyping = false;

    void Start()
    {
        ShowLine();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        // 타이핑 중이면 즉시 출력
        if (isTyping)
        {
            skipTyping = true;
        }
        // 이미 전부 출력된 상태면 다음 대사
        else
        {
            NextLine();
        }
    }

    void ShowLine()
    {
        DialogueLine line = dialogueData.lines[currentLineIndex];

        portraitImage.sprite = line.portrait;
        nameText.text = line.characterName;

        StartCoroutine(TypeDialogue(line.dialogue));
    }

    IEnumerator TypeDialogue(string dialogue)
    {
        isTyping = true;
        skipTyping = false;

        dialogText.text = dialogue;

        dialogText.maxVisibleCharacters = 0;

        int totalCharacters = dialogue.Length;

        for (int i = 0; i <= totalCharacters; i++)
        {
            if (skipTyping)
            {
                dialogText.maxVisibleCharacters = totalCharacters;
                break;
            }

            dialogText.maxVisibleCharacters = i;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= dialogueData.lines.Length)
        {
            StartCoroutine(FadeManager.Instance.FadeAndLoadScene(nextSceneName));

            return;
        }

        ShowLine();
    }
}