using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crucial.Framework.Extensions
{
    public static class Linq
    {
        public static bool ConsecutiveValidator<T, TResult>(
            this IEnumerable<T> instance,
            Func<T, TResult> selector,
            Func<TResult, TResult, bool> isAdjacentFunc)
        {
            return instance.Select((item, index) => { return (index == (instance.Count() - 1)) || isAdjacentFunc(selector.Invoke(item), selector.Invoke(instance.ElementAt(index + 1))); }).All(b => b);
        }
    }
}
