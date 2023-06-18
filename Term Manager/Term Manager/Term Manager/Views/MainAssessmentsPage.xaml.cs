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
using Plugin.LocalNotifications;


namespace Term_Manager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainAssessmentsPage : ContentPage
    {
        private Course _selectedCourse;

        private ObservableCollection<Course> _courses = new ObservableCollection<Course>();
        private ObservableCollection<Assessment> _assessments;

        private PageState _currentState;

        public MainAssessmentsPage(Course selectedCourse)
        {
            _selectedCourse = selectedCourse;
            InitializeComponent();

            DatabaseService.Instance.OnAssessmentAdded += OnAssessmentAdded;
            DatabaseService.Instance.OnAssessmentRemoved += OnAssessmentRemoved;
            DatabaseService.Instance.OnAssessmentUpdated += OnAssessmentUpdated;

            _assessments = new ObservableCollection<Assessment>(DatabaseService.Instance.GetAssessmentsForCourse(_selectedCourse.ID));
            _assessmentListItems.ItemsSource = _assessments;

            _courses.Add(_selectedCourse);
            _selectedCourseList.ItemsSource = _courses;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            InitState();
        }

        private void InitState()
        {
            if (_assessments.Count == 0)
            {
                SwitchState(PageState.NoContent);
                return;
            }

            SwitchState(PageState.ContentAvaliable);
        }

        private void OnAssessmentAdded(Assessment assessment)
        {
            _assessments.Add(assessment);
        }

        private void OnAssessmentUpdated(Assessment assessmentUpdated)
        {
            Assessment foundAssessment = _assessments.Where(a => a.ID == assessmentUpdated.ID).FirstOrDefault();

            if (foundAssessment != null)
            {
                foundAssessment.Name = assessmentUpdated.Name;
                foundAssessment.StartDate = assessmentUpdated.StartDate;
                foundAssessment.EndDate = assessmentUpdated.EndDate;
                foundAssessment.Notifications = assessmentUpdated.Notifications;
            }
        }

        private void OnAssessmentRemoved(int assessmentID)
        {
            Assessment assessmentToRemove = _assessments.Where(a => a.ID == assessmentID).FirstOrDefault();

            if (assessmentToRemove != null)
            {
                _assessments.Remove(assessmentToRemove);
            }
        }

        private void SwitchState(PageState state)
        {
            switch (state)
            {
                case PageState.NoContent:
                    {
                        _noAssessmentslabel.IsEnabled = true;
                        _noAssessmentslabel.IsVisible = true;
                        _noAssessmentsAddButton.IsEnabled = true;
                        _noAssessmentsAddButton.IsVisible = true;

                        _assessmentListItems.IsEnabled = false;
                        _assessmentListItems.IsVisible = false;
                        _addAssessmentButton.IsEnabled = false;
                        _addAssessmentButton.IsVisible = false;

                        _selectedCourseList.VerticalOptions = LayoutOptions.FillAndExpand;

                        break;
                    }
                case PageState.ContentAvaliable:
                    {
                        _noAssessmentslabel.IsEnabled = false;
                        _noAssessmentslabel.IsVisible = false;
                        _noAssessmentsAddButton.IsEnabled = false;
                        _noAssessmentsAddButton.IsVisible = false;

                        _assessmentListItems.IsEnabled = true;
                        _assessmentListItems.IsVisible = true;
                        _addAssessmentButton.IsEnabled = true;
                        _addAssessmentButton.IsVisible = true;

                        _selectedCourseList.VerticalOptions = LayoutOptions.Fill;

                        break;
                    }
            }

            _currentState = state;
        }

        private void AddAssessmentButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddAssessmentPage(_selectedCourse.ID));
        }

        private void Edit_Clicked(object sender, EventArgs e)
        {
            Assessment assessmentToUpdate = (sender as MenuItem).CommandParameter as Assessment;
            Navigation.PushAsync(new AddAssessmentPage(_selectedCourse.ID, assessmentToUpdate));
        }

        private async void Delete_Clicked(object sender, EventArgs e)
        {
            Assessment assessmentToDelete = (sender as MenuItem).CommandParameter as Assessment;

            if (assessmentToDelete != null)
            {
                bool deleteAssessment = await DisplayAlert("Delete Assessment", $"Are you sure you want to delete {assessmentToDelete.Name}?", "Okay", "Cancel");

                if (deleteAssessment)
                {
                    DatabaseService.Instance.RemoveAssessment(assessmentToDelete.ID);

                    if (_assessments.Count == 0)
                        SwitchState(PageState.NoContent);
                }
            }
        }
    }
}