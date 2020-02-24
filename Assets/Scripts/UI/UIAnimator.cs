using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimator : MonoBehaviour
{
    //Different animation types currently supported
    public enum UIAnimationTypes
    {
        Move,
        Fade,
        Scale
    }


    //Object to animate
    [SerializeField] GameObject objectToAnimate;

    //Settings for the enabling animation
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

    [SerializeField] bool destroyOnDisable = false;

    private LTDescr tweenObject;


    //When game object enabled
    public void OnEnable()
    {
        //If animation should be shown
        if (showOnEnable)
        {
            //Show it
            Show();
        }
    }

    //Show animation
    public void Show()
    {
        //Cancel tween if one already in progress
        if(objectToAnimate != null)
        {
            objectToAnimate = gameObject;
            LeanTween.cancel(objectToAnimate);
        }

        HandleTween(false, inAnimationType, inEaseType, inDelay, inDuration, inLoop, inPingpong, inStartPositionOffset);
    }

    //Function to disable the animations
    public void Disable()
    {
        //Cancel tween if one already in progress
        if (objectToAnimate != null)
        {
            objectToAnimate = gameObject;
            LeanTween.cancel(objectToAnimate);
        }

        //Tween handle
        HandleTween(true, outAnimationType, outEaseType, outDelay, outDuration, outLoop, outPingpong, outStartPositionOffset);
        //Run upon completion
        tweenObject.setOnComplete(() => {
            if (destroyOnDisable)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        });
    }

    //Handles actual tween logic
    public void HandleTween(bool isDisabling, UIAnimationTypes animationType, LeanTweenType easeType, float delay, float duration, bool loop, bool pingpong, bool startPositionOffset)
    {
        //Checks whether this script should be animating this object
        if(objectToAnimate == null)
        {
            objectToAnimate = gameObject;
        }

        //Switch for different animation types
        switch (animationType) 
        {
            //Fade 
            case UIAnimationTypes.Fade:
                if (isDisabling)
                {
                    //Fade out
                    Fade(outFrom, outTo, startPositionOffset, duration);
                }
                else
                {
                    //Fade in
                    Fade(inFrom, inTo, startPositionOffset, duration);
                }
                break;
            //Move
            case UIAnimationTypes.Move:
                if (isDisabling)
                {
                    //Move out
                    MoveAbsolute(outFrom, outTo, duration);
                }
                else
                {
                    //Move in
                    MoveAbsolute(inFrom, inTo, duration);
                }
                break;
            //Scale
            case UIAnimationTypes.Scale:
                if (isDisabling)
                {
                    //Scale out
                    Scale(outFrom, outTo, startPositionOffset, duration);
                }
                else
                {
                    //Scale in
                    Scale(inFrom, inTo, startPositionOffset, duration);
                }
                break;
        }
        
        //Tween settings
        tweenObject.setDelay(delay);
        tweenObject.setEase(easeType);

        //Loop settings
        if (loop)
        {
            tweenObject.loopCount = int.MaxValue;
        }

        //Ping pong effect
        if (pingpong)
        {
            tweenObject.setLoopPingPong();
        }
    }

    //Fade function
    public void Fade(Vector3 from, Vector3 to, bool startPositionOffset, float duration)
    {
        //Canvas group check for alpha channel
        if(gameObject.GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();
        }

        //Checks whether it starts at a different alpha
        if (startPositionOffset)
        {
            objectToAnimate.GetComponent<CanvasGroup>().alpha = from.x;
        }
        //Apply alpha change
        tweenObject = LeanTween.alphaCanvas(objectToAnimate.GetComponent<CanvasGroup>(), to.x, duration);
    }

    //Move object
    public void MoveAbsolute(Vector3 from, Vector3 to, float duration)
    {
        objectToAnimate.GetComponent<RectTransform>().anchoredPosition = from;
        tweenObject = LeanTween.move(objectToAnimate.GetComponent<RectTransform>(), to, duration);
    }

    //Scale object
    public void Scale(Vector3 from, Vector3 to, bool startPositionOffset, float duration)
    {
        if (startPositionOffset)
        {
            objectToAnimate.GetComponent<RectTransform>().localScale = from;
            tweenObject = LeanTween.scale(objectToAnimate, to, duration);
        }
    }
}
