using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;

public class MainMenu : MonoBehaviour
{
    public void OnPlayClick() => SceneManager.LoadScene("Level_1-1");
    public void OnExitClick()
    {
        // 2. 실제 완성된 게임에서는 정상적으로 프로그램이 꺼집니다.
        UnityEngine.Application.Quit();

        // 💡 (선택 꿀팁) 유니티 에디터에서도 끄고 싶다면 아래 코드를 추가하세요!
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}