using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewWindows.Config;
using ViewWindows.Model;
namespace ViewWindows
{
    public class ViewWindow : IViewWindow
    {
        private HWndCtrl _hWndControl;
        private ROIController _roiController;
        public ViewWindow(HWindowControl window)
        {
            this._hWndControl = new HWndCtrl(window);
            this._roiController = new ROIController();
            this._hWndControl.setROIController(this._roiController);
            this._hWndControl.setViewState(10);
        }
        public void ClearWindow()
        {
            this._hWndControl.clearList();
            this._hWndControl.clearHObjectList();
        }
        public void displayImage(HObject img)
        {
            this._hWndControl.addImageShow(img);
            this._roiController.reset();
            this._roiController.resetWindowImage();
        }
        public void notDisplayRoi()
        {
            this._roiController.reset();
            this._roiController.resetWindowImage();
        }
        public int getRoiCount()
        {
            return this._roiController.ROIList.Count;
        }
        public void setDrawModel(bool flag)
        {
            this._hWndControl.drawModel = flag;
        }
        public void resetWindowImage()
        {
            this._hWndControl.resetWindow();
            this._roiController.resetWindowImage();
        }
        public void mouseleave()
        {
            this._hWndControl.raiseMouseup();
        }
        public void zoomWindowImage()
        {
            this._roiController.zoomWindowImage();
        }
        public void moveWindowImage()
        {
            this._roiController.moveWindowImage();
        }
        public void noneWindowImage()
        {
            this._roiController.noneWindowImage();
        }
        public void genRect1(double row1, double col1, double row2, double col2, ref List<ROI> rois)
        {
            this._roiController.genRect1(row1, col1, row2, col2, ref rois);
        }
        public void genRect2(double row, double col, double phi, double length1, double length2, ref List<ROI> rois)
        {
            this._roiController.genRect2(row, col, phi, length1, length2, ref rois);
        }
        public void genCircle(double row, double col, double radius, ref List<ROI> rois)
        {
            this._roiController.genCircle(row, col, radius, ref rois);
        }
        public void genLine(double beginRow, double beginCol, double endRow, double endCol, ref List<ROI> rois)
        {
            this._roiController.genLine(beginRow, beginCol, endRow, endCol, ref rois);
        }
        public List<double> smallestActiveROI(out string name, out int index)
        {
            return this._roiController.smallestActiveROI(out name, out index);
        }
        public ROI smallestActiveROI(out List<double> data, out int index)
        {
            return this._roiController.smallestActiveROI(out data, out index);
        }
        public void selectROI(int index)
        {
            this._roiController.selectROI(index);
        }
        public void selectROI(List<ROI> rois, int index)
        {
            if (rois.Count > index && index >= 0)
            {
                this._hWndControl.resetAll();
                this._hWndControl.repaint();
                HTuple modelData = rois[index].getModelData();
                string type = rois[index].Type;
                if (type != null)
                {
                    if (!(type == "ROIRectangle1"))
                    {
                        if (!(type == "ROIRectangle2"))
                        {
                            if (!(type == "ROICircle"))
                            {
                                if (type == "ROILine")
                                {
                                    if (modelData != null)
                                    {
                                        this._roiController.displayLine(rois[index].Color, modelData[0].D, modelData[1].D, modelData[2].D, modelData[3].D);
                                    }
                                }
                            }
                            else
                            {
                                if (modelData != null)
                                {
                                    this._roiController.displayCircle(rois[index].Color, modelData[0].D, modelData[1].D, modelData[2].D);
                                }
                            }
                        }
                        else
                        {
                            if (modelData != null)
                            {
                                this._roiController.displayRect2(rois[index].Color, modelData[0].D, modelData[1].D, modelData[2].D, modelData[3].D, modelData[4].D);
                            }
                        }
                    }
                    else
                    {
                        if (modelData != null)
                        {
                            this._roiController.displayRect1(rois[index].Color, modelData[0].D, modelData[1].D, modelData[2].D, modelData[3].D);
                        }
                    }
                }
            }
        }
        public void displayROI(List<ROI> rois)
        {
            if (rois != null)
            {
                foreach (ROI current in rois)
                {
                    HTuple modelData = current.getModelData();
                    string type = current.Type;
                    if (type != null)
                    {
                        if (!(type == "ROIRectangle1"))
                        {
                            if (!(type == "ROIRectangle2"))
                            {
                                if (!(type == "ROICircle"))
                                {
                                    if (type == "ROILine")
                                    {
                                        if (modelData != null)
                                        {
                                            this._roiController.displayLine(current.Color, modelData[0].D, modelData[1].D, modelData[2].D, modelData[3].D);
                                        }
                                    }
                                }
                                else
                                {
                                    if (modelData != null)
                                    {
                                        this._roiController.displayCircle(current.Color, modelData[0].D, modelData[1].D, modelData[2].D);
                                    }
                                }
                            }
                            else
                            {
                                if (modelData != null)
                                {
                                    this._roiController.displayRect2(current.Color, modelData[0].D, modelData[1].D, modelData[2].D, modelData[3].D, modelData[4].D);
                                }
                            }
                        }
                        else
                        {
                            if (modelData != null)
                            {
                                this._roiController.displayRect1(current.Color, modelData[0].D, modelData[1].D, modelData[2].D, modelData[3].D);
                            }
                        }
                    }
                }
            }
        }
        public void removeActiveROI(ref List<ROI> rois)
        {
            this._roiController.removeActiveROI(ref rois);
        }
        public void setActiveRoi(int index)
        {
            this._roiController.activeROIidx = index;
        }
        public void saveROI(List<ROI> rois, string fileNmae)
        {
            List<RoiData> list = new List<RoiData>();
            for (int i = 0; i < rois.Count; i++)
            {
                list.Add(new RoiData(i, rois[i]));
            }
            SerializeHelper.Save(list, fileNmae);
        }
        public void loadROI(string fileName, out List<ROI> rois)
        {
            rois = new List<ROI>();
            List<RoiData> list = new List<RoiData>();
            list = (List<RoiData>)SerializeHelper.Load(list.GetType(), fileName);
            int i = 0;
            while (i < list.Count)
            {
                string name = list[i].Name;
                if (name != null)
                {
                    if (!(name == "Rectangle1"))
                    {
                        if (!(name == "Rectangle2"))
                        {
                            if (!(name == "Circle"))
                            {
                                if (name == "Line")
                                {
                                    this._roiController.genLine(list[i].Line.RowBegin, list[i].Line.ColumnBegin, list[i].Line.RowEnd, list[i].Line.ColumnEnd, ref rois);
                                    rois.Last<ROI>().Color = list[i].Line.Color;
                                }
                            }
                            else
                            {
                                this._roiController.genCircle(list[i].Circle.Row, list[i].Circle.Column, list[i].Circle.Radius, ref rois);
                                rois.Last<ROI>().Color = list[i].Circle.Color;
                            }
                        }
                        else
                        {
                            this._roiController.genRect2(list[i].Rectangle2.Row, list[i].Rectangle2.Column, list[i].Rectangle2.Phi, list[i].Rectangle2.Lenth1, list[i].Rectangle2.Lenth2, ref rois);
                            rois.Last<ROI>().Color = list[i].Rectangle2.Color;
                        }
                    }
                    else
                    {
                        this._roiController.genRect1(list[i].Rectangle1.Row1, list[i].Rectangle1.Column1, list[i].Rectangle1.Row2, list[i].Rectangle1.Column2, ref rois);
                        rois.Last<ROI>().Color = list[i].Rectangle1.Color;
                    }
                }
                IL_249:
                i++;
                continue;
                goto IL_249;
            }
            this._hWndControl.resetAll();
            this._hWndControl.repaint();
        }
        public void displayHobject(HObject obj, string color)
        {
            this._hWndControl.DispObj(obj, color);
        }
        public void displayHobject(HObject obj)
        {
            this._hWndControl.DispObj(obj, null);
        }
    }
}
