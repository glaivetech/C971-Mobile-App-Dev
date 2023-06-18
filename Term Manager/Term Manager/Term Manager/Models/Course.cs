using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SQLite;

namespace Term_Manager.Models
{
    [Table("course")]
    public class Course : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int ID { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged("StartDate");
            }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }

        private CourseStatus _status;
        public CourseStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        private string _instructorName;
        public string InstructorName
        {
            get { return _instructorName; }
            set
            {
                _instructorName = value;
                OnPropertyChanged("InstructorName");
            }
        }

        private string _instructorPhone;
        public string InstructorPhone
        {
            get { return _instructorPhone; }
            set
            {
                _instructorPhone = value;
                OnPropertyChanged("InstructorPhone");
            }
        }

        private string _instructorEmail;
        public string InstructorEmail
        {
            get { return _instructorEmail; }
            set
            {
                _instructorEmail = value;
                OnPropertyChanged("InstructorEmail");
            }
        }

        private int _term;
        public int Term
        {
            get { return _term; }
            set
            {
                _term = value;
                OnPropertyChanged("Term");
            }
        }

        private bool _notifications;
        public bool Notifications
        {
            get { return _notifications; }
            set
            {
                _notifications = value;
                OnPropertyChanged("Notifications");
            }
        }

        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                OnPropertyChanged("Notes");
            }
        }

        public enum CourseStatus { In_Progress, Completed, Dropped, Plan_To_Take }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string nameOfProperty)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nameOfProperty));
        }
    }
}
