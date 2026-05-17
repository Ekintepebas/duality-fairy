using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class End : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Level2";
    [SerializeField] private float fadeDuration = 2f;

    [SerializeField] public Image fadeImage;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Make sure it starts fully transparent
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.gameObject.SetActive(true);
        Debug.Log("HIT: " + other.name);
        StartCoroutine(FadeAndLoadScene());
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IEnumerator FadeAndLoadScene()
    {
        yield return new WaitForSeconds(1f); // wait 1 second before fading

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
