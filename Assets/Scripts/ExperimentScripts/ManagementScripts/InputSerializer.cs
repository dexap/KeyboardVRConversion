using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ExperimentScripts.Model;
using UnityEngine;
using Newtonsoft.Json;


public class InputSerializer
{
    private static readonly string ResultFileLocation = Application.persistentDataPath + "/Results/";

    public static readonly string DiscardedFileLocation = Application.persistentDataPath + "/DiscardedResets/";

    private readonly string _csvLogFileName;
    private readonly string _resultJsonFilePath;
    public string CsvLogFilePath { get; private set; }
    private string JsonResultFilePath { get; set; }


    private float _timeOfFirstInput = -1f;

    private readonly ExperimentData _experimentData;

    public InputSerializer(string experimentId, int experimentRound, ExperimentModalities modus, string currentText)
    {
        var time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");

        //making sure that the filename is valid
        Regex r = new Regex("^[^.<>:;,?\\\"*|/]+$");
        if (!r.IsMatch(experimentId))
        {
            Debug.LogError(
                "Using fallback filename! Provided experimentId contains illegal characters (^.<>:;,?\\\"*|/) !");
            _resultJsonFilePath = time + "-round-" + experimentRound + ".json";
        }
        else
        {
            _resultJsonFilePath = time + "-expid-" + experimentId + "-round-" + experimentRound + ".json";
        }

        JsonResultFilePath = Path.Combine(ResultFileLocation, modus.ToString(), _resultJsonFilePath);
        Directory.CreateDirectory(Path.GetDirectoryName(JsonResultFilePath)
                                  ?? throw new InvalidOperationException("Dir for JSON file could not be created!"));
        _experimentData = new ExperimentData
        {
            ExpId = experimentId,
            Round = experimentRound,
            Modus = modus.ToString(),
            Text = currentText.TrimEnd(),
            WordsPerMinute = 0f,
            ErrorRate = 0f,
            TypedKeys = new List<string>(),
            Keystrokes = new List<Keystroke>()
        };
        SaveToResultJsonFile(JsonResultFilePath);
    }

    public void LogInput(string text)
    {
        // checking if there has been an input yet
        if (_timeOfFirstInput < 0)
        {
            _timeOfFirstInput = Time.time;
        }

        var timeSinceFirstInput = Time.time - _timeOfFirstInput;

        // CSV file
        // var csvLine = timeSinceFirstInput + ";";
        // csvLine += text + ";"; 
        // AddLineToCsvLogFile(CsvLogFilePath, csvLine);

        // JSON file

        LogKeystrokeToJsonResultFile(text, timeSinceFirstInput, Manus.Utility.HandType.Invalid,
            Manus.Utility.FingerType.Invalid);
        SaveToResultJsonFile(JsonResultFilePath);
    }

    public void LogInput(string text, Manus.Utility.HandType handType, Manus.Utility.FingerType fingerType)
    {
        if (_timeOfFirstInput < 0)
        {
            _timeOfFirstInput = Time.time;
        }

        var timeSinceFirstInput = Time.time - _timeOfFirstInput;

        // CSV file
        //var csvLine = timeSinceFirstInput + ";";
        //csvLine += text + ";";
        //csvLine += handType + ";";
        //csvLine += fingerType + ";";
        //AddLineToCsvLogFile(CsvLogFilePath, csvLine);

        LogKeystrokeToJsonResultFile(text, timeSinceFirstInput, handType, fingerType);
        SaveToResultJsonFile(JsonResultFilePath);
    }


