﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class GameManager : Photon.PunBehaviour, IPunObservable
{


    public static GameManager Instance { get; private set; }

    public SequenceManager SQM;

    [SerializeField]
    private int MinimumPlayers = 2;

    public float RoundEndTime { get; set; }

    //Can't put a header above enum.
    enum Difficulty { Easy, Medium, Hard };

    [Header("Difficalty")]
    [SerializeField]
    private Difficulty difficulty;
    [Tooltip("0=Easy,1=Medium,2=Hard")]
    public float[] PossibleRoundTimes = new float[3];
    [Tooltip("0=Easy,1=Medium,2=Hard")]
    public float[] PossibleSequenceCompleteRewardTimes = new float[3];
    [Tooltip("0=Easy,1=Medium,2=Hard")]
    public float[] PossibleSequenceActionTimes = new float[3];


    [Header("Timer Info")]
    [Tooltip("Total amount of time Players have this level.")]
    public float startingRoundTime = 300.0f;
    [Tooltip("How long the timer will pause after getting a sequence right.")]
    public float sequenceCompleteReward = 15.0f;
    [Tooltip("How much time players have to compleate a sequence.")]
    public float sequenceActionTime = 10.0f;
	[HideInInspector]
	public float timerTime;


    public float RoundTimeExtension { get { return sequenceCompleteReward - (SQM.currentSequence * sequenceActionTime); } }

    [Header("Controller Objects")]
    public GameObject leftController;
    public GameObject rightController;
    [Header("VRTK Controller Objects")]
    public GameObject VRTKLeftController;
    public GameObject VRTKRightController;



    public RotationZone[] RotationZones;

    [HideInInspector]
    public InfoPanel infoPanel;

    private bool CountdownStarted { get; set; }
    private bool GameStarted { get; set; }
    private bool ReadyedUp { get; set; }
    private int ReadyUpCount { get; set; }


    public Animation BlastDoorAnimation;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);

        this.infoPanel = this.GetComponent<InfoPanel>();
        this.infoPanel.AddLine("Log Active!");
    }


    void OnEnable()
    {
        PhotonNetwork.OnEventCall += this.CountDownEvent;
        PhotonNetwork.OnEventCall += this.ReadyUpEvent;
        PhotonNetwork.OnEventCall += this.ReadyDownEvent;

    }
    void OnDisable()
    {
        PhotonNetwork.OnEventCall -= this.CountDownEvent;
        PhotonNetwork.OnEventCall -= this.ReadyUpEvent;
        PhotonNetwork.OnEventCall -= this.ReadyDownEvent;
    }

    void Update()
    {
        // NOTE(barret): Input for readying up. for testing purposes
        if (Input.GetKeyDown(KeyCode.F1) && ReadyedUp == false)
        {
            //print("F1 pressed");
            ReadyUpRaise();
            ReadyedUp = true;
        }

        if (Input.GetKeyDown(KeyCode.F2) && ReadyedUp == true)
        {
            ReadyDownRaise();
            ReadyedUp = false;
        }
    }

    void StartCountdownRaise()
    {
        if (PhotonNetwork.room.PlayerCount >= this.MinimumPlayers && CountdownStarted == false)
        {
            print("Raising Countdown event");

            RaiseEventOptions Options = new RaiseEventOptions()
            {
                Receivers = ReceiverGroup.All
            };

            PhotonNetwork.RaiseEvent(0, new byte[] { 1, 2, 5, 10 }, true, Options);
        }
    }

    void ReadyUpRaise()
    {
        RaiseEventOptions Options = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent(1, new byte[] { 1, 2, 5, 10 }, true, Options);
    }

    void ReadyDownRaise()
    {
        RaiseEventOptions Options = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent(2, new byte[] { 1, 2, 5, 10 }, true, Options);
    }

    void ReadyUpEvent(byte eventcode, object content, int senderid)
    {
        if (eventcode == 1)
        {
            print("ReadyUpEvent");

            if (PhotonNetwork.isMasterClient)
            {
                ReadyUpCount += 1;
                infoPanel.AddLine("ReadyUpCount: " + ReadyUpCount);
                print("ReadyUpCount: " + ReadyUpCount);

                if (ReadyUpCount == this.MinimumPlayers)
                {
                    print("Starting CountdownRaise");
                    infoPanel.AddLine("Starting CountdownRaise");
                    StartCountdownRaise();
                }
            }
        }
    }

    void ReadyDownEvent(byte eventcode, object content, int senderid)
    {
        if (eventcode == 2)
        {
            print("ReadyDownEvent");
            if (PhotonNetwork.isMasterClient)
            {
                ReadyUpCount -= 1;
                infoPanel.AddLine("ReadyDownCount: " + ReadyUpCount);
                print("ReadyDownCount: " + ReadyUpCount);
                if (ReadyUpCount < 0)
                    ReadyUpCount = 0;
            }
        }
    }

    void CountDownEvent(byte eventcode, object content, int senderid)
    {
        if (eventcode == 0)
        {
            StartCoroutine("StartCountdown");
        }
    }

    public IEnumerator StartCountdown()
    {
        float currCountdownValue = 10;

        while (currCountdownValue > 0)
        {
            print("Match Begins in\n" + currCountdownValue.ToString());
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }

        this.GameStarted = true;

        // TODO(barret): blast door down animation

        print("Get past loop");
        if (PhotonNetwork.isMasterClient)
        {
            print("Fire event");
            TeleportIntoScene();
        }
    }

    void TeleportIntoScene()
    {
        // TODO(barret): Need to teleport the players into the scene
        byte evCode = 4;
        byte[] content = new byte[] { 1, 2, 5, 10 };
        bool reliable = true;

        RaiseEventOptions Options = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All
        };
        PhotonNetwork.RaiseEvent(evCode, content, reliable, Options);
        infoPanel.AddLine("Raised Teleport event");
        Debug.Log("Teleport");
    }


    public void SetDifficalty()
    {
        if (difficulty == Difficulty.Easy)
        {
            startingRoundTime = PossibleRoundTimes[0];
            sequenceCompleteReward = PossibleSequenceCompleteRewardTimes[0];
            sequenceActionTime = PossibleSequenceActionTimes[0];
        }
        if (difficulty == Difficulty.Medium)
        {
            startingRoundTime = PossibleRoundTimes[1];
            sequenceCompleteReward = PossibleSequenceCompleteRewardTimes[1];
            sequenceActionTime = PossibleSequenceActionTimes[1];
        }
        if (difficulty == Difficulty.Hard)
        {
            startingRoundTime = PossibleRoundTimes[2];
            sequenceCompleteReward = PossibleSequenceCompleteRewardTimes[2];
            sequenceActionTime = PossibleSequenceActionTimes[2];
        }

    }

