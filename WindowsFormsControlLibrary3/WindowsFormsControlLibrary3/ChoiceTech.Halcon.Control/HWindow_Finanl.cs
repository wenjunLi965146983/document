using HalconDotNet;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ViewWindows;
namespace ChoiceTech.Halcon.Control
{
    public class HWindow_Final : UserControl
    {
        private HWindow hv_window;
        private ContextMenuStrip hv_MenuStrip;
        private ToolStripMenuItem fit_strip;
        private ToolStripMenuItem saveImg_strip;
        private ToolStripMenuItem saveWindow_strip;
        private ToolStripMenuItem barVisible_strip;
        private ToolStripMenuItem histogram_strip;
        private HImage hv_image;
        private int hv_imageWidth;
        private int hv_imageHeight;
        private string str_imgSize;
        private bool drawModel = false;
        public ViewWindow viewWindow;
        public HWindowControl hWindowControl;
        private IContainer components = null;
        private Label m_CtrlHStatusLabelCtrl;
        private ImageList m_CtrlImageList;
        private HWindowControl mCtrl_HWindow;
        public bool DrawModel
        {
            get
            {
                return this.drawModel;
            }
            set
            {
                this.viewWindow.setDrawModel(value);
                if (value)
                {
                    this.mCtrl_HWindow.ContextMenuStrip = null;
                }
                else
                {
                    this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
                }
                this.drawModel = value;
            }
        }
        public HImage Image
        {
            get
            {
                return this.hv_image;
            }
            set
            {
                if (value != null)
                {
                    if (this.hv_image != null)
                    {
                        this.hv_image.Dispose();
                    }
                    this.hv_image = value;
                    this.hv_image.GetImageSize(out this.hv_imageWidth, out this.hv_imageHeight);
                    this.str_imgSize = string.Format("{0}X{1}", this.hv_imageWidth, this.hv_imageHeight);
                    try
                    {
                        this.barVisible_strip.Enabled = true;
                        this.fit_strip.Enabled = true;
                        this.histogram_strip.Enabled = true;
                        this.saveImg_strip.Enabled = true;
                        this.saveWindow_strip.Enabled = true;
                    }
                    catch (Exception)
                    {
                    }
                    this.viewWindow.displayImage(this.hv_image);
                }
            }
        }
        public IntPtr HWindowHalconID
        {
            get
            {
                return this.mCtrl_HWindow.HalconID;
            }
        }
        public HWindow_Final()
        {
            
          

            this.InitializeComponent();
            this.viewWindow = new ViewWindow(this.mCtrl_HWindow);
            this.hWindowControl = this.mCtrl_HWindow;
            this.hv_window = this.mCtrl_HWindow.HalconWindow;
            this.fit_strip = new ToolStripMenuItem("适应窗口");
            this.fit_strip.Click += delegate (object s, EventArgs e)
            {
                this.DispImageFit(this.mCtrl_HWindow);
            };
            this.barVisible_strip = new ToolStripMenuItem("显示StatusBar");
            this.barVisible_strip.CheckOnClick = true;
            this.barVisible_strip.CheckedChanged += new EventHandler(this.barVisible_strip_CheckedChanged);
            this.m_CtrlHStatusLabelCtrl.Visible = false;
            this.mCtrl_HWindow.Height = base.Height;
            this.saveImg_strip = new ToolStripMenuItem("保存原始图像");
            this.saveImg_strip.Click += delegate (object s, EventArgs e)
            {
                this.SaveImage();
            };
            this.saveWindow_strip = new ToolStripMenuItem("保存窗口缩略图");
            this.saveWindow_strip.Click += delegate (object s, EventArgs e)
            {
                this.SaveWindowDump();
            };
            this.histogram_strip = new ToolStripMenuItem("显示直方图(H)");
            this.histogram_strip.CheckOnClick = true;
            this.histogram_strip.Checked = false;
            this.hv_MenuStrip = new ContextMenuStrip();
            this.hv_MenuStrip.Items.Add(this.fit_strip);
            this.hv_MenuStrip.Items.Add(this.barVisible_strip);
            this.hv_MenuStrip.Items.Add(new ToolStripSeparator());
            this.hv_MenuStrip.Items.Add(this.saveImg_strip);
            this.hv_MenuStrip.Items.Add(this.saveWindow_strip);
            this.barVisible_strip.Enabled = true;
            this.fit_strip.Enabled = false;
            this.histogram_strip.Enabled = false;
            this.saveImg_strip.Enabled = false;
            this.saveWindow_strip.Enabled = false;
            this.mCtrl_HWindow.ContextMenuStrip = this.hv_MenuStrip;
            this.mCtrl_HWindow.SizeChanged += delegate (object s, EventArgs e)
            {
                this.DispImageFit(this.mCtrl_HWindow);
            };
        }
        public HWindowControl getHWindowControl()
        {
            return this.mCtrl_HWindow;
        }
        private void barVisible_strip_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
            base.SuspendLayout();
            if (toolStripMenuItem.Checked)
            {
                this.m_CtrlHStatusLabelCtrl.Visible = true;
                this.mCtrl_HWindow.HMouseMove+=new HMouseEventHandler(this.HWindowControl_HMouseMove);
            }
            else
            {
                this.m_CtrlHStatusLabelCtrl.Visible = false;
                this.mCtrl_HWindow.HMouseMove-=new HMouseEventHandler(this.HWindowControl_HMouseMove);
            }
            base.ResumeLayout(false);
            base.PerformLayout();
        }
        public void showStatusBar()
        {
            this.barVisible_strip.Checked = true;
        }
        private void SaveWindowDump()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG图像|*.png|所有文件|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                {
                    HOperatorSet.DumpWindow(this.HWindowHalconID, "png best", saveFileDialog.FileName);
                }
            }
        }
        private void SaveImage()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "BMP图像|*.bmp|所有文件|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(saveFileDialog.FileName))
                {
                    HOperatorSet.WriteImage(this.hv_image, "bmp", 0, saveFileDialog.FileName);
                }
            }
        }
        private void DispImageFit(HWindowControl hw_Ctrl)
        {
            try
            {
                this.viewWindow.resetWindowImage();
            }
            catch (Exception)
            {
            }
        }
        private void HWindowControl_HMouseMove(object sender, HMouseEventArgs e)
        {
            if (this.hv_image != null)
            {
                try
                {
                    HTuple hTuple;
                    HOperatorSet.CountChannels(this.hv_image, out hTuple);
                    double num;
                    double num2;
                    int num3;
                    this.hv_window.GetMpositionSubPix(out num, out num2, out num3);
                    string text = string.Format("ROW: {0:0000.0}, COLUMN: {1:0000.0}", num, num2);
                    bool flag = num2 < 0.0 || num2 >= (double)this.hv_imageWidth;
                    bool flag2 = num < 0.0 || num >= (double)this.hv_imageHeight;
                    if (!flag && !flag2)
                    {
                        string text2;
                        if (hTuple == 1)
                        {
                            double grayval = this.hv_image.GetGrayval((int)num, (int)num2);
                            text2 = string.Format("Val: {0:000.0}", grayval);
                        }
                        else
                        {
                            if (hTuple == 3)
                            {
                                HImage hImage = this.hv_image.AccessChannel(1);
                                HImage hImage2 = this.hv_image.AccessChannel(2);
                                HImage hImage3 = this.hv_image.AccessChannel(3);
                                double grayval2 = hImage.GetGrayval((int)num, (int)num2);
                                double grayval3 = hImage2.GetGrayval((int)num, (int)num2);
                                double grayval4 = hImage3.GetGrayval((int)num, (int)num2);
                                hImage.Dispose();
                                hImage2.Dispose();
                                hImage3.Dispose();
                                text2 = string.Format("Val: ({0:000.0}, {1:000.0}, {2:000.0})", grayval2, grayval3, grayval4);
                            }
                            else
                            {
                                text2 = "";
                            }
                        }
                        this.m_CtrlHStatusLabelCtrl.Text = string.Concat(new string[]
                        {
                            this.str_imgSize,
                            "    ",
                            text,
                            "    ",
                            text2
                        });
                    }
                }
                catch (Exception var_15_1D3)
                {
                }
            }
        }
        public void ClearWindow()
        {
            try
            {
                base.Invoke(new Action(() =>
                {
                      this.m_CtrlHStatusLabelCtrl.Visible = false;
                      this.barVisible_strip.Enabled = false;
                      this.fit_strip.Enabled = false;
                      this.histogram_strip.Enabled = false;
                      this.saveImg_strip.Enabled = false;
                      this.saveWindow_strip.Enabled = false;
                      this.mCtrl_HWindow.HalconWindow.ClearWindow();
                      this.viewWindow.ClearWindow();
                      
                  }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void HobjectToHimage(HObject hobject)
        {
            if (hobject == null || !hobject.IsInitialized())
            {
                this.ClearWindow();
            }
            else
            {
                this.Image = new HImage(hobject);
            }
        }
        public void DispObj(HObject hObj)
        {
            bool flag = false;
            try
            {
                Monitor.Enter(this, ref flag);
                this.viewWindow.displayHobject(hObj, null);
            }
            finally
            {
                if (flag)
                {
                    Monitor.Exit(this);
                }
            }
        }
        public void DispObj(HObject hObj, string color)
        {
            bool flag = false;
            try
            {
                Monitor.Enter(this, ref flag);
                this.viewWindow.displayHobject(hObj, color);
            }
            finally
            {
                if (flag)
                {
                    Monitor.Exit(this);
                }
            }
        }
        private void mCtrl_HWindow_MouseLeave(object sender, EventArgs e)
        {
            this.viewWindow.mouseleave();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
                this.hv_MenuStrip.Dispose();
                this.mCtrl_HWindow.HMouseMove-=(new HMouseEventHandler(this.HWindowControl_HMouseMove));
            }
            if (disposing && this.hv_image != null)
            {
                this.hv_image.Dispose();
            }
            if (disposing && this.hv_window != null)
            {
                this.hv_window.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(HWindow_Final));
            this.m_CtrlHStatusLabelCtrl = new Label();
            this.m_CtrlImageList = new ImageList(this.components);
           
            this.mCtrl_HWindow = new HalconDotNet.HWindowControl();
            base.SuspendLayout();
            this.m_CtrlHStatusLabelCtrl.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.m_CtrlHStatusLabelCtrl.AutoSize = true;
            this.m_CtrlHStatusLabelCtrl.Font = new System.Drawing.Font("微软雅黑", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 134);
            this.m_CtrlHStatusLabelCtrl.ForeColor = System.Drawing.SystemColors.WindowText;
            this.m_CtrlHStatusLabelCtrl.Location = new System.Drawing.Point(3, 284);
            this.m_CtrlHStatusLabelCtrl.Margin = new Padding(3);
            this.m_CtrlHStatusLabelCtrl.Name = "m_CtrlHStatusLabelCtrl";
            this.m_CtrlHStatusLabelCtrl.Size = new System.Drawing.Size(0, 17);
            this.m_CtrlHStatusLabelCtrl.TabIndex = 1;
            //this.m_CtrlImageList.ImageStream = (ImageListStreamer)componentResourceManager.GetObject("m_CtrlImageList.ImageStream");
            this.m_CtrlImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.m_CtrlImageList.Images.SetKeyName(0, "TableIcon.png");
            this.m_CtrlImageList.Images.SetKeyName(1, "PicturesIcon.png");
            this.mCtrl_HWindow.BackColor = System.Drawing.Color.Black;
            this.mCtrl_HWindow.BorderColor=(System.Drawing.Color.Black);
            this.mCtrl_HWindow.Cursor = Cursors.Default;
            this.mCtrl_HWindow.Dock = DockStyle.Fill;
            this.mCtrl_HWindow.ImagePart=(new System.Drawing.Rectangle(0, 0, 640, 480));
            this.mCtrl_HWindow.Location = new System.Drawing.Point(0, 0);
            this.mCtrl_HWindow.Margin = new Padding(0);
            this.mCtrl_HWindow.Name = "mCtrl_HWindow";
            this.mCtrl_HWindow.Size = new System.Drawing.Size(395, 304);
            this.mCtrl_HWindow.TabIndex = 0;
            this.mCtrl_HWindow.WindowSize=(new System.Drawing.Size(395, 304));
            this.mCtrl_HWindow.HMouseMove+=new HMouseEventHandler(this.HWindowControl_HMouseMove);
            this.mCtrl_HWindow.MouseLeave += new EventHandler(this.mCtrl_HWindow_MouseLeave);
            base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            base.BorderStyle = BorderStyle.FixedSingle;
            base.Controls.Add(this.m_CtrlHStatusLabelCtrl);
            base.Controls.Add(this.mCtrl_HWindow);
            base.Name = "HWindow_Final";
            base.Size = new System.Drawing.Size(395, 304);
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}
