using UnityEngine;
/// <summary>
/// 이동 속도를 증가시키는 아이템
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
