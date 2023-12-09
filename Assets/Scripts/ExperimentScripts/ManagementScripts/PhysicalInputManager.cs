using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicalInputManager : MonoBehaviour
{

    [SerializeField]
    private ScreenView _screenView;

    public bool InputEnabled {get; set;} = false;

    public event Action<string> OnSendingSignal = delegate { };

    void Start()
    {
        if (Application.isPlaying)
        {
            // subscribes to the key press event only during runtime
            Keyboard.current.onTextInput += OnKeyPress;
        }
    }

    private void OnKeyPress(char key)
    {
        HandleInput(key);
    }

    private void HandleInput(char key)
    {
        if(!InputEnabled)
            return;

        string loggingString;

        if(!char.IsControl(key))
        {
            if(key == ' ')
            {
                loggingString = "SPACE";
                _screenView.InsertString(key.ToString());
            }
            else
            {
                Debug.Log(key);
                loggingString = key.ToString();
                _screenView.InsertString(key.ToString());
            }
        }
        else
        {
            if(key == 8) // Backspace
            {
                loggingString = "BACKSPACE";
                _screenView.RemovePreviousCharacter();
            }
            else
            {
                loggingString = "NO BEHAVIOR: "+ key.ToString();
            }
        }

        OnSendingSignal(loggingString);
    }



}
