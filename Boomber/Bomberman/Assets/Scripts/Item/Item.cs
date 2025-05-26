using UnityEngine;
/// <summary>
/// ������ �θ� Ŭ����
/// </summary>
public abstract class Item : MonoBehaviour
{
    public string itemName;
    public Sprite icon;

    /// <summary>
    /// ������ ȿ�� ��������. 
    /// </summary>
    
    public abstract StatEffectData GetEffect();
}
