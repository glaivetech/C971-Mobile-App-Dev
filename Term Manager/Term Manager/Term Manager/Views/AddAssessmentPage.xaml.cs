using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Term_Manager.Models;
using Term_Manager.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Term_Manager.Models.Assessment;

namespace Term_Manager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAssessmentPage : ContentPage
    {
        private int _courseId;

        private Assessment _assessmentToUpdate = null;

        public AddAssessmentPage(int courseId, Assessment assessmentToUpdate = null)
        {
            _courseId = courseId;
            _assessmentToUpdate = assessmentToUpdate;

            InitializeComponent();

            if(assessmentToUpdate != null)
            {
                _headerLabel.Text = "Edit Assessment";
                _assessmentNameEntry.Text = _assessmentToUpdate.Name;
                _assessmentStart.Date = _assessmentToUpdate.StartDate;
                _assessmentEnd.Date = _assessmentToUpdate.EndDate;
                foreach (string item in _assessmentTypePicker.Items)
                {
                    if (item == assessmentToUpdate.Type.ToString())
                    {
                        _assessmentTypePicker.SelectedItem = item;
                        break;
                    }
                }
                _notificationsSwitch.IsToggled = _assessmentToUpdate.Notifications;
            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            string assessmentName = _assessmentNameEntry.Text;
            DateTime startDate = _assessmentStart.Date;
            DateTime endDate = _assessmentEnd.Date;

            AssessmentType assessmentType;
            Enum.TryParse(_assessmentTypePicker.SelectedItem as string, out assessmentType);

            bool notifications = _notificationsSwitch.IsToggled;

            bool valid = await ValidateInput(assessmentName, startDate, endDate);

            if(valid)
            {
                if (_assessmentToUpdate == null)
                {
                    DatabaseService.Instance.AddAssessment(assessmentName, assessmentType, startDate, endDate, _courseId, notifications);
                    await DisplayAlert("Assessment Added", "Assessment added successfully.", "Ok");
                }
                else
                {
                    DatabaseService.Instance.UpdateAssessment(_assessmentToUpdate.ID, assessmentName, assessmentType, startDate, endDate, _courseId, notifications);
                    await DisplayAlert("Assessment Updated", "Assessment updated successfully.", "Ok");
                }

                Navigation.PopAsync();
            }
        }

        private async Task<bool> ValidateInput(string assessmentName, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(assessmentName))
            {
                await DisplayAlert("Invalid Input", "Assessment Name cannot be empty.", "Ok");
                return false;
            }

            if (_assessmentTypePicker.SelectedItem == null)
            {
                await DisplayAlert("Invalid Input", "Please select an Assessment Type", "Ok");
                return false;
            }

            if (startDate > endDate)
            {
                await DisplayAlert("Invalid Date", "Assessment start date cannot be after the Assessment end date.", "Ok");
                return false;
            }

            if (endDate < startDate)
            {
                await DisplayAlert("Invalid Date", "Assessment end date cannot be before the Assessment start date.", "Ok");
                return false;
            }

            return true;
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}