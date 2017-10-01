using AllProbe1.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllProbe1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventsAndWebsites : TabbedPage
    {
        public EventsAndWebsites ()
        {
            try
            {
                InitializeComponent();
                Children.Add(new EventsViewPage());
                Children.Add(new WebPages());

                this.CurrentPage = this.Children[GlobalServices.getMenuSelected()];
            }
            catch (System.Exception)
            {
            }
        }
    }
}
