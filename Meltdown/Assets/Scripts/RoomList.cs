using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomList : MonoBehaviour
{
    public GameObject[] RoomListArray;

	// Use this for initialization
	void Awake()
    {

    }

    public void DisplayList(RoomInfo[] Rooms)
    {
        foreach (GameObject entry in RoomListArray)
        {
            entry.SetActive(false);
        }

        for (int Index = 0;
            Index < Rooms.Length;
            Index++)
        {
            GameObject entry = RoomListArray[Index];
            RoomInfo Info = Rooms[Index];

            EntryObject entryatt = entry.GetComponent<EntryObject>();

            Text NameText = entryatt.Name;
            Text SizeText = entryatt.Size;
            entryatt.RoomListEntry = true;

            NameText.text = Info.Name;
            SizeText.text = Info.PlayerCount.ToString() + "/" + Info.MaxPlayers.ToString();

        }
    }
}
