using UnityEngine;

[System.Serializable]
public class AnimatorParameterInfo
{
    public string name;
    public int hash;
    public AnimatorControllerParameterType type;
    public int layer;
    public bool sync = true;
    public float floatThreshold = 0.01f; // small change threshold
    public bool interpolate = true; // for floats

    public AnimatorParameterInfo(string name, AnimatorControllerParameterType type, int layer = 0)
    {
        this.name = name;
        this.hash = Animator.StringToHash(name);
        this.type = type;
        this.layer = layer;
        this.sync = true;
    }
}