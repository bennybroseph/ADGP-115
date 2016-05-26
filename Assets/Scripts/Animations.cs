using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum AnimationType
{
    Fade,
    Scale,
    Rotate,
    Translate,
    Colorize,
}

[Serializable]
public class AnimationData
{
    public AnimationType animationType;
    public List<AnimationCurve> animationCurves;
}

[Serializable]
public class AnimationLayer
{
    public string name = "";

    [Range(0, 10)]
    public float delayTime;
    public List<AnimationData> animationDataList;
}

[Serializable]
public class AnimationSequence
{
    public List<AnimationLayer> animationLayers;
    public float totalAnimationTime;

    public void CacheAnimationTime()
    {
        totalAnimationTime = 0f;

        foreach (AnimationLayer animationLayer in animationLayers)
        {
            totalAnimationTime += Animations.GetEndTime(animationLayer.animationDataList) + animationLayer.delayTime;
        }
    }
}


public static class Animations
{
    public static IEnumerator Animate<TObject>(
        AnimationSequence a_AnimationSequence,
        TObject a_Object) where TObject : Graphic
    {
        Vector3 originalPosition = a_Object.transform.position;
        foreach (AnimationData animationData in a_AnimationSequence.animationLayers[0].animationDataList)
        {
            while (animationData.animationCurves.Count < 2)
                animationData.animationCurves.Add(animationData.animationCurves[0]);

            switch (animationData.animationType)
            {
                case AnimationType.Fade:
                    {
                        a_Object.color = new Color(
                            a_Object.color.r,
                            a_Object.color.g,
                            a_Object.color.b,
                            animationData.animationCurves[0].Evaluate(
                                animationData.animationCurves[0].keys[0].time));
                    }
                    break;
                case AnimationType.Scale:
                    {
                        a_Object.transform.localScale = new Vector3(
                            animationData.animationCurves[0].Evaluate(
                                animationData.animationCurves[0].keys[0].time),
                            animationData.animationCurves[1].Evaluate(
                                animationData.animationCurves[1].keys[0].time));
                    }
                    break;
                case AnimationType.Rotate:
                    {
                        a_Object.transform.eulerAngles = new Vector3(
                            animationData.animationCurves[0].Evaluate(
                                animationData.animationCurves[0].keys[0].time) * 360,
                            animationData.animationCurves[1].Evaluate(
                                animationData.animationCurves[1].keys[0].time) * 360);
                    }
                    break;
                case AnimationType.Translate:
                    {
                        a_Object.transform.position = new Vector3(
                            originalPosition.x
                            + animationData.animationCurves[0].Evaluate(animationData.animationCurves[0].keys[0].time),
                            originalPosition.y
                            + animationData.animationCurves[1].Evaluate(animationData.animationCurves[1].keys[0].time));
                    }
                    break;
            }
        }
        foreach (AnimationLayer animationLayer in a_AnimationSequence.animationLayers)
        {
            yield return new WaitForSeconds(animationLayer.delayTime);

            yield return AnimateLayer(animationLayer, a_Object);
        }
    }

    public static IEnumerator AnimateLayer<TObject>(
        AnimationLayer a_AnimationLayer,
        TObject a_Object) where TObject : Graphic
    {
        float endTime = GetEndTime(a_AnimationLayer.animationDataList);

        Vector3 originalPosition = a_Object.transform.position;

        float deltaTime = 0.0f;
        while (deltaTime < endTime)
        {
            deltaTime += Time.deltaTime;
            foreach (AnimationData animationData in a_AnimationLayer.animationDataList)
            {
                if (a_Object.IsDestroyed())
                    yield break;

                switch (animationData.animationType)
                {
                    case AnimationType.Fade:
                        {
                            if (animationData.animationCurves[0][0].time <= deltaTime)
                                a_Object.color = new Color(
                                    a_Object.color.r,
                                    a_Object.color.g,
                                    a_Object.color.b,
                                    animationData.animationCurves[0].Evaluate(deltaTime));
                        }
                        break;
                    case AnimationType.Scale:
                        {
                            if (animationData.animationCurves[0][0].time <= deltaTime)
                                a_Object.transform.localScale = new Vector3(
                                    animationData.animationCurves[0].Evaluate(deltaTime),
                                    animationData.animationCurves[1].Evaluate(deltaTime));
                        }
                        break;
                    case AnimationType.Rotate:
                        {
                            if (animationData.animationCurves[0][0].time <= deltaTime)
                                a_Object.transform.eulerAngles = new Vector3(
                                    animationData.animationCurves[0].Evaluate(deltaTime) * 360,
                                    animationData.animationCurves[1].Evaluate(deltaTime) * 360);
                        }
                        break;
                    case AnimationType.Translate:
                        {
                            if (animationData.animationCurves[0][0].time <= deltaTime)
                                a_Object.transform.position = new Vector3(
                                    originalPosition.x + animationData.animationCurves[0].Evaluate(deltaTime),
                                    originalPosition.y + animationData.animationCurves[1].Evaluate(deltaTime));
                        }
                        break;
                    default:
                        Debug.Log("Not Yet Implemented...");
                        break;
                }
            }
            yield return null;
        }
    }

    public static float GetEndTime(List<AnimationData> a_AnimationDataList)
    {
        List<AnimationCurve> animationCurves = new List<AnimationCurve>();

        foreach (AnimationData animationData in a_AnimationDataList)
            foreach (AnimationCurve animationCurve in animationData.animationCurves)
                animationCurves.Add(animationCurve);

        animationCurves.Sort(SortAnimationCurves);

        return animationCurves[0][animationCurves[0].length - 1].time;
    }

    private static int SortAnimationCurves(AnimationCurve a, AnimationCurve b)
    {
        if (a[a.length - 1].time < b[b.length - 1].time)
            return 1;
        if (a[a.length - 1].time > b[b.length - 1].time)
            return -1;

        return 0;
    }
}
