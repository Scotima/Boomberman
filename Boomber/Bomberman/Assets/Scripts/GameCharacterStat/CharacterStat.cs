using UnityEngine;

/// <summary>
///  ĳ������ �ɷ�ġ�� ������ Ŭ����.
/// </summary>
[System.Serializable]
public class CharacterStat
{
    [Header("�ɷ�ġ")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private int bombPower;
    [SerializeField] private int bombCount;

    [Header("��Ÿ ����")]
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
        bombPower = Mathf.Clamp(value, 1, 10); // 1~10 ���� �����ϴ� �ڵ�.
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
    /// ������ ȿ���� �����ϴ� �Լ�.
    ///</summary>
    
    public void ApplyItem()
    {
        //�ϴ� �Լ� ����. ������ Ŭ���� �� ����.
    }


    ///<summary>
    /// �ɷ�ġ ���纻 ����. <- �ɷ��� �󸶳� ���ӽ�ų���� ���ؼ�. �� �������·� �ǵ��� �� �� �ֱ� ������ ���� ��Ŵ��
    /// ���纻 ����.
    ///</summary>
    
    public CharacterStat Clone()
    {
        return (CharacterStat)this.MemberwiseClone();
    }




}
