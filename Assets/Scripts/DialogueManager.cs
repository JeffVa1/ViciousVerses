using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Assertions.Must;
using System;
using Unity.VisualScripting;



// Helper classes to for dialogue
[System.Serializable]
public class DialogueLine
{
    public string Speaker;
    public string Text;

    public string Flag;
}

[System.Serializable]
public class DialogueRoot
{
    public List<DialogueLine> Dialogue;
}

public class DialogueManager : MonoBehaviour
{


    public TextMeshProUGUI TextComponent;
    public float TextSpeed;
    public string JsonFilePath { get; set; }
    private List<DialogueLine> DialogueLines;
    private DialogueRoot DialogueRoot;
    private int Index;
    public int Flag { get; private set; }

    public UnityEngine.UI.Image SpeakerImage;
    public Sprite DPlayerSprite;
    public Sprite DEnemy1Sprite;
    public Sprite DEnemy2Sprite;
    public Sprite DEnemy3Sprite;

    public Sprite DNarratorSprite;

    public Sprite DBarMaidSprite;

    public GameObject DialogBox;

    private CanvasGroup DialogueCanvasGroup;


    public CanvasGroup GetCanvasGroup()
    {
        DialogueCanvasGroup = GetComponent<CanvasGroup>();
        return DialogueCanvasGroup;
    }

    public void Initialize(string JsonFilePath)
    {
        this.JsonFilePath = JsonFilePath;
        TextComponent.text = string.Empty;

    }

    public void StartDialogue()
    {
        Index = 0;
        DialogBox.SetActive(true);
        ParseDialogue();
        UpdateSpeakerSprite();
        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        DialogBox.SetActive(false);
    }

    private void ParseDialogue()
    {
        string JsonContent = File.ReadAllText(JsonFilePath);
        DialogueRoot = JsonUtility.FromJson<DialogueRoot>(JsonContent);
        DialogueLines = DialogueRoot.Dialogue;
    }



    private IEnumerator TypeLine()
    {

        DialogueLine line = DialogueLines[Index];
        Flag = int.Parse(DialogueLines[Index].Flag);
        foreach (char c in line.Text.ToCharArray())
        {
            TextComponent.text += c;
            yield return new WaitForSeconds(TextSpeed);
        }
    }

    private void NextLine()
    {
        if (Index < DialogueLines.Count - 1)
        {
            Index++;
            TextComponent.text = string.Empty;
            UpdateSpeakerSprite();
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    private void UpdateSpeakerSprite()
    {

        string speaker = DialogueLines[Index].Speaker;
        switch (speaker){
            case "":
            SpeakerImage.sprite = null;
            break;

            case "narator":
            SpeakerImage.sprite = DNarratorSprite;
            break;

            case "player":
            SpeakerImage.sprite = DPlayerSprite;
            break;

            case "barmaid":
            SpeakerImage.sprite = DBarMaidSprite;
            break;

            case "enemy1":
            SpeakerImage.sprite = DEnemy1Sprite;
            break;

            case "enemy2":
            SpeakerImage.sprite = DEnemy2Sprite;
            break;

            case "enemy3":
            SpeakerImage.sprite = DEnemy3Sprite;
            break;
        }
    }

    public void UpdateScriptJson(string filepath)
    {
        JsonFilePath = filepath;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (TextComponent.text == DialogueLines[Index].Text)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                TextComponent.text = DialogueLines[Index].Text;
            }
        }
    }

}
