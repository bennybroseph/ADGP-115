using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine.UI;

public static class Animations
{
    private delegate void Animate(params float[] a_Values);

    public static IEnumerator Fade2DGraphic<TGraphic>(TGraphic a_Graphic, params AnimationCurve[] a_AnimationCurves)
        where TGraphic : MaskableGraphic
    {
        Keyframe lastKeyframe;

        if (!ParseKeyframe(out lastKeyframe, a_AnimationCurves))
            yield return true;

        float deltaTime = 0.0f;
        while (deltaTime < lastKeyframe.time)
        {
            deltaTime += Time.deltaTime;
            a_Graphic.color =
                new Color(
                    a_Graphic.color.r,
                    a_Graphic.color.g,
                    a_Graphic.color.b,
                    a_AnimationCurves[0].Evaluate(deltaTime));

            yield return false;
        }
    }

    public static IEnumerator Scale2DTransform<TTransform>(
        TTransform a_Transform,
        params AnimationCurve[] a_AnimationCurves) where TTransform : Transform
    {
        Keyframe lastKeyframe;

        if (!ParseKeyframe(out lastKeyframe, a_AnimationCurves))
            yield return true;

        yield return ParseAnimation(
            lastKeyframe,
            a_AnimationCurves,
            delegate (float[] a_Floats)
            {
                a_Transform.localScale = new Vector3(a_Floats[0], a_Floats[1]);
            });
    }

    private static bool ParseKeyframe(out Keyframe a_LastKeyframe, params AnimationCurve[] a_AnimationCurves)
    {
        a_LastKeyframe = new Keyframe();
        if (a_AnimationCurves.Length == 0)
        {
            Debug.LogError("'Animations' was not given any animation curves");
            return false;
        }

        if (a_AnimationCurves.Length > 2)
        {
            Debug.LogError("'Animations' cannot handle more than 2 animation curves");
            return false;
        }

        switch (a_AnimationCurves.Length)
        {
            case 1:
                a_LastKeyframe = a_AnimationCurves[0][a_AnimationCurves[0].length - 1];
                break;

            default:
                a_LastKeyframe =
                    a_AnimationCurves[0].length > a_AnimationCurves[1].length
                        ? a_AnimationCurves[0][a_AnimationCurves[0].length - 1]
                        : a_AnimationCurves[1][a_AnimationCurves[1].length - 1];
                break;
        }
        return true;
    }

    private static IEnumerator ParseAnimation(Keyframe a_Keyframe, AnimationCurve[] a_AnimationCurves, Animate a_Delegate)
    {
        float deltaTime = 0.0f;
        while (deltaTime < a_Keyframe.time)
        {
            deltaTime += Time.deltaTime;
            switch (a_AnimationCurves.Length)
            {
                case 1:
                    a_Delegate(a_AnimationCurves[0].Evaluate(deltaTime), a_AnimationCurves[0].Evaluate(deltaTime));
                    break;
                default:
                    a_Delegate(a_AnimationCurves[0].Evaluate(deltaTime), a_AnimationCurves[1].Evaluate(deltaTime));
                    break;
            }
            yield return false;
        }
    }
}
