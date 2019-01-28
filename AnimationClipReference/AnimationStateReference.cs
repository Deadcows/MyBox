using System;
using UnityEngine;

[Serializable]
public class AnimationStateReference
{
    public string StateName
    {
        get { return _stateName; }
    }

    public bool Assigned
    {
        get { return _assigned; }
    }

    [SerializeField] private string _stateName = String.Empty;
    [SerializeField] private bool _assigned;
}

public static class AnimationStateReferenceExtension
{
    public static void Play(this Animator animator, AnimationStateReference state)
    {
        if (!state.Assigned) return;
        animator.Play(state.StateName);
    }
}