using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    [Header("Dropdown")]
    public TMP_Dropdown playerCountDropdown;   // 0 �� 2��, 1 �� 3��, 2 �� 4��
    public TMP_Dropdown mapSizeDropdown;       // 0 �� Small, 1 �� Medium, 2 �� Large

    [Header("��ư")]
    public Button startButton;

    [Header("����")]
    public GameModeManager gameModeManager;    // �� �ȿ� �ִ� GameModeManager ����

    private void Start()
    {
        startButton.onClick.AddListener(OnClickStart);
    }

    private void OnClickStart()
    {
        // �� ��Ӵٿ� ���� �о ���� ���������� ��ȯ
        int[] playerCountOptions = { 2, 4, 8 };
        int playerCount = playerCountOptions[playerCountDropdown.value];

        MakeMap.MapSize mapSize = (MakeMap.MapSize)mapSizeDropdown.value;

        // �� ���� �Ŵ��� ������ �� ����
        gameModeManager.mapGenerator.selectedSize = mapSize;
        gameModeManager.mapGenerator.playerCount = playerCount;

        // �� ���� ����
        gameModeManager.StartGame();

        // �� UI �����
        gameObject.SetActive(false);
    }
}
