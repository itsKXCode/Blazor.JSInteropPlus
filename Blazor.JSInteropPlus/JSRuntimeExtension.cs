using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Blazor.JSInteropPlus
{
    public static class JSRuntimeExtension
    {
        internal static List<CachedEvent> _eventCache = new List<CachedEvent>();

        /// <summary>
        /// Subscribe to a Event of the JS Window Object
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="eventName">The name of the Javascript Event, for a list of Events see <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window">HERE</see></param>
        /// <param name="eventToCall">The C# Event to be called</param>
        /// <param name="jSEventOptions">Options, how the Event will be called</param>
        /// <returns></returns>
        public static async Task SubscribeToWindowEventAsync(this IJSRuntime jsRuntime, string eventName, Func<Task> eventToCall, JSEventOptions? jSEventOptions = null)
        {
            var eventId = Guid.NewGuid();

            await jsRuntime.InvokeVoidAsync("JSInteropPlus.SubscribeToWindowEvent", eventName, eventId.ToString(), jSEventOptions?.CallbackType, jSEventOptions?.CallbackTime.TotalMilliseconds);

            _eventCache.Add(new CachedEvent() { Id = eventId, Event = eventToCall, EventName = eventName });
        }

        /// <summary>
        /// Subscribe to a Event of a JS Element
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="element">The Element to add the Eventlistener to</param>
        /// <param name="eventName">The name of the Javascript Event</param>
        /// <param name="eventToCall">The C# Event to be called</param>
        /// <param name="jSEventOptions">Options, how the Event will be called</param>
        /// <returns></returns>
        public static async Task SubscribeToElementEventAsync(this IJSRuntime jsRuntime, ElementReference element, string eventName, Func<Task> eventToCall, JSEventOptions? jSEventOptions = null)
        {
            var eventId = Guid.NewGuid();

            await jsRuntime.InvokeVoidAsync("JSInteropPlus.SubscribeToElementEvent", element, eventName, eventId.ToString(), jSEventOptions?.CallbackType, jSEventOptions?.CallbackTime.TotalMilliseconds);

            _eventCache.Add(new CachedEvent() { Id = eventId, Event = eventToCall, Element = element, EventName = eventName });
        }

        /// <summary>
        /// Unsubscribe from a Event of the JS Window Object
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="eventName">The name of the Javascript Event</param>
        /// <param name="eventToCall">The C# Event which was used in "SubscribeToWindowEventAsync"</param>
        public static void UnsubscribeFromWindowEventAsync(this IJSRuntime jsRuntime, string eventName, Func<Task> eventToCall)
        {
            var cachedEvent = _eventCache.FirstOrDefault(x => x.Element == null && x.EventName == eventName && x.Event == eventToCall);

            if (cachedEvent == null)
                return;

            //Remove it only from the list, the next time JavaScript tries to call the Event, it will remove it, with this Method
            //you can remove the Event in an Dispose function (where JS calls wouldnt be possible)
            _eventCache.Remove(cachedEvent);

        }

        /// <summary>
        /// Unsubscribe from a Event of a JS Element
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="element">The Element from which to remove the Eventlistener</param>
        /// <param name="eventName">The name of the Javascript Event</param>
        /// <param name="eventToCall">The C# Event which was used in "SubscribeToElementEventAsync"</param>
        public static void UnsubscribeFromElementEventAsync(this IJSRuntime jsRuntime, ElementReference element, string eventName, Func<Task> eventToCall)
        {
            var cachedEvent = _eventCache.FirstOrDefault(x => x.Element?.Id == element.Id && x.EventName == eventName && x.Event == eventToCall);

            if (cachedEvent == null)
                return;

            //Remove it only from the list, the next time JavaScript tries to call the Event, it will remove it, with this Method
            //you can remove the Event in an Dispose function (where JS calls wouldnt be possible)
            _eventCache.Remove(cachedEvent);
        }

        [JSInvokable]
        public static async Task<bool> OnJSEvent(string eventId)
        {
            var ev = _eventCache.FirstOrDefault(x => x.Id.ToString() == eventId);

            if (ev == null)
                return false;

            await ev.Event.Invoke();
            return true;
        }

        /// <summary>
        /// Sets a Property of the JS Window Object
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="propertyName">The name of the Property, for a list of Propertys see <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window">HERE</see></param>
        /// <param name="value">The value to set</param>
        /// <returns></returns>
        public static Task SetWindowPropertyAsync(this IJSRuntime jsRuntime, string propertyName, object? value)
        {
            return jsRuntime.InvokeVoidAsync("JSInteropPlus.SetWindowProperty", propertyName, value).AsTask();
        }

        /// <summary>
        /// Gets a Property of the JS Window Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsRuntime"></param>
        /// <param name="propertyName">The name of the Property</param>
        /// <returns></returns>
        public static Task<T> GetWindowPropertyAsync<T>(this IJSRuntime jsRuntime, string propertyName)
        {
            return jsRuntime.InvokeAsync<T>("JSInteropPlus.GetWindowProperty", propertyName).AsTask();
        }

        /// <summary>
        /// Sets a Property of a JS Element
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="element">The JS Element</param>
        /// <param name="propertyName">The name of the Property</param>
        /// <param name="value">The value to set</param>
        /// <returns></returns>
        public static Task SetElementPropertyAsync(this IJSRuntime jsRuntime, ElementReference element, string propertyName, object? value)
        {
            return jsRuntime.InvokeVoidAsync("JSInteropPlus.SetElementProperty", element, propertyName, value).AsTask();
        }

        /// <summary>
        /// Gets a Property of a JS Element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsRuntime"></param>
        /// <param name="element">The JS Element</param>
        /// <param name="propertyName">The name of the Property</param>
        /// <returns></returns>
        public static Task<T> GetElementPropertyAsync<T>(this IJSRuntime jsRuntime, ElementReference element, string propertyName)
        {
            return jsRuntime.InvokeAsync<T>("JSInteropPlus.GetElementProperty", element, propertyName).AsTask();
        }

        /// <summary>
        /// Calls a Method of the JS Window Object
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="functionName">The name of the Method, for a list of Methods see <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window">HERE</see></param>
        /// <param name="arguments">The arguments to be passed to the Method</param>
        /// <returns></returns>
        public static Task InvokeWindowMethodAsync(this IJSRuntime jsRuntime, string functionName, params object?[] arguments)
        {
            return jsRuntime.InvokeWindowMethodAsync<IJSInteropPlusVoid>(functionName, arguments);
        }

        /// <summary>
        /// Calls a Method of a JS Element
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="element">The JS Element</param>
        /// <param name="functionName">The name of the Method</param>
        /// <param name="arguments">The arguments to be passed to the Method</param>
        /// <returns></returns>
        public static Task InvokeElementMethodAsync(this IJSRuntime jsRuntime, ElementReference element, string functionName, params object?[] arguments)
        {
            return jsRuntime.InvokeElementMethodAsync<IJSInteropPlusVoid>(element, functionName, arguments);
        }

        /// <summary>
        /// Calls a Method of the JS Window Object
        /// </summary>
        /// <typeparam name="T">The return Type</typeparam>
        /// <param name="jsRuntime"></param>
        /// <param name="functionName">The name of the Method, for a list of Methods see <see href="https://developer.mozilla.org/en-US/docs/Web/API/Window">HERE</see></param>
        /// <param name="arguments">The arguments to be passed to the Method</param>
        /// <returns></returns>
        public static Task<T> InvokeWindowMethodAsync<T>(this IJSRuntime jsRuntime, string functionName, params object?[] arguments)
        {
            return jsRuntime.InvokeAsync<T>("JSInteropPlus.CallWindowFunction", functionName, arguments).AsTask();
        }

        /// <summary>
        /// Calls a Method of a JS Element
        /// </summary>
        /// <typeparam name="T">The return Type</typeparam>
        /// <param name="jsRuntime"></param>
        /// <param name="element">The JS Element</param>
        /// <param name="functionName">The name of the Method</param>
        /// <param name="arguments">The arguments to be passed to the Method</param>
        /// <returns></returns>
        public static Task<T> InvokeElementMethodAsync<T>(this IJSRuntime jsRuntime, ElementReference element, string functionName, params object?[] arguments)
        {
            return jsRuntime.InvokeAsync<T>("JSInteropPlus.CallElementFunction", functionName, element, arguments).AsTask();
        }
    }

    internal class CachedEvent
    {
        public Guid Id { get; set; }
        public ElementReference? Element { get; set; }
        public string EventName { get; set; }
        public Func<Task> Event { get; set; }
    }
    internal interface IJSInteropPlusVoid
    {
    }
}
