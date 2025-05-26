using UnityEngine;

/// <summary>
/// 파워올려주는 아이템 
/// </summary>
public class PowerUpItem : Item
{
    public int powerBoost = 1;

    public override StatEffectData GetEffect()
    {
        return new StatEffectData { boomPowerDelta = powerBoost};

    }
}
