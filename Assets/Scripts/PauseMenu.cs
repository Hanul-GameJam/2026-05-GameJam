using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;

public class PauseMenu : MonoBehaviour
{
    // 💡 싱글톤 인스턴스 (자기 자신을 기억하는 변수)
    public static PauseMenu instance;

    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    void Awake()
    {
        // 💡 중복 생성 방지 로직
        // 이미 씬에 PauseMenu가 존재한다면, 새로 만들어진 나는 파괴한다.
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // 내가 최초의 PauseMenu라면 나를 instance로 등록하고, 씬이 넘어가도 파괴되지 않게 한다.
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // 💡 핵심 디테일: 메인 화면(MainMenu) 씬에서는 일시정지가 작동하지 않게 막기!
        // (메인 화면 씬 이름이 "MainMenu"가 아니라면 본인 프로젝트에 맞게 수정하세요)
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            return;
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;

        // 일시정지 UI를 끈 상태로 메인화면으로 돌아가기
        pauseMenuUI.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        UnityEngine.Application.Quit();
    }
}