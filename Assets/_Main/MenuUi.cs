using UnityEngine;
using UnityEngine.UI;

public class MenuUi : MonoBehaviour
{
    public Transform viewportContent;
    public Button startGameButton;
    public GameObject vocabularyLayoutPrefab;
    public GameObject newVocabularyButtonContainerPrefab;
    
    private int _vocabularyCount;
    
    public static void Load()
    {
        var prefab = Resources.Load<GameObject>("Menu/MenuUi"); 
        var instance = Instantiate(prefab, Core.Ui.Get.transform).GetComponent<MenuUi>();
        instance.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        instance.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        instance.LoadMenuElements();
    }
    
    private void LoadMenuElements()
    {
        LoadSavedVocabularies();
        LoadVocabularyButton();
        LoadStartGameButton();
    }

    private void LoadSavedVocabularies()
    {
        foreach (var vocabularyInfo in Core.Data.Get.state.vocabularies) { CreateVocabularyLayout(vocabularyInfo); }
    }

    private void LoadVocabularyButton()
    {
        var newVocabularyButtonContainer = Instantiate(newVocabularyButtonContainerPrefab, viewportContent);
        var newVocabularyButton = newVocabularyButtonContainer.GetComponentInChildren<Button>();
        newVocabularyButton.onClick.AddListener(OnNewVocabularyButtonClick);
    }

    private void OnNewVocabularyButtonClick()
    {
        var info = new VocabularyInfo();
        Core.Data.Get.state.vocabularies.Add(info);
        info.vocabularyName = $"Словарь {_vocabularyCount + 1}";
        CreateVocabularyLayout(info);
    }

    private void CreateVocabularyLayout(VocabularyInfo info)
    {
        var vocabularyLayoutObj = Instantiate(vocabularyLayoutPrefab, viewportContent);
        var vocabularyLayout = vocabularyLayoutObj.GetComponent<VocabularyLayout>();
        vocabularyLayout.InitializeElements(info);
        _vocabularyCount++;
    }

    private void LoadStartGameButton()
    {
        startGameButton.onClick.AddListener(OnStartButtonClick);
    }
    
    private void OnStartButtonClick()
    {
        if (Core.Data.Get.state.vocabularies.Count == 0)
        { Debug.Log("No vocabulary found"); return; }
        Destroy(gameObject);
        Core.Event.Invoke(new LoadGameEvent());
    }
}
