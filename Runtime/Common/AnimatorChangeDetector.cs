using System.Collections.Generic;
using UnityEngine;

public class AnimatorChangeDetector
{
    private readonly Animator animator;
    private readonly AnimatorControllerParameter[] parameters;

    private readonly float[] floatCache;
    private readonly int[] intCache;
    private readonly bool[] boolCache;

    public AnimatorChangeDetector(Animator anim)
    {
        animator = anim;
        parameters = animator.parameters;

        floatCache = new float[parameters.Length];
        intCache = new int[parameters.Length];
        boolCache = new bool[parameters.Length];

        // Initialize caches
        for (int i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];
            switch (p.type)
            {
                case AnimatorControllerParameterType.Float:
                    floatCache[i] = animator.GetFloat(p.nameHash);
                    break;
                case AnimatorControllerParameterType.Int:
                    intCache[i] = animator.GetInteger(p.nameHash);
                    break;
                case AnimatorControllerParameterType.Bool:
                    boolCache[i] = animator.GetBool(p.nameHash);
                    break;
            }
        }
    }

    public IEnumerable<(AnimatorControllerParameter param, int index)> AllParameters()
    {
        for (int i = 0; i < parameters.Length; i++)
        {
            yield return (parameters[i], i);
        }
    }

    public bool HasChanged(int index, float floatThreshold)
    {
        var p = parameters[index];
        switch (p.type)
        {
            case AnimatorControllerParameterType.Float:
                float f = animator.GetFloat(p.nameHash);
                if (Mathf.Abs(f - floatCache[index]) > floatThreshold)
                {
                    floatCache[index] = f;
                    return true;
                }
                return false;

            case AnimatorControllerParameterType.Int:
                int iv = animator.GetInteger(p.nameHash);
                if (iv != intCache[index])
                {
                    intCache[index] = iv;
                    return true;
                }
                return false;

            case AnimatorControllerParameterType.Bool:
                bool bv = animator.GetBool(p.nameHash);
                if (bv != boolCache[index])
                {
                    boolCache[index] = bv;
                    return true;
                }
                return false;

            default:
                return false;
        }
    }

    public object GetValue(int index)
    {
        var p = parameters[index];
        switch (p.type)
        {
            case AnimatorControllerParameterType.Float:
                return animator.GetFloat(p.nameHash);
            case AnimatorControllerParameterType.Int:
                return animator.GetInteger(p.nameHash);
            case AnimatorControllerParameterType.Bool:
                return animator.GetBool(p.nameHash);
            default:
                return null;
        }
    }
}
