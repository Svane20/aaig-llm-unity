using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameSystems.CustomEventSystems.Tutorial;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Tutorial
{
    public class TutorialUI : MonoBehaviour
    {
        [SerializeField]
        private List<Sprite> unPressed;
        [SerializeField]
        private List<Sprite> pres;
        [SerializeField]
        private List<Sprite> pressed;
        private Dictionary<string, Image> _childImageComponents = new Dictionary<string, Image>();
        private Dictionary<string, Sprite> _unPressedSprite = new Dictionary<string, Sprite>();
        private Dictionary<string, Sprite> _presSprite = new Dictionary<string, Sprite>();
        private Dictionary<string, Sprite> _pressedSprite = new Dictionary<string, Sprite>();
        private Dictionary<string, bool> _hasBeenPressed = new Dictionary<string, bool>();
        private string _imgReplace;

        // private void Start()
        // {
        //     TutorialHandler.Instance.TutorialButtonPressed += ButtonPressed;
        //     ChildImageComponentsToDictionary();
        // }

        private void OnEnable()
        {
            TutorialHandler.Instance.TutorialButtonPressed += ButtonPressed;
            ChildImageComponentsToDictionary();
        }


        private void OnDisable()
        {
            if (TutorialHandler.Instance != null)
            {
                TutorialHandler.Instance.TutorialButtonPressed -= ButtonPressed;
            }
        }
        
        private void ButtonPressed(InputAction.CallbackContext ctx)
        {
            if (ctx.action.bindings.Count <= 0) return;
            if (ctx.action.bindings[0].name == "WASD")
            {
                switch (ctx.ReadValue<Vector2>().ToString())
                {
                    case "(0.0, 1.0)":
                        _imgReplace = "ForwardButton";
                        SpriteUpdate(_imgReplace, ctx);
                        break;
                    case "(1.0, 0.0)":
                        _imgReplace = "RightButton";
                        SpriteUpdate(_imgReplace, ctx);
                        break;
                    case "(-1.0, 0.0)":
                        _imgReplace = "LeftButton";
                        SpriteUpdate(_imgReplace, ctx);
                        break;
                    case "(0.0, -1.0)":
                        _imgReplace = "BackwardsButton";
                        SpriteUpdate(_imgReplace, ctx);
                        break;
                }
                if (ctx.canceled && _imgReplace != null && _hasBeenPressed[_imgReplace])
                {
                    ButtonCancel(_imgReplace);
                }
            }
            else
            {
                SpriteUpdate("InteractButton", ctx);
            }

        }

        private void SpriteUpdate(string imgReplace, InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                _childImageComponents[imgReplace].sprite = _presSprite[imgReplace];
                _hasBeenPressed[imgReplace] = true;
            }
            if (ctx.canceled && _hasBeenPressed[imgReplace])
            {
                ButtonCancel(imgReplace);
            }


        }

        private void ButtonCancel(string imgReplace)
        {
            _childImageComponents[imgReplace].sprite = _pressedSprite[imgReplace];

            if (!_hasBeenPressed.ContainsValue(false))
            {
                Destroy(transform.gameObject);
            }
        }

        private void ChildImageComponentsToDictionary()
        {
            var childrenImages = gameObject.GetComponentsInChildren<Image>().Where(
                x=> x.gameObject.transform.parent.name.Contains("Button")).ToArray();
            for (var index = 0; index < childrenImages.Count(); index++)
            {
                var image = childrenImages[index];
                var parentName = image.gameObject.transform.parent.name;
                if (!_childImageComponents.ContainsKey(parentName))
                {
                    _childImageComponents.Add(parentName, image);
                }
                if (!_unPressedSprite.ContainsKey(parentName))
                {
                    _unPressedSprite.Add(parentName, unPressed[index]);
                }
                if (!_presSprite.ContainsKey(parentName))
                {
                    _presSprite.Add(parentName, pres[index]);
                }
                if (!_pressedSprite.ContainsKey(parentName))
                {
                    _pressedSprite.Add(parentName, pressed[index]);
                }
                if (!_hasBeenPressed.ContainsKey(parentName))
                {
                    _hasBeenPressed.Add(parentName, false);
                }
            }
        }

    }
}