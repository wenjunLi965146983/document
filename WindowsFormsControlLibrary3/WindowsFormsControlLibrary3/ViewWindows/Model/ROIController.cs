using HalconDotNet;
using System;
using System.Collections;
using System.Collections.Generic;
namespace ViewWindows.Model
{
    public class ROIController
    {
        public delegate void IconicDelegate(int i);
        public const int MODE_ROI_POS = 21;
        public const int MODE_ROI_NEG = 22;
        public const int MODE_ROI_NONE = 23;
        public const int EVENT_UPDATE_ROI = 50;
        public const int EVENT_CHANGED_ROI_SIGN = 51;
        public const int EVENT_MOVING_ROI = 52;
        public const int EVENT_DELETED_ACTROI = 53;
        public const int EVENT_DELETED_ALL_ROIS = 54;
        public const int EVENT_ACTIVATED_ROI = 55;
        public const int EVENT_CREATED_ROI = 56;
        private ROI roiMode;
        private int stateROI;
        private double currX;
        private double currY;
        public int activeROIidx;
        public int deletedIdx;
        public ArrayList ROIList;
        public HRegion ModelROI;
        private string activeCol = "green";
        private string activeHdlCol = "red";
        private string inactiveCol = "yellow";
        public HWndCtrl viewController;
        public IconicDelegate NotifyRCObserver;
        protected internal ROIController()
        {
            this.stateROI = 23;
            this.ROIList = new ArrayList();
            this.activeROIidx = -1;
            this.ModelROI = new HRegion();
            this.NotifyRCObserver = new IconicDelegate(this.dummyI);
            this.deletedIdx = -1;
            this.currX = (this.currY = -1.0);
        }
        public void setViewController(HWndCtrl view)
        {
            this.viewController = view;
        }
        public HRegion getModelRegion()
        {
            return this.ModelROI;
        }
        public ArrayList getROIList()
        {
            return this.ROIList;
        }
        public ROI getActiveROI()
        {
            ROI result;
            try
            {
                if (this.activeROIidx != -1)
                {
                    result = (ROI)this.ROIList[this.activeROIidx];
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        public int getActiveROIIdx()
        {
            return this.activeROIidx;
        }
        public void setActiveROIIdx(int active)
        {
            this.activeROIidx = active;
        }
        public int getDelROIIdx()
        {
            return this.deletedIdx;
        }
        public void setROIShape(ROI r)
        {
            this.roiMode = r;
            this.roiMode.setOperatorFlag(this.stateROI);
        }
        public void setROISign(int mode)
        {
            this.stateROI = mode;
            if (this.activeROIidx != -1)
            {
                ((ROI)this.ROIList[this.activeROIidx]).setOperatorFlag(this.stateROI);
                this.viewController.repaint();
                this.NotifyRCObserver(51);
            }
        }
        public void removeActive()
        {
            if (this.activeROIidx != -1)
            {
                this.ROIList.RemoveAt(this.activeROIidx);
                this.deletedIdx = this.activeROIidx;
                this.activeROIidx = -1;
                this.viewController.repaint();
                this.NotifyRCObserver(53);
            }
        }
        public bool defineModelROI()
        {
            bool result;
            if (this.stateROI == 23)
            {
                result = true;
            }
            else
            {
                HRegion hRegion = new HRegion();
                HRegion hRegion2 = new HRegion();
                hRegion.GenEmptyRegion();
                hRegion2.GenEmptyRegion();
                for (int i = 0; i < this.ROIList.Count; i++)
                {
                    switch (((ROI)this.ROIList[i]).getOperatorFlag())
                    {
                        case 21:
                            {
                                HRegion hRegion3 = ((ROI)this.ROIList[i]).getRegion();
                                hRegion = hRegion3.Union2(hRegion);
                                break;
                            }
                        case 22:
                            {
                                HRegion hRegion3 = ((ROI)this.ROIList[i]).getRegion();
                                hRegion2 = hRegion3.Union2(hRegion2);
                                break;
                            }
                    }
                }
                this.ModelROI = null;
                double num;
                double num2;
                if (hRegion.AreaCenter(out num, out num2) > 0)
                {
                    HRegion hRegion3 = hRegion.Difference(hRegion2);
                    if (hRegion3.AreaCenter(out num, out num2) > 0)
                    {
                        this.ModelROI = hRegion3;
                    }
                }
                result = (this.ModelROI != null && this.ROIList.Count != 0);
            }
            return result;
        }
        public void reset()
        {
            this.ROIList.Clear();
            this.activeROIidx = -1;
            this.ModelROI = null;
            this.roiMode = null;
            this.NotifyRCObserver(54);
        }
        public void resetROI()
        {
            this.activeROIidx = -1;
            this.roiMode = null;
        }
        public void setDrawColor(string aColor, string aHdlColor, string inaColor)
        {
            if (aColor != "")
            {
                this.activeCol = aColor;
            }
            if (aHdlColor != "")
            {
                this.activeHdlCol = aHdlColor;
            }
            if (inaColor != "")
            {
                this.inactiveCol = inaColor;
            }
        }
        public void paintData(HWindow window)
        {
            window.SetDraw("margin");
            window.SetLineWidth(1);
            if (this.ROIList.Count > 0)
            {
                window.SetDraw("margin");
                for (int i = 0; i < this.ROIList.Count; i++)
                {
                    window.SetColor(((ROI)this.ROIList[i]).Color);
                    window.SetLineStyle(((ROI)this.ROIList[i]).flagLineStyle);
                    ((ROI)this.ROIList[i]).draw(window);
                }
                if (this.activeROIidx != -1)
                {
                    window.SetColor(this.activeCol);
                    window.SetLineStyle(((ROI)this.ROIList[this.activeROIidx]).flagLineStyle);
                    ((ROI)this.ROIList[this.activeROIidx]).draw(window);
                    window.SetColor(this.activeHdlCol);
                    ((ROI)this.ROIList[this.activeROIidx]).displayActive(window);
                }
            }
        }
        public int mouseDownAction(double imgX, double imgY)
        {
            int num = -1;
            double num2 = 10000.0;
            double num3 = 35.0;
            if (this.roiMode != null)
            {
                this.roiMode.createROI(imgX, imgY);
                this.ROIList.Add(this.roiMode);
                this.roiMode = null;
                this.activeROIidx = this.ROIList.Count - 1;
                this.viewController.repaint();
                this.NotifyRCObserver(56);
            }
            else
            {
                if (this.ROIList.Count > 0)
                {
                    this.activeROIidx = -1;
                    for (int i = 0; i < this.ROIList.Count; i++)
                    {
                        double num4 = ((ROI)this.ROIList[i]).distToClosestHandle(imgX, imgY);
                        if (num4 < num2 && num4 < num3)
                        {
                            num2 = num4;
                            num = i;
                        }
                    }
                    if (num >= 0)
                    {
                        this.activeROIidx = num;
                        this.NotifyRCObserver(55);
                    }
                    this.viewController.repaint();
                }
            }
            return this.activeROIidx;
        }
        public void mouseMoveAction(double newX, double newY)
        {
            try
            {
                if (newX != this.currX || newY != this.currY)
                {
                    ((ROI)this.ROIList[this.activeROIidx]).moveByHandle(newX, newY);
                    this.viewController.repaint();
                    this.currX = newX;
                    this.currY = newY;
                    this.NotifyRCObserver(52);
                }
            }
            catch (Exception)
            {
            }
        }
        public void dummyI(int v)
        {
        }
        public void displayRect1(string color, double row1, double col1, double row2, double col2)
        {
            this.setROIShape(new ROIRectangle1());
            if (this.roiMode != null)
            {
                this.roiMode.createRectangle1(row1, col1, row2, col2);
                this.roiMode.Type = this.roiMode.GetType().Name;
                this.roiMode.Color = color;
                this.ROIList.Add(this.roiMode);
                this.roiMode = null;
                this.activeROIidx = this.ROIList.Count - 1;
                this.viewController.repaint();
                this.NotifyRCObserver(56);
            }
        }
        public void displayRect2(string color, double row, double col, double phi, double length1, double length2)
        {
            this.setROIShape(new ROIRectangle2());
            if (this.roiMode != null)
            {
                this.roiMode.createRectangle2(row, col, phi, length1, length2);
                this.roiMode.Type = this.roiMode.GetType().Name;
                this.roiMode.Color = color;
                this.ROIList.Add(this.roiMode);
                this.roiMode = null;
                this.activeROIidx = this.ROIList.Count - 1;
                this.viewController.repaint();
                this.NotifyRCObserver(56);
            }
        }
        public void displayCircle(string color, double row, double col, double radius)
        {
            this.setROIShape(new ROICircle());
            if (this.roiMode != null)
            {
                this.roiMode.createCircle(row, col, radius);
                this.roiMode.Type = this.roiMode.GetType().Name;
                this.roiMode.Color = color;
                this.ROIList.Add(this.roiMode);
                this.roiMode = null;
                this.activeROIidx = this.ROIList.Count - 1;
                this.viewController.repaint();
                this.NotifyRCObserver(56);
            }
        }
        public void displayLine(string color, double beginRow, double beginCol, double endRow, double endCol)
        {
            this.setROIShape(new ROILine());
            if (this.roiMode != null)
            {
                this.roiMode.createLine(beginRow, beginCol, endRow, endCol);
                this.roiMode.Type = this.roiMode.GetType().Name;
                this.roiMode.Color = color;
                this.ROIList.Add(this.roiMode);
                this.roiMode = null;
                this.activeROIidx = this.ROIList.Count - 1;
                this.viewController.repaint();
                this.NotifyRCObserver(56);
            }
        }
        protected internal void genRect1(double row1, double col1, double row2, double col2, ref List<ROI> rois)
        {
            this.setROIShape(new ROIRectangle1());
            if (rois == null)
            {
                rois = new List<ROI>();
            }
            if (this.roiMode != null)
            {
                this.roiMode.createRectangle1(row1, col1, row2, col2);
                this.roiMode.Type = this.roiMode.GetType().Name;
                rois.Add(this.roiMode);
                this.ROIList.Add(this.roiMode);
                this.roiMode = null;
                this.activeROIidx = this.ROIList.Count - 1;
                this.viewController.repaint();
                this.NotifyRCObserver(56);
            }
        }
        protected internal void genRect2(double row, double col, double phi, double length1, double length2, ref List<ROI> rois)
        {
            this.setROIShape(new ROIRectangle2());
            if (rois == null)
            {
                rois = new List<ROI>();
            }
            if (this.roiMode != null)
            {
                this.roiMode.createRectangle2(row, col, phi, length1, length2);
                this.roiMode.Type = this.roiMode.GetType().Name;
                rois.Add(this.roiMode);
                this.ROIList.Add(this.roiMode);
                this.roiMode = null;
                this.activeROIidx = this.ROIList.Count - 1;
                this.viewController.repaint();
                this.NotifyRCObserver(56);
            }
        }
        protected internal void genCircle(double row, double col, double radius, ref List<ROI> rois)
        {
            this.setROIShape(new ROICircle());
            if (rois == null)
            {
                rois = new List<ROI>();
            }
            if (this.roiMode != null)
            {
                this.roiMode.createCircle(row, col, radius);
                this.roiMode.Type = this.roiMode.GetType().Name;
                rois.Add(this.roiMode);
                this.ROIList.Add(this.roiMode);
                this.roiMode = null;
                this.activeROIidx = this.ROIList.Count - 1;
                this.viewController.repaint();
                this.NotifyRCObserver(56);
            }
        }
        protected internal void genLine(double beginRow, double beginCol, double endRow, double endCol, ref List<ROI> rois)
        {
            this.setROIShape(new ROILine());
            if (rois == null)
            {
                rois = new List<ROI>();
            }
            if (this.roiMode != null)
            {
                this.roiMode.createLine(beginRow, beginCol, endRow, endCol);
                this.roiMode.Type = this.roiMode.GetType().Name;
                rois.Add(this.roiMode);
                this.ROIList.Add(this.roiMode);
                this.roiMode = null;
                this.activeROIidx = this.ROIList.Count - 1;
                this.viewController.repaint();
                this.NotifyRCObserver(56);
            }
        }
        protected internal List<double> smallestActiveROI(out string name, out int index)
        {
            name = "";
            int activeROIIdx = this.getActiveROIIdx();
            index = activeROIIdx;
            List<double> result;
            if (activeROIIdx > -1)
            {
                ROI activeROI = this.getActiveROI();
                Type type = activeROI.GetType();
                name = type.Name;
                HTuple modelData = activeROI.getModelData();
                List<double> list = new List<double>();
                for (int i = 0; i < modelData.Length; i++)
                {
                    list.Add(modelData[i].D);
                }
                result = list;
            }
            else
            {
                result = null;
            }
            return result;
        }
        protected internal ROI smallestActiveROI(out List<double> data, out int index)
        {
            ROI result;
            try
            {
                int activeROIIdx = this.getActiveROIIdx();
                index = activeROIIdx;
                data = new List<double>();
                if (activeROIIdx > -1)
                {
                    ROI activeROI = this.getActiveROI();
                    Type type = activeROI.GetType();
                    HTuple modelData = activeROI.getModelData();
                    for (int i = 0; i < modelData.Length; i++)
                    {
                        data.Add(modelData[i].D);
                    }
                    result = activeROI;
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception)
            {
                data = null;
                index = 0;
                result = null;
            }
            return result;
        }
        protected internal void removeActiveROI(ref List<ROI> roi)
        {
            int activeROIIdx = this.getActiveROIIdx();
            if (activeROIIdx > -1)
            {
                this.removeActive();
                roi.RemoveAt(activeROIIdx);
            }
        }
        protected internal void selectROI(int index)
        {
            this.activeROIidx = index;
            this.NotifyRCObserver(55);
            this.viewController.repaint();
        }
        protected internal void resetWindowImage()
        {
            this.viewController.repaint();
        }
        protected internal void zoomWindowImage()
        {
            this.viewController.setViewState(11);
        }
        protected internal void moveWindowImage()
        {
            this.viewController.setViewState(12);
        }
        protected internal void noneWindowImage()
        {
            this.viewController.setViewState(10);
        }
    }
}