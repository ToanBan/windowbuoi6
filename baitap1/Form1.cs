using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BUS;
using System.Windows.Forms;
using DAL.Entity;
using System.Runtime.Remoting.Contexts;
using System.Xml.Linq;

namespace baitap1
{
    public partial class Form1 : Form
    {
        private readonly StudentService student = new StudentService();
        private readonly FacultyService fal = new FacultyService();
        private readonly MajorService major = new MajorService();  
        Model1 context = new Model1();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var listFaculty = fal.GetAll();
            var listStudent = student.GetAll();
            FillFacultyCombobox(listFaculty);
            BindGrid(listStudent);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void FillFacultyCombobox(List<Faculty> listFaculty)
        {
            listFaculty.Insert(0, new Faculty());
            this.txtKhoa.DataSource = listFaculty;
            this.txtKhoa.DisplayMember = "FacultyName";
            this.txtKhoa.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudent)
        {
            dataGridView1.Rows.Clear();
            foreach(var item in listStudent)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells["colMa"].Value = item.StudentID;
                dataGridView1.Rows[index].Cells["colName"].Value = item.FullName;
                dataGridView1.Rows[index].Cells["colKhoa"].Value = item.Faculty.FacultyName;
                dataGridView1.Rows[index].Cells["colDiem"].Value = item.AverageScore;
                
            }
        }

        private void LoadDataGridView()
        {
            List<Student> students = context.Students.ToList();
            dataGridView1.Rows.Clear();
            foreach (var item in students)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells["colMa"].Value = item.StudentID;
                dataGridView1.Rows[index].Cells["colName"].Value = item.FullName;
                dataGridView1.Rows[index].Cells["colKhoa"].Value = item.Faculty.FacultyName;
                dataGridView1.Rows[index].Cells["colDiem"].Value = item.AverageScore;
                dataGridView1.Rows[index].Cells["colCn"].Value = item.Major.MajorID;
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtMa.Text.Trim(), out int studentID))
                {
                    MessageBox.Show("Vui lòng nhập định dạng số hợp lệ cho Mã sinh viên.");
                    txtMa.Focus();
                    return;
                }

                string fullName = txtName.Text.Trim();
                if (string.IsNullOrEmpty(fullName))
                {
                    MessageBox.Show("Tên sinh viên không được để trống.");
                    txtName.Focus();
                    return;
                }

                if (txtKhoa.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn Khoa.");
                    txtKhoa.Focus();
                    return;
                }
                int facultyID = (int)txtKhoa.SelectedValue;

                if (!decimal.TryParse(txtDiem.Text.Trim(), out decimal averageScore))
                {
                    MessageBox.Show("Vui lòng nhập định dạng số hợp lệ cho Điểm trung bình.");
                    txtDiem.Focus();
                    return;
                }
                var existingStudent = context.Students.FirstOrDefault(student => student.StudentID == studentID);
                if (existingStudent != null)
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại. Vui lòng nhập mã khác.");
                    txtMa.Focus();
                    return;
                }
                Student s = new Student()
                {
                    StudentID = studentID,
                    FullName = fullName,
                    FacultyID = facultyID,
                    AverageScore = averageScore,
                    MajorID = 1,
                    Avatar = "test.jpg" 
                };

                student.AddStudent(s);
                LoadDataGridView();
                MessageBox.Show("Thêm sinh viên thành công!");
                txtMa.Clear();
                txtName.Clear();
                txtDiem.Clear();
                txtKhoa.SelectedIndex = 0;
            }
            catch (FormatException)
            {
                MessageBox.Show("Vui lòng nhập đúng định dạng cho Mã, Điểm.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm sinh viên: " + ex.Message);
            }
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int studentID = (int)dataGridView1.SelectedRows[0].Cells["colMa"].Value;
                var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?",
                                             "Xác nhận xóa!",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes)
                {
                   
                    string result = student.DeleteStudent(studentID);
                    MessageBox.Show(result, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (result.Contains("thành công"))
                    {
                        LoadDataGridView();
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sinh viên để xóa.");
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                try
                {
                    int originalStudentID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["colMa"].Value);
                    var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn chỉnh sửa sinh viên này?",
                                             "Xác nhận chỉnh sửa!",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);
                    if (confirmResult == DialogResult.Yes)
                    {
                        if (!int.TryParse(txtMa.Text.Trim(), out int newStudentID))
                        {
                            MessageBox.Show("Vui lòng nhập định dạng số hợp lệ cho Mã sinh viên.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtMa.Focus();
                            return;
                        }
                        string fullName = txtName.Text.Trim();
                        if (string.IsNullOrEmpty(fullName))
                        {
                            MessageBox.Show("Tên sinh viên không được để trống.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtName.Focus();
                            return;
                        }
                        if (txtKhoa.SelectedValue == null || (int)txtKhoa.SelectedValue == 0)
                        {
                            MessageBox.Show("Vui lòng chọn Khoa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtKhoa.Focus();
                            return;
                        }
                        int facultyID = (int)txtKhoa.SelectedValue;
                        if (!int.TryParse(txtKhoa.SelectedValue?.ToString(), out int majorID) || majorID == 0)
                        {
                            MessageBox.Show("Vui lòng chọn Chuyên Ngành.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtKhoa.Focus();
                            return;
                        }
                        if (!decimal.TryParse(txtDiem.Text.Trim(), out decimal averageScore))
                        {
                            MessageBox.Show("Vui lòng nhập định dạng số hợp lệ cho Điểm trung bình.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtDiem.Focus();
                            return;
                        }
                        string avatar = "test.jpg"; 
                        Student updatedStudent = new Student()
                        {
                            StudentID = newStudentID,
                            FullName = fullName,
                            FacultyID = facultyID,
                            AverageScore = averageScore,
                            MajorID = majorID,
                            Avatar = avatar
                        };
                        string result = student.EditStudent(updatedStudent);
                        MessageBox.Show(result);

                        if (result.Contains("thành công"))
                        {
                            LoadDataGridView();
                            txtMa.Clear();
                            txtName.Clear();
                            txtDiem.Clear();
                            txtKhoa.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Thao tác chỉnh sửa đã bị hủy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi cập nhật sinh viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một sinh viên để chỉnh sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}
