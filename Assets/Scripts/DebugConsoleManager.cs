using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DebugConsoleManager : MonoBehaviour
{
    private TextField inputField;
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

    private void OnEnterPressed(KeyDownEvent e) {
        Debug.Log("DeBuGdS: OnEnterPressed called");
        if (e.keyCode == KeyCode.Return) 
        {
            string input = inputField.value.Trim();
            if (!string.IsNullOrEmpty(input))  {
                LogMessage("> " + input);
                ParseCommand(input);
                inputField.value = "";
            }
        }
    }

    private void LogMessage(string message){
        Debug.Log("DeBuGdS: LogMessage Called");
        Label newLog = new Label(message);
        logView.Add(newLog);
    }

    private void ParseCommand(string input){
        Debug.Log("DeBuGdS: ParseCommand called");
        string[] splitInput = input.Split(' ');
        if (splitInput.Length == 0) return;
        
        string command = splitInput[0];
        string[] args = splitInput.Length > 1 ? splitInput[1..] : new string[0];

        if (commands.ContainsKey(command)) {
            commands[command].Invoke(args);
        }
        else {
            LogMessage("Unknown command: {command} ");
        }
    }

    private void RegisterCommands() {
        Debug.Log("DeBuGdS: RegisterCommands called");
        commands.Add("load_scene", args => LoadScene(args));
    }

    private void LoadScene(string[] args) {
        Debug.Log("DeBuGdS: Load Scene Called ");
        if (args.Length < 1) {
            LogMessage("Usage: load_scene <scene_name>");
            return;
        }
        string sceneName = args[0];
        try {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            LogMessage($"Loaded scene: {sceneName}");
        }
        catch (Exception e) {
            LogMessage($"Erorr loading scene: {e.Message}");
        }
    }


}
