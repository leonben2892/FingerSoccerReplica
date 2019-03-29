using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Puck : MonoBehaviour
{
    public Sprite[] teamLogo;

    /// <summary>
    /// Initializing the puck team logo image based on a given index
    /// </summary>
    /// <param name="teamLogoIndex"></param>
    public void InitPuckImg(int teamLogoIndex)
    {
        GetComponent<SpriteRenderer>().sprite = teamLogo[teamLogoIndex];
    }
}
