using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puzzleManager : MonoBehaviour
{
    public static puzzleManager instance;

    public PuzzleCube red;
    public PuzzleCube blue;
    public PuzzleCube green;


    private void Awake()
    {
        instance = this;
    }

    public void callDisableBlue()
    {
        StartCoroutine(DisableBlue());
    }
    public void callDisableGreen()
    {
        StartCoroutine(DisableGreen());

    }
    public void callDisableRedAndGreen()
    {
        StartCoroutine(DisableRedAndGreen());

    }

    public IEnumerator DisableBlue()
    {
        blue.gameObject.SetActive(false);

        yield return new WaitForSeconds(2);

        blue.gameObject.SetActive(true);
    }
    public IEnumerator DisableGreen()
    {
        green.gameObject.SetActive(false);

        yield return new WaitForSeconds(2);

        green.gameObject.SetActive(true);
    }
    public IEnumerator DisableRedAndGreen()
    {
        red.gameObject.SetActive(false);
        green.gameObject.SetActive(false);

        yield return new WaitForSeconds(2);

        red.gameObject.SetActive(true);
        green.gameObject.SetActive(true);
    }
}
