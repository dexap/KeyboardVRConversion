using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class InputSerializer
{
    public static readonly string ResultFileLocation = Application.persistentDataPath + "/Results/";

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
        if(_timeOfFirstInput == -1f)
        {
            _timeOfFirstInput = Time.time;
        }

        float timeSinceFirstInput = Time.time - _timeOfFirstInput;

        string csv = timeSinceFirstInput + ";";
        csv += text + ";";
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
}
