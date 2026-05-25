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

        // 💡 기존 코드: 
        // StartCoroutine(TypeDialogue(line.dialogue));

        // 💡 수정된 코드: 대사를 코루틴에 넘기기 전에 띄어쓰기를 줄바꿈 방지 공백으로 바꿔줍니다!
        StartCoroutine(TypeDialogue(line.dialogue.Replace(" ", "\u00A0")));
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
            FadeManager.Instance.LoadSceneWithFade(nextSceneName);

            return;
        }

        ShowLine();
    }
}