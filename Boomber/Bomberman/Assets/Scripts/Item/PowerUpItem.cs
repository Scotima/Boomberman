using UnityEngine;

/// <summary>
/// �Ŀ��÷��ִ� ������ 
/// </summary>
public class PowerUpItem : Item
{
    public int powerBoost = 1;

    public override StatEffectData GetEffect()
    {
        return new StatEffectData { boomPowerDelta = powerBoost};

    }
}
