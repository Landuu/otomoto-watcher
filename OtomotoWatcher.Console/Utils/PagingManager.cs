using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Web;

namespace OtomotoWatcher.Console.Utils
{
    internal class PagingManager
    {
        private bool _backwardsButtonDisabled = false;
        private bool _forwardsButtonDisabled = false;

        public List<PageData> Pages { get; private set; } = [];

        public PagingManager(IDocument document)
        {
            if (string.IsNullOrEmpty(document.Origin))
                throw new Exception("Paging: document origin null");

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
                throw new Exception("Paging: document origin invalid");
            }

            var uri = new Uri(document.Url);
            var queryStringValues = HttpUtility.ParseQueryString(uri.Query);
            var queryPage = queryStringValues.GetValues("page")?.SingleOrDefault();
            if (string.IsNullOrWhiteSpace(queryPage))
                throw new Exception("Paging: current page from url missing");
            int currentPage = int.Parse(queryPage);

            Pages = Pages.OrderBy(x => x.PageNumber).ToList();
            for (int i = 0; i < Pages.Count; i++)
            {
                var page = Pages[i];

                if (page.PageNumber == 1)
                    page.PageType = PageType.First;

                if (page.PageNumber == currentPage)
                {
                    page.PageStatus = PageStatus.Current;


                    if (i > 0)
                        Pages[i - 1].PageStatus = PageStatus.Previous;

                    if (i + 1 < Pages.Count)
                        Pages[i + 1].PageStatus = PageStatus.Next;
                    else if (_forwardsButtonDisabled)
                        page.PageType = PageType.Last;
                }
            }


        }

        private void CreateDefault(IDocument document)
        {
            Pages.Add(new PageData()
            {
                PageNumber = 1,
                Url = document.Url,
                PageType = PageType.First,
                PageStatus = PageStatus.Current
            });
        }

        private void CreateOtomoto(IDocument document)
        {
            var elePagerAll = document.QuerySelectorAll("[data-testid='pagination-list']");
            var elePager = elePagerAll.SingleOrDefault();

            if (elePager == null)
            {
                CreateDefault(document);
                return;
            }

            var elePageButtons = elePager.GetElementsByClassName("pagination-item");

            foreach (var eleButton in elePageButtons)
            {
                bool skip = false;
                var cst = (IHtmlElement)eleButton;

                if (cst.Dataset.Any(x => x.Key == "testid" && x.Value == "pagination-step-backwards"))
                {
                    skip = true;
                    _backwardsButtonDisabled = cst.GetAttribute("aria-disabled") == "true";
                }

                if (cst.Dataset.Any(x => x.Key == "testid" && x.Value == "pagination-step-forwards"))
                {
                    skip = true;
                    _forwardsButtonDisabled = cst.GetAttribute("aria-disabled") == "true";
                }

                if (skip)
                    continue;

                var eleSpan = eleButton.GetElementsByTagName("span");
                var spanValue = eleSpan.Single().TextContent;
                var pageNumber = int.Parse(spanValue);

                var hrefUri = eleButton
                    .GetElementsByTagName("a")
                    .SingleOrDefault()?
                    .GetAttribute("href");

                if (string.IsNullOrEmpty(hrefUri))
                    throw new Exception("Paging: href uri missing");

                InnerAddPage(document, pageNumber, hrefUri);
            }
        }

        private void CreateOlx(IDocument document)
        {
            var elePagerAll = document.QuerySelectorAll("[data-testid='pagination-list']");
            var elePager = elePagerAll.SingleOrDefault();

            if (elePager == null)
            {
                CreateDefault(document);
                return;
            }

            if (elePager.QuerySelector("[data-testid='pagination-back']") == null)
                _backwardsButtonDisabled = true;

            if (elePager.QuerySelector("[data-testid='pagination-forward']") == null)
                _forwardsButtonDisabled = true;

            var elePageButtons = elePager.GetElementsByClassName("pagination-item");

            foreach(var eleButton in elePageButtons)
            {
                var eleHref = eleButton.GetElementsByTagName("a").Single();
                var innerValue = eleHref.TextContent;
                var pageNumber = int.Parse(innerValue);
                var hrefUri = eleHref?.GetAttribute("href");

                if (string.IsNullOrEmpty(hrefUri))
                    throw new Exception("Paging: href uri missing");

                InnerAddPage(document, pageNumber, hrefUri);
            }
        }

        private void InnerAddPage(IDocument document, int pageNumber, string hrefUri)
        {
            Pages.Add(new PageData()
            {
                PageNumber = pageNumber,
                Url = document.Origin + hrefUri,
                PageStatus = PageStatus.None,
                PageType = PageType.None
            });
        }

        public class PageData
        {
            public required int PageNumber { get; set; }
            public required string Url { get; set; }
            public required PageType PageType { get; set; }
            public required PageStatus PageStatus { get; set; }
        }

        public enum PageStatus
        {
            None = 0,
            Current = 1,
            Next = 2,
            Previous = 3
        }

        public enum PageType
        {
            None = 0,
            First = 1,
            Last = 2
        }
    }
}
