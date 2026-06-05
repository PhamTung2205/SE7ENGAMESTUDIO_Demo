using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;

    [Header("UI")]
    public Button kickButton;
    public Button restartButton;
    private void Start()
    {
        if (kickButton != null)
        {
            kickButton.onClick.RemoveAllListeners();
            kickButton.onClick.AddListener(KickButtonClick);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartButtonClick);
        }
    }

    private void Update()
    {
        UpdateKickButton();
    }

    private void UpdateKickButton()
    {
        if (player == null || kickButton == null)
            return;

        kickButton.gameObject.SetActive(
            player.CanKick()
        );
    }

    public void KickButtonClick()
    {
        if (player == null)
            return;

        if (player.CanKick())
        {
            player.BeginKick();
        }
    }

    public void RestartButtonClick()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}