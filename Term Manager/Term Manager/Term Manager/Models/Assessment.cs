using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SQLite;

namespace Term_Manager.Models
{
    [Table("assessment")]
    public class Assessment : INotifyPropertyChanged
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

        private AssessmentType _type;
        public AssessmentType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
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

        private int _courseID;
        public int CourseID
        {
            get { return _courseID; }
            set
            {
                _courseID = value;
                OnPropertyChanged("CourseID");
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

        public enum AssessmentType { Objective, Performance}

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string nameOfProperty)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nameOfProperty));
        }
    }
}
