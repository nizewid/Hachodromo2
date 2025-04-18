using Hachodromo.Mobile.Models;
using Hachodromo.Mobile.PageModels;

namespace Hachodromo.Mobile.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}