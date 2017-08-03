using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Serialization;

namespace Lounging
{
	public partial class LoungingPage : ContentPage
	{
		private readonly HybridWebView hybrid;
		public LoungingPage()
		{
            InitializeComponent();
			this.Content = this.hybrid = new HybridWebView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.Blue
			};

			this.hybrid.RegisterCallback("dataCallback", t =>
				Device.BeginInvokeOnMainThread(() =>
				{
					this.DisplayAlert("Data callback", t, "OK");
				})
			);

			this.hybrid.RegisterCallback("sendObject", s =>
			{
				var serializer = Resolver.Resolve<IJsonSerializer>();

				var o = serializer.Deserialize<SendObject>(s);

				this.DisplayAlert("Object", string.Format("JavaScript sent x: {0}, y: {1}", o.X, o.Y), "OK");
			});

			this.hybrid.RegisterNativeFunction("funcCallback", t => new object[] { "From Func callback: " + t });
		}

		#region Overrides of Page
		protected override void OnAppearing()
		{
			base.OnAppearing();
            this.hybrid.Uri = new Uri("http://lounging-angular.azurewebsites.net");
            DisplayAlert("App Loaded", "Lounging app has been loaded", "Cancel");
			//var assembly = this.GetType().GetTypeInfo().Assembly;
       		//using (var reader = new StreamReader(assembly.GetManifestResourceStream("Lounging.HTML.src.index.html")))
			//{
			//	var str = reader.ReadToEnd();
			//}
		}

		#endregion
	}

	public class SendObject
	{
		public double X { get; set; }
		public double Y { get; set; }
	}
}
