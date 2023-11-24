using System;
using UnityEngine;
using UnityEngine.UI;

public class FinishButton : MonoBehaviour
{
    [SerializeField]
    private float _maxSecondsBetweenPresses = 1.0f;

    [SerializeField]
    private float _requiredChainLength = 5f;

    private float _timeOfLastPress = -1f;

    private int _chainedPresses = 0;

    public event Action OnFinishSignal = delegate { };

    [SerializeField]
    private Text _labelText;

    void Update()
    {
        if(!IsWithinTimeFrame())
        {
            ResetButton();
            return;
        }

        if(IsThresholdReached())
        {
            OnFinishSignal();
            ResetButton();
            return;
        }

        UpdateLabel();
    }

    public void OnButtonPress()
    {
        if(_chainedPresses == 0)
        {
            AddToChainCounter();
            return;
        }

        if(IsWithinTimeFrame())
        {
            AddToChainCounter();
        }
        else
        {
            _chainedPresses = 0;
        }
    }

    private void AddToChainCounter()
    {
        _chainedPresses += 1;
        _timeOfLastPress = Time.time;
    }

    private bool IsThresholdReached()
    {
        if(_chainedPresses >= _requiredChainLength)
        {
            return true;
        }

        return false;
    }

    private bool IsWithinTimeFrame()
    {
        if(_timeOfLastPress + _maxSecondsBetweenPresses > Time.time)
        {
            return true;
        }

        return false;
    }

    private void UpdateLabel()
    {
        string text = _chainedPresses +" / "+_requiredChainLength;

        _labelText.text = text;
    }

    private void ResetButton()
    {
        _chainedPresses = 0;
        UpdateLabel();
    }

}
