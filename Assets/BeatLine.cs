using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatLine : MonoBehaviour
{
    Conductor c;

    public bool isLeft;

    float timer = 0;
    float timeWait = 5;

    void Start()
    {
        c = Conductor.instance;

        //Debug.Log((c.beatsShownInAdvance - (c.secPerBeat - c.songPositionInBeats)) / c.beatsShownInAdvance);

        if (isLeft)
        {
            LeanTween.move(gameObject, c.targetForBeatLine.position, (c.beatsShownInAdvance - (c.currentBeatNumber - c.songPositionInBeats)) / c.beatsShownInAdvance).setOnComplete(() => Destroy(gameObject));
            //LeanTween.move(gameObject, c.targetForBeatLine.position, c.secPerBeat);
        }
        else
        {
            LeanTween.move(gameObject, c.targetForBeatLine.position, (c.beatsShownInAdvance - (c.currentBeatNumber - c.songPositionInBeats)) / c.beatsShownInAdvance).setOnComplete(() => Destroy(gameObject));
            //LeanTween.move(gameObject, c.targetForBeatLine.position, c.secPerBeat);
        }

    }

    //private void Update()
    //{
    //    timer += Time.deltaTime;

    //    if (timer <= timeWait)
    //    {
    //        transform.position = Vector2.Lerp(c.originForBeatLineRight.position, c.targetForBeatLine.position, timer / timeWait);
    //    }
    //}
}
