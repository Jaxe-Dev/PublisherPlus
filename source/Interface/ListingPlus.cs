using UnityEngine;
using Verse;

namespace PublisherPlus.Interface
{
  internal class Listing_StandardPlus : Listing_Standard
  {
    private static readonly GUIContent TextContent = new GUIContent();

    public bool CheckboxLabeled(string label, bool value, string tooltip, Color? color = null)
    {
      var previousColor = GUI.color;
      if (color != null) { GUI.color = color.Value; }
      var check = value;

      TextContent.text = label;
      var rect = GetRect(Text.CurFontStyle.CalcHeight(TextContent, ColumnWidth));

      if (!BoundingRectCached.HasValue || rect.Overlaps(BoundingRectCached.Value))
      {
        if (!tooltip.NullOrEmpty())
        {
          if (Mouse.IsOver(rect)) { Widgets.DrawHighlight(rect); }
          TooltipHandler.TipRegion(rect, tooltip);
        }
        Widgets.CheckboxLabeled(rect, label, ref check);
      }
      Gap(verticalSpacing);

      GUI.color = previousColor;
      return check;
    }
  }
}
