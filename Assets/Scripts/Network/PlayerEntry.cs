using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PlayerEntry : MonoBehaviour
{
    public Player attachedPlayer;
    public GameCreationHandler gameFinder;

    [SerializeField]
    private Color waitingColor;
    [SerializeField]
    private Color readyColor;

    bool _isReady;
    public bool IsReady => _isReady;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        SetReady(false);
    }

    public void SetReady(bool value)
    {
        _isReady = value;
        if (_isReady)
            image.color = readyColor;
        else
            image.color = waitingColor;
    }

    public void ChangeReadyState()
    {
        gameFinder.SetPlayerReady(attachedPlayer, !_isReady);
    }
}
