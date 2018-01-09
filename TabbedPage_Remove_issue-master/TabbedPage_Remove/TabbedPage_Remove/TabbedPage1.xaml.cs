using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TabbedPage_Remove
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedPage1 : TabbedPage
    {
        ContentPage newTabPage;

        public TabbedPage1 ()
        {
            InitializeComponent();

            //this.CurrentPageChanged += (object sender, EventArgs e) =>
            //{
            //    Debug.WriteLine($"Current Page Changed to '{CurrentPage.Title}'");

            //    if ((CurrentPage == this.Children[0] ||
            //    CurrentPage == this.Children[1] ||
            //    CurrentPage == this.Children[2]) && this.Children.Count > 3)
            //    {
            //        this.Children.RemoveAt(3);
            //        Debug.WriteLine($"Current Page is '{CurrentPage.Title}'");
            //    }
            //};

        }

        protected override void OnCurrentPageChanged()
        {
            Debug.WriteLine($"Current Page Changed to '{CurrentPage.Title}'");

            if ((CurrentPage == this.Children[0] ||
            CurrentPage == this.Children[1] ||
            CurrentPage == this.Children[2]) && this.Children.Count > 3)
            {
                this.Children.RemoveAt(3);
                Debug.WriteLine($"Current Page is '{CurrentPage.Title}'");
            }

            base.OnCurrentPageChanged();
        }

        public void Button1_Clicked(Object sender, EventArgs e)
        {
            Debug.WriteLine("Creating a New Tab");

            Label label1 = new Label
            {
                Text = "This tab will be removed when navigated out",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            newTabPage = new ContentPage
            {
                Title = "New Tab",
                Padding = new Thickness(10),
                Content = label1
            };

            this.Children.Add(newTabPage);
        }
    }
}