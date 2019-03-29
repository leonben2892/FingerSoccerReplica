using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_SPController : MonoBehaviour
{
    //Delegate Decleration
    public delegate void OnClick();

    public static OnClick onRestartGameButtonClick;
    public static OnClick onMainMenuButtonClick;
    public static OnClick onSentChatMessageButtonClick;

    public void RestartGameButtonClick()
    {
        if (onRestartGameButtonClick != null)
        {
            onRestartGameButtonClick();
        }
    }

    public void MainMenuButtonClick()
    {
        if (onMainMenuButtonClick != null)
        {
            onMainMenuButtonClick();
        }
    }

    public void SentChatMessageButtonClick()
    {
        if(onSentChatMessageButtonClick != null)
        {
            onSentChatMessageButtonClick();
        }
    }
}
