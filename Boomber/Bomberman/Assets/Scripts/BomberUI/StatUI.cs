using TMPro;
using UnityEngine;

public class StatUI : MonoBehaviour
{
    public TextMeshProUGUI textHp;
    public TextMeshProUGUI textSpeed;
    public TextMeshProUGUI textPower;
    public TextMeshProUGUI textBombCount;

    public void UpdateUI(CharacterStat stat)
    {
        textHp.text = $"HP: {stat.GetCurrentHP()}";
        textSpeed.text = $"SPD: {stat.GetMoveSpeed():0.0}";
        textPower.text = $"PWR: {stat.GetBombPower()}";
        textBombCount.text = $"BMB: {stat.GetBombCount()}";
    }

}
