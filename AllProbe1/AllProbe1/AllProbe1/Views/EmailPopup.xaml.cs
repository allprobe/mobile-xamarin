using AllProbe1.Droid;
using AllProbe1.Models;
using AllProbe1.Services;
using Rg.Plugins.Popup.Pages;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllProbe1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EmailPopup : PopupPage
    {
        string webSite = null;
		public EmailPopup (string webSite)
		{
			InitializeComponent ();
            this.lblClose.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => lblClosePopup()), });
            this.webSite = webSite;
		}

        private void ClickSend(object sender, EventArgs e)
        {
            string email = this.emailEntry.Text;

            if (string.IsNullOrEmpty(email))
            {
                this.messageLabel.Text = Android.App.Application.Context.Resources.GetString(Resource.String.emailMandatory);
                return;
            }
            if (!GlobalServices.IsValid(email))
            {
                this.messageLabel.Text = Android.App.Application.Context.Resources.GetString(Resource.String.invalidEmail);
                return;
            }

            SessionState sessionState = SessionState.GetInstance();
            IServices services = new Services.Services();
            services.PostReport(email, webSite, sessionState.SessionId);

            Navigation.PopModalAsync();
        }

        private void lblClosePopup()
        {
            Navigation.PopModalAsync();
        }
    }
}
