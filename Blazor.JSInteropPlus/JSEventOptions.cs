using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.JSInteropPlus
{
    public class JSEventOptions
    {
        /// <summary>
        /// Specifies how the JS Event will invoke the C# Method
        /// </summary>
        public Callback CallbackType { get; set; }
        public TimeSpan CallbackTime { get; set; }
    }

    public enum Callback
    {
        /// <summary>
        /// The C# Event will immediatly be triggered once the JS Event happens
        /// </summary>
        Instant,
        /// <summary>
        /// The C# Event will be triggered in a fixed interval
        /// </summary>
        Throttle,
        /// <summary>
        /// The C# Event will only be triggered if there are no more JS Events in a specified interval
        /// </summary>
        Debounce
    }
}
