using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using Random = System.Random;
using System.IO;
using System.Collections.Generic;
public enum LessonType
{
    Visual,
    Auditory
}

public enum BoardState
{
    Start,
    Lesson,
    LessonEnded,
    Exam,
    ExamEnded,
    End
}

public enum ChoiseOption
{
    A = 0,
    B = 1,
    C = 2,
    D = 3,
    Empty = -1
}

public class Board : MonoBehaviour
{
    public TextMeshPro LessonBoardText;
    public TextMeshPro ExamQuestionBoardText;
    public TextMeshPro OptionAText;
    public TextMeshPro OptionBText;
    public TextMeshPro OptionCText;
    public TextMeshPro OptionDText;

    [SerializeField] private VrClassButton startButton;
    [SerializeField] private VrClassButton optionAButton;
    [SerializeField] private VrClassButton optionBButton;
    [SerializeField] private VrClassButton optionCButton;
    [SerializeField] private VrClassButton optionDButton;

    //AudioSource[] audioSources = Object.FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
    public AudioSource DogBark;
    public AudioSource PhoneNoise;
    public AudioSource Bus;
    public AudioSource PaperFold;
    public AudioSource PenClick;
    public AudioSource Ambulance;


    private BoardState boardState = BoardState.Start;
    private int sessions = 2;
    private int wordsPerSession = 10;
    private int currentSessionIndex = 0;
    private int wrongTranslationsPerQuestion = 3;
    private Lesson[] lessons;
    private Exam[] exams;
    private Random random;

    private ChoiseOption UserAnswerChoiseIndex { get; set; }
    private bool WaitForUserToAnswer { get; set; }

    private int secondsToWait = 3;

