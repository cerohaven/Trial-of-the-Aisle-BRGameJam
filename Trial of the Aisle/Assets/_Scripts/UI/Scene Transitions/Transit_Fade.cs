using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transit_Fade : MonoBehaviour, ISceneTransition
{
    [SerializeField] private AnimationClip _enterSceneAnimationClip;
    [SerializeField] private AnimationClip _exitSceneAnimationClip;
    public AnimationClip EnterSceneAnimationClip { get { return _enterSceneAnimationClip; } }

    public AnimationClip ExitSceneAnimationClip { get { return _exitSceneAnimationClip; } }

    public void Initialize()
    {
        
    }
}
