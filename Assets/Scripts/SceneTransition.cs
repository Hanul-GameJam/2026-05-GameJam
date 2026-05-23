using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필수!

public class SceneTransition : MonoBehaviour
{
    [SerializeField]
    public string nextSceneName;

    // 트리거 충돌이 일어났을 때 자동으로 실행되는 함수 (2D 기준)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 닿은 오브젝트의 태그가 "Player"인지 확인합니다.
        if (collision.CompareTag("Player"))
        {
            UnityEngine.Debug.Log(nextSceneName + " 씬으로 이동합니다!");

            // 시간을 정상 속도로 돌려놓습니다 (일시정지 상태에서 넘어가는 버그 방지)
            Time.timeScale = 1f;

            // 다음 씬 불러오기
            SceneManager.LoadScene(nextSceneName);
        }
    }
}