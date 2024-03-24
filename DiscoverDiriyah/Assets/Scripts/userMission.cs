using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Storage;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class userMission : MonoBehaviour
{
    public TMP_Text choice1;
    public TMP_Text choice2;
    public TMP_Text choice3;
    public TMP_Text message;
    public TMP_Text question;
    public TMP_Text title;
    public TMP_Text attemptsLeft;
    public Button choice1Button;
    public Button choice2Button;
    public Button choice3Button;
    private string correctAnswer;
    public Image checkmarkImage;
    public Image xmarkImage;
    public Image xmarkImage2;
    private int attemptsRemaining = 2;


    FirebaseFirestore db;
    private string missionId = "M09VqxzPfF9VuAs22kZj";

    void Start()
    {

        db = FirebaseFirestore.DefaultInstance;
        LoadMission();
        InitializeButtonListeners();
        UpdateAttemptsText();

    }

    private void UpdateAttemptsText()
    {
        attemptsLeft.text = "Attempts Left: " + attemptsRemaining.ToString();
    }

    private void InitializeButtonListeners()
    {
        // Add listeners to buttons
        choice1Button.onClick.AddListener(() => OnChoiceSelected(choice1.text, choice1Button));
        choice2Button.onClick.AddListener(() => OnChoiceSelected(choice2.text, choice2Button));
        choice3Button.onClick.AddListener(() => OnChoiceSelected(choice3.text, choice3Button));
    }

    

    private void OnChoiceSelected(string selectedAnswer, Button selectedButton)
    {
        // Return early if no attempts left. 
        if (attemptsRemaining <= 0) return;

        // Decrease attempts if choice has been made.
        attemptsRemaining--;
        UpdateAttemptsText(); 

        // Reset button colors to default before coloring the selected one.
        ResetButtonColors();

        
        selectedButton.image.color = new Color(0.247f, 0.212f, 0.188f);
        selectedButton.GetComponentInChildren<TMP_Text>().color = Color.white;

        
        if (selectedAnswer == correctAnswer)
        {
            message.text = "Correct Answer! \n Mission Successfully Done";
            message.color = new Color(3f / 255f, 147f / 255f, 123f / 255f); // Green color
            checkmarkImage.gameObject.SetActive(true); // Show the checkmark
            xmarkImage.gameObject.SetActive(false); // Hide the first x
            xmarkImage2.gameObject.SetActive(false); // hide the second x 
            attemptsLeft.text = "";
            DisableButtons(); 
        }
        else // wrong answer:
        {
            
            if (attemptsRemaining > 0)
            {
                // If there are attempts left, prompt to try again.
                message.text = "Wrong Answer. Try again!";
                message.color = new Color(200f / 255f, 6f / 255f, 6f / 255f); // Red color
                checkmarkImage.gameObject.SetActive(false); // Hide the checkmark
                xmarkImage.gameObject.SetActive(true); // Show the first x
                xmarkImage2.gameObject.SetActive(false); // hide the second x 
            }
            else
            {
                // If no attempts left after the wrong answer:
                message.text = "No attempts left!";
                message.color = new Color(200f / 255f, 6f / 255f, 6f / 255f); // Red color
                checkmarkImage.gameObject.SetActive(false); // Hide the checkmark
                xmarkImage.gameObject.SetActive(false); // hide the first x
                xmarkImage2.gameObject.SetActive(true); // Show the second x 
                DisableButtons(); 
            }
        }
    } 


    private void ResetButtonColors()
    {
        // Reset button colors here
        Button[] buttons = { choice1Button, choice2Button, choice3Button };
        foreach (var button in buttons)
        {
            button.image.color = Color.white; // Reset background color
            button.GetComponentInChildren<TMP_Text>().color = Color.black; // Reset text color
        }
    }

    private void DisableButtons()
    {
        choice1Button.interactable = false;
        choice2Button.interactable = false;
        choice3Button.interactable = false;
    }

    private void LoadMission()
    {
        DocumentReference docRef = db.Collection("AR Mission").Document(missionId);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Debug.Log($"Document data for {snapshot.Id} document:");

                Mission mission = new Mission();

                mission.Title = snapshot.GetValue<string>("Title");
                mission.Question = snapshot.GetValue<string>("Question");
                mission.CorrectAnswer = snapshot.GetValue<string>("CorrectAnswer");
                mission.WrongAnswer1 = snapshot.GetValue<string>("WrongAnswer1");
                mission.WrongAnswer2 = snapshot.GetValue<string>("WrongAnswer2");
                Debug.Log("finished retreiving mission data");
                DisplayMissionData(mission);
            }
            else
            {
                Debug.Log($"Document {snapshot.Id} does not exist!");
            }
        });


    }

    private List<string> ShuffleList(List<string> inputList)
    {
        for (int i = 0; i < inputList.Count; i++)
        {
            string temp = inputList[i];
            int randomIndex = Random.Range(0, inputList.Count);
            inputList[i] = inputList[randomIndex];
            inputList[randomIndex] = temp;
        }
        return inputList;
    }


    private void DisplayMissionData(Mission mission)
    {
        question.text = mission.Question;
        title.text = mission.Title + " Mission";

        List<string> answers = new List<string>()
        {
        mission.CorrectAnswer,
        mission.WrongAnswer1,
        mission.WrongAnswer2
        };

        // Shuffle the answers list
        answers = ShuffleList(answers);

        // Assign the shuffled answers to the buttons
        choice1.text = answers[0];
        choice2.text = answers[1];
        choice3.text = answers[2];
        correctAnswer = mission.CorrectAnswer;         

    }




}
