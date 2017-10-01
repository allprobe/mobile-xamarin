using AllProbe1.Droid;
using AllProbe1.Services;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllProbe1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Settings : ContentPage
    {
        private List<string> severityList = null;
        private string selectedItem = null;

        public Settings()
        {
            InitializeComponent();
            Initialize();
          
        }

        private void Initialize()
        {
            severityList = GlobalServices.SeverityList;
            foreach (string item in severityList)
                this.pkSeverity.Items.Add(item);

            object selectedSeverity = null;
            if (Application.Current.Properties.ContainsKey(Android.App.Application.Context.Resources.GetString(Resource.String.selectedSeverity)))
            {
                selectedSeverity = Application.Current.Properties[Android.App.Application.Context.Resources.GetString(Resource.String.selectedSeverity)];

                if (selectedSeverity != null)
                    this.pkSeverity.SelectedIndex = Convert.ToInt32(selectedSeverity);
            }
            else
            {
                Application.Current.Properties[Android.App.Application.Context.Resources.GetString(Resource.String.selectedSeverity)] = 0;
            }
           
        }

        private void SelectedItem_handler()
        {
            Application.Current.Properties[Android.App.Application.Context.Resources.GetString(Resource.String.selectedSeverity)] = this.pkSeverity.SelectedIndex;
        }

    }
}
