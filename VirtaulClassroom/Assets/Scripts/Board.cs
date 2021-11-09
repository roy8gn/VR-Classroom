using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    public BoardState boardState = BoardState.Start;
    public TextMeshPro text;
    public Lesson lesson = new Lesson();
    public Lesson[] lessons = new Lesson[3];
    public int secondsToWait = 3;
    
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        text.text = "Welcome!\nPress Start to begin";
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnStartButtonPressed()
    {
        if (boardState == BoardState.Start)
        {
            ChangeBoardStatus(BoardState.Lesson);
            StartNewLesson();
        }
        else
        {
            if (boardState == BoardState.LessonEnded)
            {
                ChangeBoardStatus(BoardState.Exam);

            }
        }
    }
    void ChangeBoardStatus(BoardState newState)
    {
        boardState = newState;
    }

    void StartNewLesson()
    {
        SetBoardText("Lesson has started,\n be prepared...");
        StartCoroutine(RunLesson());
    }

    private IEnumerator RunLesson()
    {
        for(int i=0; i<lesson.words.Length; i++)
        {
            yield return new WaitForSeconds(secondsToWait);
            DisplayWordOnBoard(this.lesson.words[i]);
        }

        yield return new WaitForSeconds(secondsToWait);
        SetBoardText("Lesson has ended.\nPress Start to begin the exam.");
        ChangeBoardStatus(BoardState.LessonEnded);
    }
    public void DisplayWordOnBoard(Word w)
    {
        text = GetComponentInChildren<TextMeshPro>();
        text.text = string.Format($"{w.ForiegnWord} = {w.EnglishTranslation}");
    }

    public void SetBoardText(string boardText)
    {
        text = GetComponentInChildren<TextMeshPro>();
        text.text = boardText;
    }
}
public class Word
{
    public string ForiegnWord { get; set; }
    public string EnglishTranslation { get; set; }
    public string WrongTranslation1 { get; set; }
    public string WrongTranslation2 { get; set; }
    public string WrongTranslation3 { get; set; }

    public Word(string foriegnWord, string englishTranslation)
    {
        ForiegnWord = foriegnWord;
        EnglishTranslation = englishTranslation;
    }
}

public class Lesson
{
    public Word[] words;
    public Lesson()
    {
        words = new Word[7];
        words[0] = new Word("Kelev", "Dog");
        words[1] = new Word("Hatul", "Cat");
        words[2] = new Word("Anglit", "English");
        words[3] = new Word("Halav", "Milk");
        words[4] = new Word("Tapuz", "Orange");
        words[5] = new Word("Adom", "Red");
        words[6] = new Word("Mayim", "Water");
    }
}