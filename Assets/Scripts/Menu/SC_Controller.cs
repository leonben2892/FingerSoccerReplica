using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Controller : MonoBehaviour
{
    //Delegate Decleration
    public delegate void OnClick();

    //Login Screen Events
    public static OnClick onLoginButtonClick;

    //Main Menu Screen Events
    public static OnClick onLogoutButtonClick;
    public static OnClick onSinglePlayerButtonClick;
    public static OnClick onMultiplayerButtonClick;
    public static OnClick onGameTacticsButtonClick;
    public static OnClick onStudentInfoButtonClick;

    //Multiplayer Screen Events
    public static OnClick onPlayMultiplayerButtonClick;
    public static OnClick onMultiplayerMoneySliderChange;
    public static OnClick onMultiplayerCloseStatusButtonClick;

    //Student Info Screen Events
    public static OnClick onCVButtonClick;

    //Game Tactics Screen Events
    public static OnClick onPreviousTeamButtonClick;
    public static OnClick onNextTeamButtonClick;
    public static OnClick onPreviousFormationButtonClick;
    public static OnClick onNextFormationButtonClick;


    //Options Screen Events
    public static OnClick onOptionsBackButtonClick;
    public static OnClick onOptionsBackgroundVolumeSliderChange;
    public static OnClick onOptionsSfxVolumeSliderChange;
    public static OnClick onOptionsBackgroundMusicToggleChange;
    public static OnClick onOptionsSfxSoundToggleChange;

    //All Screens Events
    public static OnClick onBackButtonClick;
    public static OnClick onOptionsButtonClick;



    public void LoginButtonClick()
    {
        if (onLoginButtonClick != null)
        {
            onLoginButtonClick();
        }
    }

    public void LogoutButtonClick()
    {
        if(onLogoutButtonClick != null)
        {
            onLogoutButtonClick();
        }
    }

    public void SinglePlayerButtonClick()
    {
        if(onSinglePlayerButtonClick != null)
        {
            onSinglePlayerButtonClick();
        }
    }

    public void MultiplayerButtonClick()
    {
        if(onMultiplayerButtonClick != null)
        {
            onMultiplayerButtonClick();
        }
    }

    public void GameTacticsButtonClick()
    {
        if(onGameTacticsButtonClick != null)
        {
            onGameTacticsButtonClick();
        }
    }

    public void StudentInfoButtonClick()
    {
        if(onStudentInfoButtonClick != null)
        {
            onStudentInfoButtonClick();
        }
    }

    public void PlayMultiplayerButtonClick()
    {
        if(onPlayMultiplayerButtonClick != null)
        {
            onPlayMultiplayerButtonClick();
        }
    }

    public void MultiplayerMoneySliderChange()
    {
        if (onMultiplayerMoneySliderChange != null)
        {
            onMultiplayerMoneySliderChange();
        }
    }

    public void MultiplayerCloseStatusButtonClick()
    {
        if(onMultiplayerCloseStatusButtonClick != null)
        {
            onMultiplayerCloseStatusButtonClick();
        }
    }

    public void CVButtonClick()
    {
        if (onCVButtonClick != null)
        {
            onCVButtonClick();
        }
    }

    public void PreviousTeamButtonClick()
    {
        if(onPreviousTeamButtonClick != null)
        {
            onPreviousTeamButtonClick();
        }
    }

    public void NextTeamButtonClick()
    {
        if (onNextTeamButtonClick != null)
        {
            onNextTeamButtonClick();
        }
    }

    public void PreviousFormationButtonClick()
    {
        if (onPreviousFormationButtonClick != null)
        {
            onPreviousFormationButtonClick();
        }
    }

    public void NextFormationButtonClick()
    {
        if (onNextFormationButtonClick != null)
        {
            onNextFormationButtonClick();
        }
    }

    public void OptionsBackButtonClick()
    {
        if (onOptionsBackgroundMusicToggleChange != null)
        {
            onOptionsBackButtonClick();
        }
    }

    public void OptionsBackgroundVolumeSliderChange()
    {
        if (onOptionsBackgroundVolumeSliderChange != null)
        {
            onOptionsBackgroundVolumeSliderChange();
        }
    }

    public void OptionsSfxVolumeSliderChange()
    {
        if (onOptionsSfxVolumeSliderChange != null)
        {
            onOptionsSfxVolumeSliderChange();
        }
    }

    public void OptionsBackgroundMusicToggleChange()
    {
        if (onOptionsBackgroundMusicToggleChange != null)
        {
            onOptionsBackgroundMusicToggleChange();
        }
    }

    public void OptionsSfxSoundToggleChange()
    {
        if (onOptionsSfxSoundToggleChange != null)
        {
            onOptionsSfxSoundToggleChange();
        }
    }

    public void BackButtonClick()
    {
        if(onBackButtonClick != null)
        {
            onBackButtonClick();
        }
    }

    public void OptionsButtonClick()
    {
        if(onOptionsButtonClick != null)
        {
            onOptionsButtonClick();
        }
    }
}
