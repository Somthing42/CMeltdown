using UnityEngine;
using System.Collections;

[RequireComponent( typeof( PhotonView ) )]
public class RequestOwnership : Photon.MonoBehaviour
{
	private PhotonView pView;

	void Start()
	{
		pView = GetComponent<PhotonView>();
	}
	
	//void Update()
	public void OnInteractableObjectTouched()
	{
		//if (player touches object) {
		if (pView.ownerId == PhotonNetwork.player.ID) {
				Debug.Log ("Not requesting ownership. Already mine.");
				return;
			}

		pView.RequestOwnership ();
		Debug.Log ("Fireing");
		//}
	}
		
}

