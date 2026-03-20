using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class ScreenFader : MonoBehaviour
{
    private Image fadeImage;

    void Start()
    {
        fadeImage = GetComponent<Image>();
    }

    public IEnumerator FadeToBlackAndBack(System.Action onFullBlack)
    {
        // 1. Fade to Black
        for (float t = 0; t <= 1; t += Time.deltaTime)
        {
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }

        // 2. Perform the transport logic while screen is black
        onFullBlack?.Invoke();

        // 3. Fade back to Transparent
        for (float t = 1; t >= 0; t -= Time.deltaTime)
        {
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }
    }
}

