using UnityEngine;
/// <summary>
/// �̵� �ӵ��� ������Ű�� ������
/// </summary>
public class SpeedUpItem : Item
{
    public float speedBoost = 0.5f;

    public override StatEffectData GetEffect()
    {
        return new StatEffectData
        {
            moveSpeedDelta = speedBoost
        };

    }
}
