using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

    [SyncVar(hook = nameof(HandleTeamColorUpdated))]
    private Color teamColor = new Color();

    #region server

    public override void OnStartServer()
    {
        Player player = connectionToClient.identity.GetComponent<Player>();
        teamColor = player.GetTeamColor();
    }

    #endregion

    #region client

    private void HandleTeamColorUpdated(Color oldColor, Color newColor)
    {
        foreach(Renderer renderer in colorRenderers)
        {
            renderer.material.SetColor("_Color", newColor);
        }
    }

    #endregion
}
