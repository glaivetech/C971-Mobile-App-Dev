using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Term_Manager.Models;
using Term_Manager.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Term_Manager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTermPage : ContentPage
    {
        private Term _termToUpdate = null;

        public AddTermPage(Term termToUpdate = null)
        {
            _termToUpdate = termToUpdate;

            InitializeComponent();

            if (_termToUpdate != null)
            {
                _headerLabel.Text = "Edit Term";
                _termNameEntry.Text = _termToUpdate.Title;
                _termStart.Date = _termToUpdate.StartDate;
                _termEnd.Date = _termToUpdate.EndDate;
            }
        }

        private async void SaveTermButton_Clicked(object sender, EventArgs e)
        {
            string termName = _termNameEntry.Text;
            DateTime start = _termStart.Date;
            DateTime end = _termEnd.Date;

            if (_termToUpdate == null)
            {
                DatabaseService.Instance.AddTerm(termName, start, end);
                await DisplayAlert("Term Added", "Term added successfully.", "Ok");
            }
            else
            {
                DatabaseService.Instance.UpdateTerm(_termToUpdate.ID, termName, start, end);
                await DisplayAlert("Term Updated", "Term updated successfully.", "Ok");
            }

            CancelButton_Clicked(sender, e);
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}