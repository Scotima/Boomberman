using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

///<summary>
/// ���� ��ü ���¿� ĳ���� �ɷ�ġ�� �����ϴ� �̱��� Ŭ����.
///</summary>

public class GameInstance : MonoBehaviour
{
    public static GameInstance Instance { get;private set; }

    private Dictionary<string, CharacterStat> characterStats = new();


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// ���� ĳ���� ���(1��~8�����.)
    /// </summary>
    /// 
    public void RegisterCharacter(string characterId, CharacterStat stat)
    {
        if(!characterStats.ContainsKey(characterId))
        {
            characterStats[characterId] = stat; // ĳ���Ͱ� ���� �ɷ�ġ�� �������� ���ߴٸ� �ɷ�ġ �Ҵ�.
        }
    }

    ///<summary>
    ///ĳ���� �ɷ�ġ ��ȸ
    ///</summary>
    
    public CharacterStat GetCharacterStat(string characterId)
    {
        return characterStats.TryGetValue(characterId, out var stat) ? stat : null;
    }

    ///<summary>
    /// ������ ȿ�� ����.
    ///</summary>
    
    public void ApplyItemToCharacter(string characterid)
    {
        //todo ������ Ŭ���� ���� ���Ŀ�.
    }
}
