using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimator : MonoBehaviour
{
    public enum UIAnimationTypes
    {
        Move, 
        Fade
    }

    public GameObject objectToAnimate;

    public UIAnimationTypes animationType;
    public LeanTweenType easeType;
    public float duration;
    public float delay;

    public bool loop;
    public bool pingpong;

    public bool startPositionOffset;
    public Vector3 from;
    public Vector3 to;

    private LTDescr tweenObject;

    public bool showOnEnable;
    public bool WorkOnDisable;

    public void OnEnable()
    {
        if (showOnEnable)
        {
            Show();
        }
    }

    public void Show()
    {
        HandleTween();
    }

    public void HandleTween()
    {
        if(objectToAnimate == null)
        {
            objectToAnimate = gameObject;
        }

        switch (animationType) 
        {
            case UIAnimationTypes.Fade:
                Fade();
                break;
            case UIAnimationTypes.Move:
                MoveAbsolute();
                break;
        }

        tweenObject.setDelay(delay);
        tweenObject.setEase(easeType);

        if (loop)
        {
            tweenObject.loopCount = int.MaxValue;
        }

        if (pingpong)
        {
            tweenObject.setLoopPingPong();
        }
    }

    public void Fade()
    {
        if(gameObject.GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();
        }

        if (startPositionOffset)
        {
            objectToAnimate.GetComponent<CanvasGroup>().alpha = from.x;
        }
        tweenObject = LeanTween.alphaCanvas(objectToAnimate.GetComponent<CanvasGroup>(), to.x, duration);
    }

    public void MoveAbsolute()
    {
        objectToAnimate.GetComponent<RectTransform>().anchoredPosition = from;
        tweenObject = LeanTween.move(objectToAnimate.GetComponent<RectTransform>(), to, duration);
    }

    void SwapDirection()
    {
        var temp = from;
        from = to;
        to = temp;
    }

    public void Disable()
    {
        SwapDirection();
        HandleTween();
        tweenObject.setOnComplete(() =>
        {
            SwapDirection();
            gameObject.SetActive(false);
        });
    }
}
