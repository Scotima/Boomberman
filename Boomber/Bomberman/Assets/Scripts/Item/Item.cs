using UnityEngine;
/// <summary>
/// 아이템 부모 클래스
/// </summary>
public abstract class Item : MonoBehaviour
{
    public string itemName;
    public Sprite icon;

    /// <summary>
    /// 아이템 효과 가져오기. 
    /// </summary>
    
    public abstract StatEffectData GetEffect();
}
