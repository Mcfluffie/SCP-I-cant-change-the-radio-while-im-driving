using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInAndOut : MonoBehaviour
{

    public float fadeDuration;
    public Color fadeColor;
    public Renderer rend;


    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeOut(float alphaIn, float alphaOut)
    {

        StartCoroutine(Fading(alphaIn, alphaOut));

    }

    IEnumerator Fading(float alphaIn, float alphaOut)
    {
        float timer = 0;
        while(timer <= fadeDuration)
        {

            Color newColour = fadeColor;
            newColour.a = Mathf.Lerp(alphaIn, alphaOut, timer / fadeDuration);

            rend.material.SetColor("_Color", newColour);

            timer += Time.deltaTime;
            yield return null;
        }
    }


}
