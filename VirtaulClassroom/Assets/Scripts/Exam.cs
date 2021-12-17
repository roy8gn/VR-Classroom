using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Exam
{
    public Question[] questions;
    public int score;
    public Exam(Word[] w, Random rand)
    {

        questions = new Question[w.Length];
        for (int i = 0; i < w.Length; i++)
        {
            questions[i] = new Question(w[i], rand);
        }

        score = 0;
    }
}
