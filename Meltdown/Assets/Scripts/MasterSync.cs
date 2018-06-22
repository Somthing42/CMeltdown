using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetBase;

public class MasterSync : MonoBehaviour {

	public bool isMaster = false;

	[Tooltip("Photon View")]
	public PhotonView observed;
	[Tooltip("Network Object")]
	public NetworkObject toObserev;

	// Use this for initialization
	void Start () {
		observed = GetComponent<PhotonView> ();
		toObserev = GetComponent<NetworkObject> ();
		ObserveTheObject ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void ObserveTheObject() {

		if (PhotonNetwork.isMasterClient)
		{
			observed.ObservedComponents.Add (toObserev);
			isMaster = true;
		}
	}
}
