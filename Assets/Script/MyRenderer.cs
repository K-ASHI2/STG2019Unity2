using System.Collections;
using UnityEngine;

namespace Assets.Script
{
    public class Renderer : MonoBehaviour
    {
        // 透過度 0～1で表される
        [SerializeField] private float transparency = 1.0f;

        private void Awake()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, transparency);
        }
    }
}