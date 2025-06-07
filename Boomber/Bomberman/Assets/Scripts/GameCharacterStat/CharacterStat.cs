using UnityEngine;

/// <summary>
///  캐릭터의 능력치를 정의한 클래스.
/// </summary>
[System.Serializable]
public class CharacterStat
{
    [Header("능력치")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int bombPower = 1;
    [SerializeField] private int bombCount = 1;

    [Header("기타 상태")]
    public bool canDestory;
    public bool isInvincible;

    [Header("체력 상태")]
    private int maxHP = 3;
    private int currentHP = 3;


    // ==GETTERS==

    public float GetMoveSpeed() => moveSpeed;
    public int GetBombPower() => bombPower;
    public int GetBombCount() => bombCount;
    public bool GetCanDestory() => canDestory;
    public bool GetIsInvincible() => isInvincible;

    public int GetCurrentHP () => currentHP;
    public bool GetIsDead() => currentHP <= 0;



    // ==SETTERS==

    public void SetMoveSpeed(float value)
    {
        moveSpeed = Mathf.Max(0.1f, value);
    }

    public void SetBombPower(int value)
    {
        bombPower = Mathf.Clamp(value, 1, 10); // 1~10 사이 제한하는 코드.
    }

    public void SetBombCount(int value)
    {
        bombCount = Mathf.Clamp(value, 1, 10);
    }

    public void SetCanDestory(bool value)
    {
        canDestory = value;
    }

    public void SetIsInvincivle(bool value)
    {
        isInvincible = value;
    }

    public void ApplyDamage(int amount)
    {
        currentHP = Mathf.Max(0, currentHP - amount);
    }




    ///<summary>
    /// 능력치 복사본 생성. <- 능력을 얼마나 지속시킬지를 위해서. 즉 원래상태로 되돌릴 수 도 있기 때문에 원본 지킴이
    /// 복사본 생성.
    ///</summary>
    
    public CharacterStat Clone()
    {
        return (CharacterStat)this.MemberwiseClone();
    }




}
