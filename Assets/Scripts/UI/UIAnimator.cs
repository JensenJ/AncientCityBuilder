using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimator : MonoBehaviour
{
    public enum UIAnimationTypes
    {
        Move,
        Fade,
        Scale
    }

    public GameObject objectToAnimate;

    [Header("Enable Settings")]
    [SerializeField] UIAnimationTypes inAnimationType = UIAnimationTypes.Fade;
    [SerializeField] LeanTweenType inEaseType = LeanTweenType.notUsed;

    [SerializeField] float inDuration = 0.5f;
    [SerializeField] float inDelay = 0.0f;

    [SerializeField] bool inLoop = false;
    [SerializeField] bool inPingpong = false;
    [SerializeField] bool inStartPositionOffset = true;

    [SerializeField] Vector3 inFrom = new Vector3();
    [SerializeField] Vector3 inTo = new Vector3();

    [SerializeField] bool showOnEnable = true;

    [Header("Disable Settings")]
    [SerializeField] UIAnimationTypes outAnimationType = UIAnimationTypes.Fade;
    [SerializeField] LeanTweenType outEaseType = LeanTweenType.notUsed;

    [SerializeField] float outDuration = 0.5f;
    [SerializeField] float outDelay = 0.0f;

    [SerializeField] bool outLoop = false;
    [SerializeField] bool outPingpong = false;
    [SerializeField] bool outStartPositionOffset = true;

    [SerializeField] Vector3 outFrom = new Vector3();
    [SerializeField] Vector3 outTo = new Vector3();

    private LTDescr tweenObject;


    public void OnEnable()
    {
        if (showOnEnable)
        {
            Show();
        }
    }

    public void Show()
    {
        HandleTween(false, inAnimationType, inEaseType, inDelay, inDuration, inLoop, inPingpong, inStartPositionOffset);
    }

    public void HandleTween(bool isDisabling, UIAnimationTypes animationType, LeanTweenType easeType, float delay, float duration, bool loop, bool pingpong, bool startPositionOffset)
    {
        if(objectToAnimate == null)
        {
            objectToAnimate = gameObject;
        }

        switch (animationType) 
        {
            case UIAnimationTypes.Fade:
                if (isDisabling)
                {
                    Fade(outFrom, outTo, startPositionOffset, duration);
                }
                else
                {
                    Fade(inFrom, inTo, startPositionOffset, duration);
                }
                break;
            case UIAnimationTypes.Move:
                if (isDisabling)
                {
                    MoveAbsolute(outFrom, outTo, duration);
                }
                else
                {
                    MoveAbsolute(inFrom, inTo, duration);
                }
                break;
            case UIAnimationTypes.Scale:
                if (isDisabling)
                {
                    Scale(outFrom, outTo, startPositionOffset, duration);
                }
                else
                {
                    Scale(inFrom, inTo, startPositionOffset, duration);
                }
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

    public void Fade(Vector3 from, Vector3 to, bool startPositionOffset, float duration)
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

    public void MoveAbsolute(Vector3 from, Vector3 to, float duration)
    {
        objectToAnimate.GetComponent<RectTransform>().anchoredPosition = from;
        tweenObject = LeanTween.move(objectToAnimate.GetComponent<RectTransform>(), to, duration);
    }

    public void Scale(Vector3 from, Vector3 to, bool startPositionOffset, float duration)
    {
        if (startPositionOffset)
        {
            objectToAnimate.GetComponent<RectTransform>().localScale = from;
            tweenObject = LeanTween.scale(objectToAnimate, to, duration);
        }
    }

    public void Disable(bool destroyObject)
    {
        HandleTween(true, outAnimationType, outEaseType, outDelay, outDuration, outLoop, outPingpong, outStartPositionOffset);
        tweenObject.setOnComplete(() => {
            if (destroyObject)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        });
    }
}
