using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInAndOut : MonoBehaviour
{
    
    public float fadeDuration;
    public Color fadeColor;
    public Material fadeMaterial;
    public Renderer rend;


    // Start is called before the first frame update
    private void Awake()
    {
        FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            FadeOut();
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
            FadeIn();
        }

    }



    public void FadeOut()
    {
        Fade(0, 1);
        
    }

    public void FadeIn()
    {
        Fade(1, 0);
    }


    public void Fade(float alphaIn, float alphaOut)
    {
        
        StartCoroutine(Fading(alphaIn, alphaOut));

    }

    IEnumerator Fading(float alphaIn, float alphaOut)
    {
        float timer = 0;
        while(timer <= fadeDuration)
        {
            Debug.Log("its fading");
            Color newColour = fadeMaterial.color;
            newColour.a = Mathf.Lerp(alphaIn, alphaOut, timer / fadeDuration);

            Debug.Log(newColour.a);
            fadeMaterial.color = newColour;
            // couldn't find the proper ID string for the material at all
            // rend.material.SetColor("_Color", newColour); 

            timer += Time.deltaTime;
            yield return null;
        }
    }


}
