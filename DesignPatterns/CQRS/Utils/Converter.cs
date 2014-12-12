﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crucial.DesignPatterns.CQRS.Utils
{
    public static class Converter
    {
        public static Action<object> Convert<T>(Action<T> myActionT)
        {
            if (myActionT == null) return null;
            return new Action<object>(o => myActionT((T)o));
        }

        public static dynamic ChangeTo(dynamic source, Type dest)
        {

            return System.Convert.ChangeType(source, dest);
        }
    }
}
