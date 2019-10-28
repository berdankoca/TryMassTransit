using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace TryMassTransit.Shared
{
    public interface GetMessages
    {
        //Like a query string filter
        //text=deneme&age!=30
        string EndWithFilter { get; set; }
    }
}