    // Start is called before the first frame update
    void Start()
    {
        //startButton.ButtonPressed += OnStartButtonPressed;
        //optionAButton.ButtonPressed += OnOptionAButtonPressed;
        //optionBButton.ButtonPressed += OnOptionBButtonPressed;
        //optionCButton.ButtonPressed += OnOptionCButtonPressed;
        //optionDButton.ButtonPressed += OnOptionDButtonPressed;

        startButton?.onPressed.AddListener(OnStartButtonPressed);
        optionAButton?.onPressed.AddListener(OnOptionAButtonPressed);
        optionBButton?.onPressed.AddListener(OnOptionBButtonPressed);
        optionCButton?.onPressed.AddListener(OnOptionCButtonPressed);
        optionDButton?.onPressed.AddListener(OnOptionDButtonPressed);


        LessonBoardText = GetComponentInChildren<TextMeshPro>();
        LessonBoardText.SetText("Welcome!\nPress 'Start' to begin.");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnDestroy()
    {
        startButton?.onPressed.RemoveListener(OnStartButtonPressed);
        optionAButton?.onPressed.RemoveListener(OnOptionAButtonPressed);
        optionBButton?.onPressed.RemoveListener(OnOptionBButtonPressed);
        optionCButton?.onPressed.RemoveListener(OnOptionCButtonPressed);
        optionDButton?.onPressed.RemoveListener(OnOptionDButtonPressed);
    }

    void OnStartButtonPressed()
    {
        switch (boardState)
        {
            case BoardState.Start:
                random = new Random();
                ChangeBoardStatus(BoardState.Lesson);
                LoadWordsFromDataSets();
                StartCoroutine(RunLesson());
                break;

            case BoardState.LessonEnded:
                ChangeBoardStatus(BoardState.Exam);
                StartCoroutine(RunExam());
                break;

            case BoardState.ExamEnded:
                ChangeBoardStatus(BoardState.Lesson);
                StartCoroutine(RunLesson());
                break;

            default:
                break;
        }
    }

    public void FillWordsWithAudio(Word[] words)
    {
        AudioSource[] audioSources = new AudioSource[6];
        audioSources[0] = DogBark;
        audioSources[1] = PhoneNoise;
        audioSources[2] = PenClick;
        audioSources[3] = Ambulance;
        audioSources[4] = PaperFold;
        audioSources[5] = Bus;
        int currentRandomIndex;
        for (int i = 0; i < words.Length; i++)
        {
            currentRandomIndex = random.Next(audioSources.Length);
            words[i].WordDist = new AudioDistraction(audioSources[currentRandomIndex]);
        }


    }

    public void LoadWordsFromDataSets()
    {
        List<Word> wordsDataSet = LoadForeignWordsFromCsvFile();
        List<string> englishWordsDataSet = LoadEnglishWordsFromCsvFile();

        int totalnumberOfWords = sessions * wordsPerSession;
        Word[] chosenWords = ChooseWordsRandomly(wordsDataSet, totalnumberOfWords);
        FillWordsWithWrongTranslations(chosenWords, englishWordsDataSet);
        FillWordsWithAudio(chosenWords);

        lessons = new Lesson[sessions];
        exams = new Exam[sessions];

        InitSessions(lessons, exams, chosenWords);
    }

    public void InitSessions(Lesson[] ls, Exam[] es, Word[] ws)
    {
        for (int i = 0; i < sessions; i++)
        {
            Word[] wordsForSession = new Word[wordsPerSession];
            for (int j = 0; j < wordsPerSession; j++)
            {
                wordsForSession[j] = ws[i * wordsPerSession + j];
            }
            if (i % 2 == 0)
            {
                ls[i] = new Lesson(wordsForSession, LessonType.Visual);
            }
            else
            {
                ls[i] = new Lesson(wordsForSession, LessonType.Auditory);
            }
            es[i] = new Exam(wordsForSession, random);
        }
    }

    public List<Word> LoadForeignWordsFromCsvFile()
    {
        List<Word> words = new List<Word>();
        using (var reader = new StreamReader("./Assets/WordsDataSets/ForeignWords.csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                try
                {
                    words.Add(new Word(values[0], values[1]));
                }
                catch (ArgumentException) { }
            }
        }
        return words;
    }

    public List<string> LoadEnglishWordsFromCsvFile()
    {
        List<string> words = new List<string>();
        using (var reader = new StreamReader("./Assets/WordsDataSets/EnglishWords.csv"))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split();
                try
                {
                    words.Add(values[0]);
                }
                catch (ArgumentException) { }
            }
        }

