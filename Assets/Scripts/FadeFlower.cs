using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class FadeFlower : MonoBehaviour
{
    public SpriteRenderer[] flowers;
    public float fadeDuration = 1f;


    private void OnTriggerEnter2D(Collider2D other)
    {

        Debug.Log("HITFade: " + other.name);
        StartCoroutine(FadeRoutine());

    }

    IEnumerator FadeRoutine()
    {
        float t = 0f;

        Color[] originalColors = new Color[flowers.Length];
        Debug.Log("original colors " + originalColors);
        for (int i = 0; i < flowers.Length; i++)
            originalColors[i] = flowers[i].color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float lerp = t / fadeDuration;

            for (int i = 0; i < flowers.Length; i++)
            {
                Color grey = Color.Lerp(originalColors[i], Color.grey, lerp);
                flowers[i].color = grey;
            }

            yield return null;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
