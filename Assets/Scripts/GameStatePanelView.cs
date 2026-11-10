using TMPro;
using UnityEngine;

public class GameStatePanelView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textPlayerLives;
    void Start()
    {
        UpdateLivesText(0);
    }

    public void UpdateLivesText(int playerLives)
    {
        if (textPlayerLives == null)
        {
            Debug.LogError("Text Player Lives is not assigned!");
            return;
        }
        textPlayerLives.text = $"Lives : {playerLives}";
    }
}
