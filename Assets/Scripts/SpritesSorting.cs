using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NonPlayerObjects
{
    public class SpritesSorting : MonoBehaviour
    {
        [SerializeField] private int sortingOrderBase = 5000;
        [SerializeField] private int offset = 0;
        [SerializeField] private bool runOnlyOnce = false;

        private Renderer _renderer;
        private GameObject _player;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _player = GameObject.Find("Player");
        }

        private void LateUpdate()
        { 
            switch (gameObject.name)
            {
                case "UpperPart":
                    CalculateOrder(50);
                    break;
                case "TreeMisc":
                    CalculateOrder(51);
                    break;
                case "TestSign":
                    CalculateOrder(0);
                    break;
                case "StartingLevelMisc":
                    CalculateOrder(0);
                    break; 
                default:
                    CalculateOrder(0);
                    break;
            }
            if (runOnlyOnce)
            {
                Destroy(this);
            }
        }

        private void CalculateOrder(int addition)
        {
            var container = transform.parent.transform.parent;
            if (_renderer.sortingOrder == _player.GetComponent<SpriteRenderer>().sortingOrder && container.gameObject.name == "Signs")
            {
                _renderer.sortingLayerName = "Sign";
            }
            if (gameObject.name != "Player")
            {
                _renderer.sortingOrder = (int) (sortingOrderBase - _player.transform.position.y - offset) + addition;
                return;
            }
        }
    }
}