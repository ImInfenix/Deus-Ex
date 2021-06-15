using UnityEngine;
using UnityEngine.UI;

public class PlayerTeamDisplayer : MonoBehaviour
{
    public void SetTeam(string value)
    {
        GetComponent<Text>().text = value;
    }
}
