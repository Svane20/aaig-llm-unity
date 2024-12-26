using System;
using GameSystems.CustomEventSystems.Interaction;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

namespace GameSystems.Dialogue
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class ShowBubble : MonoBehaviour
    {
        public int id;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private static readonly int Showing = Animator.StringToHash("Showing");
        private static readonly int Exit = Animator.StringToHash("Exit");
        private static readonly int Enter = Animator.StringToHash("Enter");

        void Start()
        {
            InteractionHandler.Instance.ShowBubble += Show;
            InteractionHandler.Instance.HideBubble += Hide;
        }

        private void OnEnable()
        {
            updareREF();
        }

        private void updareREF()
        {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _animator = gameObject.GetComponent<Animator>();
            _spriteRenderer.sortingOrder = 5;
        }

        private void OnDestroy()
        {
            _spriteRenderer = null;
            _animator = null;
            if (InteractionHandler.Instance != null)
            {
                InteractionHandler.Instance.ShowBubble -= Show;
                InteractionHandler.Instance.HideBubble -= Hide;
            }
        }

        private void Show(int id)
        {
            updareREF();
            if (this.id == id)
            {
                _animator.SetTrigger(Enter);
                _animator.SetBool(Showing, true);
            }
        }

        private void Hide(int id)
        {
            updareREF();
            if (this.id == id)
            {
                _animator.SetTrigger(Exit);
                _animator.SetBool(Showing, false);
            }
        }
    }
}
