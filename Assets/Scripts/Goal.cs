using TMPro;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI goalText;
    public PlayerController player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (goalText != null)
        {
            goalText.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (goalText == null || player == null)
            return;

        if (!player.IsBallCam() && goalText.gameObject.activeSelf)
        {
            goalText.gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("GOAL!");
            if (goalText != null)
            {
                goalText.gameObject.SetActive(true);
            }
        }
    }
}
