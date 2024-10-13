using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtomotoWatcher.Console.Utils
{
    internal class ListingsManager
    {
        public List<Listing> Listings { get; private set; } = [];

        public ListingsManager(IDocument document)
        {
            if (string.IsNullOrEmpty(document.Origin))
                throw new Exception("Listings: document origin null");

            if (document.Origin.Contains("olx.pl", StringComparison.InvariantCultureIgnoreCase))
            {
                CreateOlx(document);
            }
            else if (document.Origin.Contains("otomoto.pl", StringComparison.InvariantCultureIgnoreCase))
            {
                CreateOtomoto(document);
            }
            else
            {
                throw new Exception("Listings: document origin invalid");
            }
        }

        private void CreateOtomoto(IDocument document)
        {

        }

        private void CreateOlx(IDocument document)
        {
            var eleContainer = document.QuerySelector("[data-testid='listing-grid']") 
                ?? throw new Exception("Listings: container not found");

            var eleListingAll = eleContainer.QuerySelectorAll("[data-testid='l-card']");
            foreach (var eleListing in eleListingAll)
            {
                var eleFavorite = eleListing.QuerySelector("[data-testid='adAddToFavorites']")
                    ?? throw new Exception("Listings: add favorite not found");

                var eleParent = eleFavorite.ParentElement
                    ?? throw new Exception("Listings: favorite parent not found");

                var eleHref = eleParent.QuerySelector<IHtmlAnchorElement>("a")
                    ?? throw new Exception("Listings: href not found");

                var href = eleHref.Href
                    ?? throw new Exception("Listings: href value null");

                var uri = new Uri(href);
                var lastChunk = uri.Segments.LastOrDefault()
                    ?? throw new Exception("Listings: href last chunk null");
                if (!lastChunk.EndsWith(".html"))
                    throw new Exception("Listings: href last chunk invalid");
                lastChunk = lastChunk.TrimEnd('l', 'm', 't', 'h', '.');

                Listings.Add(new Listing()
                {
                    Key = lastChunk,
                    Url = href
                });
            }
        }


        public class Listing
        {
            public required string Key { get; set; }
            public required string Url { get; set; }
        }
    }
}
