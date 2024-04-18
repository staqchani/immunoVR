using Platinio.TweenEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VRUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI text = null;
    [SerializeField] private Image bg = null;
    [SerializeField] private Color textSelectedColor;
    [SerializeField] private Color textNormalColor;
    [SerializeField] private Color backgroundSelectedColor;
    [SerializeField] private Color backgroundNormalColor;
    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField] private Ease ease = Ease.EaseOutExpo;

    
    private void Awake()
    {
        text.color = textNormalColor;
        bg.color = backgroundNormalColor;
    }




    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.CancelAllTweens();
        PlatinioTween.instance.ColorTween(text.color, textSelectedColor, fadeTime).SetOnUpdateColor(delegate (Color c)
        {
            text.color = c;
        }).SetOwner(gameObject).SetEase(ease);

        bg.ColorTween(backgroundSelectedColor, fadeTime).SetOwner(gameObject).SetEase(ease);

    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        gameObject.CancelAllTweens();
        PlatinioTween.instance.ColorTween(text.color, textNormalColor, fadeTime).SetOnUpdateColor(delegate (Color c)
        {
            text.color = c;
        }).SetOwner(gameObject).SetEase(ease);

        bg.ColorTween(backgroundNormalColor, fadeTime).SetOwner(gameObject).SetEase(ease);
    }

   
}