    private void LogKeystrokeToJsonResultFile(string text, float timeSinceFirstInput, Manus.Utility.HandType handType,
        Manus.Utility.FingerType fingerType)
    {
        Keystroke keystroke = new()
        {
            Time = timeSinceFirstInput,
            Key = text,
            Hand = handType.ToString(),
            Finger = fingerType.ToString()
        };

        switch (keystroke.Key)
        {
            case "Backspace" or "BACKSPACE":
                _experimentData.TypedKeys.RemoveAt(_experimentData.TypedKeys.Count - 1);
                break;
            case "Space" or "SPACE":
                _experimentData.TypedKeys.Add(" ");
                break;
            case "NO BEHAVIOR: \r" or "NO BEHAVIOR: \n" or "NO BEHAVIOR: \r\n" or "NO BEHAVIOR: \n\r" or "NO BEHAVIOR: ":
                break;
            case "LeftShift" or "RightShift" or "LeftControl" or "RightControl" or "LeftAlt" or "RightAlt":
                break;
            default:
                _experimentData.TypedKeys.Add(keystroke.Key); 
                break;
        }

        _experimentData.Keystrokes.Add(keystroke);

        // Calculated Data
        _experimentData.WordsPerMinute = CalculateWordsPerMinute(timeSinceFirstInput, _experimentData.TypedKeys);
        _experimentData.ErrorRate = CalculateErrorRate(_experimentData.Text, _experimentData.TypedKeys);
    }

    private static float CalculateErrorRate(string originalText, List<string> typedKeys)
    {
        var typedText = string.Join("", typedKeys).Replace(" ", "");
        //var levenshteinDistance = ComputeLevenshteinDistance(originalText.ToLower(), typedText.ToLower());
        var levenshteinDistance = ComputeLevenshteinDistance(originalText, typedText);
        var errorRate = (float)levenshteinDistance / originalText.Length;
        return errorRate;
    }


    private static float CalculateWordsPerMinute(float timeOfLastKeystroke, List<string> typedKeys)
    {
        var typedTextWithoutSpaces = string.Join("", typedKeys).Replace(" ", "");
        var fiveLetterString = typedTextWithoutSpaces.Length / 5;
        var timeInMinutes = timeOfLastKeystroke / 60;
        return fiveLetterString / timeInMinutes;
    }

    private static int ComputeLevenshteinDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source)) return string.IsNullOrEmpty(target) ? 0 : target.Length;
        if (string.IsNullOrEmpty(target)) return source.Length;

        var d = new int[source.Length + 1, target.Length + 1];

        for (var i = 0; i <= source.Length; i++)
            d[i, 0] = i;

        for (var j = 0; j <= target.Length; j++)
            d[0, j] = j;

        for (var i = 1; i <= source.Length; i++)
        {
            for (var j = 1; j <= target.Length; j++)
            {
                var cost = target[j - 1] == source[i - 1] ? 0 : 1;
                d[i, j] = Mathf.Min(
                    Mathf.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost
                );
            }
        }

        return d[source.Length, target.Length];
    }


    private static void AddLineToCsvLogFile(string filePath, string csvLine)
    {
        Directory.CreateDirectory(ResultFileLocation);
        using StreamWriter sw = File.AppendText(filePath);
        sw.WriteLine(csvLine);
    }

    private void SaveToResultJsonFile(string filePath)
    {
        var json = JsonConvert.SerializeObject(_experimentData);
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Should only be called when a round gets reset.
    /// Moves the current log file out of the results folder and puts it into a "discarded" folder.
    /// It is highly recommended to use another InputSerializer instance afterward!
    /// </summary>
    public void DiscardLogFile()
    {
        Directory.CreateDirectory(DiscardedFileLocation);
        /*
        if (_csvLogFileName != null)
        {
            File.Move(CsvLogFilePath, DiscardedFileLocation + _csvLogFileName);
            //making sure that just in case content is added even after discarding, it goes into the right file
            CsvLogFilePath = DiscardedFileLocation + _csvLogFileName;
        }
        */

        if (JsonResultFilePath != null)
        {
            File.Move(JsonResultFilePath, DiscardedFileLocation + _resultJsonFilePath);
            JsonResultFilePath = DiscardedFileLocation + _resultJsonFilePath;
        }
    }
}