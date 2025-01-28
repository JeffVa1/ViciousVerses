using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;



// Helper classes to for dialogue
[System.Serializable]
public class DialogueLine
{
    public string Speaker;
    public string Text;
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
    private string JsonFilePath;
    private List<DialogueLine> DialogueLines;

    private TextAsset DialogueJson;
    private DialogueRoot DialogueRoot;
    private int Index;

    public UnityEngine.UI.Image SpeakerImage;
    private Sprite PlayerSprite;
    private Sprite EnemySprite;

    public GameObject DialogBox;


    public void Initialize(string JsonFilePath, Sprite PlayerSprite, Sprite EnemySprite) {
        this.JsonFilePath = JsonFilePath;
        this.PlayerSprite = PlayerSprite;
        this.EnemySprite = EnemySprite;

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

    private void EndDialogue() {
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
        foreach (char c in line.Text.ToCharArray())
        {
            TextComponent.text += c;
            yield return new WaitForSeconds(TextSpeed);
        }
    }

    private void NextLine()
    {
        if (Index < DialogueLines.Count -1)
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
        Debug.Log("Index = "+ Index);
        string speaker = DialogueLines[Index].Speaker;
        if (speaker == "player")
        {
            SpeakerImage.sprite = PlayerSprite;
        }
        else if (speaker == "enemy")
        {
            SpeakerImage.sprite = EnemySprite;
        }
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
