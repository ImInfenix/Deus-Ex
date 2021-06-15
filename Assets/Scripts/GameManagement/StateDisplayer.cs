using UnityEngine;
using UnityEngine.UI;

public class StateDisplayer : MonoBehaviour
{
    public Text displayText;

    public void SetState(string state)
    {
        displayText.text = state;
    }
}
