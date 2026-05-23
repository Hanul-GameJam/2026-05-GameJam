using UnityEngine;

public class DonDestroy : MonoBehaviour
{
    void Awake()
    {
        // 같은 이름을 가진 오브젝트가 이미 씬에 있는지 확인
        GameObject[] objs = GameObject.FindGameObjectsWithTag("AudioPlayer");

        // 이미 씬에 내가 있다면? 나는 사라져라! (중복 방지)
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