#if false
    //function to reset all teleport poitns to yellow after teleporting.
    public void resetTeleportPoint()
	{
		for (int i = 0; i < teleportPoints.Length; i++)
		{
			teleportPoints[i].Reset();
			teleportPoints[i].gameObject.SetActive(false);
		}
	}

	public void showTeleportPoints()
	{
		for (int i = 0; i < teleportPoints.Length; i++)
		{
			teleportPoints[i].gameObject.SetActive(true);
		}
	}
	
#endif

   /* [PunRPC]
    void SendButtonPress(int buttonValue)
    {
        // Create a new player at the appropriate spawn spot
        CheckButtonInput(buttonValue);
    }

    [PunRPC]
    void updateCurrentSequenceInformation(int sentCurrentSequence)
    {
        // Create a new player at the appropriate spawn spot
        Instance.currentSequence = sentCurrentSequence;
    }

    [PunRPC]
    void updateCurrentStepInformation(int sentCurrentStep)
    {
        // Create a new player at the appropriate spawn spot
        Instance.currentStep = sentCurrentStep;
    }*/

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
			stream.SendNext(timerTime);
        }
        else
        {
			timerTime = (float)stream.ReceiveNext();
        }
    }

	/*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
	{
		if(stream.isWriting) 
		{
			stream.SendNext(difficulty);
			stream.SendNext(timerTime);


		}
		else 
		{
			object Next = stream.ReceiveNext();
			string Serialized = Next.ToString ();
			float timer = float.Parse (Serialized);
			GameManager.Instance.timerTime = timer;

		}

	}*/
}

