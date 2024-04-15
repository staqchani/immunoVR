using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Platinio.TweenEngine;
using System;
using VRBeats;
using UnityEngine.SceneManagement;

public class ComplexityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI complexityText = null;
    [SerializeField] private Image bg = null;
    [SerializeField] private Color textSelectedColor;
    [SerializeField] private Color textNormalColor;
    [SerializeField] private Color backgroundSelectedColor;
    [SerializeField] private Color backgroundNormalColor;
    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField] private Ease ease = Ease.EaseOutExpo;

    public enum Complexty
    {
        Easy,
        Medium,
        Hard
    }

    public Complexty complextyLevel;
    private void Awake()
    {
        complexityText.color = textNormalColor;
        bg.color = backgroundNormalColor;
    }

  


    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.CancelAllTweens();
        PlatinioTween.instance.ColorTween(complexityText.color, textSelectedColor, fadeTime).SetOnUpdateColor(delegate (Color c)
        {
            complexityText.color = c;
        }).SetOwner(gameObject).SetEase(ease);

        bg.ColorTween(backgroundSelectedColor, fadeTime).SetOwner(gameObject).SetEase(ease);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.CancelAllTweens();
        PlatinioTween.instance.ColorTween(complexityText.color, textNormalColor, fadeTime).SetOnUpdateColor(delegate (Color c)
        {
            complexityText.color = c;
        }).SetOwner(gameObject).SetEase(ease);

        bg.ColorTween(backgroundNormalColor, fadeTime).SetOwner(gameObject).SetEase(ease);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerPrefs.SetInt("Complexity", (int)complextyLevel);
        PlayableManager.SetSelectedTrackIndex(0);
        //string sceneName = trackInfo.Mode == Mode.Boxing ? boxingStyleSceneName : saberStyleScenName;
        SceneManager.LoadScene(1);
    }
}