using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Question
{
    public Word word;
    public ChoiceOption UserAnswer { get; set; } // User's answer
    private ChoiceOption CorrectAnswerIndex; // The correct answer
    public bool IsAnswerCorrect { get; set; }
    public string[] options; // For the display of the question on the board
    private Random random;

    public Question(Word w, Random rand)
    {
        this.word = w;
        options = new string[4];
        random = rand;
        generateQuestionOptions();
    }

    public void generateQuestionOptions()
    {
        CorrectAnswerIndex = (ChoiceOption)random.Next(0, 4);
        //Debug.Log(CorrectAnswerIndex);
        options[(int)CorrectAnswerIndex] = string.Copy(word.EnglishTranslation);
        int i = 0, j = 0;
        while (i < options.Length)
        {
            if (i != (int)CorrectAnswerIndex)
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
        if (UserAnswer == CorrectAnswerIndex)
            IsAnswerCorrect = true;
        else
            IsAnswerCorrect = false;
    }

    public void UserAnswerQuestion(ChoiceOption answer)
    {
        UserAnswer = answer;
    }
}
