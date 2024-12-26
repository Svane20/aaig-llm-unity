using System;
using GameSystems.CustomEventSystems.Interaction;
using GameSystems.Dialogue;
using NonPlayerObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utilities;

namespace Dialogue.Objects
{
    public enum InteractableDirection{
        Up,
        Down,
        Left,
        Right,
        AllAround
    }
    
    [RequireComponent(typeof(ArcCollider2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class InteractableScript : MonoBehaviour
    {
        public InteractableDirection direction;
        public Vector2 offset;
        public Vector2 size;
        public int id;
        public float radius;
        public TextAsset json;


        private TextAsset _previousJson;
    
        private PolygonCollider2D _polygonCollider;
        private ArcCollider2D _arcCollider2D;
        private BoxCollider2D _boxCollider;
        private SpriteRenderer _spriteRenderer;
        private GameObject _dialogueCanvas;
        private DialogueUIManager _dialogueUIManager;
        private DialogueManager _dialogueManager;

        //Input system
        private PlayerActionControls _playerActionControls;

    
        // Start is called before the first frame update
        private void Start()
        {
            _dialogueUIManager = DialogueUIManager.Instance;
            _dialogueManager = DialogueManager.Instance;
            DialogueHandleUpdate.Instance.OnUpdateCanvas();
            Setup();
        }
        
        private void OnValidate()
        {
            UpdateJson();
        }

        public InteractableDirection GetInteractionDirection()
        {
            return direction;
        }

        public void UpdateJson()
        {
            if (json!=null)
            {
                if (_previousJson == null) _previousJson = new TextAsset();
                if (json.GetType() != typeof(TextAsset))
                    throw new InvalidOperationException("File can only be Text Assets");
                if (_previousJson.ToString().Equals(json.ToString()) ^ _previousJson.name == json.name) return;
                _previousJson = json;
                CustomUtils.PrettifyJson(json);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                InteractionHandler.Instance.OnShowBubble(id);
                DialogueHandleUpdate.Instance.OnUpdateJson(json);
                InteractionHandler.Instance.OnLookingAt(gameObject, true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                InteractionHandler.Instance.OnHideBubble(id);
                DialogueHandleUpdate.Instance.OnUnloadJson();
                InteractionHandler.Instance.OnLookingAt(null, false);
                DialogueUIHandler.Instance.OnExitDialogue();
            }
        }
        
        private void Setup()
        {
            // Init
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _arcCollider2D = GetComponent<ArcCollider2D>();
            _polygonCollider = GetComponent<PolygonCollider2D>();
            _dialogueCanvas = GameObject.Find("DialogueCanvas");
            _playerActionControls = new PlayerActionControls();

            //modify
            _arcCollider2D.PizzaSlice = true;
            switch (direction)
            {
                case InteractableDirection.Up:
                    SetDirection(radius, 10, 160);
                    break;
                case InteractableDirection.Down:
                    SetDirection(radius, 190, 160);
                    break;
                case InteractableDirection.Left:
                    SetDirection(radius, 100, 160);
                    break;
                case InteractableDirection.Right:
                    SetDirection(radius, 280, 160);
                    break;
                case InteractableDirection.AllAround:
                    SetDirection(radius, 280, 360);
                    break;
                
            }
            
            _polygonCollider.isTrigger = true;

            _boxCollider.offset = offset;
            _boxCollider.size = size;
        }

        private void SetDirection(float radius, int offset, int angle)
        {
            _arcCollider2D.Radius = radius;
            _arcCollider2D.OffsetRotation = offset;
            _arcCollider2D.TotalAngle = angle;
        }
        
    }
}




