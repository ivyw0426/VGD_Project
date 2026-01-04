using UnityEngine;
using TMPro;

public class Speed : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] PlayerMovement player; // your movement script

    void Update()
    {
        speedText.text = $"Speed: {player.moveSpeed:F1}";
    }
}