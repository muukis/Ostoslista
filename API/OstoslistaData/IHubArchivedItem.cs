using System;

namespace OstoslistaData
{
    public interface IHubArchivedItem : IHubItem
    {
        DateTime? Archived { get; set; }
    }
}