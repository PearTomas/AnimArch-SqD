using System;
using UnityEngine;

namespace Visualization.ClassDiagram.Diagrams
{
    public class SpriteChanger : MonoBehaviour
    {
        [SerializeField] private Sprite selectedSprite;

        private void Start()
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = selectedSprite;
        }
    }
}