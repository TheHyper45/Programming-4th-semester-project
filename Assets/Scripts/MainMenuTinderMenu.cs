using UnityEngine;
using UnityEngine.UI;

public class MainMenuTinderMenu : MonoBehaviour {
    [SerializeField]
    private DatabaseManagement databaseManagement;
    [SerializeField]
    private GameObject titleScreen;
    [SerializeField]
    private GameObject matchingUserScreen;
    [SerializeField]
    private Button goButton;

    private void OnEnable() {
        titleScreen.SetActive(true);
        matchingUserScreen.SetActive(false);
        goButton.onClick.AddListener(() => {
            titleScreen.SetActive(false);
            matchingUserScreen.SetActive(true);
        });
    }
}
