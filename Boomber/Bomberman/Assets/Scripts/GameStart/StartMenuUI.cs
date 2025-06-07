using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    [Header("Dropdown")]
    public TMP_Dropdown playerCountDropdown;   // 0 → 2명, 1 → 3명, 2 → 4명
    public TMP_Dropdown mapSizeDropdown;       // 0 → Small, 1 → Medium, 2 → Large

    [Header("버튼")]
    public Button startButton;

    [Header("참조")]
    public GameModeManager gameModeManager;    // 씬 안에 있는 GameModeManager 연결

    private void Start()
    {
        startButton.onClick.AddListener(OnClickStart);
    }

    private void OnClickStart()
    {
        // ▼ 드롭다운 값을 읽어서 실제 설정값으로 변환
        int[] playerCountOptions = { 2, 4, 8 };
        int playerCount = playerCountOptions[playerCountDropdown.value];

        MakeMap.MapSize mapSize = (MakeMap.MapSize)mapSizeDropdown.value;

        // ▼ 게임 매니저 설정에 값 전달
        gameModeManager.mapGenerator.selectedSize = mapSize;
        gameModeManager.mapGenerator.playerCount = playerCount;

        // ▼ 게임 시작
        gameModeManager.StartGame();

        // ▼ UI 숨기기
        gameObject.SetActive(false);
    }
}
