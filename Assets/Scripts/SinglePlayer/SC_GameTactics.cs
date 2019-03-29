using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GameTactics : MonoBehaviour
{
    /// <summary>
    /// Selecting the player formation
    /// </summary>
    /// <param name="_ChosenFormation">Index of the player chosen formation</param>
    /// <returns>Vector2 array that holds the pucks intial position according to the player chosen formation</returns>
    public static Vector2[] ChosenFormation(int _ChosenFormation)
    {
        switch (_ChosenFormation)
        {
            //1-2-2
            case 0:
                Vector2[] initialPucksPos1 = { new Vector2(-555, 0), new Vector2(-352, 252), new Vector2(-352, -252),
                    new Vector2(-140, 99), new Vector2(-140, -99) };
                return initialPucksPos1;

            //3-1-1
            case 1:
                Vector2[] initialPucksPos2 = { new Vector2(-555, 0), new Vector2(-441, 199), new Vector2(-441, -199),
                    new Vector2(-300, 0), new Vector2(-137, 0) };
                return initialPucksPos2;

            //2-2-1
            case 2:
                Vector2[] initialPucksPos3 = { new Vector2(-505, -83), new Vector2(-505, 83), new Vector2(-272, -225),
                    new Vector2(-272, 225), new Vector2(-137, 0) };
                return initialPucksPos3;

            //2-1-2
            case 3:
                Vector2[] initialPucksPos4 = { new Vector2(-505, -83), new Vector2(-505, 83), new Vector2(-390, 0),
                    new Vector2(-170, 83), new Vector2(-170, -83) };
                return initialPucksPos4;

            //1-3-1
            case 4:
                Vector2[] initialPucksPos5 = { new Vector2(-555, 0), new Vector2(-380, 0), new Vector2(-272, 225),
                    new Vector2(-272, -225), new Vector2(-137, 0) };
                return initialPucksPos5;

            //1-2-2
            default:
                Vector2[] defaultPos = { new Vector2(-555, 0), new Vector2(-352, 252), new Vector2(-352, -252),
                    new Vector2(-140, 99), new Vector2(-140, -99) };
                return defaultPos;

        }
    }

    /// <summary>
    /// Selecting the enemy formation
    /// </summary>
    /// <param name="_ChosenFormation">Index of the enemy chosen formation</param>
    /// <returns>Vector2 array that holds the pucks intial position according to the enemy chosen formation</returns>
    public static Vector2[] EnemyFormation(int _ChosenFormation)
    {
        switch (_ChosenFormation)
        {
            //1-2-2
            case 0:
                Vector2[] initialPucksPos1 = { new Vector2(555, 0), new Vector2(352, 252), new Vector2(352, -252),
                    new Vector2(140, 99), new Vector2(140, -99) };
                return initialPucksPos1;

            //3-1-1
            case 1:
                Vector2[] initialPucksPos2 = { new Vector2(555, 0), new Vector2(441, 199), new Vector2(441, -199),
                    new Vector2(300, 0), new Vector2(137, 0) };
                return initialPucksPos2;

            //2-2-1
            case 2:
                Vector2[] initialPucksPos3 = { new Vector2(505, -83), new Vector2(505, 83), new Vector2(272, -225),
                    new Vector2(272, 225), new Vector2(137, 0) };
                return initialPucksPos3;

            //2-1-2
            case 3:
                Vector2[] initialPucksPos4 = { new Vector2(505, -83), new Vector2(505, 83), new Vector2(390, 0),
                    new Vector2(170, 83), new Vector2(170, -83) };
                return initialPucksPos4;

            //1-3-1
            case 4:
                Vector2[] initialPucksPos5 = { new Vector2(555, 0), new Vector2(380, 0), new Vector2(272, 225),
                    new Vector2(272, -225), new Vector2(137, 0) };
                return initialPucksPos5;

            //1-2-2
            default:
                Vector2[] defaultPos = { new Vector2(555, 0), new Vector2(352, 252), new Vector2(352, -252),
                    new Vector2(140, 99), new Vector2(140, -99) };
                return defaultPos;

        }
    }
}
