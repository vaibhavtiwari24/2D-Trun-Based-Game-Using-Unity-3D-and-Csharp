using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    public Player photonPlayer;
    public string[] unitsToSpawn;
    public Transform[] spawnPoints;

    public List<Unit> units = new List<Unit>();
    private Unit selectedUnit;

    public static PlayerController me;
    public static PlayerController enemy;

    [PunRPC]
    private void Initialize(bool isMine)
{
    if (isMine)
    {
        me = this;
        SpawnUnits();
    }
    else
    {
        enemy = this;
    }
    GameUI.instance.SetPlayerText(this);
}


void SpawnUnits()
{
    if (unitsToSpawn == null || unitsToSpawn.Length == 0 || spawnPoints == null || spawnPoints.Length == 0)
    {
        Debug.LogError("Failed to spawn units: unitsToSpawn or spawnPoints is null or empty.");
        return;
    }

    for (int x = 0; x < unitsToSpawn.Length; ++x)
    {
        GameObject unit = PhotonNetwork.Instantiate(unitsToSpawn[x], spawnPoints[x].position, Quaternion.identity);

        unit.GetPhotonView().RPC("Initialize", RpcTarget.Others, false);
        unit.GetPhotonView().RPC("Initialize", photonPlayer, true);

    }
}

    private void Update()
    {
        if (!photonView.IsMine)
            return;
        if (Input.GetMouseButtonDown(0) && GameManager.instance.currentPlayer == this)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            TrySelect(new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), 0));
        }
    }

    void TrySelect(Vector3 selectPos)
    {
        Unit unit = units.Find(x => x.transform.position == selectPos);
        if (unit != null)
        {
            SelectUnit(unit);
            return;
        }
        if (!selectedUnit)
            return;
        Unit enemyUnit = enemy.units.Find(x => x.transform.position == selectPos);
        if (enemyUnit != null)
        {
            TryAttack(enemyUnit);
            return;
        }
        TryMove(selectPos);
    }

    void SelectUnit(Unit unitToSelect)
    {
        if (!unitToSelect.CanSelect())
            return;
        if (selectedUnit != null)
            selectedUnit.ToggleSelect(false);
        selectedUnit = unitToSelect;
        selectedUnit.ToggleSelect(true);

        GameUI.instance.SetUnitInfoText(selectedUnit);
    }

    void DeSelectUnit()
    {
        selectedUnit.ToggleSelect(false);
        selectedUnit = null;

        GameUI.instance.unitInfoText.gameObject.SetActive(false);
    }

    void SelectNextAvailableUnit()
    {
        Unit availableUnit = units.Find(x => x.CanSelect());
        if (availableUnit != null)
            SelectUnit(availableUnit);
        else
            DeSelectUnit();
    }

    void TryAttack(Unit enemyUnit)
    {
        if (selectedUnit.CanAttack(enemyUnit.transform.position))
        {
            selectedUnit.Attack(enemyUnit);
            SelectNextAvailableUnit();

            GameUI.instance.UpdateWaitingUnitText(units.FindAll(x => x.CanSelect()).Count);
        }
    }

    void TryMove(Vector3 movePos)
    {
        if (selectedUnit.CanMove(movePos))
        {
            selectedUnit.Move(movePos);
            SelectNextAvailableUnit();
            GameUI.instance.UpdateWaitingUnitText(units.FindAll(x => x.CanSelect()).Count);
        }
    }

    public void EndTurn()
    {
        if (selectedUnit != null)
            DeSelectUnit();
        GameManager.instance.photonView.RPC("SetNextTurn", RpcTarget.All);
    }
    public void BeginTurn()
    {
        foreach (Unit unit in units)
            unit.usedthisTurn = false;
        GameUI.instance.UpdateWaitingUnitText(units.Count);
    }
}
