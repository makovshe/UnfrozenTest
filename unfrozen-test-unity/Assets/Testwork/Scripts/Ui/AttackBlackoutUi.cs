using System.Collections;
using Testwork.Events;
using Testwork.Scripts;
using Testwork.Scripts.Battle.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Testwork.Ui
{
    [RequireComponent(typeof(Image))]
    public sealed class AttackBlackoutUi : MonoBehaviour, IGameEventListener<CharacterAttackEvent>, IGameEventListener<AfterAttackEvent>
    {
        [SerializeField] private float _switchProcessTime = 0.5f;
        [SerializeField] private float _maxAlpha;
        
        private GameEventsManager _eventsManager;
        private Image _image;
        private IEnumerator _coroutine;
        private bool _isProcessing;

        private void Awake()
        {
            _eventsManager = FindObjectOfType<GameEventsManager>();
            _image = GetComponent<Image>();
            _eventsManager.AddListener<CharacterAttackEvent>(this);
            _eventsManager.AddListener<AfterAttackEvent>(this);

            SetAlpha(0);
        }

        public void OnEvent(CharacterAttackEvent gameEvent)
        {
            if (_isProcessing)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = ProcessAlpha(_maxAlpha, true);
            StartCoroutine(_coroutine);
        }
        
        private void SetAlpha(float value)
        {
            var color = _image.color;
            color.a = value;
            _image.color = color;
        }
        
        private IEnumerator ProcessAlpha(float targetAlpha, bool increase)
        {
            _isProcessing = true;
            var time = _switchProcessTime;
            var speed = _maxAlpha / time;
            while (time >= 0)
            {
                var delta = Time.deltaTime;
                if (increase)
                {
                    SetAlpha(_image.color.a + (delta * speed));
                }
                else
                {
                    SetAlpha(_image.color.a - (delta * speed));
                }
                time -= delta;
                yield return null;
            }
            SetAlpha(targetAlpha);
            _isProcessing = false;
        }
        
        public void OnEvent(AfterAttackEvent gameEvent)
        {
            if (_isProcessing)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = ProcessAlpha(0, false);
            StartCoroutine(_coroutine);
        }

        private void OnDestroy()
        {
            _eventsManager.RemoveListener<CharacterAttackEvent>(this);
            _eventsManager.RemoveListener<AfterAttackEvent>(this);
        }
    }
}