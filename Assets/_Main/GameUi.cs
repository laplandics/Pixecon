using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUi : MonoBehaviour
{
    public RectTransform gameUiContainer;
    public TMP_Text wordLettersText;
    public Button pauseButton;
    public Button exitButton;
    
    [Space]
    public RectTransform gameOverContainer;
    public Button gameOverExitButton;
    
    [Space]
    public RectTransform gameWonContainer;
    public Button gameWonExitButton;
    
    private bool _isGamePaused;
    private string _currentEnteredWord;
    
    public static void Load(out GameUi ui)
    {
        var prefab = Resources.Load<GameObject>("Game/GameUi"); 
        var instance = Instantiate(prefab, Core.Ui.Get.transform).GetComponent<GameUi>();
        instance.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        instance.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        instance.LoadGameElements();
        ui = instance;
    }
    
    private void LoadGameElements()
    {
        _currentEnteredWord = string.Empty;
        Core.Event.Subscribe<CorrectLetterEnteredEvent>(OnCorrectLetterEntered);
        Core.Event.Subscribe<WordLettersDoneEvent>(OnWordLettersDoneEvent);
        Core.Event.Subscribe<GameOverEvent>(OnGameOver);
        Core.Event.Subscribe<GameWonEvent>(OnGameWon);
        pauseButton.onClick.AddListener(OnPauseButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
        gameOverExitButton.onClick.AddListener(OnGameWonOrGameOverExitButtonClick);
        gameWonExitButton.onClick.AddListener(OnGameWonOrGameOverExitButtonClick);
    }

    private void OnPauseButtonClick()
    {
        if (!_isGamePaused) Core.Event.Invoke(new PauseGameEvent());
        else Core.Event.Invoke(new ResumeGameEvent());
        _isGamePaused = !_isGamePaused;
    }

    private void OnExitButtonClick()
    {
        Core.Event.Invoke(new GameExitEvent());
    }

    private void OnGameWonOrGameOverExitButtonClick()
    {
        Core.Event.Invoke(new LoadMenuEvent());
    }
    
    private void OnCorrectLetterEntered(CorrectLetterEnteredEvent eventData)
    {
        _currentEnteredWord += eventData.Letter;
        wordLettersText.text = _currentEnteredWord;
    }

    private void OnWordLettersDoneEvent(WordLettersDoneEvent _)
    {
        _currentEnteredWord = string.Empty;
        wordLettersText.text = "";
    }

    private void OnGameOver(GameOverEvent _)
    {
        gameUiContainer.gameObject.SetActive(false);
        gameOverContainer.gameObject.SetActive(true);
    }

    private void OnGameWon(GameWonEvent _)
    {
        gameUiContainer.gameObject.SetActive(false);
        gameWonContainer.gameObject.SetActive(true);
    }
    
    public void UnLoad()
    {
        Core.Event.UnSubscribe<GameOverEvent>(OnGameOver);
        Core.Event.UnSubscribe<GameWonEvent>(OnGameWon);
        Core.Event.UnSubscribe<WordLettersDoneEvent>(OnWordLettersDoneEvent);
        Core.Event.UnSubscribe<CorrectLetterEnteredEvent>(OnCorrectLetterEntered);
        Core.Event.Invoke(new ResumeGameEvent());
        _isGamePaused = false;
        Destroy(gameObject);
    }
}