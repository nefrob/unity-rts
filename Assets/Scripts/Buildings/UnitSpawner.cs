using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform gatherPoint = null;
    [SerializeField] private Health health = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private int maxUnitQueue = 5;
    [SerializeField] private float spawnMoveRange = 7f;
    [SerializeField] private float unitSpawnDuration = 5f;

    // Gather point
    [SerializeField] private Building building = null;
    [SerializeField] private LayerMask groundMask = 0;

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;

    [SyncVar]
    private float unitTimer;

    private float progressImageVelocity;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
            if (building.GetIsSelected()) GatherPointInput();
        }
    }

    #region server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ProduceUnits()
    {
        if (queuedUnits == 0) return;

        unitTimer += Time.deltaTime;

        if (unitTimer < unitSpawnDuration) return;

        GameObject unitInstance = Instantiate(
            unitPrefab.gameObject,
            transform.position + (gatherPoint.position - transform.position).normalized * 2.0f,
            gatherPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);

        Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = gatherPoint.position.y;

        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(gatherPoint.position + spawnOffset);

        queuedUnits--;
        unitTimer = 0.0f;
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {   
        if (queuedUnits == maxUnitQueue) return;

        Player player = connectionToClient.identity.GetComponent<Player>();
        if (player.GetResources() < unitPrefab.GetPrice()) return;

        player.SetResources(player.GetResources() - unitPrefab.GetPrice());
        queuedUnits++;
    }

    [Command]
    private void CmdSetGatherPoint(Vector3 pos)
    {
        gatherPoint.position = pos;
    }

    #endregion

    #region client

    [Client]
    private void GatherPointInput()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame) return;
        
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            Vector3 pos = hit.point;
            pos.y = gatherPoint.position.y;
            CmdSetGatherPoint(pos);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!hasAuthority) return;
        if (!building.GetIsSelected()) return;

        CmdSpawnUnit();
    }

    private void UpdateTimerDisplay()
    {
        float newProgress = unitTimer / unitSpawnDuration;

        if (newProgress < unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = newProgress;
        } else
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp(
                unitProgressImage.fillAmount,
                newProgress,
                ref progressImageVelocity,
                0.1f
            );
        }
    }

    private void ClientHandleQueuedUnitsUpdated(int oldCount, int newCount)
    {
        remainingUnitsText.text = newCount.ToString();
    }

    #endregion
}
