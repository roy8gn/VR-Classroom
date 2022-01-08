using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using Random = System.Random;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

public class Board : MonoBehaviour
{
    // Text meshes
    public TextMeshPro LessonBoardText;
    public TextMeshPro ExamQuestionBoardText;
    public TextMeshPro OptionAText;
    public TextMeshPro OptionBText;
    public TextMeshPro OptionCText;
    public TextMeshPro OptionDText;

    // Audio Sources in the scene - additional ones need to be added here
    public AudioSource DogBark;
    public AudioSource PhoneNoise;
    public AudioSource Bus;
    public AudioSource PaperFold;
    public AudioSource PenClick;
    public AudioSource Ambulance;
    public AudioSource FireWorks;
    public AudioSource Tractor;
    public AudioSource Hammering;
    public AudioSource PhoneTyping;
    
    public Transform HeadTracker;
    public bool HeadOutOfRange = false;

    // Application properties
    private BoardState boardState = BoardState.Start;
    private int sessions = 3;
    private int wordsPerSession = 10;
    private int currentSessionIndex = 0;
    private int distractionTypes = 3; // Visual, Auditory, None
    private int wrongTranslationsPerQuestion = 3;
    private int secondsToWait = 2;
    private Lesson[] lessons;
    private Exam[] exams;
    private Random random;
    VisualDistraction[] visualDistractions;
    AudioDistraction[] audioDistractions;

