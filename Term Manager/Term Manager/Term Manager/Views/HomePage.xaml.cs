using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class HomePage : ContentPage
    {
        private ObservableCollection<Term> _terms = new ObservableCollection<Term>();

        private HomeState _currentState;

        public NavigationPage NavPage { get; set; }
        public enum HomeState { NoTerms, TermsAvailable, EditMode }

        public HomePage()
        {
            InitializeComponent();

            DatabaseService.Instance.OnTermAdded += OnTermAdded;
            DatabaseService.Instance.OnTermRemoved += OnTermRemoved;
            DatabaseService.Instance.OnTermUpdated += OnTermUpdated;

            _terms = new ObservableCollection<Term>(DatabaseService.Instance.GetAllTerms());
            _termListItems.ItemsSource = _terms;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            InitState();
        }

        private void OnTermAdded(Term term)
        {
            _terms.Add(term);
        }

        private void OnTermRemoved(int id)
        {
            Term termToRemove = _terms.Where(t => t.ID == id).FirstOrDefault();

            if (termToRemove != null)
            {
                _terms.Remove(termToRemove);
            }
        }

        private void OnTermUpdated(Term updatedTerm)
        {
            Term foundTerm = _terms.Where(t => t.ID == updatedTerm.ID).FirstOrDefault();

            if (foundTerm != null)
            {
                foundTerm.Title = updatedTerm.Title;
                foundTerm.StartDate = updatedTerm.StartDate;
                foundTerm.EndDate = updatedTerm.EndDate;
            }
        }


        private bool UserHasTerms()
        {
            return DatabaseService.Instance.GetAllTerms().Count > 0;
        }

        private void InitState()
        {
            if (_terms.Count == 0)
            {
                SwitchState(HomeState.NoTerms);
                return;
            }

            SwitchState(HomeState.TermsAvailable);
        }

        private void SwitchState(HomeState state)
        {
            switch (state)
            {
                case HomeState.NoTerms:
                    {
                        _noTermslabel.IsEnabled = true;
                        _noTermslabel.IsVisible = true;

                        _noTermsAddButton.IsEnabled = true;
                        _noTermsAddButton.IsVisible = true;

                        _termListItems.IsEnabled = false;
                        _termListItems.IsVisible = false;

                        _addTermButton.IsEnabled = false;
                        _addTermButton.IsVisible = false;

                        break;
                    }
                case HomeState.TermsAvailable:
                    {
                        _noTermslabel.IsEnabled = false;
                        _noTermslabel.IsVisible = false;

                        _termListItems.IsEnabled = true;
                        _termListItems.IsVisible = true;

                        _noTermsAddButton.IsEnabled = false;
                        _noTermsAddButton.IsVisible = false;

                        _addTermButton.IsEnabled = true;
                        _addTermButton.IsVisible = true;

                        break;
                    }
            }

            _currentState = state;
        }

        private void AddTermButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddTermPage());
        }

        private async void Delete_Clicked(object sender, EventArgs e)
        {
            Term termToDelete = (sender as MenuItem).CommandParameter as Term;

            if (termToDelete != null)
            {
                bool deleteTask = await DisplayAlert("Delete Term", $"Are you sure you want to delete {termToDelete.Title}? \n Deleting this term will delete all assigned courses and assessments with it.", "Okay", "Cancel");
                if (deleteTask)
                {
                    DatabaseService.Instance.RemoveTerm(termToDelete.ID);

                    if (_terms.Count == 0)
                        SwitchState(HomeState.NoTerms);
                }
            }
        }

        private void Courses_Clicked(object sender, EventArgs e)
        {
            Term selectedTerm = (sender as MenuItem).CommandParameter as Term;
            Navigation.PushAsync(new MainCoursesPage(selectedTerm));
        }

        private void Change_Clicked(object sender, EventArgs e)
        {
            Term termToUpdate = (sender as MenuItem).CommandParameter as Term;
            Navigation.PushAsync(new AddTermPage(termToUpdate));
        }
    }
}