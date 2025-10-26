using UnityEngine;

public class RestartButtonController : MonoBehaviour
{
    public GameManager gameManager;
    public void ButtonClick()
    {
        Debug.Log("Onclick restart button");
        gameManager.GameRestart();
    }
}
