using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using XLabs.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Serialization;

namespace Lounging.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : XFormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			this.SetIoc();

			//XLabs.Forms.Controls.HybridWebViewRenderer.CopyBundleDirectory("HTML");

			global::Xamarin.Forms.Forms.Init();

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}

		/// <summary>
		/// Sets the IoC.
		/// </summary>
		private void SetIoc()
		{
			var resolverContainer = new global::XLabs.Ioc.SimpleContainer();

			var app = new XFormsAppiOS();
			app.Init(this);

			var documents = app.AppDataDirectory;
		
			resolverContainer.Register<IDevice>(t => AppleDevice.CurrentDevice)
			                 .Register<IJsonSerializer, XLabs.Serialization.JsonNET.JsonSerializer>();

			Resolver.SetResolver(resolverContainer.GetResolver());
		}
	}
}
