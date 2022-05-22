using Testwork.Events;
using Testwork.Scripts;
using Testwork.Scripts.Battle.Events;
using Testwork.Ui.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Testwork.Ui
{
    [RequireComponent(typeof(Button))]
    public abstract class PlayerButtonUi<T> : MonoBehaviour, IGameEventListener<CharacterTurnEvent>,
        IGameEventListener<CharacterAttackEvent>, IGameEventListener<PlayerButtonEvent> where T : IGameEvent, new()
    {
        [SerializeField] private Sprite _pressedStateSprite;
        
        private GameEventsManager _eventsManager;
        private Button _button;
        private Sprite _defaultSprite;

        private void Awake()
        {
            _eventsManager = FindObjectOfType<GameEventsManager>();
            _eventsManager.AddListener<CharacterTurnEvent>(this);
            _eventsManager.AddListener<CharacterAttackEvent>(this);
            _eventsManager.AddListener<PlayerButtonEvent>(this);
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonDown);
            _defaultSprite = _button.image.sprite;
        }

        public void OnEvent(CharacterTurnEvent gameEvent)
        {
            _button.interactable = gameEvent.Character.IsControlledByPlayer;
            SetDefaultSprite();
        }

        private void SetDefaultSprite()
        {
            _button.image.sprite = _defaultSprite;
        }

        private void SetPressedSprite()
        {
            _button.image.sprite = _pressedStateSprite;
        }
        
        public void OnEvent(CharacterAttackEvent gameEvent)
        {
            _button.interactable = false;
        }
        
        public void OnEvent(PlayerButtonEvent gameEvent)
        {
            if (gameEvent.Source != gameObject)
            {
                SetDefaultSprite();
            }
        }

        private void OnButtonDown()
        {
            _eventsManager.SendEvent(new T());
            _eventsManager.SendEvent(new PlayerButtonEvent{Source = gameObject});
            SetPressedSprite();
        }

        private void OnDestroy()
        {
            _eventsManager.RemoveListener<CharacterTurnEvent>(this);
            _eventsManager.RemoveListener<CharacterAttackEvent>(this);
            _eventsManager.RemoveListener<PlayerButtonEvent>(this);
        }
    }
}