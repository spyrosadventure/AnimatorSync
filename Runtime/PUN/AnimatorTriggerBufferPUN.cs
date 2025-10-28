using System.Collections.Generic;
using UnityEngine;

public class AnimatorTriggerBufferPUN
{
    private const int BUFFER_LIMIT = 32;
    private readonly Animator animator;
    private readonly Queue<int> triggerQueue = new Queue<int>(BUFFER_LIMIT);
    private readonly Dictionary<int, string> hashToName = new Dictionary<int, string>();

    public AnimatorTriggerBufferPUN(Animator animator)
    {
        this.animator = animator;
        foreach (var p in animator.parameters)
        {
            if (p.type == AnimatorControllerParameterType.Trigger)
                hashToName[p.nameHash] = p.name;
        }
    }

    public void AddTrigger(int hash)
    {
        if (triggerQueue.Count >= BUFFER_LIMIT)
            triggerQueue.Dequeue();


        triggerQueue.Enqueue(hash);
        ApplyTrigger(hash);
    }

    public void ReceiveTrigger(int hash)
    {
        if (hashToName.TryGetValue(hash, out string name))
        {
            animator.SetTrigger(name);
        }
    }

    private void ApplyTrigger(int hash)
    {
        if (hashToName.TryGetValue(hash, out string name))
        {
            animator.SetTrigger(name);
        }
    }
}