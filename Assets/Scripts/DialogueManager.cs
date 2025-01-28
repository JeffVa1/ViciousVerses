using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Assertions.Must;
using System;



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
    private Sprite PlayerSprite;
    private Sprite EnemySprite;
    private Sprite NarratorSprite;

    public GameObject DialogBox;

    private CanvasGroup DialogueCanvasGroup;


    public CanvasGroup GetCanvasGroup()
    {
        DialogueCanvasGroup = GetComponent<CanvasGroup>();
        return DialogueCanvasGroup;
    }




    public void Initialize(string JsonFilePath, Sprite PlayerSprite, Sprite EnemySprite, Sprite NarratorSprite)
    {
        this.JsonFilePath = JsonFilePath;
        this.PlayerSprite = PlayerSprite;
        this.EnemySprite = EnemySprite;
        this.NarratorSprite = NarratorSprite;

        DialogBox.SetActive(true);
        TextComponent.text = string.Empty;
        StartDialogue();
    }

    private void StartDialogue()
    {
        Index = 0;
        ParseDialogue();
        UpdateSpeakerSprite();
        StartCoroutine(TypeLine());
    }

    private void EndDialogue()
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
        if (speaker == "player")
        {
            SpeakerImage.sprite = PlayerSprite;
        }
        else if (speaker == "enemy")
        {
            SpeakerImage.sprite = EnemySprite;
        }
        else if (speaker == "narator")
        {
            SpeakerImage.sprite = NarratorSprite;
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
