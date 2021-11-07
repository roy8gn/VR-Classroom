using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum BoardState
{
    Start,
    Lesson,
    Exam,
    End
}

public class Board : MonoBehaviour
{
    public BoardState boardState = BoardState.Start;
    public TextMeshPro text;
    public Lesson lesson = new Lesson();
    
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
    }

    void ChangeBoardStatus(BoardState newState)
    {
        boardState = newState;
    }

    void StartNewLesson()
    {
        SetBoardText("Lesson has started,\n be prepared...");
        StartCoroutine(WaitBeforeShow());
    }

    private IEnumerator WaitBeforeShow()
    {
        for(int i=0; i<lesson.words.Length; i++)
        {
            yield return new WaitForSeconds(2);
            SetBoardText(this.lesson.words[i].EnglishTranslation);
        }
        
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