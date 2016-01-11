using Cindeck.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindeck
{
    public static class Utils
    {
        public static SortDescription ToSortDescription(this SortOption option)
        {
            return new SortDescription { Direction = option.Direction, PropertyName = option.Column };
        }

        public static SortOption ToSortOption(this SortDescription desc)
        {
            return new SortOption { Direction = desc.Direction, Column = desc.PropertyName };
        }
    }
}
