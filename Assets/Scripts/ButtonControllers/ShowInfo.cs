using UnityEngine;

public class ShowInfo : MonoBehaviour
{
    public GameObject showInfoScreen; 
    public bool isShowingInfo = false;

    void Start()
    {
        showInfoScreen.SetActive(false);
    }

    public void ToggleInformation()
    {
        isShowingInfo = !isShowingInfo;
        showInfoScreen.SetActive(isShowingInfo);
        Time.timeScale = isShowingInfo ? 0f : 1f;
    }
}