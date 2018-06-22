using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
	public Text InfoPanelText;
    public GameObject InfoCanvas; 

	int LineCount = 0;
	public void AddLine(string Line)
	{
		string PanelString = InfoPanelText.text;
		InfoPanelText.text = PanelString + Line + "\n";
		++LineCount;

	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
            ToggleDisplay();
    }

    bool Visable = true; 
    public void ToggleDisplay()
    {
        Visable = !Visable;

        if (Visable)
            InfoCanvas.SetActive(true);
        else
            InfoCanvas.SetActive(false);

        AddLine("Set to " + Visable);
    }
}
