using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Term_Manager.Models
{
    [Table("course")]
    public class Course
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CourseStatus Status { get; set; }
        public string InstructorName { get; set; }
        public string InstructorPhone { get; set; }
        public string InstructorEmail { get; set; }
        public int Term { get; set; }
        public bool Notifications { get; set; }
        public string Notes { get; set; }

        public enum CourseStatus { In_Progress, Completed, Dropped, Plan_To_Take}
    }
}