    private ChoiceOption UserAnswerChoiceIndex { get; set; }
    private bool WaitForUserToAnswer { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        LessonBoardText = GetComponentInChildren<TextMeshPro>();
        LessonBoardText.SetText("Welcome!\nPress 'Start' to begin.");
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (!(HeadTracker.rotation.y > -0.35 && HeadTracker.rotation.y < 0.35 && HeadTracker.rotation.x > -0.2 && HeadTracker.rotation.x < 0.2)) // Check every frame if user loses attention of board
            {
                HeadOutOfRange = true;
            }
        }
        catch(Exception) { }
    }

    public void OnDestroy()
    {
    }

    public void OnStartButtonPressed()
    {
        switch (boardState)
        {
            case BoardState.Start:
                random = new Random();
                LoadWordsFromDataSets();
                ChangeBoardStatus(BoardState.Lesson);
                StartCoroutine(RunLesson());
                break;

            case BoardState.LessonEnded:
                ChangeBoardStatus(BoardState.Exam);
                StartCoroutine(RunExam());
                break;

            case BoardState.ExamEnded:
                ChangeBoardStatus(BoardState.Lesson);
                StartCoroutine(RunLesson());
                break;

            case BoardState.End:
                AnalyzeResults();
                break;

            default:
                break;
        }
    }

    public void LoadWordsFromDataSets()
    {
        List<Word> wordsDataSet = LoadForeignWordsFromCsvFile(); // Load foreign words and their translations
        List<string> englishWordsDataSet = LoadEnglishWordsFromCsvFile(); // Load random incorrect translations

        int totalnumberOfWords = sessions * wordsPerSession;
        Word[] chosenWords = ChooseWordsRandomly(wordsDataSet, totalnumberOfWords);
        FillWordsWithWrongTranslations(chosenWords, englishWordsDataSet);

        lessons = new Lesson[sessions];
        exams = new Exam[sessions];

        InitSessions(lessons, exams, chosenWords);
    }

    public void InitSessions(Lesson[] ls, Exam[] es, Word[] ws)
    {
        Dictionary<int, DistractionTypeForLesson> dict = RandomOrderOfDistractions();
        for (int i = 0; i < sessions; i++)
        {
            Word[] wordsForSession = new Word[wordsPerSession];
            for (int j = 0; j < wordsPerSession; j++)
            {
                wordsForSession[j] = ws[i * wordsPerSession + j];
            }
            DistractionTypeForLesson distractionForLesson = dict[i % distractionTypes];
            FillWordsWithDistractions(wordsForSession, distractionForLesson);
            ls[i] = new Lesson(wordsForSession, LessonType.Visual, distractionForLesson);
            es[i] = new Exam(wordsForSession, random);
        }
    }

    
    public Dictionary<int, DistractionTypeForLesson> RandomOrderOfDistractions()
    {
        Dictionary<int, DistractionTypeForLesson> dict = new Dictionary<int, DistractionTypeForLesson>();
        HashSet<int> randomIndecies = new HashSet<int>();
        foreach (DistractionTypeForLesson dt in Enum.GetValues(typeof(DistractionTypeForLesson)))
        {
            int randomIndex = random.Next(distractionTypes);
            while (randomIndecies.Add(randomIndex) == false) {
                randomIndex = random.Next(distractionTypes);
            }
            dict.Add(randomIndex, dt);
        }

        return dict;
    }

    public List<Word> LoadForeignWordsFromCsvFile()
    {
        List<Word> words = new List<Word>();
        TextAsset file = Resources.Load("ForeignWords") as TextAsset; // Note - when loading from Resources folder, no specifying extension
        string content = file.ToString();
        var lines = content.Split('\n');
        foreach (string line in lines)
        {
            try
            {
                var values = line.Split(',');
                words.Add(new Word(values[0], values[1]));
            }
            catch (Exception) { break; }
        }

        return words;
    }

    public List<string> LoadEnglishWordsFromCsvFile()
    {
        List<string> words = new List<string>();
        TextAsset file = Resources.Load("EnglishWords") as TextAsset; // Note - when loading from Resources folder, no specifying extension
        string content = file.ToString();
        var lines = content.Split('\n');
        foreach (string line in lines)
        {
            try
            {
                words.Add(line);
            }
            catch (Exception) { break; }
        }

        return words;
    }

    public Word[] ChooseWordsRandomly(List<Word> words, int numberOfWords)
    {
        Word[] chosenWords = new Word[numberOfWords];
        HashSet<int> chosenIndices = new HashSet<int>();
        int currentRandomIndex;
        int i = 0;
        while (chosenIndices.Count < numberOfWords)
        {
            currentRandomIndex = random.Next(words.Count);
            if (chosenIndices.Add(currentRandomIndex) == true)
            {
                chosenWords[i] = words[currentRandomIndex];
                i++;
            }
        }

        return chosenWords;
    }

    public void FillWordsWithWrongTranslations(Word[] words, List<string> englishWords)
    {
        foreach (Word word in words)
        {
            HashSet<int> chosenIndices = new HashSet<int>();
            string[] wrongTranslations = new string[wrongTranslationsPerQuestion];
            int i = 0;
            while (chosenIndices.Count < 3)
            {
                int currentRandomIndex = random.Next(words.Length);
                if (chosenIndices.Add(currentRandomIndex) == true)
                {
                    wrongTranslations[i] = englishWords[currentRandomIndex];
                    i++;
                }
            }
            word.SetWrongTranslations(wrongTranslations);
        }
    }

    public void FillWordsWithDistractions(Word[] words, DistractionTypeForLesson dt) // Assign distractions to Word objects
    {
        switch (dt)
        {
            case DistractionTypeForLesson.Auditory:
                FillWordsWithAudioDistractions(words);
                break;
            case DistractionTypeForLesson.Visual:
                FillWordsWithVisualDistractions(words);
                break;
            case DistractionTypeForLesson.None:
                FillWordsWithNoDistractions(words);
                break;
            default:
                break;

        }
    }

    public void FillWordsWithNoDistractions(Word[] words)
    {
        NoDistraction nd = new NoDistraction();
        foreach(Word w in words)
        {
            w.WordDistraction = nd;
        }
    }


    public void FillWordsWithAudioDistractions(Word[] words)
    {
        audioDistractions = new AudioDistraction[10];
        audioDistractions[0] = new AudioDistraction(DogBark);
        audioDistractions[1] = new AudioDistraction(PhoneNoise);
        audioDistractions[2] = new AudioDistraction(PenClick);
        audioDistractions[3] = new AudioDistraction(Ambulance);
        audioDistractions[4] = new AudioDistraction(PaperFold);
        audioDistractions[5] = new AudioDistraction(Bus);
        audioDistractions[6] = new AudioDistraction(FireWorks);
        audioDistractions[7] = new AudioDistraction(Tractor);
        audioDistractions[8] = new AudioDistraction(Hammering);
        audioDistractions[9] = new AudioDistraction(PhoneTyping);
        // Add new AudioSources here for more distractions

        for (int i = 0; i < words.Length; i++)
        {
            int currentRandomIndex = random.Next(audioDistractions.Length);
            words[i].WordDistraction = audioDistractions[currentRandomIndex];
        }
    }

    public void FillWordsWithVisualDistractions(Word[] words)
    {
        visualDistractions = new VisualDistraction[7];
        
        // Creating primitive objects to use as visual distractions
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.AddComponent<Rigidbody>();
        sphere.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule.AddComponent<Rigidbody>();
        capsule.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.AddComponent<Rigidbody>();
        cylinder.GetComponent<Renderer>().material.SetColor("_Color", Color.green);

        GameObject square1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        square1.AddComponent<Rigidbody>();
        square1.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);

        GameObject square2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        square2.AddComponent<Rigidbody>();
        square2.GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);

        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.AddComponent<Rigidbody>();
        ball.GetComponent<Renderer>().material.SetColor("_Color", Color.black);

        // Three vector parameters represent starting position in world, size of the object , and direction of force applied to them
        visualDistractions[0] = new VisualDistraction(sphere, new Vector3(27, 15, 10), new Vector3(5, 5, 5), new Vector3(-20, 0, 5));
        visualDistractions[1] = new VisualDistraction(capsule, new Vector3(-27, 15, 10), new Vector3(2, 2, 2), new Vector3(20, 0, 5));
        visualDistractions[2] = new VisualDistraction(cylinder, new Vector3(-20, 15, 20), new Vector3(1, 1, 1), new Vector3(20, 0, -10));
        visualDistractions[3] = new VisualDistraction(square1, new Vector3(20, 15, 10), new Vector3(4, 1, 1), new Vector3(-25, 0, 5));
        visualDistractions[4] = new VisualDistraction(square2, new Vector3(25, 2, 25), new Vector3(1, 2, 4), new Vector3(-30, 0, 0));
        visualDistractions[5] = new VisualDistraction(ball, new Vector3(40, 1, 40), new Vector3(2, 2, 2), new Vector3(-50, 0, 0));
        visualDistractions[6] = new VisualDistraction(ball, new Vector3(-40, 1, 40), new Vector3(2, 2, 2), new Vector3(50, 0, 0));

        for (int i = 0; i < words.Length; i++)
        {
            int currentRandomIndex = random.Next(visualDistractions.Length);
            words[i].WordDistraction = visualDistractions[currentRandomIndex];
        }
    }

    public void SetDeafultValuesForAnswer()
    {
        WaitForUserToAnswer = false;
        UserAnswerChoiceIndex = ChoiceOption.Empty;
    }

    public void UserAnswerQuestion(ChoiceOption answer)
    {
        if (boardState == BoardState.Exam)
        {
            UserAnswerChoiceIndex = answer;
            WaitForUserToAnswer = true;
        }
    }

    public void OnOptionAButtonPressed()
    {
        if(boardState==BoardState.Exam)
            UserAnswerQuestion(ChoiceOption.A);
    }

    public void OnOptionBButtonPressed()
    {
        if (boardState == BoardState.Exam)
            UserAnswerQuestion(ChoiceOption.B);
    }

    public void OnOptionCButtonPressed()
    {
        if (boardState == BoardState.Exam)
            UserAnswerQuestion(ChoiceOption.C);
    }

    public void OnOptionDButtonPressed()
    {
        if (boardState == BoardState.Exam)
            UserAnswerQuestion(ChoiceOption.D);
    }

    void ChangeBoardStatus(BoardState newState)
    {
        boardState = newState;
    }

    private IEnumerator RunLesson()
    {
        DisplayTextOnBoard(string.Format("Lesson has started,\n be prepared..."));
        yield return new WaitForSeconds(secondsToWait);

        for (int i = 0; i < GetCurrentLesson().words.Length; i++)
        {
            HeadOutOfRange = false;
            DisplayWordOnBoard(GetCurrentLesson().words[i]);

            GetCurrentLesson().words[i].WordDistraction.StartDistraction();
            yield return new WaitForSeconds(secondsToWait);
            GetCurrentLesson().words[i].HeadOutOfRange = HeadOutOfRange;
            GetCurrentLesson().words[i].WordDistraction.StopDistraction();
            //Debug.Log(GetCurrentLesson().words[i].ForiegnWord + ", Out of range: " + GetCurrentLesson().words[i].HeadOutOfRange);
        }

        LessonBoardText.SetText(string.Format("Lesson has ended.\nPress 'Start' to begin the exam."));
        ChangeBoardStatus(BoardState.LessonEnded);
    }

    private IEnumerator RunExam()
    {
        DisplayTextOnBoard(string.Format("Exam has started,\n be prepared..."));
        yield return new WaitForSeconds(secondsToWait);

        foreach (Question q in GetCurrentExam().questions)
        {
            SetDeafultValuesForAnswer();
            DisplayQuestionOnBoard(q);
            yield return new WaitUntil(() => WaitForUserToAnswer);
            HandleUserAnswer(q);
        }

        GetCurrentExam().CalculateScore();

        if (currentSessionIndex == sessions - 1)
        {
            ChangeBoardStatus(BoardState.End);
            DisplayTextOnBoard("Press 'Start' results.");
        }

        else
        {
            ProgressToNextSession();
            ChangeBoardStatus(BoardState.ExamEnded);
            DisplayTextOnBoard("Exam ended.\nPress 'Start' for a new Lesson.");
        }
    }

    public void AnalyzeResults()
    {
        string recommendationString; // String to be displayed in JSON report
        string[] boardString = new string[sessions]; // Strings to be displayed on board
        string finalBoardString; // Final string to display on board
        Recommendation userRecommendation = new Recommendation();
        for(int i=0;i<sessions;i++)
        {
            int HeadOutOfRangeCountInALesson = lessons[i].words.Where(x => x.HeadOutOfRange == true).Count(); // Count number of times user moved head away from the board
   
            if (exams[i].Score < 55 && HeadOutOfRangeCountInALesson >= wordsPerSession / 2) // First scenario - user distracted by type of distraction, failed exam
            {
                switch (lessons[i].DistractionType)
                {
                    case DistractionTypeForLesson.Visual:
                        boardString[i]= "Visual distractions and stimulations are particularly bothersome for you.";
                        recommendationString = "Visual distractions and stimulations are particularly bothersome for you. It is recommended that you keep your learning environment" +
                            " tidy, clean and remove various objects from the periphery of your vision while studying.";
                        userRecommendation.RecommendationDictionary.Add(DistractionTypeForLesson.Visual, recommendationString);
                        userRecommendation.ExamScores.Add(DistractionTypeForLesson.Visual, exams[i].Score);
                        break;

                    case DistractionTypeForLesson.Auditory:
                        boardString[i] = "Auditory distractions and stimulations are particularly bothersome for you.";
                        recommendationString = "Auditory distractions and stimulations are somewhat bothersome for you. It is recommended that you keep your learning environment " +
                            "silent. Close all doors and windows, turn off noisy electronic devices and ask people in the room to keep their voices down.";
                        userRecommendation.RecommendationDictionary.Add(DistractionTypeForLesson.Auditory, recommendationString);
                        userRecommendation.ExamScores.Add(DistractionTypeForLesson.Auditory, exams[i].Score);
                        break;

                    case DistractionTypeForLesson.None:
                        boardString[i] = "You are easily distracted even when no auditory or visual distractions are present.";
                        recommendationString = "You are easily distracted even when no auditory or visual distractions are present.";
                        userRecommendation.RecommendationDictionary.Add(DistractionTypeForLesson.None, recommendationString);
                        userRecommendation.ExamScores.Add(DistractionTypeForLesson.None, exams[i].Score);
                        break;

                    default: 
                        break;
                }
            }

            else
            {
                if (exams[i].Score >= 55 && HeadOutOfRangeCountInALesson >= wordsPerSession / 2) // Second scenario - user distracted, passed exam
                {
                    switch (lessons[i].DistractionType)
                    {
                        case DistractionTypeForLesson.Visual:
                            boardString[i] = "Visual distractions and stimulations are somewhat bothersome for you.";
                            recommendationString = "Visual distractions and stimulations are somewhat bothersome for you. It is recommended that you keep your learning environment " +
                            "tidy.";
                            userRecommendation.RecommendationDictionary.Add(DistractionTypeForLesson.Visual, recommendationString);
                            userRecommendation.ExamScores.Add(DistractionTypeForLesson.Visual, exams[i].Score);
                            break;

                        case DistractionTypeForLesson.Auditory:
                            boardString[i] = "Auditory distractions and stimulations are somewhat bothersome for you.";
                            recommendationString = "Auditory distractions and stimulations are somewhat bothersome for you. It is recommended that you avoid extreme noise in your learning" +
                            " environment.";
                            userRecommendation.RecommendationDictionary.Add(DistractionTypeForLesson.Auditory, recommendationString);
                            userRecommendation.ExamScores.Add(DistractionTypeForLesson.Auditory, exams[i].Score);
                            break;

                        case DistractionTypeForLesson.None:
                            boardString[i] = "You are easily distracted even when no auditory or visual distractions are present.";
                            recommendationString = "You are easily distracted even when no auditory or visual distractions are present.";
                            userRecommendation.RecommendationDictionary.Add(DistractionTypeForLesson.None, recommendationString);
                            userRecommendation.ExamScores.Add(DistractionTypeForLesson.None, exams[i].Score);
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    switch (lessons[i].DistractionType) // Other scenarios
                    {
                        case DistractionTypeForLesson.Visual:
                            boardString[i] = "Visual distractions and stimulations do not appear to bother you.";
                            recommendationString = "Visual distractions and stimulations do not appear to bother you.";
                            userRecommendation.RecommendationDictionary.Add(DistractionTypeForLesson.Visual, recommendationString);
                            userRecommendation.ExamScores.Add(DistractionTypeForLesson.Visual, exams[i].Score);
                            break;

                        case DistractionTypeForLesson.Auditory:
                            boardString[i] = "Auditory distractions and stimulations do not appear to bother you.";
                            recommendationString = "Auditory distractions and stimulations do not appear to bother you.";
                            userRecommendation.RecommendationDictionary.Add(DistractionTypeForLesson.Auditory, recommendationString);
                            userRecommendation.ExamScores.Add(DistractionTypeForLesson.Auditory, exams[i].Score);
                            break;

                        case DistractionTypeForLesson.None:
                            boardString[i] = "Total silence does not appear to bother you.";
                            recommendationString = "Total silence does not appear to bother you.";
                            userRecommendation.RecommendationDictionary.Add(DistractionTypeForLesson.None, recommendationString);
                            userRecommendation.ExamScores.Add(DistractionTypeForLesson.None, exams[i].Score);
                            break;

                        default:
                            break;
                    }
                }
            }
        }
        finalBoardString = string.Join("\n", boardString); // Concat board strings
        File.WriteAllText(Application.persistentDataPath + "/RecommendationReport.json", JsonConvert.SerializeObject(userRecommendation, Formatting.Indented)); // Note - persistentDataPath is different for each platform. Can't use Windows paths for Android applications.
        DisplayReportOnBoard(string.Format("Final Recommendations: \n" + finalBoardString));
    }

    public void DisplayReportOnBoard(string s)
    {
        LessonBoardText.fontSize = 12;
        LessonBoardText.SetText(string.Format($"{s}"));
        ExamQuestionBoardText.SetText("");
        OptionAText.SetText(string.Empty);
        OptionBText.SetText(string.Empty);
        OptionCText.SetText(string.Empty);
        OptionDText.SetText(string.Empty);
    }

    public void HandleUserAnswer(Question q)
    {
        q.UserAnswerQuestion(UserAnswerChoiceIndex);
        q.CheckUserAnswer();
    }

    public Lesson GetCurrentLesson()
    {
        return lessons[currentSessionIndex];
    }

    public Exam GetCurrentExam()
    {
        return exams[currentSessionIndex];
    }

    public void ProgressToNextSession()
    {
        currentSessionIndex++;
    }

    public void DisplayWordOnBoard(Word w)
    {
        LessonBoardText.SetText(string.Format($"{w.ForiegnWord} = {w.EnglishTranslation}"));
        ExamQuestionBoardText.SetText(string.Empty);
        OptionAText.SetText(string.Empty);
        OptionBText.SetText(string.Empty);
        OptionCText.SetText(string.Empty);
        OptionDText.SetText(string.Empty);
    }

    public void DisplayQuestionOnBoard(Question q)
    {
        LessonBoardText.SetText(string.Empty);
        ExamQuestionBoardText.SetText(string.Format($"'{q.word.ForiegnWord}' is...?"));
        OptionAText.SetText(string.Format($"A. {q.options[0]}"));
        OptionBText.SetText(string.Format($"B. {q.options[1]}"));
        OptionCText.SetText(string.Format($"C. {q.options[2]}"));
        OptionDText.SetText(string.Format($"D. {q.options[3]}"));
    }

    public void DisplayTextOnBoard(string s)
    {
        LessonBoardText.SetText(string.Format($"{s}"));
        ExamQuestionBoardText.SetText("");
        OptionAText.SetText(string.Empty);
        OptionBText.SetText(string.Empty);
        OptionCText.SetText(string.Empty);
        OptionDText.SetText(string.Empty);
    }
}

// Enumerators - add to these if we want to add new types of distractions or lesson sessions
public enum LessonType
{
    Visual,
    Auditory
}

public enum DistractionTypeForLesson
{
    Visual,
    Auditory,
    None
}

public enum BoardState
{
    Start,
    Lesson,
    LessonEnded,
    Exam,
    ExamEnded,
    End
}

public enum ChoiceOption
{
    A = 0,
    B = 1,
    C = 2,
    D = 3,
    Empty = -1
}

