using Spine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.Unity;

public class Character : MonoBehaviour
{
    [SpineAnimation, SerializeField] private string _idleAnimationName;
    [SpineAnimation, SerializeField] private string _attackAnimationName;
    [SpineAnimation, SerializeField] private string _damagedAnimationName;
    
    private SkeletonAnimation _skeletonAnimation;
    private Spine.AnimationState _spineAnimationState;
    private Skeleton _skeleton;
    
    private void Awake()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        _spineAnimationState = _skeletonAnimation.AnimationState;
        _skeleton = _skeletonAnimation.Skeleton;
    }

    public void PlayIdle()
    {
        _spineAnimationState.SetAnimation(0, _idleAnimationName, true);
    }

    public void FlipSkeleton()
    {
        _skeleton.ScaleX = -_skeleton.ScaleX;
    }
}
