using UnityEngine;
using System.IO;

public static class ExperimentConfigLoader
{
    public static readonly string ConfigFileLocation = Application.persistentDataPath;
    public static readonly string ConfigFilePath = Path.Combine(ConfigFileLocation, "config.json");

    public static ExperimentPermutation FetchExperimentConfig()
    {
        ExperimentPermutation experimentPermutations;

        experimentPermutations = LoadConfig();

        return experimentPermutations;
    }

    private static ExperimentPermutation LoadConfig()
    {
        ExperimentPermutation experimentPermutations = null;

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

        return experimentPermutations;
    }
}