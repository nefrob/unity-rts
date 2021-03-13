using TMPro;
using UnityEngine;
using Mirror;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject displayParent = null;
    [SerializeField] private TMP_Text winnerNameText = null;

    private void Start()
    {
        GameOverManager.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverManager.ClientOnGameOver -= ClientHandleGameOver;
    }

    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        } else
        {
            NetworkManager.singleton.StopClient();
        }
    }

    private void ClientHandleGameOver(string winner)
    {
        winnerNameText.text = $"{winner} Has Won!";
        displayParent.SetActive(true);
    }
}
