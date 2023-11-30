[System.Serializable]
public class ExperimentPermutation
{
    public ExperimentSequence[] permutations;
}

[System.Serializable]
public class ExperimentSequence
{
    public int[] modes;

    public string[] texts;
}
