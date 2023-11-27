using UnityEngine;
using System.IO;

public static class ExperimentConfigLoader
{
    private static ExperimentPermutation experimentPermutations;

    public static readonly string ConfigFileLocation = Application.persistentDataPath;
    public static readonly string ConfigFilePath = Path.Combine(ConfigFileLocation, "config.json");

    public static ExperimentPermutation FetchExperimentConfig()
    {
        if(experimentPermutations == null)
        {
            LoadConfig();
        }

        return experimentPermutations;
    }

    private static void LoadConfig()
    {
        if (File.Exists(ConfigFilePath))
        {
            string json = File.ReadAllText(ConfigFilePath);

            experimentPermutations = JsonUtility.FromJson<ExperimentPermutation>(json);
            if (experimentPermutations == null)
            {
                Debug.LogError("Failed to parse JSON data!");
            }
        }
        else
        {
            Debug.Log(ConfigFilePath);

            Debug.LogError("Config file not found!");
        }
    }
}