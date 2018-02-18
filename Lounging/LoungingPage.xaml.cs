using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Lounging.Extensions;
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
		}

		#region Overrides of Page
		protected override void OnAppearing()
		{
			base.OnAppearing();
            this.hybrid.Uri = new Uri("http://lounging-angular.azurewebsites.net");
		}

		#endregion
	}

	public class SendObject
	{
		public double X { get; set; }
		public double Y { get; set; }
	}

    public class BridgeManager 
    {
        private HybridWebView bridge;
        private LoungingPage page;
        private IJsonSerializer serializer;

        public BridgeManager(HybridWebView hybrid, LoungingPage loungingPage)
        {
            bridge = hybrid;
            page = loungingPage;
            serializer = Resolver.Resolve<IJsonSerializer>();
            SetUpCallBacks();
        }

        private void SetUpCallBacks()
        {
            this.bridge.RegisterCallback("dataCallback", t =>
                Device.BeginInvokeOnMainThread(() =>
                {
                    page.DisplayAlert("Data callback", t, "OK");
                })
            );

            this.bridge.RegisterCallback("sendObject", s =>
            {
                var o = serializer.Deserialize<SendObject>(s);

                page.DisplayAlert("Object", string.Format("JavaScript sent x: {0}, y: {1}", o.X, o.Y), "OK");
            });

            this.bridge.RegisterNativeFunction("funcCallback", t => new object[] { "From Func callback: " + t });

            this.bridge.RegisterNativeFunction_Custom("XRequest", (string req) =>
            {
                var request = serializer.Deserialize<BridgeRequest>(req);
                switch (request.Type)
                {
                    case BridgeMessageType.GET:
                        return new object[]{ HandleGetRequest(request) };
                    case BridgeMessageType.SUBSCRIBE:
                        return new object[] { HandleSubscribeRequest(request) };
                    default:
                        break;
                }
                return new object[] {  };
            });
        }

        private object HandleSubscribeRequest(BridgeRequest request)
        {
            throw new NotImplementedException();
        }

        private BridgeMessage HandleGetRequest(BridgeRequest request) 
        {
            switch(request.HandlerName){
                case "Loungers":
                    var loungers = new List<Lounger>();
                    loungers.Add(new Lounger() { FirstName = "David", UserName = "TheGoliath"});
                    loungers.Add(new Lounger() { FirstName = "Mona", UserName = "QuiteBeastly" });
                    loungers.Add(new Lounger() { FirstName = "Jacob", UserName = "ObaOtokpo"});
                    var response = new BridgeResponse<Lounger[]>(loungers.ToArray(), BridgeResultType.JSON, true );
                return response;
            }
            return new BridgeResponse<string>();
        }
    }

    public class BridgeMessage
    {
        public BridgeMessageType Type { get; set; }
        public string HandlerName { get; set; }
    }

    public class BridgeRequest : BridgeMessage
    {
        private string paramsJSON;
        public string Params { 
            get { return paramsJSON; } 
            set{
                if(value != paramsJSON){
					var serializer = Resolver.Resolve<IJsonSerializer>();
					ParamsObject = serializer.Deserialize<object>(paramsJSON);
                } 
                paramsJSON = value;
            }
        }
        public object ParamsObject { get; set; }
        public string CallbackFunctionName { get; set; }
    }

    public class BridgeResponse<T> : BridgeMessage
    {
        IJsonSerializer serializer;
        public BridgeResponse(){}
        public BridgeResponse(T result, BridgeResultType type, bool success = true){
            ResultObject = result;
            serializer = Resolver.Resolve<IJsonSerializer>();
            ResultType = type;
        }
        public string Result { get; set; }
        public T ResultObject { 
            get {return serializer.Deserialize<T>(Result);} 
            set{ Result = serializer.SerializeToString(value);} 
        }
        public BridgeResultType ResultType { get; set; }
        public bool Success { get; set; }
    }

    public enum BridgeMessageType
    {
        GET = 1,
        POST = 2, 
        SUBSCRIBE = 3, 
        ACT = 4
    }

    public enum BridgeResultType
    {
        TEXT = 1, 
        JSON = 2, 
        STREAM = 3, 
    }
}
