using System;
using System.IO;
using System.Linq;
using PublisherPlus.Data;
using PublisherPlus.Patch;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.Steam;

namespace PublisherPlus.Interface
{
    internal class Dialog_Publish : Window
    {
        private const float Padding = 12f;
        private const float ScrollBarWidth = 20f;
        private const float ButtonHeight = 50f;

        private readonly WorkshopPackage _pack;
        private Vector2 _scroll;
        public override Vector2 InitialSize => new Vector2(600f, 600f);

        private int _page;

        public Dialog_Publish(WorkshopItemHook hook)
        {
            _pack = new WorkshopPackage(hook);
            doCloseButton = false;
            doCloseX = true;
            absorbInputAroundWindow = true;
            closeOnClickedOutside = false;
        }

        private string GetTitle()
        {
            if (_page == 0) { return Lang.Get("Title.Details"); }
            if (_page == 1) { return Lang.Get("Title.Contents"); }
            if (_page == 2) { return Lang.Get("Title.Finalize"); }

            throw new ArgumentOutOfRangeException();
        }

        public override void OnCancelKeyPressed() => PreviousPage();

        private void NextPage()
        {
            SoundDefOf.Tick_High.PlayOneShotOnCamera();

            if (_page == 2)
            {
                Package();
                return;
            }
            _page++;
        }

        private void PreviousPage()
        {
            SoundDefOf.Tick_High.PlayOneShotOnCamera();

            if (_page == 0)
            {
                Close();
                return;
            }

            _page--;
        }

        public override void DoWindowContents(Rect inRect)
        {
            var previousFont = Text.Font;
            Text.Font = GameFont.Medium;
            var titleRect = new Rect(inRect.x, inRect.y, inRect.width, Text.LineHeight);
            Widgets.Label(titleRect, GetTitle());
            Text.Font = previousFont;
            Widgets.DrawLineHorizontal(titleRect.x, titleRect.yMax + (Padding / 2f), titleRect.width);

            var contentRect = new Rect(inRect.x, titleRect.yMax + Padding, inRect.width, inRect.height - (titleRect.height + (Padding * 2f) + ButtonHeight));

            if (_page == 0) { DoDetails(contentRect); }
            else if (_page == 1) { DoContents(contentRect); }
            else if (_page == 2) { DoFinalize(contentRect); }

            var buttonRect = new Rect(inRect.x, contentRect.yMax + Padding, inRect.width, ButtonHeight);
            var grid = new GridLayout(buttonRect, 6);

            if (WidgetsPlus.ButtonText(grid.GetCellRect(0, 0, 2), _page == 0 ? Lang.Get("Button.Close") : Lang.Get("Button.Back"))) { PreviousPage(); }
            if (WidgetsPlus.ButtonText(grid.GetCellRect(2, 0), Lang.Get("Button.Default"))) { ResetConfig(); }
            if (WidgetsPlus.ButtonText(grid.GetCellRect(3, 0), Lang.Get("Button.Save"))) { SaveConfig(); }
            if (WidgetsPlus.ButtonText(grid.GetCellRect(4, 0, 2), _page == 2 ? Lang.Get("Button.Publish") : Lang.Get("Button.Next"), _pack.HasContent())) { NextPage(); }
        }

        private void DoDetails(Rect rect)
        {
            var listing = new Listing_StandardPlus();
            listing.Begin(rect);
            listing.Gap();

            listing.Label(Lang.Get("FileId").Bold());
            listing.Label(_pack.Id.Italic());
            listing.GapLine();

            listing.Label(Lang.Get("Title").Bold());
            _pack.Title = listing.TextEntry(_pack.Title);
            const string experimentalMode = "*#exp#"; // Experimental Mode: Can load tags in xml
            if (_pack.Title.EndsWith(experimentalMode))
            {
                _pack.Title = _pack.Title.Substring(0, _pack.Title.Length - experimentalMode.Length);
                Mod.ExperimentalMode = true;
                Mod.Warning("Experimental Mode activated");
            }
            listing.GapLine();

            listing.Label(Lang.Get("Description").Bold() + (_pack.IsNewCreation ? null : Lang.Get("DescriptionLocked")));
            var description = listing.TextEntry(_pack.Description, 6);
            if (_pack.IsNewCreation) { _pack.Description = description; }
            listing.GapLine();

            listing.Label(Lang.Get("Tags").Bold());
            listing.Label(_pack.Tags.ToCommaList().Italic());
            listing.GapLine();

            listing.Label(Lang.Get("PreviewFile").Bold() + (_pack.PreviewExists ? null : Lang.Get("PreviewNotFound").Italic()));
            _pack.Preview = listing.TextEntry(_pack.Preview);

            listing.End();
        }

        private void DoContents(Rect rect)
        {
            var l = new Listing_StandardPlus();
            l.Begin(rect);
            l.Gap();
            l.Label(($"{Lang.Get("ContentDirectory")} ").Bold());
            l.Label(_pack.SourceDirectory.FullName.Italic());
            l.GapLine();
            l.End();


            var filterList = new Listing_StandardPlus();
            var filterRect = new Rect(rect.x, rect.y + l.CurHeight, rect.width, rect.height - l.CurHeight);

            var listingSize = (Text.LineHeight + filterList.verticalSpacing);
            var listingCount = _pack.AllContent.Count();
            var filterViewRect = new Rect(0f, 0f, rect.width - ScrollBarWidth, listingCount * listingSize);

            Widgets.BeginScrollView(filterRect, ref _scroll, filterViewRect);
            filterList.Begin(new Rect(0, _scroll.y,  filterViewRect.width, filterRect.height));

            //Scroller Performance Fix - Telefonmast
            var startIndex = (int)(_scroll.y / listingSize);
            var indexRange = Math.Min((int)(filterRect.height / listingSize) + 1, listingCount);
            var endIndex = startIndex + indexRange;

            if (startIndex >= 0 && endIndex <= listingCount)
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    var item = _pack.AllContent.ElementAt(i);
                    var path = _pack.GetRelativePath(item);
                    var isDirectory = item is DirectoryInfo;

                    var isIncluded = _pack.IsIncluded(item);
                    var include = filterList.CheckboxLabeled(isDirectory ? path.Bold() + "\\" : path, isIncluded, item.FullName, isIncluded ? (Color?)null : Color.red);
                    if (include != isIncluded) { _pack.SetIncluded(item, include); }
                }
            }

            filterList.End();
            Widgets.EndScrollView();
        }

        private void DoFinalize(Rect rect)
        {
            var l = new Listing_StandardPlus();
            l.Begin(rect);
            l.Gap();
            l.Label(Lang.Get("FinalInformation", _pack.Title.Bold(), _pack.Id.Bold()));
            l.End();
        }

        private void SaveConfig() => _pack.SaveConfig();
        private void ResetConfig() => _pack.ResetConfig();

        private void Package()
        {
            _pack.Upload();
            Close();
        }
    }
}
