using Spine;
using Spine.Unity;
using Testwork.Events;
using UnityEngine;
using Event = Spine.Event;

namespace Testwork.Scripts.Characters
{
    [RequireComponent(typeof(SkeletonAnimation))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Character : MonoBehaviour
    {
        [SpineAnimation, SerializeField] private string _idleAnimationName;
        [SpineAnimation, SerializeField] private string _attackAnimationName;
        [SpineAnimation, SerializeField] private string _damagedAnimationName;
        [SerializeField] private string _attackHitEventName;
        [SerializeField] private int _attackOrderInLayer;

        public bool IsControlledByPlayer { get; private set; }
        public bool IsLeft { get; private set; }
        
        private SkeletonAnimation _skeletonAnimation;
        private Spine.AnimationState _spineAnimationState;
        private Skeleton _skeleton;
        private MeshRenderer _meshRenderer;
        private int _defaultOrderInLayer;

        private GameEventsManager _gameEventsManager;
    
        private void Awake()
        {
            _skeletonAnimation = GetComponent<SkeletonAnimation>();
            _spineAnimationState = _skeletonAnimation.AnimationState;
            _skeleton = _skeletonAnimation.Skeleton;
            _spineAnimationState.Event += SpineAnimationStateOnEvent;

            _gameEventsManager = FindObjectOfType<GameEventsManager>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _defaultOrderInLayer = _meshRenderer.sortingOrder;
            IsLeft = true;
        }

        public void PlayIdle()
        {
            var trackEntry = _spineAnimationState.SetAnimation(0, _idleAnimationName, true);
            _spineAnimationState.Update(UnityEngine.Random.Range(0, trackEntry.Animation.Duration));
        }

        public float PlayAttack()
        {
            return _spineAnimationState.SetAnimation(0, _attackAnimationName, false).Animation.Duration;
        }

        private void SpineAnimationStateOnEvent(TrackEntry trackentry, Event e)
        {
            if (e.Data.Name.Equals(_attackHitEventName))
            {
                _gameEventsManager.SendEvent(new AttackHitAnimationEvent());
            }
        }

        public float PlayDamaged()
        {
            return _spineAnimationState.SetAnimation(0, _damagedAnimationName, false).Animation.Duration;
        }
        
        public void FlipSkeleton()
        {
            _skeleton.ScaleX = -_skeleton.ScaleX;
            IsLeft = false;
        }

        public void SetControlledByPlayer(bool controlled)
        {
            IsControlledByPlayer = controlled;
        }

        public void SetAttackOrderInLayer()
        {
            _meshRenderer.sortingOrder = _attackOrderInLayer;
        }

        public void SetDefaultOrderInLayer()
        {
            _meshRenderer.sortingOrder = _defaultOrderInLayer;
        }
    }
}
