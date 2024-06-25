using UnityEngine;
using System.IO;

public static class ExperimentTextLoader
{
    public static readonly string TextFileLocation = Path.Combine(ExperimentConfigLoader.ConfigFileLocation, "Texts/");

    public static string FetchText(string textFileName)
    {
        string textFilePath = SearchForTextFile(textFileName);

        if (textFilePath == null)
        {
          Debug.LogError("Unable to find the text file named `" + textFileName + "`, that is referenced in the config file, " +
          "in the text file location `" + TextFileLocation + "`!");
          return null;
        }

        return LoadText(textFilePath);
        
  }

    private static string LoadText(string textFilePath)
    {
        string text;

        text = File.ReadAllText(textFilePath);

        return ChooseRandomLine(text);
    }

    private static string ChooseRandomLine(string text)
    {
        string[] lines = text.Split('\n');
        int randomIndex = Random.Range(0, lines.Length);
        return lines[randomIndex];
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