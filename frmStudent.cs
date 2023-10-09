using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BUS;
using DAL.Entities;
using System.IO;

namespace GUI
{
    public partial class frmStudent : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        public frmStudent()
        {
            InitializeComponent();
        }

        public void setGridViewStyle(DataGridView dgview)
        {
            dgview.BorderStyle = BorderStyle.None;
            dgview.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgview.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgview.BackgroundColor = Color.White;
            dgview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void FillFacultyComboBox(List<Faculty> faculties)
        {
            this.cmbFaculty.DataSource = faculties;
            this.cmbFaculty.DisplayMember = "FacultyName";
            this.cmbFaculty.ValueMember = "FacultyID";
        }

        private void ShowAvatar(string ImageName)
        {
            if (string.IsNullOrEmpty(ImageName))
                picAva.Image = null;
            else
            {
                string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string imagePath = Path.Combine(parentDirectory, "Images", ImageName);
                picAva.Image = Image.FromFile(imagePath);
                picAva.Refresh();
            }
        }

        private void BindGrid(List<Student> students)
        {
            dgvStudents.Rows.Clear();
            
            foreach(var item in students)
            {
                int index = dgvStudents.Rows.Add();
                
                dgvStudents.Rows[index].Cells[0].Value = item.StudentID;
                
                dgvStudents.Rows[index].Cells[1].Value = item.FullName;
                
                if (item.FacultyID != null)
                    dgvStudents.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                
                dgvStudents.Rows[index].Cells[3].Value = item.AverageScore + "";
                
                if (item.MajorID != null)
                    dgvStudents.Rows[index].Cells[4].Value = item.Major.Name + "";

                ShowAvatar(item.Avatar);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dgvStudents);
                var faculties = facultyService.GetAll();
                var students = studentService.GetAll();
                FillFacultyComboBox(faculties);
                BindGrid(students);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chkMajor_CheckedChanged(object sender, EventArgs e)
        {
            var students = new List<Student>();

            if (this.chkMajor.Checked)
                students = studentService.GetAllHasNoMajor();
            else
                students = studentService.GetAll();

            BindGrid(students);
        }

        private void thoatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("Ban co muon thoat?", "Thong bao", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
                this.Close();
        }

        private void btnAddUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtStudentID.Text == "" || txtFullName.Text == "" || txtAve.Text == "")
                    throw new Exception("Vui long nhap day du thong tin");

                Student s = new Student() { StudentID = txtStudentID.Text, FullName = txtFullName.Text, FacultyID = cmbFaculty.SelectedIndex + 1, AverageScore = float.Parse(txtAve.Text), Avatar = txtStudentID.Text + Path.GetFileName(picAva.ImageLocation).Substring(Path.GetFileName(picAva.ImageLocation).Length - 4) };
                studentService.InsertUpdate(s);

                var students = new List<Student>();

                if (this.chkMajor.Checked)
                    students = studentService.GetAllHasNoMajor();
                else
                    students = studentService.GetAll();

                BindGrid(students);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSelPic_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "JPG file|*.jpg|PNG file|*.png";

            if (dlg.ShowDialog() == DialogResult.OK)
                picAva.ImageLocation = dlg.FileName;
        }

        private void dgvStudents_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count == 1 
                && dgvStudents.SelectedRows[0].Cells[0].Value != null)
            {
                Student s = studentService.FindByID(dgvStudents.SelectedRows[0].Cells[0].Value.ToString());
                txtStudentID.Text = s.StudentID.ToString();
                txtFullName.Text = s.FullName.ToString();
                cmbFaculty.SelectedItem = s.FacultyID - 1;
                txtAve.Text = s.AverageScore.ToString();
                if (s.Avatar != null)
                {
                    picAva.ImageLocation = ;
                    ShowAvatar(s.Avatar.ToString());
                }
                else
                    picAva.Image = picAva.InitialImage;
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (studentService.Delete(txtStudentID.Text) == 1)
            {
                MessageBox.Show("Xoa thanh cong");

                var students = new List<Student>();

                if (this.chkMajor.Checked)
                    students = studentService.GetAllHasNoMajor();
                else
                    students = studentService.GetAll();

                BindGrid(students);
            }
            else
                MessageBox.Show("Khong tim thay sinh vien voi ma so");

        }
    }
}
