using HalconDotNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using ViewROI.Config;
namespace ViewWindows.Model
{
    public class HWndCtrl
    {
        public delegate void FuncDelegate();
        public delegate void IconicDelegate(int i);
       // public delegate void GCDelegate(String s);
        public const int MODE_VIEW_NONE = 10;
        public const int MODE_VIEW_ZOOM = 11;
        public const int MODE_VIEW_MOVE = 12;
        public const int MODE_VIEW_ZOOMWINDOW = 13;
        public const int MODE_INCLUDE_ROI = 1;
        public const int MODE_EXCLUDE_ROI = 2;
        public const int EVENT_UPDATE_IMAGE = 31;
        public const int ERR_READING_IMG = 32;
        public const int ERR_DEFINING_GC = 33;
        private const int MAXNUMOBJLIST = 2;
        private int stateView;
        private bool mousePressed = false;
        private double startX;
        private double startY;
        private HWindowControl viewPort;
        private ROIController roiManager;
        private int dispROI;
        public bool drawModel = false;
        private int windowWidth;
        private int windowHeight;
        private int imageWidth;
        private int imageHeight;
        private int[] CompRangeX;
        private int[] CompRangeY;
        private int prevCompX;
        private int prevCompY;
        private double stepSizeX;
        private double stepSizeY;
        private double ImgRow1;
        private double ImgCol1;
        private double ImgRow2;
        private double ImgCol2;
        public string exceptionText = "";
        public FuncDelegate addInfoDelegate;
        public IconicDelegate NotifyIconObserver;
        private HWindow ZoomWindow;
        private double zoomWndFactor;
        private double zoomAddOn;
        private int zoomWndSize;
        private ArrayList HObjImageList;
        private GraphicsContext mGC;
        private List<HObjectWithColor> hObjectList = new List<HObjectWithColor>();
        protected internal HWndCtrl(HWindowControl view)
        {
            this.viewPort = view;
            this.stateView = 10;
            this.windowWidth = this.viewPort.Size.Width;
            this.windowHeight = this.viewPort.Size.Height;
            this.zoomWndFactor = (double)this.imageWidth / (double)this.viewPort.Width;
            this.zoomAddOn = Math.Pow(0.9, 5.0);
            this.zoomWndSize = 150;
            this.CompRangeX = new int[]
            {
                0,
                100
            };
            this.CompRangeY = new int[]
            {
                0,
                100
            };
            this.prevCompX = (this.prevCompY = 0);
            this.dispROI = 1;
            this.viewPort.HMouseUp+=new HMouseEventHandler(this.mouseUp);
            this.viewPort.HMouseDown+=new HMouseEventHandler(this.mouseDown);
            this.viewPort.HMouseWheel+=new HMouseEventHandler(this.HMouseWheel);
            this.viewPort.HMouseMove+=new HMouseEventHandler(this.mouseMoved);
            this.addInfoDelegate = new FuncDelegate(this.dummyV);
            this.NotifyIconObserver = new IconicDelegate(this.dummy);
            this.HObjImageList = new ArrayList(20);
            this.mGC = new GraphicsContext();
            this.mGC.gcNotification =new GraphicsContext.GCDelegate(this.exceptionGC) ;
        }
        private void HMouseWheel(object sender, HMouseEventArgs e)
        {
            if (!this.drawModel)
            {
                double scale;
                if (e.Delta > 0)
                {
                    scale = 0.9;
                }
                else
                {
                    scale = 1.1111111111111112;
                }
                this.zoomImage(e.X, e.Y, scale);
            }
        }
        private void setImagePart(HImage image)
        {
            string text;
            int c;
            int r;
            image.GetImagePointer1(out text, out c, out r);
            this.setImagePart(0, 0, r, c);
        }
        private void setImagePart(int r1, int c1, int r2, int c2)
        {
            this.ImgRow1 = (double)r1;
            this.ImgCol1 = (double)c1;
            this.imageHeight = r2;
            this.ImgRow2 = (double)r2;
            this.imageWidth = c2;
            this.ImgCol2 = (double)c2;
            System.Drawing.Rectangle imagePart = this.viewPort.ImagePart;
            imagePart.X = (int)this.ImgCol1;
            imagePart.Y = (int)this.ImgRow1;
            imagePart.Height = this.imageHeight;
            imagePart.Width = this.imageWidth;
            this.viewPort.ImagePart=imagePart;
        }
        protected internal void setViewState(int mode)
        {
            this.stateView = mode;
            if (this.roiManager != null)
            {
                this.roiManager.resetROI();
            }
        }
        private void dummy(int val)
        {
        }
        private void dummyV()
        {
        }
        private void exceptionGC(string message)
        {
            this.exceptionText = message;
          
            this.NotifyIconObserver (33);
        }
        public void setDispLevel(int mode)
        {
            this.dispROI = mode;
        }
        private void zoomImage(double x, double y, double scale)
        {
            if (!this.drawModel)
            {
                double num = (x - this.ImgCol1) / (this.ImgCol2 - this.ImgCol1);
                double num2 = (y - this.ImgRow1) / (this.ImgRow2 - this.ImgRow1);
                double num3 = (this.ImgCol2 - this.ImgCol1) * scale;
                double num4 = (this.ImgRow2 - this.ImgRow1) * scale;
                this.ImgCol1 = x - num3 * num;
                this.ImgCol2 = x + num3 * (1.0 - num);
                this.ImgRow1 = y - num4 * num2;
                this.ImgRow2 = y + num4 * (1.0 - num2);
                int num5 = (int)Math.Round(num3);
                int num6 = (int)Math.Round(num4);
                System.Drawing.Rectangle imagePart = this.viewPort.ImagePart;
                imagePart.X = (int)Math.Round(this.ImgCol1);
                imagePart.Y = (int)Math.Round(this.ImgRow1);
                imagePart.Width = ((num5 > 0) ? num5 : 1);
                imagePart.Height = ((num6 > 0) ? num6 : 1);
                this.viewPort.ImagePart=imagePart;
                double num7 = scale * this.zoomWndFactor;
                if (this.zoomWndFactor < 0.01 && num7 < this.zoomWndFactor)
                {
                    this.resetWindow();
                }
                else
                {
                    if (this.zoomWndFactor > 100.0 && num7 > this.zoomWndFactor)
                    {
                        this.resetWindow();
                    }
                    else
                    {
                        this.zoomWndFactor = num7;
                        this.repaint();
                    }
                }
            }
        }
        public void zoomImage(double scaleFactor)
        {
            if (this.ImgRow2 - this.ImgRow1 == scaleFactor * (double)this.imageHeight && this.ImgCol2 - this.ImgCol1 == scaleFactor * (double)this.imageWidth)
            {
                this.repaint();
            }
            else
            {
                this.ImgRow2 = this.ImgRow1 + (double)this.imageHeight;
                this.ImgCol2 = this.ImgCol1 + (double)this.imageWidth;
                double imgCol = this.ImgCol1;
                double imgRow = this.ImgRow1;
                this.zoomWndFactor = (double)this.imageWidth / (double)this.viewPort.Width;
                this.zoomImage(imgCol, imgRow, scaleFactor);
            }
        }
        public void scaleWindow(double scale)
        {
            this.ImgRow1 = 0.0;
            this.ImgCol1 = 0.0;
            this.ImgRow2 = (double)this.imageHeight;
            this.ImgCol2 = (double)this.imageWidth;
            this.viewPort.Width = (int)(this.ImgCol2 * scale);
            this.viewPort.Height = (int)(this.ImgRow2 * scale);
            this.zoomWndFactor = (double)this.imageWidth / (double)this.viewPort.Width;
        }
        public void setZoomWndFactor()
        {
            this.zoomWndFactor = (double)this.imageWidth / (double)this.viewPort.Width;
        }
        public void setZoomWndFactor(double zoomF)
        {
            this.zoomWndFactor = zoomF;
        }
        private void moveImage(double motionX, double motionY)
        {
            this.ImgRow1 += -motionY;
            this.ImgRow2 += -motionY;
            this.ImgCol1 += -motionX;
            this.ImgCol2 += -motionX;
            System.Drawing.Rectangle imagePart = this.viewPort.ImagePart;
            imagePart.X = (int)Math.Round(this.ImgCol1);
            imagePart.Y = (int)Math.Round(this.ImgRow1);
            this.viewPort.ImagePart=imagePart;
            this.repaint();
        }
        protected internal void resetAll()
        {
            this.ImgRow1 = 0.0;
            this.ImgCol1 = 0.0;
            this.ImgRow2 = (double)this.imageHeight;
            this.ImgCol2 = (double)this.imageWidth;
            this.zoomWndFactor = (double)this.imageWidth / (double)this.viewPort.Width;
            System.Drawing.Rectangle imagePart = this.viewPort.ImagePart;
            imagePart.X = (int)this.ImgCol1;
            imagePart.Y = (int)this.ImgRow1;
            imagePart.Width = this.imageWidth;
            imagePart.Height = this.imageHeight;
            this.viewPort.ImagePart=imagePart;
            if (this.roiManager != null)
            {
                this.roiManager.reset();
            }
        }
        protected internal void resetWindow()
        {
            this.ImgRow1 = 0.0;
            this.ImgCol1 = 0.0;
            this.ImgRow2 = (double)this.imageHeight;
            this.ImgCol2 = (double)this.imageWidth;
            this.zoomWndFactor = (double)this.imageWidth / (double)this.viewPort.Width;
            System.Drawing.Rectangle imagePart = this.viewPort.ImagePart;
            imagePart.X = (int)this.ImgCol1;
            imagePart.Y = (int)this.ImgRow1;
            imagePart.Width = this.imageWidth;
            imagePart.Height = this.imageHeight;
            this.viewPort.ImagePart=imagePart;
        }
        private void mouseDown(object sender, HMouseEventArgs e)
        {
            if (!this.drawModel)
            {
                this.stateView = 12;
                this.mousePressed = true;
                int num = -1;
                if (this.roiManager != null && this.dispROI == 1)
                {
                    num = this.roiManager.mouseDownAction(e.X, e.Y);
                }
                if (num == -1)
                {
                    switch (this.stateView)
                    {
                        case 12:
                            this.startX = e.X;
                            this.startY = e.Y;
                            break;
                        case 13:
                            this.activateZoomWindow((int)e.X, (int)e.Y);
                            break;
                    }
                }
            }
        }
        private void activateZoomWindow(int X, int Y)
        {
            if (this.ZoomWindow != null)
            {
                this.ZoomWindow.Dispose();
            }
            HOperatorSet.SetSystem("border_width", 10);
            this.ZoomWindow = new HWindow();
            double num = ((double)X - this.ImgCol1) / (this.ImgCol2 - this.ImgCol1) * (double)this.viewPort.Width;
            double num2 = ((double)Y - this.ImgRow1) / (this.ImgRow2 - this.ImgRow1) * (double)this.viewPort.Height;
            int num3 = (int)((double)(this.zoomWndSize / 2) * this.zoomWndFactor * this.zoomAddOn);
            this.ZoomWindow.OpenWindow((int)num2 - this.zoomWndSize / 2, (int)num - this.zoomWndSize / 2, this.zoomWndSize, this.zoomWndSize, this.viewPort.HalconID, "visible", "");
            this.ZoomWindow.SetPart(Y - num3, X - num3, Y + num3, X + num3);
            this.repaint(this.ZoomWindow);
            this.ZoomWindow.SetColor("black");
        }
        public void raiseMouseup()
        {
            this.mousePressed = false;
            if (this.roiManager != null && this.roiManager.activeROIidx != -1 && this.dispROI == 1)
            {
                this.roiManager.NotifyRCObserver(50);
            }
            else
            {
                if (this.stateView == 13)
                {
                    this.ZoomWindow.Dispose();
                }
            }
        }
        private void mouseUp(object sender, HMouseEventArgs e)
        {
            if (!this.drawModel)
            {
                this.mousePressed = false;
                if (this.roiManager != null && this.roiManager.activeROIidx != -1 && this.dispROI == 1)
                {
                    this.roiManager.NotifyRCObserver(50);
                }
                else
                {
                    if (this.stateView == 13)
                    {
                        this.ZoomWindow.Dispose();
                    }
                }
            }
        }
        private void mouseMoved(object sender, HMouseEventArgs e)
        {
            if (!this.drawModel)
            {
                if (this.mousePressed)
                {
                    if (this.roiManager != null && this.roiManager.activeROIidx != -1 && this.dispROI == 1)
                    {
                        this.roiManager.mouseMoveAction(e.X, e.Y);
                    }
                    else
                    {
                        if (this.stateView == 12)
                        {
                            double num = e.X - this.startX;
                            double num2 = e.Y - this.startY;
                            if ((int)num != 0 || (int)num2 != 0)
                            {
                                this.moveImage(num, num2);
                                this.startX = e.X - num;
                                this.startY = e.Y - num2;
                            }
                        }
                        else
                        {
                            if (this.stateView == 13)
                            {
                                HSystem.SetSystem("flush_graphic", "false");
                                this.ZoomWindow.ClearWindow();
                                double num3 = (e.X - this.ImgCol1) / (this.ImgCol2 - this.ImgCol1) * (double)this.viewPort.Width;
                                double num4 = (e.Y - this.ImgRow1) / (this.ImgRow2 - this.ImgRow1) * (double)this.viewPort.Height;
                                double num5 = (double)(this.zoomWndSize / 2) * this.zoomWndFactor * this.zoomAddOn;
                                this.ZoomWindow.SetWindowExtents((int)num4 - this.zoomWndSize / 2, (int)num3 - this.zoomWndSize / 2, this.zoomWndSize, this.zoomWndSize);
                                this.ZoomWindow.SetPart((int)(e.Y - num5), (int)(e.X - num5), (int)(e.Y + num5), (int)(e.X + num5));
                                this.repaint(this.ZoomWindow);
                                HSystem.SetSystem("flush_graphic", "true");
                                this.ZoomWindow.DispLine(-100.0, -100.0, -100.0, -100.0);
                            }
                        }
                    }
                }
            }
        }
        public void setGUICompRangeX(int[] xRange, int Init)
        {
            this.CompRangeX = xRange;
            int num = xRange[1] - xRange[0];
            this.prevCompX = Init;
            this.stepSizeX = (double)this.imageWidth / (double)num * (double)(this.imageWidth / this.windowWidth);
        }
        public void setGUICompRangeY(int[] yRange, int Init)
        {
            this.CompRangeY = yRange;
            int num = yRange[1] - yRange[0];
            this.prevCompY = Init;
            this.stepSizeY = (double)this.imageHeight / (double)num * (double)(this.imageHeight / this.windowHeight);
        }
        public void resetGUIInitValues(int xVal, int yVal)
        {
            this.prevCompX = xVal;
            this.prevCompY = yVal;
        }
        public void moveXByGUIHandle(int valX)
        {
            double num = (double)(valX - this.prevCompX) * this.stepSizeX;
            if (num != 0.0)
            {
                this.moveImage(num, 0.0);
                this.prevCompX = valX;
            }
        }
        public void moveYByGUIHandle(int valY)
        {
            double num = (double)(valY - this.prevCompY) * this.stepSizeY;
            if (num != 0.0)
            {
                this.moveImage(0.0, num);
                this.prevCompY = valY;
            }
        }
        public void zoomByGUIHandle(double valF)
        {
            double x = this.ImgCol1 + (this.ImgCol2 - this.ImgCol1) / 2.0;
            double y = this.ImgRow1 + (this.ImgRow2 - this.ImgRow1) / 2.0;
            double num = (this.ImgCol2 - this.ImgCol1) / (double)this.imageWidth;
            double scale = 1.0 / num * (100.0 / valF);
            this.zoomImage(x, y, scale);
        }
        public void repaint()
        {
            this.repaint(this.viewPort.HalconWindow);
        }
        public void repaint(HWindow window)
        {
            try
            {
                int count = this.HObjImageList.Count;
                HSystem.SetSystem("flush_graphic", "false");
                window.ClearWindow();
                this.mGC.stateOfSettings.Clear();
                for (int i = 0; i < count; i++)
                {
                    HObjectEntry hObjectEntry = (HObjectEntry)this.HObjImageList[i];
                    this.mGC.applyContext(window, hObjectEntry.gContext);
                    window.DispObj(hObjectEntry.HObj);
                }
                this.showHObjectList();
                this.addInfoDelegate();
                if (this.roiManager != null && this.dispROI == 1)
                {
                    this.roiManager.paintData(window);
                }
                HSystem.SetSystem("flush_graphic", "true");
                window.SetColor("black");
                window.DispLine(-100.0, -100.0, -101.0, -101.0);
            }
            catch (Exception)
            {
            }
        }
        public void addIconicVar(HObject img)
        {
            for (int i = 0; i < this.HObjImageList.Count; i++)
            {
                ((HObjectEntry)this.HObjImageList[i]).clear();
            }
            if (img != null)
            {
                HTuple hTuple = null;
                HOperatorSet.GetObjClass(img, out hTuple);
                if (hTuple.S.Equals("image"))
                {
                    HImage hImage = new HImage(img);
                    if (hImage != null)
                    {
                        double num2;
                        double num3;
                        int num = hImage.GetDomain().AreaCenter(out num2, out num3);
                        string text;
                        int num4;
                        int num5;
                        hImage.GetImagePointer1(out text, out num4, out num5);
                        if (num == num4 * num5)
                        {
                            this.clearList();
                            if (num5 != this.imageHeight || num4 != this.imageWidth)
                            {
                                this.imageHeight = num5;
                                this.imageWidth = num4;
                                this.zoomWndFactor = (double)this.imageWidth / (double)this.viewPort.Width;
                                this.setImagePart(0, 0, num5, num4);
                            }
                        }
                    }
                    HObjectEntry value = new HObjectEntry(hImage, this.mGC.copyContextList());
                    this.HObjImageList.Add(value);
                    this.clearHObjectList();
                    if (this.HObjImageList.Count > 2)
                    {
                        ((HObjectEntry)this.HObjImageList[0]).clear();
                        this.HObjImageList.RemoveAt(1);
                    }
                }
            }
        }
        public void clearList()
        {
            this.HObjImageList.Clear();
        }
        public int getListCount()
        {
            return this.HObjImageList.Count;
        }
        public void changeGraphicSettings(string mode, string val)
        {
            if (mode != null)
            {
                if (!(mode == "Color"))
                {
                    if (!(mode == "DrawMode"))
                    {
                        if (!(mode == "Lut"))
                        {
                            if (!(mode == "Paint"))
                            {
                                if (mode == "Shape")
                                {
                                    this.mGC.setShapeAttribute(val);
                                }
                            }
                            else
                            {
                                this.mGC.setPaintAttribute(val);
                            }
                        }
                        else
                        {
                            this.mGC.setLutAttribute(val);
                        }
                    }
                    else
                    {
                        this.mGC.setDrawModeAttribute(val);
                    }
                }
                else
                {
                    this.mGC.setColorAttribute(val);
                }
            }
        }
        public void changeGraphicSettings(string mode, int val)
        {
            if (mode != null)
            {
                if (!(mode == "Colored"))
                {
                    if (mode == "LineWidth")
                    {
                        this.mGC.setLineWidthAttribute(val);
                    }
                }
                else
                {
                    this.mGC.setColoredAttribute(val);
                }
            }
        }
        public void changeGraphicSettings(string mode, HTuple val)
        {
            if (mode != null)
            {
                if (mode == "LineStyle")
                {
                    this.mGC.setLineStyleAttribute(val);
                }
            }
        }
        public void clearGraphicContext()
        {
            this.mGC.clear();
        }
        public Hashtable getGraphicContext()
        {
            return this.mGC.copyContextList();
        }
        protected internal void setROIController(ROIController rC)
        {
            this.roiManager = rC;
            rC.setViewController(this);
            this.setViewState(10);
        }
        protected internal void addImageShow(HObject image)
        {
            this.addIconicVar(image);
        }
        public void DispObj(HObject hObj)
        {
            this.DispObj(hObj, null);
        }
        public void DispObj(HObject hObj, string color)
        {
            bool flag = false;
            try
            {
                Monitor.Enter(this, ref flag);
                if (color != null)
                {
                    HOperatorSet.SetColor(this.viewPort.HalconWindow, color);
                }
                else
                {
                    HOperatorSet.SetColor(this.viewPort.HalconWindow, "red");
                }
                if (hObj != null && hObj.IsInitialized())
                {
                    HObject hObject = new HObject(hObj);
                    this.hObjectList.Add(new HObjectWithColor(hObject, color));
                    this.viewPort.HalconWindow.DispObj(hObject);
                }
                HOperatorSet.SetColor(this.viewPort.HalconWindow, "red");
            }
            finally
            {
                if (flag)
                {
                    Monitor.Exit(this);
                }
            }
        }
        public void clearHObjectList()
        {
            foreach (HObjectWithColor current in this.hObjectList)
            {
                current.HObject.Dispose();
            }
            this.hObjectList.Clear();
        }
        private void showHObjectList()
        {
            try
            {
                foreach (HObjectWithColor current in this.hObjectList)
                {
                    if (current.Color != null)
                    {
                        HOperatorSet.SetColor(this.viewPort.HalconWindow, current.Color);
                    }
                    else
                    {
                        HOperatorSet.SetColor(this.viewPort.HalconWindow, "red");
                    }
                    if (current != null && current.HObject.IsInitialized())
                    {
                        this.viewPort.HalconWindow.DispObj(current.HObject);
                        HOperatorSet.SetColor(this.viewPort.HalconWindow, "red");
                    }
                }
            }
            catch (Exception var_1_E7)
            {
            }
        }
    }
}