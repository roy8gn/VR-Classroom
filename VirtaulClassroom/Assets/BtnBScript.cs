using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnBScript : MonoBehaviour
{
    Board classBoard;
    public void DoClick()
    {
        classBoard = GameObject.FindGameObjectWithTag("BoardTag").GetComponent<Board>();
        classBoard.OnOptionBButtonPressed();
    }
}
