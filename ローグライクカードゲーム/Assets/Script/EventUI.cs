using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class EventUI : MonoBehaviour
{
    public static EventUI Instance { get; private set; }
    [SerializeField] GameObject eventPanel;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Transform choiceButtonArea;
    [SerializeField] GameObject choiceButtonPrefab;
    [SerializeField] Button confirmButton;
    [SerializeField] AudioClip typeSound;
    [SerializeField] Text floorCountText;
    [SerializeField] Image image;
    [SerializeField] GameObject floorCountPanel;
    [SerializeField] GameObject battlePanel;
    [SerializeField] Transform cardArea;
    [SerializeField] GameObject cardButtonPrefab;
    AudioSource audioSource;

    readonly HashSet<char> excludedCharacters = new HashSet<char> { '、', '。', ' ', '　', '\n' };

    void Awake(){
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
        eventPanel.SetActive(false);
        floorCountPanel.SetActive(false);
        battlePanel.SetActive(false);
    }

    void Start(){

    }

    public void ShowStory(StoryData storyData, System.Action onComplete){
        StartCoroutine(ShowStoryCoroutine(storyData.stories, onComplete));
    }

    IEnumerator ShowStoryCoroutine(List<Story> stories, System.Action onComplete)
    {
        eventPanel.SetActive(true);
        foreach (Story story in stories)
        {
            bool frag = false;
            yield return StartCoroutine(TypeText(story.StoryText, () => frag = true));
            while (!frag) yield return null;

            if (story.Choices != null && story.Choices.Count > 0){
                confirmButton.gameObject.SetActive(false);
                bool selected = false;
                foreach (var choice in story.Choices)
                {
                    var btnObj = Instantiate(choiceButtonPrefab, choiceButtonArea);
                    var btn = btnObj.GetComponent<Button>();
                    var text = btnObj.GetComponentInChildren<Text>();
                    text.text = choice.Text;
                    btn.onClick.AddListener(() =>
                    {
                        choice.ExecuteActions(PlayerCTR.Instance);
                        if (choice.NextStories != null && choice.NextStories.Count > 0) {
                            StartCoroutine(ShowStoryCoroutine(choice.NextStories, onComplete));
                        }
                        
                        else {
                            onComplete?.Invoke();
                        }

                        for(int j = 0; j < choiceButtonArea.childCount; j++){
                            Destroy(choiceButtonArea.GetChild(j).gameObject);
                        }
                        selected = true;
                    });
                }
                while (!selected) yield return null;
                yield break;
            }
            else {
                confirmButton.gameObject.SetActive(true);
                bool next = false;
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(() =>
                {
                    next = true;
                    confirmButton.gameObject.SetActive(false);
                });
                while (!next) yield return null;
                }
            }
        Hide();
        onComplete?.Invoke();
    }

    /*
    public void ShowEvent(StoryData storyData, System.Action onConfirm)
    {
        Debug.Log("ShowEvent called with storyData: " + storyData);
        eventPanel.SetActive(true);
        StartCoroutine(ShowStoriesCoroutine(storyData, () =>
        {
            confirmButton.gameObject.SetActive(true);
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() =>
            {
                Hide();
                onConfirm?.Invoke();
            });
        }));
    }

    public void ShowEventWithoutButton(StoryData storyData, System.Action onConfirm){
        Debug.Log("ShowEvent called with storyData: " + storyData);
        eventPanel.SetActive(true);
        confirmButton.gameObject.SetActive(false);
        StartCoroutine(ShowStoriesCoroutine(storyData, ()=>{
            Hide();
            onConfirm?.Invoke();
        }));
    }

    public void ShowChoices(StoryData storyData, List<(string choiceText, System.Action onChoose)> choices){
        eventPanel.SetActive(true);
        confirmButton.gameObject.SetActive(false);
        foreach (Transform child in choiceButtonArea)
            Destroy(child.gameObject);
        
        StartCoroutine(ShowStoriesCoroutine(storyData, () =>{
            int i = 0;
            foreach (var choice in choices){
                GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceButtonArea);
                // ボタンの位置を調整
                RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(0, -i * 50);
                Button button = buttonObj.GetComponent<Button>();
                Text buttonText = buttonObj.GetComponentInChildren<Text>();
                buttonText.text = choice.choiceText;
                button.onClick.AddListener(() =>{
                    for(int j = 0; j < choiceButtonArea.childCount; j++){
                        Destroy(choiceButtonArea.GetChild(j).gameObject);
                    }
                    Hide();
                    choice.onChoose?.Invoke();
                });
                i++;
            }
        }));
    }
    */

    public void ShowFloorCountUI(int floorCount, System.Action onComplete)
    {
        floorCountPanel.SetActive(true);
        floorCountText.text = $"{floorCount}階";
        StartCoroutine(FadeInAndOut(1f, () =>
        {
            floorCountPanel.SetActive(false);
            onComplete?.Invoke();
        }));
    }

    public void ShowBattleUI()
    {
        battlePanel.SetActive(true);
        eventPanel.SetActive(false);
    }

    /*

    public void ShowChoiceUI(List<CardData> cards, System.Action<CardData> onSelected)
    {
        eventPanel.SetActive(true);

        foreach (var card in cards)
        {
            var btn = Instantiate(cardButtonPrefab, cardArea);
            btn.GetComponent<CardButton>().Setup(card);
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log("カードが選ばれました");
                eventPanel.SetActive(false);
                for (int i = 0; i < cardArea.childCount; i++)
                {
                    Destroy(cardArea.GetChild(i).gameObject);
                }
                onSelected?.Invoke(card);
                Hide();
            });
        }
    }
    */

    IEnumerator FadeInAndOut(float duration, System.Action onComplete)
    {
        // フェードイン
        image.DOFade(1, duration);
        floorCountText.DOFade(1, duration);
        yield return new WaitForSeconds(duration);

        yield return new WaitForSeconds(1f);
        // フェードアウト
        image.DOFade(0, duration);
        floorCountText.DOFade(0, duration);
        yield return new WaitForSeconds(duration);

        onComplete?.Invoke();
    }

    IEnumerator TypeText(string text, System.Action onComplete)
    {
        descriptionText.text = "";
        float typeSpeed = 0.05f;
        foreach (char c in text)
        {
            descriptionText.text += c;
            if (Input.GetKey(KeyCode.Return))
            {
                descriptionText.text = text;
                break;
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                typeSpeed = 0.01f;
            }
            else typeSpeed = 0.05f;

            if (!excludedCharacters.Contains(c))
            {
                audioSource.PlayOneShot(typeSound);
            }
            yield return new WaitForSeconds(typeSpeed);
        }
        onComplete?.Invoke();
        yield return null;
    }

    /*
    IEnumerator ShowStoriesCoroutine(StoryData storyData, System.Action onComplete){
        foreach(var story in storyData.stories){
            bool finished = false;
            yield return StartCoroutine(TypeText(story.StoryText, ()=> finished = true));
            while(!finished) yield return null;
        }
        onComplete?.Invoke();
    }
    */

    public void Hide()
    {
        eventPanel.SetActive(false);
    }
}
