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
using static Term_Manager.Models.Course;
using static Term_Manager.Models.Assessment;

namespace Term_Manager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        private ObservableCollection<Term> _terms = new ObservableCollection<Term>();

        private PageState _currentState;

        public NavigationPage NavPage { get; set; }

        public HomePage()
        {
            InitializeComponent();

            DatabaseService.Instance.OnTermAdded += OnTermAdded;
            DatabaseService.Instance.OnTermRemoved += OnTermRemoved;
            DatabaseService.Instance.OnTermUpdated += OnTermUpdated;

            _terms = new ObservableCollection<Term>(DatabaseService.Instance.GetAllTerms());
            _termListItems.ItemsSource = _terms;

            if (_terms.Count == 0)
                AddEvalData();

            HandleNotifications();
        }

        private void AddEvalData()
        {
            string termName = "Summer Term";
            DateTime startDate = DateTime.Now.Date;
            DateTime endDate = startDate.AddMonths(6);

            DatabaseService.Instance.AddTerm(termName, startDate, endDate);

            Term addedTerm = _terms[0];

            string courseName = "C971 Mobile App Development";
            DateTime courseStart = startDate;
            DateTime courseEnd = startDate.AddMonths(1);
            CourseStatus courseStatus = CourseStatus.In_Progress;
            string instructorName = "Courtney Blakley";
            string instructorPhone = "999-999-9999";
            string instructorEmail = "cbla470@my.wgu.edu";

            DatabaseService.Instance.AddCourse(courseName, courseStart, courseEnd, courseStatus, instructorName, instructorEmail, instructorPhone, _terms[0].ID, true, "This class will teach you how to make mobile apps in Xamarin");

            Course addedCourse = DatabaseService.Instance.GetCoursesForTerm(addedTerm.ID)[0];

            string assesment1Name = "LP1 - PA Mobile App";
            DateTime assessmentStart1 = courseEnd.AddDays(-7);
            DateTime assessmentEnd1 = courseEnd;
            AssessmentType assessment1Type = AssessmentType.Performance;

            string assesment2Name = "LP2 - OA Mobile App";
            DateTime assessmentStart2 = courseEnd.AddDays(-7);
            DateTime assessmentEnd2 = courseEnd;
            AssessmentType assessment2Type = AssessmentType.Objective;

            DatabaseService.Instance.AddAssessment(assesment1Name, assessment1Type, assessmentStart1, assessmentEnd1, addedCourse.ID, true);
            DatabaseService.Instance.AddAssessment(assesment2Name, assessment2Type, assessmentStart2, assessmentEnd2, addedCourse.ID, true);
        }

        private void HandleNotifications()
        {
            List<Course> courses = DatabaseService.Instance.GetAllCourses();
            List<Assessment> assessments = DatabaseService.Instance.GetAllAssessments();

            foreach (Course course in courses)
            {
                if (course.Notifications)
                {
                    if (course.StartDate.Date == DateTime.Now.Date)
                    {
                        CrossLocalNotifications.Current.Show("Course Start", "Your Course starts today! Learning is fun!");
                    }
                    if (course.EndDate.Date == DateTime.Now.Date)
                    {
                        CrossLocalNotifications.Current.Show("Course End", "Your Course ends today! Finish strong!");
                    }
                }
            }

            foreach (Assessment assessment in assessments)
            {
                if (assessment.Notifications)
                {
                    if (assessment.StartDate.Date == DateTime.Now.Date)
                    {
                        CrossLocalNotifications.Current.Show("Assessment Start", "Your Assessment starts today! Do your best!");
                    }
                    if (assessment.EndDate.Date == DateTime.Now.Date)
                    {
                        CrossLocalNotifications.Current.Show("Assessment End", "Your Assessment ends today! You got this!");
                    }
                }
            }
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
                SwitchState(PageState.NoContent);
                return;
            }

            SwitchState(PageState.ContentAvaliable);
        }

        private void SwitchState(PageState state)
        {
            switch (state)
            {
                case PageState.NoContent:
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
                case PageState.ContentAvaliable:
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
                        SwitchState(PageState.NoContent);
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