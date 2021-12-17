using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson
{
    public Word[] words;
    public LessonType lessonType;
    public Lesson(Word[] ws, LessonType lt)
    {
        words = new Word[ws.Length];
        for (int i = 0; i < ws.Length; i++)
        {
            words[i] = new Word(ws[i]);
        }
        lessonType = lt;
    }
}
