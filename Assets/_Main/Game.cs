using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static void Begin() { new GameObject("Game").AddComponent<Game>().StartGame(); }
    public static void End() { FindFirstObjectByType<Game>()?.StopGame(); }

    private VocabularyInfo _currentVocabulary;
    private int _currentWordTranslationPairIndex;
    
    private GameUi _ui;
    private Pause _pause;
    private Field _field;
    private string _translation;

    public List<WordLetter> WordLetters { get; private set; }

    private void StartGame()
    {
        Core.Observer.Register<Game>(this);
        _pause = new Pause();
        GameUi.Load(out _ui);
        TryLoadVocabulary();
        LoadLetters();
        Core.Event.Subscribe<GameExitEvent>(OnGameExit);
        Core.Event.Subscribe<GameOverEvent>(OnGameOver);
        Core.Event.Subscribe<GameWonEvent>(OnGameWon);
        Core.Event.Subscribe<CorrectLetterEnteredEvent>(CheckWord);
        Core.Coroutine.Start(LoadScene);
    }

    private bool TryLoadVocabulary()
    {
        VocabularyInfo newVocab = null;
        foreach (var vocab in Core.Data.Get.state.vocabularies)
        {
            if (vocab.wordTranslationPairs.Count <= 0) continue;
            if (vocab.isDone) continue;
            newVocab = vocab;
        }

        if (newVocab == null) { return false; }
        _currentVocabulary = newVocab;
        return true;
    }

    private IEnumerator LoadScene()
    {
        yield return CreateGameField();
        Core.Event.Invoke(new GameSceneLoadedEvent());
    }

    private void CheckWord(CorrectLetterEnteredEvent eventData)
    {
        foreach (var wordLetter in WordLetters)
        { if (!wordLetter.IsEntered) return; }
        _currentVocabulary.wordTranslationPairs[_currentWordTranslationPairIndex].isDone = true;
        Core.Event.Invoke(new WordLettersDoneEvent());
        
        foreach (var wordTranslationPair in _currentVocabulary.wordTranslationPairs)
        {
            if (!wordTranslationPair.isDone)
            { _currentWordTranslationPairIndex++; break; }
            _currentVocabulary.isDone = true;
            _currentWordTranslationPairIndex = 0;
        }
        
        if (!TryLoadVocabulary()) { Core.Event.Invoke(new GameWonEvent()); return; }
        
        LoadLetters();
    }
    
    private void LoadLetters()
    {
        var wordPairs = _currentVocabulary.wordTranslationPairs;
        if (wordPairs.Count < _currentWordTranslationPairIndex) { Debug.LogWarning("No words detected"); return; }
        var rWordPair = wordPairs[_currentWordTranslationPairIndex];
        WordLetters = new List<WordLetter>();
        foreach (var letter in rWordPair.word.ToCharArray())
        { WordLetters.Add(new WordLetter { Letter = letter, IsEntered = false}); }
        _translation = rWordPair.translation;
    }
    
    private IEnumerator CreateGameField()
    {
        _field = new GameObject("Field").AddComponent<Field>();
        yield return _field.Init();
    }

    private void OnGameExit(GameExitEvent _)
    {
        Debug.Log("Game exit");
        Core.Coroutine.Start(ExitGame);
        return;
        IEnumerator ExitGame()
        {
            yield return _field.ClearField();
            Core.Event.Invoke(new LoadMenuEvent());
        }
    }
    
    private void OnGameOver(GameOverEvent _)
    {
        Debug.Log("GameOver");
        Core.Coroutine.Start(_field.ClearField);
    }
    
    private void OnGameWon(GameWonEvent _)
    {
        Debug.Log("Game won");
        Core.Coroutine.Start(_field.ClearField);
    }
    
    private void StopGame()
    {
        Core.Observer.Unregister<Game>();
        Core.Event.UnSubscribe<GameExitEvent>(OnGameExit);
        Core.Event.UnSubscribe<GameOverEvent>(OnGameOver);
        Core.Event.UnSubscribe<GameWonEvent>(OnGameWon);
        Core.Event.UnSubscribe<CorrectLetterEnteredEvent>(CheckWord);
        foreach (var vocabulary in Core.Data.Get.state.vocabularies)
        {
            vocabulary.isDone = false;
            foreach (var pair in vocabulary.wordTranslationPairs)
            { pair.isDone = false; }
        }
        _ui.UnLoad();
        _pause.Dispose();
        _field.DeInit();
        _field = null;
        _pause = null;
        _ui = null;
        Core.Coroutine.Stop(LoadScene);
    }
}