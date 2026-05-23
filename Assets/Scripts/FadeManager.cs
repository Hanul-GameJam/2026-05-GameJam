using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    public Image fadePanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeOut()
    {
        float[] alphaSteps =
        {
            0.25f,
            0.5f,
            0.75f,
            1f
        };

        foreach (float alpha in alphaSteps)
        {
            fadePanel.color =
                new Color(0, 0, 0, alpha);

            yield return new WaitForSeconds(0.2f);
        }
    }

    public IEnumerator FadeIn()
    {
        float[] alphaSteps =
        {
            0.75f,
            0.5f,
            0.25f,
            0f
        };

        foreach (float alpha in alphaSteps)
        {
            fadePanel.color =
                new Color(0, 0, 0, alpha);

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        // 어두워짐
        yield return StartCoroutine(FadeOut());

        // 씬 변경
        SceneManager.LoadScene(sceneName);

        // 한 프레임 기다리기
        yield return null;

        // 다시 밝아짐
        yield return StartCoroutine(FadeIn());
    }
}
