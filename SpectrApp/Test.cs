using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrApp
{
    static class Test
    {
        public static void InvokeEx<T>(this T @this, Action<T> action) where T : ISynchronizeInvoke
        {
            if (@this.InvokeRequired)
            {
                @this.BeginInvoke(action, new object[] { @this });
            }
            else
            {
                action(@this);
            }
        }
    }
}
