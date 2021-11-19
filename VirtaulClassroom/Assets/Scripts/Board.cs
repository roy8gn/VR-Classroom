using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using Random = System.Random;
using static OptionAButton;
using static StartButton;

public enum BoardState
{
    Start,
    Lesson,
    LessonEnded,
    Exam,
    ExamEnded,
    End
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

    private Lesson currentLesson;
    private Exam currentExam;

    private bool waitingForUserAnswer = false;

    private int secondsToWait = 3;

    // Start is called before the first frame update
    void Start()
    {
        //startButton.ButtonPressed += OnStartButtonPressed;
        //optionAButton.ButtonPressed += OnOptionAButtonPressed;
        //optionBButton.ButtonPressed += OnOptionBButtonPressed;
        //optionCButton.ButtonPressed += OnOptionCButtonPressed;
        //optionDButton.ButtonPressed += OnOptionDButtonPressed;

        LessonBoardText = GetComponentInChildren<TextMeshPro>();
        LessonBoardText.SetText("Welcome!\nPress Start to begin");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnDestroy()
    {
        //startButton.ButtonPressed -= OnStartButtonPressed;
        //optionAButton.ButtonPressed -= OnOptionAButtonPressed;
    }

    public void OnStartButtonPressed()
    {
        switch (boardState)
        {
            case BoardState.Start:
                ChangeBoardStatus(BoardState.Lesson);
                lessons = new Lesson[sessions];
                exams = new Exam[sessions];
                StartNewLesson();
                break;

            case BoardState.LessonEnded:
                ChangeBoardStatus(BoardState.Exam);
                StartNewExam();
                break;

            case BoardState.ExamEnded:
                ChangeBoardStatus(BoardState.Lesson);
                //StartNewLesson();
                break;

            default:
                break;
        }
    }

    public void OnOptionAButtonPressed()
    {
        //waitingForUserAnswer = false;
        //currentExam.questions[0].userAnswerIndex = 0;

        Debug.Log("A was pressed.");
    }

    public void OnOptionBButtonPressed()
    {
        Debug.Log("B was pressed.");
    }

    public void OnOptionCButtonPressed()
    {

    }

    public void OnOptionDButtonPressed()
    {

    }

    void ChangeBoardStatus(BoardState newState)
    {
        boardState = newState;
    }

    void StartNewLesson()
    {
        LessonBoardText.SetText(string.Format("Lesson has started,\n be prepared..."));
        lessons[currentSessionIndex] = new Lesson();
        currentLesson = lessons[currentSessionIndex];
        StartCoroutine(RunLesson()); // Run a lesson
    }

    public void StartNewExam()
    {
        LessonBoardText.SetText(string.Format("Exam has started,\n be prepared..."));
        exams[currentSessionIndex] = new Exam(lessons[currentSessionIndex].words);
        currentExam = exams[currentSessionIndex];
        BoardWaitForSeconds(secondsToWait);
        DisplayQuestionOnBoard(currentExam.questions[0]);
    }

    private IEnumerator BoardWaitForSeconds(int sec)
    {
        yield return new WaitForSeconds(sec);
    }

    private IEnumerator RunLesson()
    {
        for(int i=0; i<currentLesson.words.Length; i++)
        {
            yield return new WaitForSeconds(secondsToWait);
            DisplayWordOnBoard(currentLesson.words[i]);
        }

        yield return new WaitForSeconds(secondsToWait);
        LessonBoardText.SetText(string.Format("Lesson has ended.\nPress Start to begin the exam."));
        ChangeBoardStatus(BoardState.LessonEnded);
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
        waitingForUserAnswer = true;
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
        words[1] = new Word("Gato", "Cat", "a", "b", "c");
        words[2] = new Word("Inglés", "English", "a", "b", "c");
        words[3] = new Word("Leche", "Milk", "a", "b", "c");
        words[4] = new Word("Naranja", "Orange", "a", "b", "c");
        words[5] = new Word("Rojo", "Red", "Orange", "Rooster", "Rain");
        words[6] = new Word("Agua", "Water", "Door", "Milk", "Chair");
    }
}

public class Exam
{
    public Question[] questions;

    public int score;
    public Exam(Word[] w)
    {
        questions = new Question[w.Length];
        for(int i=0; i<w.Length; i++)
        {
            questions[i] = new Question(w[i]);
        }
        
        score = 0;
    }
}

public class Question
{
    public Word word;
    public int? userAnswerIndex; // The index of user's answer
    int correctAnswerIndex;
    bool? isCorrectAnswer;
    public string[] options;

    public Question(Word w)
    {
        this.word = w;
        options = new string[4];
        generateQuestionOptions();
    }

    public void generateQuestionOptions()
    {
        Random random = new Random();
        correctAnswerIndex = random.Next(0, 3);
        options[correctAnswerIndex] = String.Copy(word.EnglishTranslation);
        int i = 0, j = 0;
        while(i<options.Length)
        {
            if (i != correctAnswerIndex)
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
        if (userAnswerIndex != null)
        {
            if (userAnswerIndex == correctAnswerIndex)
                isCorrectAnswer = true;
            else
                isCorrectAnswer = false;
        }
        else
            isCorrectAnswer = false;
    }
 
}
