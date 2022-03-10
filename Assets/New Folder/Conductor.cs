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

    public float timer = 0;

    public float beatTime = 0;

    public float timeDifference = 0;

    public static bool tookAction = false;


    public Dummy[] allEnemies;


    public int successBeatCount;

    public int beatsShownInAdvance;

    public GameObject beatLinePrefabRight;
    public GameObject beatLinePrefabLeft;

    public Transform originForBeatLineRight;
    public Transform originForBeatLineLeft;
    public Transform targetForBeatLine;

    public bool twoSided;

    //public Transform beatParent;

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

        if(currentBeatNumber > previousBeatNumber)
        {
            InstantiateBeatLine();

            Debug.Log("beat");
            beatTime = Time.time;
            previousBeatNumber = currentBeatNumber;

            //if(currentBeatNumber > 1)
            //{
            //    InstantiateBeatLine();
            //}

            StartCoroutine(CheckMissedBeat());

            foreach (Dummy d in allEnemies)
            {
                if (d.isMelee && !d.isStunned)
                {
                    d.CheckCanAttackPlayer();
                }

                if (d.isRange && !d.isStunned)
                {
                    d.CheckCanAttackPlayerRange();
                }
            }
        }

        //determine how many beats since the song started
        songPositionInBeats = songPosition / secPerBeat;

        //Debug.Log((int)songPositionInBeats);
    }

    public void CheckActionToBPM()
    {
        tookAction = true;

        timer = Time.time;

        timeDifference = timer - beatTime;

        //Debug.Log(timeDifference);

        if(timeDifference < 0.35f || timeDifference > 0.64f)
        {
            Debug.Log("ON BEAT");
            successBeatCount++;
            
            if(successBeatCount % 10 == 0)
            {
                PlayerController.instance.AddAttackPower();
            }
        }
        else
        {
            Debug.Log("OFF BEAT");
            PlayerController.instance.ResetAttackPower();
            successBeatCount = 0;
        }

        //if(timeDifference > 0.6f)
        //{
        //    Debug.Log(timer);
        //    Debug.Log(beatTime);

        //    Debug.Break();
        //}
    }

    IEnumerator CheckMissedBeat()
    {
        yield return new WaitForSeconds(0.35f);

        if (!tookAction)
        {
            Debug.Log("MISSED BEAT");
            successBeatCount = 0;
            PlayerController.instance.ResetAttackPower();
        }

        tookAction = false;
    }


    public void InstantiateBeatLine()
    {
        Instantiate(beatLinePrefabRight, originForBeatLineRight);

        if (twoSided)
        {
            Instantiate(beatLinePrefabLeft, originForBeatLineLeft);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
