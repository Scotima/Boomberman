using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

///<summary>
/// 게임 전체 상태와 캐릭터 능력치를 관리하는 싱글턴 클래스.
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
    /// 게임 캐릭터 등록(1명~8명까지.)
    /// </summary>
    /// 
    public void RegisterCharacter(string characterId, CharacterStat stat)
    {
        if(!characterStats.ContainsKey(characterId))
        {
            characterStats[characterId] = stat; // 캐릭터가 아직 능력치를 배정받지 못했다면 능력치 할당.
        }
    }

    ///<summary>
    ///캐릭터 능력치 조회
    ///</summary>
    
    public CharacterStat GetCharacterStat(string characterId)
    {
        return characterStats.TryGetValue(characterId, out var stat) ? stat : null;
    }

    ///<summary>
    /// 아이템 효과 적용.
    ///</summary>
    
    public void ApplyItemToCharacter(string characterid)
    {
        //todo 아이템 클래스 구현 이후에.
    }
}
