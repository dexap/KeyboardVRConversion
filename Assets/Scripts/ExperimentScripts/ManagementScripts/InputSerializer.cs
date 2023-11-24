using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class InputSerializer
{
    public static readonly string ResultFileLocation = Application.persistentDataPath + "/Results/";

    public static readonly string DiscardedFileLocation = Application.persistentDataPath + "/DiscardedResets/";

    private string _fileName;
    public string ResultFilePath {get; private set;}

    private float _timeOfFirstInput = -1f;

    public InputSerializer(string experimentId, int experimentRound)
    {
        string time = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        
        //making sure that the filename is valid
        Regex r = new Regex("^[^.<>:;,?\\\"*|/]+$");
        if (!r.IsMatch(experimentId))
        {
            Debug.LogError("Using fallback filename! Provided experimentId contains illegal characters (^.<>:;,?\\\"*|/) !");
            _fileName = time + "-round-" + experimentRound +".csv";
        }
        else
        {
            _fileName = time +"-expid-"+ experimentId + "-round-"+ experimentRound +".csv";
        }

        ResultFilePath = ResultFileLocation+_fileName;
        AddToFile(ResultFilePath, experimentId+ "-" +experimentRound);
    }

    public void LogInput(string text)
    {
        // checking if there has been an input yet
        if(_timeOfFirstInput < 0)
        {
            _timeOfFirstInput = Time.time;
        }

        float timeSinceFirstInput = Time.time - _timeOfFirstInput;

        string csv = timeSinceFirstInput + ";";
        csv += text + ";";
        AddToFile(ResultFilePath, csv);
    }

    public void LogInput(string text, Manus.Utility.HandType handType, Manus.Utility.FingerType fingerType)
    {
        if(_timeOfFirstInput == -1f)
        {
            _timeOfFirstInput = Time.time;
        }

        float timeSinceFirstInput = Time.time - _timeOfFirstInput;

        string csv = timeSinceFirstInput + ";";
        csv += text + ";";
        csv += handType + ";";
        csv += fingerType + ";";
        AddToFile(ResultFilePath, csv);
    }

    private static void AddToFile(string filePath, string csvLine)
    {
        System.IO.Directory.CreateDirectory(ResultFileLocation);
        using(StreamWriter sw = File.AppendText(filePath))
        {
            sw.WriteLine(csvLine);
        }
    }

    /// <summary>
    /// Should only be called when a round gets reset.
    /// Moves the current log file out of the results folder and puts it into a "discarded" folder.
    /// It is highly recommended to use another InputSerializer instance afterwards!
    /// </summary>
    public void DiscardLogFile()
    {
        System.IO.Directory.CreateDirectory(DiscardedFileLocation);
        if(_fileName != null)
        {   
            File.Move(ResultFilePath, DiscardedFileLocation+_fileName);
            //making sure that just in case content is added even after discarding, it goes into the right file
            ResultFilePath = DiscardedFileLocation+_fileName;
        }
    }
}
