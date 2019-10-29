using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace TryMassTransit.Shared
{
    public interface GetMessages
    {
        //Like a query string filter "text=deneme&age!=30"
        //or dynamic linq https://www.strathweb.com/2018/01/easy-way-to-create-a-c-lambda-expression-from-a-string-with-roslyn/
        string EndWithFilter { get; set; }
    }
}
