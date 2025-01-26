using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;



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
    private List<DialogueLine> DialogueLines;

    public TextAsset DialogueJson;
    private DialogueRoot DialogueRoot;
    private int Index;

    public UnityEngine.UI.Image SpeakerImage;
    public Sprite PlayerSprite;
    public Sprite EnemySprite;

    void Start()
    {
        TextComponent.text = string.Empty;
        StartDialogue();
    }

    void StartDialogue()
    {
        Index = 0;
        ParseDialogue();
        UpdateSpeakerSprite();
        StartCoroutine(TypeLine());
    }
    private void ParseDialogue()
    {
        Debug.Log("DialogJson.text= " + DialogueJson.text);

        DialogueRoot = JsonUtility.FromJson<DialogueRoot>(DialogueJson.text);

        Debug.Log("DialogueRoot.Dialogue " + DialogueRoot.Dialogue.Count);
        DialogueLines = DialogueRoot.Dialogue;
    }

    IEnumerator TypeLine()
    {

        DialogueLine line = DialogueLines[Index];
        foreach (char c in line.Text.ToCharArray())
        {
            TextComponent.text += c;
            yield return new WaitForSeconds(TextSpeed);
        }
    }

    void NextLine()
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
            gameObject.SetActive(false);
        }
    }

    void UpdateSpeakerSprite()
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

    void Update()
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
