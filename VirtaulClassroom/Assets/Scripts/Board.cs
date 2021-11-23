using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using Random = System.Random;

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

    private BoardState boardState = BoardState.Start;
    private int sessions = 3;
    private int currentSessionIndex = 0;
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
                lessons = new Lesson[sessions];
                exams = new Exam[sessions];
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
        lessons[currentSessionIndex] = new Lesson();
        
        for (int i=0; i< GetCurrentLesson().words.Length; i++)
        {
            yield return new WaitForSeconds(secondsToWait);
            DisplayWordOnBoard(GetCurrentLesson().words[i]);
        }

        yield return new WaitForSeconds(secondsToWait);
        LessonBoardText.SetText(string.Format("Lesson has ended.\nPress 'Start' to begin the exam."));
        ChangeBoardStatus(BoardState.LessonEnded);
    }

    private IEnumerator RunExam()
    {
        exams[currentSessionIndex] = new Exam(GetCurrentLesson().words, random); // there is a problem here
        //SetDeafultValuesForAnswer();

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


public class Word
{
    public string ForiegnWord { get; set; }
    public string EnglishTranslation { get; set; }
    public string[] WrongTranslations;

    public Word(string foriegnWord, string englishTranslation, string wt1, string wt2, string wt3)
    {
        ForiegnWord = foriegnWord;
        EnglishTranslation = englishTranslation;
        WrongTranslations = new string[3];
        WrongTranslations[0] = wt1;
        WrongTranslations[1] = wt2;
        WrongTranslations[2] = wt3;
    }
}


public class Lesson
{
    public Word[] words;
    public Lesson()
    {
        words = new Word[7];
        words[0] = new Word("Perro", "Dog", "Cat", "Pig", "Crocodile");
        words[1] = new Word("Gato", "Cat", "Avocado", "Banana", "Camel");
        words[2] = new Word("Inglés", "English", "Hebrow", "Russian", "German");
        words[3] = new Word("Leche", "Milk", "Liquer", "Leaf", "Coca-Cola");
        words[4] = new Word("Naranja", "Orange", "Narnia", "Jump", "Case");
        words[5] = new Word("Rojo", "Red", "Orange", "Rooster", "Rain");
        words[6] = new Word("Agua", "Water", "Door", "Milk", "Chair");
    }
}

public class Exam
{
    public Question[] questions;
    public int score;
    public Exam(Word[] w, Random rand)
    {

        questions = new Question[w.Length];
        for(int i=0; i<w.Length; i++)
        {
            questions[i] = new Question(w[i], rand);
        }
        
        score = 0;
    }
}

public class Question
{
    public Word word;
    private ChoiseOption UserAnswer; // User's answer
    private ChoiseOption CorrectAnswerIndex; // The correct answer
    private bool isCorrectAnswer;
    public string[] options; // For the display of the question on the board
    private Random random;

    public Question(Word w, Random rand)
    {
        this.word = w;
        options = new string[4];
        random = rand;
        generateQuestionOptions();
    }

    public void generateQuestionOptions()
    {
        CorrectAnswerIndex = (ChoiseOption) random.Next(0, 4);
        Debug.Log(CorrectAnswerIndex);
        options[(int)CorrectAnswerIndex] = String.Copy(word.EnglishTranslation);
        int i = 0, j = 0;
        while(i<options.Length)
        {
            if (i != (int)CorrectAnswerIndex)
            {
                options[i] = word.WrongTranslations[j];
                i++;
                j++;
            }
            else
                i++;
        }
    }

    public void CheckUserAnswer()
    {
        if (UserAnswer == CorrectAnswerIndex)
            isCorrectAnswer = true;
        else
            isCorrectAnswer = false;
    }

    public void UserAnswerQuestion(ChoiseOption answer)
    {
        UserAnswer = answer;
    }

    public bool IsAnswerCorrect()
    {
        return isCorrectAnswer;
    }

}
