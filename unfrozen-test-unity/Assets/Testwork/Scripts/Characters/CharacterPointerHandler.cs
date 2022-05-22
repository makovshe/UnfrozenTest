using Testwork.Events;
using Testwork.Scripts.Characters.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Testwork.Scripts.Characters
{
    [RequireComponent(typeof(Character))]
    public sealed class CharacterPointerHandler: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private GameEventsManager _gameEventsManager;
        private Character _character;
        
        private void Awake()
        {
            _gameEventsManager = FindObjectOfType<GameEventsManager>();
            _character = GetComponent<Character>();
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _gameEventsManager.SendEvent(new CharacterPointerEnterEvent{Character = _character});
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _gameEventsManager.SendEvent(new CharacterPointerExitEvent{Character = _character});
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _gameEventsManager.SendEvent(new CharacterPointerClickEvent{Character = _character});
        }
    }
}