using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SFXPlayer instance;

    void Awake()
    {
        // 이미 씬에 다른 SFXPlayer 있다면, 새로 만들어진 나는 파괴한다 (중복 방지)
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // 내가 최초라면 나를 등록하고 씬 전환 시 파괴되지 않게 설정
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}