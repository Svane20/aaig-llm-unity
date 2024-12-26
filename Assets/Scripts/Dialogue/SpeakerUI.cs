using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Image = UnityEngine.UI.Image;

namespace Dialogue
{
    public class SpeakerUI : MonoBehaviour
    {
        private Image _image;
        private Text _characterName;
        private Text _dialogue;

        public string CharacterName
        {
            get => _characterName.text;
            set => _characterName.text = value;
        }

        public string Dialogue
        {
            get => _dialogue.text;
            set => _dialogue.text = value;
        }

        private void Start()
        {
            var componentList = GetComponentsInChildren<Component>().ToList();
            _image = CustomUtils.InstanceOfType <Image> (componentList);
            _characterName = CustomUtils.InstanceOfType <Text> (componentList, "Name");
            _dialogue = CustomUtils.InstanceOfType <Text> (componentList, "Dialogue");
        }
        
        
        
        

        

        
    }
}