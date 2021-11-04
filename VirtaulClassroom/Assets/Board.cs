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

//public class BoardReactToButton : Board
//{
//    [SerializeField] private VrClassButton vrClassButton;

//    public void Start()
//    {
//        switch (vrClassButton)
//        {
//            case StartButton sb:
//                sb.StartButtonPressed += base.OnButtonStartPressed();
//                break;
//            default:
//                break;
//        }
//    }
//}

public class Board : MonoBehaviour
{
    public BoardState boardState;
    public TextMeshPro text;
    public Lesson lesson;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        lesson = new Lesson();
        text.text = "Welcome!\nPress Start to begin";
        boardState = BoardState.Start;
    }

    // Update is called once per frame
    void Update()
    {
    }

    protected void OnButtonStartPressed()
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
        text.text = lesson.words[0].EnglishTranslation;
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
        words = new Word[2];
        words[0] = new Word("Kelev", "Dog");
        words[1] = new Word("Hatul", "Cat");
    }
}