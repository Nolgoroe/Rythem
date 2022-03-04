using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    public static Conductor instance;

    //This is determined by the song you're trying to sync up to
    public float songBpm;

    //The number of seconds for each song beat
    public float secPerBeat;

    //Current song position, in seconds
    public float songPosition;

    //Current song position, in beats
    public float songPositionInBeats;

    //How many seconds have passed since the song started
    public float dspSongTime;

    //an AudioSource attached to this GameObject that will play the music.
    public AudioSource musicSource;

    //The offset to the first beat of the song in seconds
    public float firstBeatOffset;


    public int currentBeatNumber, previousBeatNumber;

    float timer = 0;

    public float beatTime = 0;

    public float timeDifference = 0;

    public static bool tookAction = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //Load the AudioSource attached to the Conductor GameObject
        musicSource = GetComponent<AudioSource>();

        //Calculate the number of seconds in each beat
        secPerBeat = 60f / songBpm;

        //Record the time when the music starts
        dspSongTime = (float)AudioSettings.dspTime;

        //Start the music
        musicSource.Play();

        previousBeatNumber = currentBeatNumber;
    }

    void Update()
    {
        songPosition = (float)(AudioSettings.dspTime - dspSongTime - firstBeatOffset);

        currentBeatNumber = (int)songPositionInBeats;

        if(currentBeatNumber == previousBeatNumber + 1)
        {
            Debug.Log("beat");
            beatTime = Time.time;
            previousBeatNumber = currentBeatNumber;
            StartCoroutine(CheckMissedBeat());

        }

        // determine how many seconds since the song started
        //determine how many beats since the song started
        songPositionInBeats = songPosition / secPerBeat;



        //Debug.Log((int)songPositionInBeats);
    }

    public void CheckActionToBPM()
    {
        tookAction = true;

        timer = Time.time;

        timeDifference = timer - beatTime;

        Debug.Log(timeDifference);

        if(timeDifference < 0.35f)
        {
            Debug.Log("ON BEAT");
        }
        else
        {
            Debug.Log("OFF BEAT");
        }
    }

    IEnumerator CheckMissedBeat()
    {
        yield return new WaitForSeconds(0.35f);

        if (!tookAction)
        {
            Debug.Log("MISSED BEAT");
        }

        tookAction = false;
    }
}
