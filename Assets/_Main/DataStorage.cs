using System.Collections.Generic;

[System.Serializable]
public class Data
{
    public Preferences preferences = new();
    public State state = new();
}

[System.Serializable]
public class Preferences
{
    
}

[System.Serializable]
public class State
{
    public List<VocabularyInfo> vocabularies = new();
}

[System.Serializable]
public class VocabularyInfo
{
    public string vocabularyName;
    public bool isDone;
    public List<VocabularyWordTranslationPairInfo> wordTranslationPairs = new();
}

[System.Serializable]
public class VocabularyWordTranslationPairInfo
{
    public int index;
    public string word;
    public string translation;
    public bool isDone;
}