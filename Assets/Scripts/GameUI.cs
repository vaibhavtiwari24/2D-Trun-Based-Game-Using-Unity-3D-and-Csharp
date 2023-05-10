using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public Button endTurnButton;
    public TextMeshProUGUI leftPlayerText;
    public TextMeshProUGUI rightPlayerText;
    public TextMeshProUGUI waitingUnitText;
    public TextMeshProUGUI unitInfoText;
    public TextMeshProUGUI winText;

    public static GameUI instance;

    private void Awake()
    {
        instance = this;
    }

    public void OnEndTurnButton()
    {
        PlayerController.me.EndTurn();
    }

    public void ToggleEndTurnButton(bool toggle)
    {
        endTurnButton.interactable = toggle;
        waitingUnitText.gameObject.SetActive(toggle);
    }

    public void UpdateWaitingUnitText(int waitingUnits)
    {
        waitingUnitText.text = waitingUnits + " Units Waiting";
    }

    public void SetPlayerText(PlayerController player)
    {
        TextMeshProUGUI text = player == GameManager.instance.leftPlayer ? leftPlayerText : rightPlayerText;
        text.text = player.photonPlayer.NickName;
    }

    public void SetUnitInfoText(Unit unit)
    {
        unitInfoText.gameObject.SetActive(true);
        unitInfoText.text = "";

        unitInfoText.text += string.Format("<b>HP:</b> {0}/{1}", unit.currentHP, unit.maxHP);
        unitInfoText.text += string.Format("\n<b>Damage:</b> {0}-{1}", unit.minDamage, unit.maxDamage);
        unitInfoText.text += string.Format("\n<b>Move Range:</b> {0}", unit.maxMoveDistance);
        unitInfoText.text += string.Format("\n<b>Attack Range:</b> {0}", unit.maxAttackDistance);
    }

    public void SetWinText(string winnerName)
    {
        winText.gameObject.SetActive(true);
        winText.text = winnerName + " Wins";
    }
}
