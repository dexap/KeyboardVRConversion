using UnityEngine;
using UnityEngine.InputSystem;
using KeyInputVR.KeyMaps;
using KeyInputVR.Keyboard;
using System;

public class KeyPressManager : MonoBehaviour
{
    [SerializeField]
    private ScreenView _screenView;

    [SerializeField]
    private KeyboardInfo _keyboard;

    //added to fetch which signals are about to get sent to the screen
    public event Action<string> OnSendingSignal = delegate { };
    public event Action<string, Manus.Utility.HandType, Manus.Utility.FingerType> OnSendingSignalWithGloves = delegate { };

    private void Awake()
    {   
        KeyInfo[] keyInfos;
        
        if(_keyboard == null)
        {
            Debug.LogWarning("No keyboard info attached to '"+ transform.name +"'! Directly accessing all KeyInfos inside the scene instead.", gameObject);
            keyInfos = Resources.FindObjectsOfTypeAll<KeyInfo>();
        }
        else
        {
            keyInfos = _keyboard.KeyInfos.ToArray();
        }


        foreach(KeyInfo info in keyInfos)
        {   
            info.OnKeyActivated += KeyActivated;
            info.OnKeyActivatedGloves += KeyActivatedWithGloves;
        }
    }

    private void KeyActivated(IKeyMap keyMap, Key key)
    {
        KeyDefinition keyDefinition = keyMap.GetKeyMap()[key];

        HandleInput(keyDefinition, key, Manus.Utility.HandType.Invalid, Manus.Utility.FingerType.Invalid);
    }

    private void KeyActivatedWithGloves(IKeyMap keyMap, Key key, Manus.Utility.HandType handType, Manus.Utility.FingerType fingerType)
    {
        KeyDefinition keyDefinition = keyMap.GetKeyMap()[key];

        HandleInput(keyDefinition, key, handType, fingerType);
    }

    private void HandleInput(KeyDefinition keyDefinition, Key key, Manus.Utility.HandType handType, Manus.Utility.FingerType fingerType)
    {
        string loggingString;

        switch(keyDefinition.KeyType)
        {
            case KeyType.REGULAR:
                {
                    string text = ShouldOutputShiftedVariant() ? keyDefinition.ShiftedOutput : keyDefinition.BaseOutput;
                    _screenView.InsertString(text);
                    loggingString = text;
                    ApplyShiftKeyState(false);
                    break;
                }
            case KeyType.ENTER:
                {
                    _screenView.BeginNewLine();
                    loggingString = key.ToString();
                    break;
                }
            case KeyType.SPACE:
                {
                    _screenView.InsertString(" ");
                    loggingString = key.ToString();
                    ApplyShiftKeyState(false);
                    break;
                }
            case KeyType.BACKSPACE:
                {
                    _screenView.RemovePreviousCharacter();
                    loggingString = key.ToString();
                    break;
                }
            case KeyType.DELETE:
                {
                    _screenView.RemoveNextCharacter();
                    loggingString = key.ToString();
                    break;
                }
            case KeyType.TAB:
                {
                    //four spaces
                    _screenView.InsertString("    ");
                    loggingString = key.ToString();
                    ApplyShiftKeyState(false);
                    break;
                }
            case KeyType.SHIFT:
                {
                    ApplyShiftKeyState(true);
                    loggingString = key.ToString();
                    break;
                }
            case KeyType.CAPSLOCK:
                {
                    InvertCapsLockState();
                    loggingString = key.ToString();
                    break;
                }
            case KeyType.ARROW_LEFT:
                {   
                    _screenView.MoveCaretToPreviousCharacter();
                    loggingString = key.ToString();
                    break;
                }
            case KeyType.ARROW_RIGHT:
                {
                    _screenView.MoveCaretToNextCharacter();
                    loggingString = key.ToString();
                    break;
                }
            default:
                {
                    loggingString = "NO BEHAVIOR: "+ key.ToString();
                    break;
                }
        }

        if(handType != Manus.Utility.HandType.Invalid && fingerType != Manus.Utility.FingerType.Invalid)
        {
            OnSendingSignalWithGloves(loggingString, handType, fingerType);
        }
        else
        {
            OnSendingSignal(loggingString);
        }
    }

    private void InvertCapsLockState()
    {
        if(_keyboard != null)
        {
            _keyboard.IsCapsLocked = !_keyboard.IsCapsLocked;
            _keyboard.DisplayCapsLockKeysAsActive(_keyboard.IsCapsLocked);
        }
    }

    private void ApplyShiftKeyState(bool lastKeyWasShift)
    {
        if(_keyboard != null)
        if(lastKeyWasShift && !_keyboard.IsShifted)
        {
            _keyboard.IsShifted = true;
            _keyboard.DisplayShiftKeysAsActive(true);
        }
        else
        {
            _keyboard.IsShifted = false;
            _keyboard.DisplayShiftKeysAsActive(false);
        }
    }

    private bool ShouldOutputShiftedVariant()
    {
        return _keyboard.IsSetToUppercase;
    }
}
