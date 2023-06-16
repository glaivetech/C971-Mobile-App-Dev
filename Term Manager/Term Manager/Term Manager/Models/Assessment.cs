using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Term_Manager.Models
{
    [Table("assessment")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int ID { get; set; }
        public string Name { get; set; }
        public AssessmentType Type { get; set; }
        public int CourseID { get; set; }
        public bool Notications { get; set; }

        public enum AssessmentType { Objective, Performance}
    }
}
