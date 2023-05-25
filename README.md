
# Easily Access Javascript Element Propertys and Functions trough C#

## How to Install

Add the Nuget Package to your Solution

    Install-Package Blazor.JSInteropPlus
    
Add the JavaScript File to your _Host.cshtml

    <script src="_content/Blazor.JSInteropPlus/JSInteropPlus.js"></script>

Add the Namespace to your _Imports.razor

    @using Blazor.Blazor.JSInteropPlus

## How to Use

This Library exposes extensions Methods for the Original IJSRuntime Interface provided by Blazor.

Inject the IJSRuntime Interface to your Component

    @inject IJSRuntime _jsRuntime

And use one of the Following extension Methods:

### Propertys

Get the Value of a Property of a specific JS Element / Window

	<div @ref=_divRef>...</div>
	
	@code{
		private ElementReference _divRef;

		protected override async Task OnAfterRenderAsync(bool firstRender)
	    {
		    var width = await _jsInterop.GetElementPropertyAsync<double>(_divRef, "clientWidth");
			//Or for the Window Object
			var windowWidth = await _jsInterop.GetWindowPropertyAsync<double>("innerWidth");
	    }
	}

Set the Value of a Property of a specific JS Element / Window

	<div @ref=_divRef>...</div>
	
	@code{
		private ElementReference _divRef;

		protected override async Task OnAfterRenderAsync(bool firstRender)
	    {
		    await _jsInterop.SetElementPropertyAsync(_divRef, "clientWidth", 500);
			//Or for the Window Object
			await _jsInterop.SetWindowPropertyAsync("name", "ThisIsMyNewName");
	    }
	}

### Methods

Call a Method a specific JS Element / Window

	<div @ref=_divRef>...</div>
	
	@code{
		private ElementReference _divRef;

		protected override async Task OnAfterRenderAsync(bool firstRender)
	    {
		    await _jsInterop.InvokeElementMethodAsync(_divRef, "scrollIntoView", {Optional Parameters});
			//Or for the Window Object
			await _jsInterop.InvokeWindowMethodAsync("print", {Optional Parameters});
	    }
	}
### Events

Subscribe to a Event of a specific Javascript Element /Window

	<div @ref=_divRef>...</div>
	
	@code{
		private ElementReference _divRef;

	    protected override async Task OnAfterRenderAsync(bool firstRender)
	    {
			await _jsRuntime.SubscribeToElementEventAsync(_divRef, "click", FunctionToCall, new JSEventOptions() { CallbackTime = TimeSpan.FromSeconds(1), CallbackType = Callback.Debounce });
			//Or for the Window Object
		    await _jsRuntime.SubscribeToWindowEventAsync("blur", FunctionToCall, new JSEventOptions() { CallbackTime = TimeSpan.FromSeconds(1), CallbackType = Callback.Debounce });
	    }

		private async Task FunctionToCall()
		{
			...
		}
	}

The Class 'JSEventOptions' is a Optional Parameter to Specify the behavior of the Callback, more down below.
	
Unsubscribe from a already subscribed Event of a specific Javascript Element /Window

	<div @ref=_divRef>...</div>
	
	@code{
		private ElementReference _divRef;

	    protected async Task Unsubscribe()
	    {
			await _jsRuntime.UnsubscribeFromElementEventAsync(_divRef, "click", FunctionToCall);
			//Or for the Window Object
		    await _jsRuntime.UnsubscribeFromWindowEventAsync("blur", FunctionToCall);
	    }
	    
		private async Task FunctionToCall()
		{
			...
		}
	}

## JSEventOptions
You can specify the behavior of JSEvent Callbacks:

	public class JSEventOptions
    {
        /// <summary>
        /// Specifies how the JS Event will invoke the C# Method
        /// </summary>
        public required Callback CallbackType { get; set; }
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

## License
Blazor.JSInteropPlus is MIT licensed.
