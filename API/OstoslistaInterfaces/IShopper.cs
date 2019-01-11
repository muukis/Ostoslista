using System;

namespace OstoslistaInterfaces
{
    public interface IShopper
    {
        DateTime? Created { get; set; }
        Guid? Id { get; set; }
        DateTime? Modified { get; set; }
        string Name { get; set; }
    }
}