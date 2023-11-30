using UnityEngine;
using System.IO;

public static class ExperimentTextLoader
{
    public static readonly string TextFileLocation = Path.Combine(ExperimentConfigLoader.ConfigFileLocation, "Texts/");

    public static string FetchText(string textFileName)
    {
        string textFilePath = SearchForTextFile(textFileName);

        if(textFilePath != null)
        {
            return LoadText(textFilePath);
        }
        else
        {
            Debug.LogError("Unable to find the text file named `"+textFileName+"`, that is referenced in the config file, "+
            "in the text file location `"+TextFileLocation+"`!");
            return null;
        }
    }

    private static string LoadText(string textFilePath)
    {
        string text;

        text = File.ReadAllText(textFilePath);

        return text;
    }

    private static string SearchForTextFile(string textFileName)
    {   
        string originalFilePath = Path.Combine(TextFileLocation, textFileName);
        string pathWithFileEnding = Path.Combine(TextFileLocation, textFileName+".txt");

        if(File.Exists(originalFilePath))
        {
            return originalFilePath;
        }
        if(File.Exists(pathWithFileEnding))
        {
            return pathWithFileEnding;
        }

        return null;
    }
}