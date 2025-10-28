using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(Animator), typeof(PhotonView))]
public class AnimatorSyncPUN : MonoBehaviour, IPunObservable
{
    [Header("Sync Settings")]
    public bool ownerWrites = true;
    public float sendRate = 10f;
    public bool interpolateFloats = true;

    private Animator animator;
    private PhotonView photonView;
    private AnimatorControllerParameter[] parameters;

    private Dictionary<int, float> floatTargets = new Dictionary<int, float>();
    private Dictionary<int, float> floatCurrents = new Dictionary<int, float>();
    private AnimatorTriggerBufferPUN triggerBuffer;

    private float lastSendTime;

    void Awake()
    {
        animator = GetComponent<Animator>();
        photonView = GetComponent<PhotonView>();
        parameters = animator.parameters;
        triggerBuffer = new AnimatorTriggerBufferPUN(animator);
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            if (!interpolateFloats) return;
            foreach (var kvp in floatTargets)
            {
                int hash = kvp.Key;
                float target = kvp.Value;
                float current = floatCurrents.ContainsKey(hash) ? floatCurrents[hash] : animator.GetFloat(hash);
                float next = Mathf.Lerp(current, target, Time.deltaTime * sendRate);
                animator.SetFloat(hash, next);
                floatCurrents[hash] = next;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && (!ownerWrites || photonView.IsMine))
        {
            foreach (var p in parameters)
            {
                switch (p.type)
                {
                    case AnimatorControllerParameterType.Float:
                        stream.SendNext(animator.GetFloat(p.nameHash));
                        break;
                    case AnimatorControllerParameterType.Int:
                        stream.SendNext(animator.GetInteger(p.nameHash));
                        break;
                    case AnimatorControllerParameterType.Bool:
                        stream.SendNext(animator.GetBool(p.nameHash));
                        break;
                }
            }
        }
        else if (stream.IsReading)
        {
            foreach (var p in parameters)
            {
                switch (p.type)
                {
                    case AnimatorControllerParameterType.Float:
                        float f = (float)stream.ReceiveNext();
                        if (interpolateFloats)
                            floatTargets[p.nameHash] = f;
                        else
                            animator.SetFloat(p.nameHash, f);
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(p.nameHash, (int)stream.ReceiveNext());
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(p.nameHash, (bool)stream.ReceiveNext());
                        break;
                }
            }
        }
    }

    public void SetTrigger(string triggerName)
    {
        int hash = Animator.StringToHash(triggerName);
        triggerBuffer.AddTrigger(hash);
        photonView.RPC(nameof(RPC_Trigger), RpcTarget.Others, hash);
    }

    [PunRPC]
    void RPC_Trigger(int hash)
    {
        triggerBuffer.ReceiveTrigger(hash);
    }
}