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
using Xamarin.Essentials;

namespace Term_Manager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainCoursesPage : ContentPage
    {
        private Term _term;

        private ObservableCollection<Term> _terms = new ObservableCollection<Term>();
        private ObservableCollection<Course> _courses;

        private PageState _currentState;

        public NavigationPage NavPage { get; set; }
        public MainCoursesPage(Term term)
        {
            _term = term;
            InitializeComponent();
            DatabaseService.Instance.OnCourseAdded += OnCourseAdded;
            DatabaseService.Instance.OnCourseRemoved += OnCourseRemoved;
            DatabaseService.Instance.OnCourseUpdated += OnCourseUpdated;

            _courses = new ObservableCollection<Course>(DatabaseService.Instance.GetCoursesForTerm(term.ID));
            _courseListItems.ItemsSource = _courses;

            _terms.Add(_term);
            _selectedTermList.ItemsSource = _terms;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            InitState();
        }

        private void InitState()
        {
            if (_courses.Count == 0)
            {
                SwitchState(PageState.NoContent);
                return;
            }

            SwitchState(PageState.ContentAvaliable);
        }

        private void OnCourseAdded(Course course)
        {
            _courses.Add(course);
        }

        private void OnCourseUpdated(Course courseUpdated)
        {
            Course foundCourse = _courses.Where(c => c.ID == courseUpdated.ID).FirstOrDefault();

            if (foundCourse != null)
            {
                foundCourse.Name = courseUpdated.Name;
                foundCourse.StartDate = courseUpdated.StartDate;
                foundCourse.EndDate = courseUpdated.EndDate;
                foundCourse.Status = courseUpdated.Status;
                foundCourse.InstructorName = courseUpdated.InstructorName;
                foundCourse.InstructorEmail = courseUpdated.InstructorEmail;
                foundCourse.InstructorPhone = courseUpdated.InstructorPhone;
                foundCourse.Notifications = courseUpdated.Notifications;
                foundCourse.Notes = courseUpdated.Notes;
            }
        }

        private void OnCourseRemoved(int courseID)
        {
            Course courseToRemove = _courses.Where(c => c.ID == courseID ).FirstOrDefault();

            if (courseToRemove != null)
            {
                _courses.Remove(courseToRemove);
            }
        }

        private void SwitchState(PageState state)
        {
            switch (state)
            {
                case PageState.NoContent:
                    {
                        _noCourseslabel.IsEnabled = true;
                        _noCourseslabel.IsVisible = true;
                        _noCoursesAddButton.IsEnabled = true;
                        _noCoursesAddButton.IsVisible = true;

                        _courseListItems.IsEnabled = false;
                        _courseListItems.IsVisible = false;
                        _addCourseButton.IsEnabled = false;
                        _addCourseButton.IsVisible = false;

                        _selectedTermList.VerticalOptions = LayoutOptions.FillAndExpand;

                        break;
                    }
                case PageState.ContentAvaliable:
                    {
                        _noCourseslabel.IsEnabled = false;
                        _noCourseslabel.IsVisible = false;
                        _noCoursesAddButton.IsEnabled = false;
                        _noCoursesAddButton.IsVisible = false;

                        _courseListItems.IsEnabled = true;
                        _courseListItems.IsVisible = true;
                        _addCourseButton.IsEnabled = true;
                        _addCourseButton.IsVisible = true;

                        _selectedTermList.VerticalOptions = LayoutOptions.Fill;

                        break;
                    }
            }

            _currentState = state;
        }
        private void AddCourseButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddCoursePage(_term.ID));
        }

        private void Assessments_Clicked(object sender, EventArgs e)
        {
            Course selectedCourse = (sender as MenuItem).CommandParameter as Course;
            Navigation.PushAsync(new MainAssessmentsPage(selectedCourse));
        }

        private async void Share_Clicked(object sender, EventArgs e)
        {
            Course selectedCourse = (sender as MenuItem).CommandParameter as Course;

            string notes = $"Course Notes for: {selectedCourse.Name} \n Notes: {selectedCourse.Notes}";
            ShareTextRequest req = new ShareTextRequest(notes, "Share Course Notes");

            await Share.RequestAsync(req);
        }

        private void Edit_Clicked(object sender, EventArgs e)
        {
            Course courseToUpdate = (sender as MenuItem).CommandParameter as Course;
            Navigation.PushAsync(new AddCoursePage(_term.ID, courseToUpdate));
        }

        private async void Delete_Clicked(object sender, EventArgs e)
        {
            Course courseToDelete = (sender as MenuItem).CommandParameter as Course;

            if (courseToDelete != null)
            {
                bool deleteCourse = await DisplayAlert("Delete Course", $"Are you sure you want to delete {courseToDelete.Name}? \n Deleting this course will delete all assigned assessments with it.", "Okay", "Cancel");

                if (deleteCourse)
                {
                    DatabaseService.Instance.RemoveCourse(courseToDelete.ID);

                    if (_courses.Count == 0)
                        SwitchState(PageState.NoContent);
                }
            }
        }
    }
}