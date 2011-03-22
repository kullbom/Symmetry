// Copyright 2011 Johan Kullbom (see the file LICENSE)

namespace Symmetry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class StringExt {

        public static String Concat(this IEnumerable<String> that) {
            return 
                that.Aggregate(new StringBuilder(), (sb, str) => sb.Append(str))
                   .ToString();
        }
    }
}
