using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private Image targetCrystalImage;

    public void SetMovesNumber(int number)
    {
        movesText.text = number.ToString();
    }
    public void SetTargetNumber(int number)
    {
        targetText.text = number.ToString();
    }
    public void SetTargetCrystal(Sprite sprite)
    {
        targetCrystalImage.sprite = sprite;
    }
}
