using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VocabularyLayout : MonoBehaviour
{
    public Button removeWordPairButton;
    public TMP_InputField vocabularyNameInput;
    public Button newWordPairButton;
    public GameObject worldAndTranslationLayoutPrefab;
    
    private VocabularyInfo _vocabularyInfo;
    private Dictionary<int, WorldAndTranslationLayout> _vocabulary;
    private int _wordPairsCount;
    
    public void InitializeElements(VocabularyInfo info)
    {
        _vocabulary = new Dictionary<int, WorldAndTranslationLayout>();
        vocabularyNameInput.placeholder.GetComponent<TextMeshProUGUI>().text = info.vocabularyName;
        newWordPairButton.onClick.AddListener(OnNewWordPairButtonClick);
        removeWordPairButton.onClick.AddListener(OnRemoveWordPairButtonClick);
        vocabularyNameInput.onEndEdit.AddListener(OnNewVocabularyNameEntered);
        _vocabularyInfo = info;
        LoadVocabulary();
    }

    private void LoadVocabulary()
    {
        vocabularyNameInput.text = _vocabularyInfo.vocabularyName;
        if (_vocabularyInfo.wordTranslationPairs.Count == 0) return;
        foreach (var pairInfo in _vocabularyInfo.wordTranslationPairs)
        { CreateWordTranslationPair(pairInfo); }
    }
    
    private void OnNewWordPairButtonClick()
    {
        var wordPairInfo = new VocabularyWordTranslationPairInfo
        { index = _wordPairsCount, word = string.Empty, translation = string.Empty };
        CreateWordTranslationPair(wordPairInfo);
        _vocabularyInfo.wordTranslationPairs.Add(wordPairInfo);
    }

    private void CreateWordTranslationPair(VocabularyWordTranslationPairInfo info)
    {
        var worldAndTranslationLayoutObject = Instantiate(worldAndTranslationLayoutPrefab, transform);
        var worldAndTranslationLayout = new WorldAndTranslationLayout();
        var wordInput = worldAndTranslationLayoutObject.transform.GetChild(0).GetComponent<TMP_InputField>();
        var translationInput = worldAndTranslationLayoutObject.transform.GetChild(1).GetComponent<TMP_InputField>();
        
        if (info.word != string.Empty) wordInput.text = info.word;
        if (info.translation != string.Empty) translationInput.text = info.translation;
        
        wordInput.onEndEdit.AddListener(worldAndTranslationLayout.UpdateWordInfo);
        translationInput.onEndEdit.AddListener(worldAndTranslationLayout.UpdateTranslationInfo);
        worldAndTranslationLayout.Layout = worldAndTranslationLayoutObject;
        worldAndTranslationLayout.PairInfo = info;
        _wordPairsCount++;
        _vocabulary.Add(_wordPairsCount, worldAndTranslationLayout);
    }
    
    private void OnRemoveWordPairButtonClick()
    {
        if (_wordPairsCount == 0) { DeleteVocabulary(); return; }
        
        Destroy(_vocabulary[_wordPairsCount].Layout);
        _vocabulary.Remove(_wordPairsCount);
        _vocabularyInfo.wordTranslationPairs.Remove(_vocabularyInfo.wordTranslationPairs[^1]);
        _wordPairsCount--;
    }

    private void DeleteVocabulary()
    {
        Core.Data.Get.state.vocabularies.Remove(_vocabularyInfo);
        Destroy(gameObject);
    }

    private void OnNewVocabularyNameEntered(string newName)
    { _vocabularyInfo.vocabularyName = vocabularyNameInput.text; }
}

public class WorldAndTranslationLayout
{
    public GameObject Layout;
    public VocabularyWordTranslationPairInfo PairInfo;
    
    public void UpdateWordInfo(string word)
    { PairInfo.word = word; }

    public void UpdateTranslationInfo(string translation)
    { PairInfo.translation = translation; }
}