using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardScript : MonoBehaviour
{
    public TextMeshPro text;
    public Lesson lesson;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshPro>();
        lesson = new Lesson();
        text.text = lesson.words[0].ForiegnWord + " " + lesson.words[0].EnglishTranslation;
    }

    // Update is called once per frame
    void Update()
    {
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