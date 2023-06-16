using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Term_Manager.Models;
using Xamarin.Essentials;
using static Term_Manager.Models.Assessment;
using static Term_Manager.Models.Course;

namespace Term_Manager.Services
{
    public class DatabaseService
    {
        public static bool INITIALIZED = false;

        private static DatabaseService _instance;

        private SQLiteConnection _db;

        #region Term Events

        public delegate void TermAdd(Term term);

        public event TermAdd OnTermAdded;

        public delegate void TermRemoved(int termId);

        public event TermRemoved OnTermRemoved;

        public delegate void TermUpdated(Term term);

        public event TermUpdated OnTermUpdated;

        #endregion

        #region Course Events

        public delegate void CourseAdd(Course course);

        public event CourseAdd OnCourseAdded;

        public delegate void CourseRemoved(Course course);

        public event CourseRemoved OnCourseRemoved;

        public delegate void CourseUpdated(Course course);

        public event CourseUpdated OnCourseUpdated;

        #endregion

        public static DatabaseService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DatabaseService();
                }
                return _instance;
            }
        }

        private DatabaseService()
        {
            INITIALIZED = true;
            if (_db != null)
            {
                return;
            }

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "term_tracker.db");
            _db = new SQLiteConnection(databasePath);

            _db.CreateTable<Term>();
            _db.CreateTable<Course>();
            _db.CreateTable<Assessment>();

            /*foreach(Term t in GetAllTerms())
            {
                RemoveTerm(t.ID);
            }*/
        }
        #region Term
        public void AddTerm(string title, DateTime startDate, DateTime endDate)
        {
            Term term = new Term()
            {
                Title = title,
                StartDate = startDate,
                EndDate = endDate,
            };

            _db.Insert(term);

            OnTermAdded?.Invoke(term);
        }

        public void UpdateTerm(int id, string title, DateTime startDate, DateTime endDate)
        {
            Term term = _db.Table<Term>().Where(t => t.ID == id).FirstOrDefault();

            if (term != null)
            {
                term.Title = title;
                term.StartDate = startDate;
                term.EndDate = endDate;

                _db.Update(term);

                OnTermUpdated?.Invoke(term);
            }
        }

        public void RemoveTerm(int id)
        {
            List<Course> courses = GetCoursesForTerm(id);

            foreach (Course course in courses)
            {
                RemoveCourse(course.ID);
            }

            _db.Delete<Term>(id);
            OnTermRemoved?.Invoke(id);
        }

        public List<Term> GetAllTerms()
        {
            var terms = _db.Table<Term>().ToList();
            return terms;
        }
        #endregion

        #region Course
        public void AddCourse(string name, DateTime startDate, DateTime endDate, CourseStatus status,
            string instructorName, string instructorEmail, string instructorPhone, int term, bool notif, string notes)
        {
            Course course = new Course()
            {
                Name = name,
                StartDate = startDate,
                EndDate = endDate,
                Status = status,
                InstructorName = instructorName,
                InstructorEmail = instructorEmail,
                InstructorPhone = instructorPhone,
                Term = term,
                Notifications = notif,
                Notes = notes,
            };

            _db.Insert(course);
        }

        public void UpdateCourse(int id, string name, DateTime startDate, DateTime endDate, CourseStatus status,
            string instructorName, string instructorEmail, string instructorPhone, int term, bool notif, string notes)
        {
            Course course = _db.Table<Course>().Where(c => c.ID == id).FirstOrDefault();

            if (course != null)
            {
                course.Name = name;
                course.StartDate = startDate;
                course.EndDate = endDate;
                course.Status = status;
                course.InstructorName = instructorName;
                course.InstructorEmail = instructorEmail;
                course.InstructorPhone = instructorPhone;
                course.Term = term;
                course.Notifications = notif;
                course.Notes = notes;

                _db.Update(course);
            }
        }

        public void RemoveCourse(int id)
        {
            List<Assessment> assessments = GetAllAssessments();

            foreach (Assessment assessment in assessments)
            {
                if (assessment.CourseID == id)
                {
                    RemoveAssessment(assessment.ID);
                }
            }
            _db.Delete<Course>(id);
        }

        public List<Course> GetCoursesForTerm(int termId)
        {
            var courses = _db.Table<Course>().Where(c=>c.Term == termId).ToList();
            return courses;
        }
        #endregion
        #region Assessment

        public void AddAssessment(string name, AssessmentType type, int courseID, bool notifs)
        {
            Assessment assessment = new Assessment()
            {
                Name = name,
                Type = type,
                CourseID = courseID,
                Notications = notifs
            };

            _db.Insert(assessment);
        }

        public void UpdateAssessment(int id, string name, AssessmentType type, int courseID, bool notifs)
        {
            Assessment assessment = _db.Table<Assessment>().Where(a => a.ID == id).FirstOrDefault();

            if (assessment != null)
            {
                assessment.Name = name;
                assessment.Type = type;
                assessment.CourseID = courseID;
                assessment.Notications = notifs;

                _db.Update(assessment);
            }
        }

        public void RemoveAssessment(int id)
        {
            _db.Delete<Assessment>(id);
        }

        public List<Assessment> GetAllAssessments()
        {
            var assessments = _db.Table<Assessment>().ToList();
            return assessments;
        }
        #endregion
    }
}
