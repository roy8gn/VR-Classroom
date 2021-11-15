using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
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
    public TextMeshPro boardText;

    public BoardState boardState = BoardState.Start;
    int sessions = 3;
    int currentSessionIndex = 0;
    Lesson[] lessons;
    Exam[] exams;

    Lesson currentLesson;
    Exam currentExam;
    
    public bool waitingForUserAnswer = false;

    public int secondsToWait = 3;

    // Start is called before the first frame update
    void Start()
    {
        boardText = GetComponentInChildren<TextMeshPro>();
        boardText.text = "Welcome!\nPress Start to begin";
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnStartButtonPressed()
    {
        switch (boardState)
        {
            case BoardState.Start:
                boardText = GetComponentInChildren<TextMeshPro>();
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

    }

    public void OnOptionBButtonPressed()
    {

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
        SetBoardText("Lesson has started,\n be prepared...");
        lessons[currentSessionIndex] = new Lesson();
        currentLesson = lessons[currentSessionIndex];
        StartCoroutine(RunLesson()); // Run a lesson
    }

    public void StartNewExam()
    {
        SetBoardText("Exam has started,\n be prepared...");
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
        SetBoardText("Lesson has ended.\nPress Start to begin the exam.");
        ChangeBoardStatus(BoardState.LessonEnded);
    }
    public void DisplayWordOnBoard(Word w)
    {
        boardText = GetComponentInChildren<TextMeshPro>();
        boardText.text = string.Format($"{w.ForiegnWord} = {w.EnglishTranslation}");
    }

    public void SetBoardText(string bt)
    {
        boardText = GetComponentInChildren<TextMeshPro>();
        boardText.text = bt;
    }

    public void DisplayQuestionOnBoard(Question q)
    {
        boardText = GetComponentInChildren<TextMeshPro>();

        boardText.SetText(string.Format($"{q.word.ForiegnWord} is...\nA.{q.options[0]}  B.{q.options[1]}" +
            $"\nC.{q.options[2]}   D.{q.options[3]}"));
    }
}
public class Word
{
    public string ForiegnWord { get; set; }
    public string EnglishTranslation { get; set; }
    public string []WrongTranslations;

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
        words[0] = new Word("Kelev", "Dog", "Cat", "Pig", "Cow");
        words[1] = new Word("Hatul", "Cat", "a", "b", "c");
        words[2] = new Word("Anglit", "English", "a", "b", "c");
        words[3] = new Word("Halav", "Milk", "a", "b", "c");
        words[4] = new Word("Tapuz", "Orange", "a", "b", "c");
        words[5] = new Word("Adom", "Red", "a", "b", "c");
        words[6] = new Word("Mayim", "Water", "a", "b", "c");
    }
}

public class Exam
{
    public Question[] questions;

    public int score;
    public Exam(Word[] w)
    {
        questions = new Question[w.Length];
        questions[0] = new Question(w[0]);
        score = 0;
    }
}

public class Question
{
    public Word word;
    int? userAnswerIndex; // The index of user's answer
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
        System.Random random = new System.Random();
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

 
}
