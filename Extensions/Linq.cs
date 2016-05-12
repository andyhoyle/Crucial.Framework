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
            var instanceCount = instance.Count();
            return instance.Select((item, index) => { return (index == (instanceCount - 1)) || isAdjacentFunc(selector.Invoke(item), selector.Invoke(instance.ElementAt(index + 1))); }).All(b => b);
        }
    }
    
    public static TOutput Calculate<T, TResult, TOutput>(
            this IEnumerable<T> instance,
            Func<T, TResult> selector,
            Func<IEnumerable<T>, Func<T, TResult>, TResult> firstSelector,
            Func<IEnumerable<T>, Func<T, TResult>, TResult> lastSelector,
            Func<TResult, TResult, TOutput> outputFunc)
        {
            return outputFunc.Invoke(firstSelector.Invoke(instance, selector.Invoke), lastSelector.Invoke(instance, selector.Invoke));
        }
}
