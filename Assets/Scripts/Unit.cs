using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Unit : MonoBehaviourPun
{
    public int currentHP;
    public int maxHP;
    public float moveSpeed;
    public int minDamage;
    public int maxDamage;

    public int maxMoveDistance;
    public int maxAttackDistance;

    public bool usedthisTurn;
    public GameObject selectedVisual;
    public SpriteRenderer spriteVisual;

    [Header("UI")]
    public Image healthFillImage;
    [Header("sprite variants")]
    public Sprite leftPlayerSprite;
    public Sprite rightPlayerSprite;

    [PunRPC]
    void Initialize(bool isMine)
    {
        if (isMine)
            PlayerController.me.units.Add(this);
        else
            GameManager.instance.GetOtherPlayer(PlayerController.me).units.Add(this);
        healthFillImage.fillAmount = 1.0f;
        spriteVisual.sprite = transform.position.x < 0 ? leftPlayerSprite : rightPlayerSprite;
        spriteVisual.transform.up = transform.position.x < 0 ? Vector3.left : Vector3.right;
    }


    public bool CanSelect()
    {
        return !usedthisTurn;
    }

    public bool CanMove(Vector3 movePos)
    {
        if (Vector3.Distance(transform.position, movePos) <= maxMoveDistance)
            return true;
        else
            return false;
    }

    public bool CanAttack(Vector3 attackPos)
    {
        if (Vector3.Distance(transform.position, attackPos) <= maxAttackDistance)
            return true;
        else
            return false;
    }

    public void ToggleSelect(bool selected)
    {
        selectedVisual.SetActive(selected);
    }

    public void Move(Vector3 targetPos)
    {
        usedthisTurn = true;
        Vector3 dir = (transform.position - targetPos).normalized;
        spriteVisual.transform.up = dir;
        StartCoroutine(MoveOverTime(targetPos));
    }

    IEnumerator MoveOverTime(Vector3 targetPos)
    {
        while (transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void Attack(Unit unitToAttack)
    {
        usedthisTurn = true;
        unitToAttack.photonView.RPC("TakeDamage", PlayerController.enemy.photonPlayer, Random.Range(minDamage, maxDamage + 1));
    }

    [PunRPC]
    void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
            photonView.RPC("Die", RpcTarget.All);
        else
        {
            photonView.RPC("UpdateHealthBar", RpcTarget.All, (float)currentHP / (float)maxHP);
        }
    }

    [PunRPC]
    void UpdateHealthBar(float fillAmount)
    {
        healthFillImage.fillAmount = fillAmount;
    }

    [PunRPC]
    void Die()
    {
        if (!photonView.IsMine)
            PlayerController.enemy.units.Remove(this);
        else
        {
            PlayerController.me.units.Remove(this);
            GameManager.instance.CheckWinCondition();
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
