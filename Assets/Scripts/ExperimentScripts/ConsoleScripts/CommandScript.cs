using UnityEngine;
using IngameDebugConsole;

public class CommandScript : MonoBehaviour
{
    [SerializeField]
    private ExperimentController _experimentController;

    public static readonly string CMD_START = "xpstart";
    public static readonly string CMD_MODE_NOW = "modenow";
    public static readonly string CMD_NEXT_MODE = "modenext";
    public static readonly string CMD_MODE_RESET = "modereset";
    public static readonly string CMD_CONFIG_LOCATION = "confloc";

    void Start()
    {
        DebugLogConsole.AddCommand<int>(CMD_START, "Starts experiment with a given ID", XPStart);
        DebugLogConsole.AddCommand(CMD_MODE_NOW, "Displays the current modality of the experiment", ModeNow);
        DebugLogConsole.AddCommand(CMD_NEXT_MODE, "Switches to the next modality of the experiment", ModeNext);
        DebugLogConsole.AddCommand(CMD_MODE_RESET, "Resets the round of the current modality", ModeReset);
        DebugLogConsole.AddCommand(CMD_CONFIG_LOCATION, "Opens the config files location", OpenConfigFilePath);
    }

    private void XPStart(int experimentId)
    {
        _experimentController.StartExperiment(experimentId);
    }

    public void ModeNow()
    {
        DisplayCurrentModality();
    }

    private void ModeNext()
    {
        _experimentController.ActivateNextModality();
    }

    private void ModeReset()
    {
        _experimentController.ResetCurrentModalityRound();
    }

    private void OpenConfigFilePath()
    {
        System.Diagnostics.Process.Start(ExperimentConfigLoader.ConfigFileLocation);
    }

    private void DisplayCurrentModality()
    {
        Debug.Log("Current mode: "+_experimentController.CurrentModality());
    }
}
