using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트를 다루기 위해 필수!

public class HealthUIManager : MonoBehaviour
{
    public static HealthUIManager instance;

    [Header("Health UI Image Left First")]
    public GameObject[] healthIcons;

    // 💡 투명도와 색상을 조절하기 위한 변수
    private Color originalColor = Color.white; // 하얀색 = 원본 이미지 색상 그대로 출력
    private Color grayColor = new Color(0.3f, 0.3f, 0.3f, 1f); // 어두운 회색 (숫자가 작을수록 더 까매짐)

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateHealthDisplay(int currentHealth)
    {
        for (int i = 0; i < healthIcons.Length; i++)
        {
            // 💡 GameObject에 붙어있는 Image 컴포넌트를 꺼내옵니다.
            Image iconImage = healthIcons[i].GetComponent<Image>();

            if (i < currentHealth)
            {
                // 체력이 남아있는 칸이면 원본 색상으로
                iconImage.color = originalColor;
            }
            else
            {
                // 체력이 깎인 칸이면 이미지를 끄지 않고 색상만 칙칙한 회색으로 변경!
                iconImage.color = grayColor;
            }
        }
    }
}