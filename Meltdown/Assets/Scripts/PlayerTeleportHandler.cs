using UnityEngine;
using System.Collections;

public class PlayerTeleportHandler : Photon.PunBehaviour
{

	public int PlayerIndex;
	//PhotonView photonView;

    private Vector3 StartLocation; 


    // Use this for initialization
    void Start()
	{
		//photonView = GetComponent<PhotonView>();
        StartLocation = transform.position;
	}


	void OnEnable()
	{
        print("OnEnable PlayerTeleportHandler");
		PhotonNetwork.OnEventCall += this.GameStartTeleport;
	}
	void OnDisable()
	{
		PhotonNetwork.OnEventCall -= this.GameStartTeleport;
	}

    public override void OnLeftRoom()
    {
        VRTK.VRTK_HeightAdjustTeleport TS = GetComponent<VRTK.VRTK_HeightAdjustTeleport>();

        TS.ForceTeleport(StartLocation, Quaternion.identity);
    }

    void GameStartTeleport(byte eventcode, object content, int senderid)
	{
        print("event registered");
		if (eventcode == 4)
		{
            print("eventcode == 4");
			
            print("(photonView.isMine)");
            GameManager.Instance.infoPanel.AddLine("Teleporting to " + PlayerManager.instance.spawns[PlayerIndex].transform.position.ToString() + " with Index " + PlayerIndex);
				//print("Teleport Fired");
			VRTK.VRTK_HeightAdjustTeleport TS = GetComponent<VRTK.VRTK_HeightAdjustTeleport>();

			TS.Teleport(PlayerManager.instance.spawns[PlayerIndex].transform, PlayerManager.instance.spawns[PlayerIndex].transform.position, transform.rotation);
			GameManager.Instance.infoPanel.AddLine("New Position is " + transform.position.ToString());
			
		}

	}

    

    public bool CanTeleport = true;
    public IEnumerator TeleportDelay(float SecondsOfDelay)
    {
        CanTeleport = false; 
        float Seconds = 0;
        while(Seconds < SecondsOfDelay)
        { 
            yield return new WaitForSeconds(1.0f);
            Seconds++;
        }

        CanTeleport = true; 
    }
}
