using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AllProbe1.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CodeAnalyze : ContentPage
	{
		public CodeAnalyze (string webSite)
		{
			InitializeComponent ();

            ///URL (page header) title:
            lblURL.Text = webSite;
        }
	}
}