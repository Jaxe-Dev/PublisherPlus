using UnityEngine;
using Verse;

namespace PublisherPlus.Interface
{
    internal class Listing_StandardPlus : Listing_Standard
    {
        public bool CheckboxLabeled(string text, bool value, string tooltip, Color? color = null)
        {
            var previousColor = GUI.color;
            if (color != null) { GUI.color = color.Value; }
            var check = value;
            CheckboxLabeled(text, ref check, tooltip);
            GUI.color = previousColor;
            return check;
        }
    }
}
