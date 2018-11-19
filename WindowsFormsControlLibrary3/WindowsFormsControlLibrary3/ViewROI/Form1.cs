using ChoiceTech.Halcon.Control;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ViewWindows.Model;
namespace ViewROI
{
    public partial class Form1 : Form
    {
        private List<ROI> regions;
        private HObject ho_ModelImage;
        private string roiConfigPath = Application.StartupPath + "\\ROIConfig.xml";
        private IContainer components = null;
        private Button btn_LoadImage;
        private GroupBox groupBox2;
        private Button CircleButton;
        public Button DelActROIButton;
        public Button Rect2Button;
        public Button Rect1Button;
        private Button btn_SaveROI;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private GroupBox groupBox5;
        private GroupBox groupBox3;
        private DataGridView dgv_ROI;
        private Button button1;
        private Button button2;
        private Button button3;
        private HWindow_Final hWindow_Fit1;
        public Form1()
        {
            this.InitializeComponent();
        }
        private void binDataGridView(DataGridView dgv, List<ROI> config)
        {
            try
            {
                dgv.DataSource = null;
                DataGridViewTextBoxColumn dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                dataGridViewTextBoxColumn.DataPropertyName = "Type";
                dataGridViewTextBoxColumn.HeaderText = "类型";
                dataGridViewTextBoxColumn.Name = "Type";
                dataGridViewTextBoxColumn.Width = 90;
                dataGridViewTextBoxColumn.ReadOnly = true;
                dgv.Columns.AddRange(new DataGridViewColumn[]
                {
                    dataGridViewTextBoxColumn
                });
                dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.AllowUserToDeleteRows = true;
                dgv.AllowUserToAddRows = false;
                dgv.MultiSelect = false;
                dgv.AllowUserToAddRows = false;
                dgv.AllowUserToDeleteRows = true;
                dgv.DataSource = config;
                dgv.Refresh();
                if (config.Count > 0)
                {
                    dgv.Rows[config.Count - 1].Cells[0].Selected = true;
                }
            }
            catch (Exception)
            {
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.regions = new List<ROI>();
            if (File.Exists(this.roiConfigPath))
            {
                this.hWindow_Fit1.viewWindow.loadROI(this.roiConfigPath, out this.regions);
                this.binDataGridView(this.dgv_ROI, this.regions);
            }
            this.hWindow_Fit1.hWindowControl.MouseUp += new MouseEventHandler(this.Hwindow_MouseUp);
        }
        private void btn_LoadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "所有图像文件 | *.bmp; *.pcx; *.png; *.jpg; *.gif;*.tif; *.ico; *.dxf; *.cgm; *.cdr; *.wmf; *.eps; *.emf";
            openFileDialog.Title = "打开图像文件";
            openFileDialog.ShowHelp = true;
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;
                HOperatorSet.GenEmptyObj(out this.ho_ModelImage);
                this.ho_ModelImage.Dispose();
                HOperatorSet.ReadImage(out this.ho_ModelImage, fileName);
                this.hWindow_Fit1.HobjectToHimage(this.ho_ModelImage);
                this.hWindow_Fit1.viewWindow.displayROI(this.regions);
                HObject hObject = new HObject();
                HOperatorSet.GenCrossContourXld(out hObject, 500, 500, 1000, 1);
                HObject hObj = new HObject();
                HObject hObject2 = new HObject();
                HOperatorSet.GenRectangle1(out hObject2, 1100, 0, 1600, 200);
                this.hWindow_Fit1.DispObj(hObj);
                this.hWindow_Fit1.DispObj(hObject);
                this.hWindow_Fit1.DispObj(hObject2);
                hObject.Dispose();
                hObject2.Dispose();
            }
        }
        private void Rect1Button_Click(object sender, EventArgs e)
        {
            this.hWindow_Fit1.viewWindow.genRect1(110.0, 110.0, 210.0, 210.0, ref this.regions);
            this.regions.Last<ROI>().Color = "blue";
            this.binDataGridView(this.dgv_ROI, this.regions);
        }
        private void Rect2Button_Click(object sender, EventArgs e)
        {
            this.hWindow_Fit1.viewWindow.genRect2(200.0, 200.0, 0.0, 60.0, 30.0, ref this.regions);
            this.regions.Last<ROI>().Color = "blue";
            this.binDataGridView(this.dgv_ROI, this.regions);
        }
        private void CircleButton_Click(object sender, EventArgs e)
        {
            this.hWindow_Fit1.viewWindow.genCircle(200.0, 200.0, 60.0, ref this.regions);
            this.regions.Last<ROI>().Color = "blue";
            this.binDataGridView(this.dgv_ROI, this.regions);
        }
        private void LineButton_Click(object sender, EventArgs e)
        {
            this.hWindow_Fit1.viewWindow.genLine(100.0, 100.0, 100.0, 200.0, ref this.regions);
            this.regions.Last<ROI>().Color = "blue";
            this.binDataGridView(this.dgv_ROI, this.regions);
        }
        private void DelActROIButton_Click(object sender, EventArgs e)
        {
            this.hWindow_Fit1.viewWindow.removeActiveROI(ref this.regions);
            this.binDataGridView(this.dgv_ROI, this.regions);
        }
        private void dgv_ROI_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.hWindow_Fit1.viewWindow.selectROI(e.RowIndex);
            List<double> list;
            int num;
            ROI rOI = this.hWindow_Fit1.viewWindow.smallestActiveROI(out list, out num);
            if (num > -1)
            {
                string name = rOI.GetType().Name;
            }
        }
        private void btn_SaveROI_Click(object sender, EventArgs e)
        {
            this.hWindow_Fit1.viewWindow.saveROI(this.regions, this.roiConfigPath);
        }
        private void Hwindow_MouseUp(object sender, MouseEventArgs e)
        {
            List<double> list;
            int num;
            ROI rOI = this.hWindow_Fit1.viewWindow.smallestActiveROI(out list, out num);
            if (num > -1)
            {
                string name = rOI.GetType().Name;
                this.dgv_ROI.Rows[num].Cells[0].Selected = true;
                this.regions[num] = rOI;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.hWindow_Fit1.DrawModel = true;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.hWindow_Fit1.DrawModel = false;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.btn_LoadImage = new Button();
            this.groupBox2 = new GroupBox();
            this.CircleButton = new Button();
            this.button1 = new Button();
            this.DelActROIButton = new Button();
            this.btn_SaveROI = new Button();
            this.Rect2Button = new Button();
            this.Rect1Button = new Button();
            this.tabControl1 = new TabControl();
            this.tabPage1 = new TabPage();
            this.button3 = new Button();
            this.button2 = new Button();
            this.groupBox3 = new GroupBox();
            this.dgv_ROI = new DataGridView();
            this.tabPage2 = new TabPage();
            this.groupBox5 = new GroupBox();
            this.hWindow_Fit1 = new HWindow_Final();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((ISupportInitialize)this.dgv_ROI).BeginInit();
            this.groupBox5.SuspendLayout();
            base.SuspendLayout();
            this.btn_LoadImage.Location = new System.Drawing.Point(20, 29);
            this.btn_LoadImage.Name = "btn_LoadImage";
            this.btn_LoadImage.Size = new System.Drawing.Size(96, 35);
            this.btn_LoadImage.TabIndex = 5;
            this.btn_LoadImage.Text = "打开图片";
            this.btn_LoadImage.UseVisualStyleBackColor = true;
            this.btn_LoadImage.Click += new EventHandler(this.btn_LoadImage_Click);
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.CircleButton);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.DelActROIButton);
            this.groupBox2.Controls.Add(this.btn_SaveROI);
            this.groupBox2.Controls.Add(this.Rect2Button);
            this.groupBox2.Controls.Add(this.Rect1Button);
            this.groupBox2.Location = new System.Drawing.Point(171, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(137, 312);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Create ROI";
            this.CircleButton.Location = new System.Drawing.Point(22, 145);
            this.CircleButton.Name = "CircleButton";
            this.CircleButton.Size = new System.Drawing.Size(96, 34);
            this.CircleButton.TabIndex = 8;
            this.CircleButton.Text = "Circle";
            this.CircleButton.Click += new EventHandler(this.CircleButton_Click);
            this.button1.Location = new System.Drawing.Point(22, 28);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(96, 30);
            this.button1.TabIndex = 10;
            this.button1.Text = "Line";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.LineButton_Click);
            this.DelActROIButton.Location = new System.Drawing.Point(22, 188);
            this.DelActROIButton.Name = "DelActROIButton";
            this.DelActROIButton.Size = new System.Drawing.Size(96, 35);
            this.DelActROIButton.TabIndex = 7;
            this.DelActROIButton.Text = "Delete Active ROI";
            this.DelActROIButton.Click += new EventHandler(this.DelActROIButton_Click);
            this.btn_SaveROI.Location = new System.Drawing.Point(22, 229);
            this.btn_SaveROI.Name = "btn_SaveROI";
            this.btn_SaveROI.Size = new System.Drawing.Size(96, 35);
            this.btn_SaveROI.TabIndex = 8;
            this.btn_SaveROI.Text = "保存ROI";
            this.btn_SaveROI.UseVisualStyleBackColor = true;
            this.btn_SaveROI.Click += new EventHandler(this.btn_SaveROI_Click);
            this.Rect2Button.Location = new System.Drawing.Point(22, 104);
            this.Rect2Button.Name = "Rect2Button";
            this.Rect2Button.Size = new System.Drawing.Size(96, 35);
            this.Rect2Button.TabIndex = 1;
            this.Rect2Button.Text = "Rectangle2";
            this.Rect2Button.Click += new EventHandler(this.Rect2Button_Click);
            this.Rect1Button.Location = new System.Drawing.Point(22, 64);
            this.Rect1Button.Name = "Rect1Button";
            this.Rect1Button.Size = new System.Drawing.Size(96, 34);
            this.Rect1Button.TabIndex = 0;
            this.Rect1Button.Text = "Rectangle1";
            this.Rect1Button.Click += new EventHandler(this.Rect1Button_Click);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = DockStyle.Right;
            this.tabControl1.Location = new System.Drawing.Point(663, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(324, 651);
            this.tabControl1.TabIndex = 11;
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.btn_LoadImage);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(316, 625);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.button3.Location = new System.Drawing.Point(20, 157);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "button2";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new EventHandler(this.button3_Click);
            this.button2.Location = new System.Drawing.Point(20, 116);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new EventHandler(this.button2_Click);
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Controls.Add(this.dgv_ROI);
            this.groupBox3.Location = new System.Drawing.Point(8, 349);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(300, 226);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "groupBox3";
            this.dgv_ROI.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_ROI.Dock = DockStyle.Fill;
            this.dgv_ROI.Location = new System.Drawing.Point(3, 17);
            this.dgv_ROI.Name = "dgv_ROI";
            this.dgv_ROI.RowTemplate.Height = 23;
            this.dgv_ROI.Size = new System.Drawing.Size(294, 206);
            this.dgv_ROI.TabIndex = 1;
            this.dgv_ROI.CellClick += new DataGridViewCellEventHandler(this.dgv_ROI_CellClick);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(316, 625);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.groupBox5.Controls.Add(this.hWindow_Fit1);
            this.groupBox5.Dock = DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(0, 0);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(663, 651);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "groupBox5";
            this.hWindow_Fit1.BackColor = System.Drawing.Color.Transparent;
            this.hWindow_Fit1.BorderStyle = BorderStyle.FixedSingle;
            this.hWindow_Fit1.DrawModel = false;
            this.hWindow_Fit1.Image = null;
            this.hWindow_Fit1.Location = new System.Drawing.Point(33, 37);
            this.hWindow_Fit1.Name = "hWindow_Fit1";
            this.hWindow_Fit1.Size = new System.Drawing.Size(491, 365);
            this.hWindow_Fit1.TabIndex = 0;
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(987, 651);
            base.Controls.Add(this.groupBox5);
            base.Controls.Add(this.tabControl1);
            base.Name = "Form1";
            this.Text = "Form1";
            base.Load += new EventHandler(this.Form1_Load);
            this.groupBox2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((ISupportInitialize)this.dgv_ROI).EndInit();
            this.groupBox5.ResumeLayout(false);
            base.ResumeLayout(false);
        }
    }
}
