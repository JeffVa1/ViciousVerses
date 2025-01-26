using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;



// Helper classes to for dialogue
[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
}

[System.Serializable]
public class DialogueRoot
{
    public List<DialogueLine> dialogue;
}

public class DialogueManager : MonoBehaviour
{


    public TextMeshProUGUI textComponent;
    public float textSpeed;
    private List<DialogueLine> dialogueLines;

    public TextAsset dialogueJson;
    private DialogueRoot dialogueRoot;
    private int index;
    
    public UnityEngine.UI.Image speakerImage;
    public Sprite playerSprite;
    public Sprite enemySprite;






    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    private void ParseDialogue()
    {
        dialogueRoot = JsonUtility.FromJson<DialogueRoot>(dialogueJson.text);
        dialogueLines = dialogueRoot.dialogue;

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == dialogueLines[index].text)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = dialogueLines[index].text;
            }
        }
    }


    void StartDialogue()
    {
        index = 0;
        ParseDialogue();
        UpdateSpeakerSprite();
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {

        DialogueLine line = dialogueLines[index];
        foreach (char c in line.text.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < dialogueLines.Count - 1)
        {
            index++;
            textComponent.text = string.Empty;
            UpdateSpeakerSprite();
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void UpdateSpeakerSprite() {
        string speaker = dialogueLines[index].speaker;
        if (speaker == "player") {
            speakerImage.sprite = playerSprite;
        }
        else if (speaker == "enemy") {
            speakerImage.sprite = enemySprite;
        }
    }



}
