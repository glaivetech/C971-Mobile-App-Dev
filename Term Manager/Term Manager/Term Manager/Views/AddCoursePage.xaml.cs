using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Term_Manager.Models;
using Term_Manager.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Term_Manager.Models.Course;

namespace Term_Manager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCoursePage : ContentPage
    {
        private int _termId;

        private Course _courseToUpdate = null;
        public AddCoursePage(int termId, Course courseToUpdate = null)
        {
            _termId = termId;
            _courseToUpdate = courseToUpdate;

            InitializeComponent();

            if (courseToUpdate != null)
            {
                _headerLabel.Text = "Edit Course";
                _courseNameEntry.Text = _courseToUpdate.Name;
                _courseStart.Date = _courseToUpdate.StartDate;
                _courseEnd.Date = _courseToUpdate.EndDate;

                foreach (string item in _courseStatus.Items)
                {
                    if (item == _courseToUpdate.Status.ToString())
                    {
                        _courseStatus.SelectedItem = item;
                        break;
                    }
                }

                _instructorNameEntry.Text = _courseToUpdate.InstructorName;
                _instructorPhoneEntry.Text = _courseToUpdate.InstructorPhone;
                _instructorEmailEntry.Text = _courseToUpdate.InstructorEmail;
                _notificationsSwitch.IsToggled = _courseToUpdate.Notifications;
                _notesEntry.Text = _courseToUpdate.Notes;
            }
        }

        private async void SaveCourseButton_Clicked(object sender, EventArgs e)
        {
            string courseName = _courseNameEntry.Text;
            DateTime startDate = _courseStart.Date;
            DateTime endDate = _courseEnd.Date;

            CourseStatus courseStatus;
            Enum.TryParse(_courseStatus.SelectedItem as string, out courseStatus);

            string instructorName = _instructorNameEntry.Text;
            string instructorPhone = _instructorPhoneEntry.Text;
            string instructorEmail = _instructorEmailEntry.Text;
            bool notifications = _notificationsSwitch.IsToggled;
            string notes = _notesEntry.Text;

            bool validInput = await ValidateInput(courseName, startDate, endDate, instructorName, instructorPhone, instructorEmail);

            if(validInput)
            {
                if (_courseToUpdate == null)
                {
                    DatabaseService.Instance.AddCourse(courseName, startDate, endDate, courseStatus, instructorName, instructorEmail, instructorPhone, _termId, notifications, notes);
                    await DisplayAlert("Course Added", "Course added successfully.", "Ok");
                }
                else
                {
                    DatabaseService.Instance.UpdateCourse(_courseToUpdate.ID, courseName, startDate, endDate, courseStatus, instructorName, instructorEmail, instructorPhone, _termId, notifications, notes);
                    await DisplayAlert("Course Updated", "Course updated successfully.", "Ok");
                }

                Navigation.PopAsync();
            }
        }

        private async Task<bool> ValidateInput(string courseName, DateTime startDate, DateTime endDate, string instructorName, string instructorPhone, string instructorEmail)
        {
            if (string.IsNullOrEmpty(courseName))
            {
                await DisplayAlert("Invalid Input", "Course Name cannot be empty.", "Ok");
                return false;
            }

            if (_courseStatus.SelectedItem == null)
            {
                await DisplayAlert("Invalid Input", "Please select a Course Status", "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(instructorName))
            {
                await DisplayAlert("Invalid Input", "Instructor Name cannot be empty.", "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(instructorPhone))
            {
                await DisplayAlert("Invalid Input", "Instructor Phone cannot be empty.", "Ok");
                return false;
            }

            if (string.IsNullOrEmpty(instructorEmail))
            {
                await DisplayAlert("Invalid Input", "Instructor Email cannot be empty.", "Ok");
                return false;
            }

            if (startDate > endDate)
            {
                await DisplayAlert("Invalid Date", "Term start date cannot be after the term end date.", "Ok");
                return false;
            }

            if (endDate < startDate)
            {
                await DisplayAlert("Invalid Date", "Term end date cannot be before the term start date.", "Ok");
                return false;
            }

            return true;
        }

        public void CancelButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}