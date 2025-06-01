using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;
using UnityEngine.TextCore.Text;

///<summary>
/// ���� ��ü ���¿� ĳ���� �ɷ�ġ�� �����ϴ� �̱��� Ŭ����.
///</summary>

public class GameInstance : MonoBehaviour
{
    public static GameInstance Instance { get;private set; }

    private Dictionary<string, CharacterStat> characterStats = new();
    private Dictionary<string, int> idCounters = new();


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

        else
            Debug.LogWarning($"[GameInstance] �̹� ��ϵ� characterId: {characterId}");
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

            Debug.Log($"[GameInstance] {characterid}�� ������ ���������� ���ŵ�");

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
    /// ���ξ�(prefix)�� ���� ������ ���ڿ� ID�� �����Ѵ�.
    /// ��: "AI_1", "Player_2"
    /// </summary>
    public string GenerateCharacterId(string prefix = "Character")
    {
        if (!idCounters.ContainsKey(prefix))
            idCounters[prefix] = 1;
        else
            idCounters[prefix]++;

        return $"{prefix}_{idCounters[prefix]}";
    }
}
