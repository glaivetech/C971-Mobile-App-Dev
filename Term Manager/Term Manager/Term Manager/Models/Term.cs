using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SQLite;

namespace Term_Manager.Models
{
    [Table("term")]
    public class Term : INotifyPropertyChanged 
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int ID { get; set; }

        private string _title;
        public string Title 
        { 
            get => _title; 
            set 
            {
                _title = value; 
                OnPropertyChanged("Title"); 
            } 
        }
        private DateTime _startDate;
        public DateTime StartDate 
        {
            get=> _startDate; 
            set
            {
                _startDate = value;
                OnPropertyChanged("StartDate");
            }
        }
        private DateTime _endDate;
        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nameOfProperty)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nameOfProperty));
        }
    }
}
