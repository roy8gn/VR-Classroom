using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Exam
{
    public Question[] questions;

    public double Score { get; set; }
    
    public Exam(Word[] w, Random rand)
    {

        questions = new Question[w.Length];
        for (int i = 0; i < w.Length; i++)
        {
            questions[i] = new Question(w[i], rand);
        }

        Score = 0;
    }

    public void CalculateScore()
    {
        foreach(Question q in questions)
        {
            Debug.Log("Question is " + q.IsAnswerCorrect);
            if (q.IsAnswerCorrect == true)
            {
                Score = Score + (100 / questions.Length);
            }
        }
    }
}
