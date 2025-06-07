using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.TextCore.Text;
using System.Linq;

///<summary>
/// 게임 전체 상태와 캐릭터 능력치를 관리하는 싱글턴 클래스.
///</summary>

public class GameInstance : MonoBehaviour
{
    public static GameInstance Instance { get;private set; }

    private Dictionary<string, CharacterStat> characterStats = new();
    private Dictionary<string, int> idCounters = new();

    private Dictionary<string, AICharacter> aiCharacters = new(); // ✅ 새로 추가!

    public List<Vector2Int> destructibleBoxTiles = new();

    public MakeMap map;


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
    public void RegisterCharacter(string characterId, CharacterStat stat, AICharacter ai)
    {
        if(!characterStats.ContainsKey(characterId))
        {
            characterStats[characterId] = stat; // 캐릭터가 아직 능력치를 배정받지 못했다면 능력치 할당.
            aiCharacters[characterId] = ai;
        }

        else
            Debug.LogWarning($"[GameInstance] 이미 등록된 characterId: {characterId}");
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
    
    public void ApplyItemToCharacter(string characterid, Item item)
    {
       if(characterStats.TryGetValue(characterid, out var currentStat))
        {
            var effect = item.GetEffect();
            var newStat = currentStat.Clone();

            newStat.SetMoveSpeed(currentStat.GetMoveSpeed() + effect.moveSpeedDelta);
            newStat.SetBombPower(currentStat.GetBombPower() + effect.boomPowerDelta);
            newStat.SetBombCount(currentStat.GetBombCount() + effect.bombCountDelta);

            characterStats[characterid] = newStat;

            Debug.Log($"[GameInstance] {characterid}의 스탯이 아이템으로 갱신됨");

            foreach (var character in Object.FindObjectsByType<AICharacter>(FindObjectsSortMode.None))
            {
                if(character.characterId == characterid)
                {
                    character.UpdateStat(characterStats[characterid]);
                }
            }
        }
    }

    /// <summary>
    /// 접두어(prefix)에 따라 고유한 문자열 ID를 생성한다.
    /// 예: "AI_1", "Player_2"
    /// </summary>
    public string GenerateCharacterId(string prefix = "Character")
    {
        if (!idCounters.ContainsKey(prefix))
            idCounters[prefix] = 1;
        else
            idCounters[prefix]++;

        return $"{prefix}_{idCounters[prefix]}";
    }

    public void RegisterBox(Vector2Int tile)
    {
        if (!destructibleBoxTiles.Contains(tile))
        {
            destructibleBoxTiles.Add(tile);
            //Debug.Log($"[GameInstance] 상자 등록됨: {tile}");
        }
    }

    public void RemoveBox(Vector2Int tile)
    {
        if (destructibleBoxTiles.Remove(tile))
        {
            Debug.Log($"[GameInstance] 상자 제거됨: {tile}");
        }
    }

    public List<AICharacter> GetLivingEnemies(string myId)
    {
        return aiCharacters.Values
            .Where(ai => ai != null
                      && ai.characterId != myId
                      && ai.GetStat() != null
                      && !ai.GetStat().GetIsDead())
            .ToList();
    }
}
