using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class StudentService
    {

        private readonly Model1 context;

        public StudentService()
        {
            context = new Model1();
        }
        public List<Student> GetAll()
        {
            Model1 context = new Model1();
            return context.Students.ToList();
        }

        public string AddStudent(Student student)
        {
            try
            {
                if (context.Students.Any(s => s.StudentID == student.StudentID))
                {
                    return "Mã sinh viên đã tồn tại.";
                }
                context.Students.Add(student);
                context.SaveChanges();
                return "Thêm sinh viên thành công!";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi thêm sinh viên: {ex.Message}";
            }
        }

        public string DeleteStudent(int studentID)
        {
            try
            {
                var student = context.Students.FirstOrDefault(s => s.StudentID == studentID);
                if (student == null)
                {
                    return "Sinh viên không tồn tại!";
                }

                context.Students.Remove(student);
                context.SaveChanges();
                return "Xóa sinh viên thành công!";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi xóa sinh viên: {ex.Message}";
            }
        }

        public string EditStudent(Student updatedStudent)
        {
            try
            {
                var student = context.Students.FirstOrDefault(s => s.StudentID == updatedStudent.StudentID);
                if (student == null)
                {
                    return "Sinh viên không tồn tại!";
                }
                if (updatedStudent.StudentID != student.StudentID)
                {
                    if (context.Students.Any(s => s.StudentID == updatedStudent.StudentID))
                    {
                        return "Mã sinh viên mới đã tồn tại.";
                    }
                    student.StudentID = updatedStudent.StudentID;
                }
                student.FullName = updatedStudent.FullName;
                student.FacultyID = updatedStudent.FacultyID;
                student.AverageScore = updatedStudent.AverageScore;
                context.SaveChanges();
                return "Cập nhật sinh viên thành công!";
            }
            catch (Exception ex)
            {
                return $"Lỗi khi cập nhật sinh viên: {ex.Message}";
            }
        }
    }
}
