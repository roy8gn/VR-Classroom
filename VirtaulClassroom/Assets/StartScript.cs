using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScript : MonoBehaviour
{
    Board classBoard;
    public void DoClick()
    {
        classBoard = GameObject.FindGameObjectWithTag("BoardTag").GetComponent<Board>();
        classBoard.OnStartButtonPressed();
    }
}
