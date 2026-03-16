using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class MCQController : MonoBehaviour
{
    // ================= PANELS =================
    [Header("Panels")]
    [SerializeField] private GameObject mcqPanel;
    [SerializeField] private GameObject explanationPanel;

    // ================= QUESTION UI =================
    [Header("Question UI")]
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Image referenceImage;

    // ================= OPTIONS =================
    [Header("Options")]
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private TMP_Text[] optionTexts;

    // ================= EXPLANATION =================
    [Header("Explanation UI")]
    [SerializeField] private TMP_Text explanationText;
    [SerializeField] private Button explanationActionButton;

    [Header("Explanation Entry Buttons")]
    [SerializeField] private Button rightExplanationButton;
    [SerializeField] private Button wrongExplanationButton;

    // ================= VISUALS =================
    [Header("Sprites")]
    [SerializeField] private Sprite defaultButtonSprite;
    [SerializeField] private Sprite correctSprite;
    [SerializeField] private Sprite wrongSprite;

    // ================= DATA =================
    [Header("Data")]
    [SerializeField] private MCQQuestionData questionData;

    // ================= AUDIO =================
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip wrongClip;

    // ================= EVENT =================
    public static Action<int> OnMCQCorrectAnswered;

    // ================= STATE =================
    private class MCQState
    {
        public bool answeredCorrectly;
        public HashSet<int> wrongAttempts = new HashSet<int>();
    }

    private static Dictionary<int, MCQState> mcqStates =
        new Dictionary<int, MCQState>();

    // Replace this with your own page index logic if needed
    private int SlideIndex => gameObject.scene.buildIndex;

    // =================================================
 public UnityEvent OnQuestionAnsweredCorrectly;
    private void OnEnable()
    {
        LoadQuestion();
        BindButtons();
        RestoreState();
    }

    // ================= LOAD =================

    private void LoadQuestion()
    {
        if (questionData == null)
            return;

        questionText.text = questionData.questionText;
        explanationText.text = questionData.explanationText;

        if (referenceImage != null)
        {
            if (questionData.referenceImage != null)
            {
                referenceImage.gameObject.SetActive(true);
                referenceImage.sprite = questionData.referenceImage;
                referenceImage.color = Color.white;
            }
            else
            {
                referenceImage.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;

            optionTexts[i].text = questionData.options[i];
            optionButtons[i].interactable = true;
            optionButtons[i].GetComponent<Image>().sprite = defaultButtonSprite;

            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
        }

        mcqPanel.SetActive(true);
        explanationPanel.SetActive(false);
        HideExplanationButtons();
    }

    // ================= RESTORE =================

    private void RestoreState()
    {
        if (!mcqStates.ContainsKey(SlideIndex))
            return;

        MCQState state = mcqStates[SlideIndex];

        foreach (int wrong in state.wrongAttempts)
        {
            optionButtons[wrong].GetComponent<Image>().sprite = wrongSprite;
            optionButtons[wrong].interactable = false;
        }

        if (state.answeredCorrectly)
        {
            optionButtons[questionData.correctOptionIndex]
                .GetComponent<Image>().sprite = correctSprite;

            DisableAllOptions();
            ShowRightExplanation();
        }
    }

    // ================= ANSWER LOGIC =================

    private void OnOptionSelected(int index)
    {
        if (!mcqStates.ContainsKey(SlideIndex))
            mcqStates[SlideIndex] = new MCQState();

        MCQState state = mcqStates[SlideIndex];

        bool isCorrect = index == questionData.correctOptionIndex;

        if (isCorrect)
        {
            state.answeredCorrectly = true;

            PlaySound(correctClip);

            optionButtons[index].GetComponent<Image>().sprite = correctSprite;
            DisableAllOptions();
            ShowRightExplanation();

            // PageNavigationController.RequestNavigationUnlock();
            OnQuestionAnsweredCorrectly?.Invoke();
        }
        else
        {
            if (!state.wrongAttempts.Contains(index))
                state.wrongAttempts.Add(index);

            PlaySound(wrongClip);

            optionButtons[index].GetComponent<Image>().sprite = wrongSprite;
            optionButtons[index].interactable = false;
            ShowWrongExplanation();
        }
    }

    // ================= EXPLANATION FLOW =================

    private void BindButtons()
    {
        rightExplanationButton.onClick.RemoveAllListeners();
        wrongExplanationButton.onClick.RemoveAllListeners();
        explanationActionButton.onClick.RemoveAllListeners();

        rightExplanationButton.onClick.AddListener(OpenExplanation);
        wrongExplanationButton.onClick.AddListener(OpenExplanation);
        explanationActionButton.onClick.AddListener(CloseExplanation);
    }

    private void OpenExplanation()
    {
        mcqPanel.SetActive(false);
        explanationPanel.SetActive(true);
    }

    private void CloseExplanation()
    {
        explanationPanel.SetActive(false);
        mcqPanel.SetActive(true);
    }

    // ================= UI HELPERS =================

    private void DisableAllOptions()
    {
        foreach (var btn in optionButtons)
            btn.interactable = false;
    }

    private void HideExplanationButtons()
    {
        rightExplanationButton.gameObject.SetActive(false);
        wrongExplanationButton.gameObject.SetActive(false);
    }

    private void ShowRightExplanation()
    {
        rightExplanationButton.gameObject.SetActive(true);
        wrongExplanationButton.gameObject.SetActive(false);
    }

    private void ShowWrongExplanation()
    {
        wrongExplanationButton.gameObject.SetActive(true);
        rightExplanationButton.gameObject.SetActive(false);
    }

    // ================= AUDIO =================

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}