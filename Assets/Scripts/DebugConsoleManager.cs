using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DebugConsoleManager : MonoBehaviour
{
    
    public TextField inputField;
    private ScrollView logView;
    private Label logTemplate;

    private Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>();

    private void Awake()
    {
        Debug.Log("DeBuGdS: Console is Awake");
        
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        inputField = root.Q<TextField>("ConsoleInput");
        logView = root.Q<ScrollView>("ConsoleLog");
        

        inputField.RegisterCallback<KeyDownEvent>(OnEnterPressed);
        
        RegisterCommands();
        
    }

    private void OnEnterPressed(KeyDownEvent e)
    {
        Debug.Log("DeBuGdS: OnEnterPressed called");
        if (e.keyCode == KeyCode.Return)
        {
            string input = inputField.value.Trim();
            if (!string.IsNullOrEmpty(input))
            {
                Debug.Log("DeBuGdS: String Not Null");
                AddCommandToLog(input);
                inputField.value = "";
            }
        }
    }

    private void AddCommandToLog(string input){
        VisualElement logEntry = new VisualElement();
        logEntry.style.flexDirection = FlexDirection.ColumnReverse;

        Label inputLog = new Label(">>> " + input);
        logEntry.Add(inputLog);

        string responseMessage;
        string[] splitInput = input.Split(' ');
        string command = splitInput[0];
        string[] args = splitInput.Length > 1 ? splitInput[1..] : new string[0];
        
        if (commands.ContainsKey(command))
        {
            commands[command].Invoke(args);
            responseMessage = "Executing " + command;
        }
        else
        {
            responseMessage = $"Unknown command: {input}";
        }

        Label responseLog = new Label(responseMessage);
        logEntry.Add(responseLog);

        logView.contentContainer.Add(logEntry);

        AutoScrollToBottom();
    }
    

    private void AutoScrollToBottom() {
        logView.schedule.Execute(() => {
            logView.scrollOffset = new Vector2(0, float.MaxValue);
        }).ExecuteLater(10);
    }

    private void RegisterCommands()
    {
        Debug.Log("DeBuGdS: RegisterCommands called");
        commands.Add("load_scene", args => LoadScene(args));
    }

    private void LoadScene(string[] args)
    {
        Debug.Log("DeBuGdS: Load Scene Called ");
        if (args.Length < 1)
        {
            AddCommandToLog("Usage: load_scene <scene_name>");
            return;
        }
        string sceneName = args[0];
        try
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            AddCommandToLog($"Loaded scene: {sceneName}");
        }
        catch (Exception e)
        {
            AddCommandToLog($"Erorr loading scene: {e.Message}");
        }
    }


}
