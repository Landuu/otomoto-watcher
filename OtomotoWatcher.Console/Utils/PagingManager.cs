using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtomotoWatcher.Console.Utils
{
    internal class PagingManager
    {
        public List<PageData> Pages { get; private set; } = [];

        public PagingManager(IDocument document)
        {
            var elePagerAll = document.QuerySelectorAll("[data-testid='pagination-list']");
            var elePager = elePagerAll.SingleOrDefault();

            if (elePager != null)
            {
                var elePageButtons = elePager.GetElementsByClassName("pagination-item");
                bool backwardsButtonDisabled = false;
                bool forwardsButtonDisabled = false;

                foreach (var eleButton in elePageButtons)
                {
                    bool skip = false;
                    var cst = (IHtmlElement)eleButton;

                    if (cst.Dataset.Any(x => x.Key == "testid" && x.Value == "pagination-step-backwards"))
                    {
                        skip = true;
                        backwardsButtonDisabled = cst.GetAttribute("aria-disabled") == "true";
                    }

                    if (cst.Dataset.Any(x => x.Key == "testid" && x.Value == "pagination-step-forwards"))
                    {
                        skip = true;
                        forwardsButtonDisabled = cst.GetAttribute("aria-disabled") == "true";
                    }

                    if (skip)
                        continue;

                    var eleSpan = eleButton.GetElementsByTagName("span");
                    var spanValue = eleSpan.Single().TextContent;
                    var pageNumber = int.Parse(spanValue);
                    var active = eleButton.ClassList.Any(x => x == "pagination-item__active");

                    var hrefUri = eleButton
                        .GetElementsByTagName("a")
                        .SingleOrDefault()?
                        .GetAttribute("href");

                    if (string.IsNullOrEmpty(hrefUri))
                        throw new Exception("Paging: href uri missing");

                    Pages.Add(new PageData()
                    {
                        PageNumber = pageNumber,
                        Url = document.BaseUri + hrefUri,
                        PageStatus = active ? PageStatus.Current : PageStatus.None,
                        PageType = pageNumber == 1 ? PageType.First : PageType.None
                    });
                }

                if (!Pages.Any(x => x.PageStatus == PageStatus.Current))
                    throw new Exception("Paging: current page missing");

                Pages = Pages.OrderBy(x => x.PageNumber).ToList();
                for (int i = 0; i < Pages.Count; i++)
                {
                    var page = Pages[i];
                    if(page.PageStatus == PageStatus.Current)
                    {
                        if(i > 0)
                            Pages[i - 1].PageStatus = PageStatus.Previous;

                        if (i + 1 < Pages.Count)
                            Pages[i + 1].PageStatus = PageStatus.Next;
                        else if (forwardsButtonDisabled)
                            page.PageType = PageType.Last;
                    }
                }
            }
            else
            {
                 Pages.Add(new PageData()
                 {
                     PageNumber = 1,
                     Url = document.Url,
                     PageType = PageType.First,
                     PageStatus = PageStatus.Current
                 });
            }
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
