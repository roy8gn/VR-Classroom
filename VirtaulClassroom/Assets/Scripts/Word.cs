using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Word
{
    public string ForiegnWord { get; set; }
    public string EnglishTranslation { get; set; }
    public string[] WrongTranslations;
    public Distraction WordDistraction { get; set; }
    public bool HeadOutOfRange { get; set; } // if true - user got distracted

    public Word(string foriegnWord, string englishTranslation, string[] wt)
    {
        ForiegnWord = foriegnWord;
        EnglishTranslation = englishTranslation;
        WrongTranslations = new string[wt.Length];
        for(int i= 0; i < wt.Length; i++)
        {
            WrongTranslations[i] = wt[i];
        }
    }

    public Word(Word w)
    {
        ForiegnWord = w.ForiegnWord;
        EnglishTranslation = w.EnglishTranslation;
        WrongTranslations = new string[w.WrongTranslations.Length];
        for (int i = 0; i < w.WrongTranslations.Length; i++)
        {
            WrongTranslations[i] = w.WrongTranslations[i];
        }
        WordDistraction = w.WordDistraction;
    }

    public Word(string fw, string ew)
    {
        ForiegnWord = fw;
        EnglishTranslation = ew;
    }
    
    public void SetWrongTranslations(string[] wt)
    {
        WrongTranslations = new string[wt.Length];
        for (int i = 0; i < wt.Length; i++)
        {
            WrongTranslations[i] = wt[i];
        }
    }
}