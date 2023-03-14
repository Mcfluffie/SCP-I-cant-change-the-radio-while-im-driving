using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screenFader : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _intensity = 0f;
    [SerializeField] private Color _color = Color.black;
    [SerializeField] Material _fadeMaterial = null;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _fadeMaterial.SetFloat("_intensity", _intensity);
        _fadeMaterial.SetColor("_FadeColour", _color);
        Graphics.Blit(source, destination, _fadeMaterial);
    }

    public Coroutine StartFadeIn()
    {
        StopAllCoroutines();
        return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        while (_intensity <= 1f)
        {
            _intensity += _speed * Time.deltaTime;
            yield return null;
        }
    }

    public Coroutine StartFadeOut()
    {
        StopAllCoroutines();
        return StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        while (_intensity >= 1f)
        {
            _intensity -= _speed * Time.deltaTime;
            yield return null;
        }
    }
}
