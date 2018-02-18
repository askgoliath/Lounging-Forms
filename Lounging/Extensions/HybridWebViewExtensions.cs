using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace Lounging.Extensions
{
    public static class HybridWebViewExtensions
    {

		// I'm so insanely mad that I need to even do this…. The problem is that the Android
		// renderer for the HybridWebView does not marshal back to the UI thread when the
		// native function is complete at that causes Android to fail on Lollipop+. Since building
		// my own version of HybridWebView is insanely hard and subclassing the existing one is
		// impossible, we have our *2 version of this method which works the same way but puts
		// the callback onto the right flippin thread!
		public static void RegisterNativeFunction_Custom(this HybridWebView hybridWebView, string name, Func<string, object[]> func)
		{
			hybridWebView.RegisterCallback(name, (s) =>
			{
				// Get the appended callback ID
				var index = s.LastIndexOf('!');
				var callbackId = s.Substring(index + 1);
				s = s.Substring(0, index);

				Task.Run(() =>
				{
					// Run the native function
					var args = func(s);

					// The magic that was missing from the original...
					Device.BeginInvokeOnMainThread(() =>
					{
						hybridWebView.CallJsFunction("nativeFuncs2[" + callbackId + "]", args);
					});
				});
			});
		}
    }
}
