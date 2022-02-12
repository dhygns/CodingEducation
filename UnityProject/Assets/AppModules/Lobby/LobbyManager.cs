using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public void OnTeacherButtonClicked()
    {
        SceneManager.LoadScene("TeacherScene");
    }

    public void OnStudentButtonClicked()
    {
        SceneManager.LoadScene("StudentScene");
    }
}
