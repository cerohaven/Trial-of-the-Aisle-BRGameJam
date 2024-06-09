using UnityEngine;

public interface ISceneTransition
{
    AnimationClip EnterSceneAnimationClip { get; }
    AnimationClip ExitSceneAnimationClip { get; }

    void Initialize();
}