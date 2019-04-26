using System;

namespace OstoslistaData
{
    public interface IHubItem : IHubItemBase
    {
        string Title { get; set; }
    }
}