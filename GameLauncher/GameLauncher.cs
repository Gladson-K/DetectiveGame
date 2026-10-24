using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviour
{
    [Header("UI")]
    public GameObject mainPanel;
    public GameObject testPanel;
    public Text versionText;
    
    void Start()
    {
        if (versionText != null)
            versionText.text = "DetectiveGame V2.0";
        
        if (mainPanel != null) mainPanel.SetActive(true);
        if (testPanel != null) testPanel.SetActive(false);
    }
    
    public void PlayGame()
    {
        SceneManager.LoadScene("Main");
    }
    
    public void ShowTests()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (testPanel != null) testPanel.SetActive(true);
    }
    
    public void BackToMain()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (testPanel != null) testPanel.SetActive(false);
    }
    
    public void RunValidation()
    {
        Debug.Log("=== RUNNING VALIDATION ===");
        // Validation runs via ContentValidator
        Debug.Log("Use 'Detective Game > Validate Tale Content' in menu");
    }
    
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
