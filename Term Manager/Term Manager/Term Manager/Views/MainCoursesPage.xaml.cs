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
    public partial class MainCoursesPage : ContentPage
    {
        private Term _term;

        private ObservableCollection<Term> _terms = new ObservableCollection<Term>();
        private ObservableCollection<Course> _courses;

        private CoursePageState _currentState;

        public NavigationPage NavPage { get; set; }
        public enum CoursePageState { NoCourses, CoursesAvailable, EditMode }
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
                SwitchState(CoursePageState.NoCourses);
                return;
            }

            SwitchState(CoursePageState.CoursesAvailable);
        }

        private void OnCourseAdded(Course course)
        {
           
        }

        private void OnCourseUpdated(Course courseUpdated)
        {

        }

        private void OnCourseRemoved(Course courseRemoved)
        {

        }


        private void SwitchState(CoursePageState state)
        {
            switch (state)
            {
                case CoursePageState.NoCourses:
                    {
                        _noCourseslabel.IsEnabled = true;
                        _noCourseslabel.IsVisible = true;
                        _noCoursesAddButton.IsEnabled = true;
                        _noCoursesAddButton.IsVisible = true;

                        _courseListItems.IsEnabled = false;
                        _courseListItems.IsVisible = false;
                        _addCourseButton.IsEnabled = false;
                        _addCourseButton.IsVisible = false;

                        break;
                    }
                case CoursePageState.CoursesAvailable:
                    {
                        _noCourseslabel.IsEnabled = false;
                        _noCourseslabel.IsVisible = false;
                        _noCoursesAddButton.IsEnabled = false;
                        _noCoursesAddButton.IsVisible = false;

                        _courseListItems.IsEnabled = true;
                        _courseListItems.IsVisible = true;
                        _addCourseButton.IsEnabled = true;
                        _addCourseButton.IsVisible = true;

                        break;
                    }
            }

            _currentState = state;
        }
        private void AddCourseButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddCoursePage());
        }

        private void Assessments_Clicked(object sender, EventArgs e)
        {

        }

        private void Edit_Clicked(object sender, EventArgs e)
        {

        }
        private void Delete_Clicked(object sender, EventArgs e)
        {

        }
    }
}