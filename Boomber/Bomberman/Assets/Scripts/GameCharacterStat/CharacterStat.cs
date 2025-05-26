using UnityEngine;

/// <summary>
///  캐릭터의 능력치를 정의한 클래스.
/// </summary>
[System.Serializable]
public class CharacterStat
{
    [Header("능력치")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private int bombPower;
    [SerializeField] private int bombCount;

    [Header("기타 상태")]
    public bool canDestory;
    public bool isInvincible;

    // ==GETTERS==

    public float GetMoveSpeed() => moveSpeed;
    public int GetBombPower() => bombPower;
    public int GetBombCount() => bombCount;
    public bool GetCanDestory() => canDestory;
    public bool GetIsInvincible() => isInvincible;

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


    ///<summary>
    /// 아이템 효과를 적용하는 함수.
    ///</summary>
    
    public void ApplyItem()
    {
        //일단 함수 선언만. 아이템 클래스 미 구현.
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
