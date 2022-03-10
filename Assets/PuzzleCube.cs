using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCube : MonoBehaviour
{
    public bool isRed, isBlue, isGreen;

    public void DoAction()
    {
        if (isRed)
        {
            puzzleManager.instance.callDisableGreen();
        }

        if (isGreen)
        {
            puzzleManager.instance.callDisableBlue();
        }

        if (isBlue)
        {
            puzzleManager.instance.callDisableRedAndGreen();
        }
    }
}
