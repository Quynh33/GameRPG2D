using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
 
    public void EndCutsceneAndStartGame()
    {
        // Chuyển đến scene chơi game
        SceneManager.LoadScene("HouseInterior");
    }
}
