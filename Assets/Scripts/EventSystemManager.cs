using UnityEngine;
using UnityEngine.EventSystems; // 💡 EventSystem을 직접 제어하기 위해 꼭 추가해 주세요!

public class EventSystemManager : MonoBehaviour
{
    public static EventSystemManager instance;

    void Awake()
    {
        if (instance != null)
        {
            // 💡 추가된 핵심 코드: 오브젝트가 파괴되기 전 찰나의 순간에 
            // EventSystem 컴포넌트 자체를 즉시(즉각적으로) 꺼버립니다.
            EventSystem eventSystem = GetComponent<EventSystem>();
            if (eventSystem != null)
            {
                eventSystem.enabled = false; // 입 틀어막기!
            }
            
            Destroy(gameObject); // 그리고 파괴
            return;
        }

        instance = this;
        transform.SetParent(null); 
        DontDestroyOnLoad(gameObject);
    }
}