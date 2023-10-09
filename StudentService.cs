using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace BUS
{
    public class StudentService
    {
        public List<Student> GetAll()
        {
            StudentModel context = new StudentModel();
            return context.Student.ToList();
        }

        public List<Student> GetAllHasNoMajor()
        {
            StudentModel context = new StudentModel();
            return context.Student.Where(p => p.MajorID == null).ToList();
        }

        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            StudentModel context = new StudentModel();
            return context.Student.Where(p => p.MajorID == null &&
            p.FacultyID == facultyID).ToList();
        }

        public Student FindByID(string studentID)
        {
            StudentModel context = new StudentModel();
            return context.Student.FirstOrDefault(p => p.StudentID == studentID);
        }

        public void InsertUpdate(Student s)
        {
            StudentModel context = new StudentModel();
            Student db = context.Student.FirstOrDefault(p => p.StudentID == s.StudentID);

            if (db == null)
            {
                context.Student.Add(s);
                context.SaveChanges();
            }
            else
            {
                db.FullName = s.FullName;
                if (db.FacultyID != s.FacultyID)
                    db.MajorID = null;
                db.FacultyID = s.FacultyID;
                db.AverageScore = s.AverageScore;
                db.Avatar = s.Avatar;
                context.SaveChanges();
            }
        }

        public int Delete(string studentId)
        {
            StudentModel context = new StudentModel();
            Student db = context.Student.FirstOrDefault(p => p.StudentID == studentId);

            if (db != null)
            {
                context.Student.Remove(db);
                context.SaveChanges();
                return 1;
            }
            return -1;
        }
    }
}