        return words;
    }

    public Word[] ChooseWordsRandomly(List<Word> words, int numberOfWords)
    {
        Word[] chosenWords = new Word[numberOfWords];
        HashSet<int> chosenIndices = new HashSet<int>();
        int currentRandomIndex;
        int i = 0;
        while (chosenIndices.Count < numberOfWords)
        {
            currentRandomIndex = random.Next(words.Count);
            if (chosenIndices.Add(currentRandomIndex) == true)
            {
                chosenWords[i] = words[currentRandomIndex];
                i++;
            }
        }

        return chosenWords;
    }

    public void FillWordsWithWrongTranslations(Word[] words, List<string> englishWords)
    {
        int currentRandomIndex;
        foreach (Word word in words)
        {
            HashSet<int> chosenIndices = new HashSet<int>();
            string[] wrongTranslations = new string[wrongTranslationsPerQuestion];
            int i = 0;
            while (chosenIndices.Count < 3)
            {
                currentRandomIndex = random.Next(words.Length);
                if (chosenIndices.Add(currentRandomIndex) == true)
                {
                    wrongTranslations[i] = englishWords[currentRandomIndex];
                    i++;
                }
            }
            word.SetWrongTranslations(wrongTranslations);
        }
    }

    public void SetDeafultValuesForAnswer()
    {
        WaitForUserToAnswer = false;
        UserAnswerChoiseIndex = ChoiseOption.Empty;
    }

    public void UserAnswerQuestion(ChoiseOption answer)
    {
        if (boardState == BoardState.Exam)
        {
            UserAnswerChoiseIndex = answer;
            WaitForUserToAnswer = true;
        }
    }

    public void OnOptionAButtonPressed()
    {
        UserAnswerQuestion(ChoiseOption.A);
    }

    public void OnOptionBButtonPressed()
    {
        UserAnswerQuestion(ChoiseOption.B);
    }

    public void OnOptionCButtonPressed()
    {
        UserAnswerQuestion(ChoiseOption.C);
    }

    public void OnOptionDButtonPressed()
    {
        UserAnswerQuestion(ChoiseOption.D);
    }

    void ChangeBoardStatus(BoardState newState)
    {
        boardState = newState;
    }

    private IEnumerator RunLesson()
    {
        DisplayTextOnBoard(string.Format("Lesson has started,\n be prepared..."));
        yield return new WaitForSeconds(secondsToWait);

        for (int i = 0; i < GetCurrentLesson().words.Length; i++)
        {
            DisplayWordOnBoard(GetCurrentLesson().words[i]);
            GetCurrentLesson().words[i].WordDist.InitDistraction();
            yield return new WaitForSeconds(secondsToWait);
        }


        LessonBoardText.SetText(string.Format("Lesson has ended.\nPress 'Start' to begin the exam."));
        ChangeBoardStatus(BoardState.LessonEnded);
    }

    private IEnumerator RunExam()
    {
        DisplayTextOnBoard(string.Format("Exam has started,\n be prepared..."));
        yield return new WaitForSeconds(secondsToWait);

        foreach (Question q in GetCurrentExam().questions)
        {
            SetDeafultValuesForAnswer();
            DisplayQuestionOnBoard(q);
            yield return new WaitUntil(() => WaitForUserToAnswer);
            HandleUserAnswer(q);
            Debug.Log(string.Format($"Question is {q.IsAnswerCorrect()}"));
        }

        if (currentSessionIndex == sessions - 1)
        {
            ChangeBoardStatus(BoardState.End);
            DisplayTextOnBoard("Bye.");
        }

        else
        {
            ProgressToNextSession();
            ChangeBoardStatus(BoardState.ExamEnded);
            DisplayTextOnBoard("Exam ended.\nPress 'Start' for a new Lesson.");
        }
    }

    public void HandleUserAnswer(Question q)
    {
        q.UserAnswerQuestion(UserAnswerChoiseIndex);
        q.CheckUserAnswer();
    }

    public Lesson GetCurrentLesson()
    {
        return lessons[currentSessionIndex];
    }

    public Exam GetCurrentExam()
    {
        return exams[currentSessionIndex];
    }

    public void ProgressToNextSession()
    {
        currentSessionIndex++;
    }

    public void DisplayWordOnBoard(Word w)
    {
        LessonBoardText.SetText(string.Format($"{w.ForiegnWord} = {w.EnglishTranslation}"));
        ExamQuestionBoardText.SetText(string.Empty);
        OptionAText.SetText(string.Empty);
        OptionBText.SetText(string.Empty);
        OptionCText.SetText(string.Empty);
        OptionDText.SetText(string.Empty);
    }

    public void DisplayQuestionOnBoard(Question q)
    {
        LessonBoardText.SetText(string.Empty);
        ExamQuestionBoardText.SetText(string.Format($"'{q.word.ForiegnWord}' is...?"));
        OptionAText.SetText(string.Format($"A. {q.options[0]}"));
        OptionBText.SetText(string.Format($"B. {q.options[1]}"));
        OptionCText.SetText(string.Format($"C. {q.options[2]}"));
        OptionDText.SetText(string.Format($"D. {q.options[3]}"));
    }

    public void DisplayTextOnBoard(string s)
    {
        LessonBoardText.SetText(string.Format($"{s}"));
        ExamQuestionBoardText.SetText("");
        OptionAText.SetText(string.Empty);
        OptionBText.SetText(string.Empty);
        OptionCText.SetText(string.Empty);
        OptionDText.SetText(string.Empty);
    }
}
