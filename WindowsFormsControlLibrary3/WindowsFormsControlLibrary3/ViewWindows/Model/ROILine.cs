using HalconDotNet;
using System;
using System.Xml.Serialization;
namespace ViewWindows.Model
{
    [Serializable]
    public class ROILine : ROI
    {
        private double row1;
        private double col1;
        private double row2;
        private double col2;
        private double midR;
        private double midC;
        private HObject arrowHandleXLD;
        [XmlElement(ElementName = "RowBegin")]
        public double RowBegin
        {
            get
            {
                return this.row1;
            }
            set
            {
                this.row1 = value;
            }
        }
        [XmlElement(ElementName = "ColumnBegin")]
        public double ColumnBegin
        {
            get
            {
                return this.col1;
            }
            set
            {
                this.col1 = value;
            }
        }
        [XmlElement(ElementName = "RowEnd")]
        public double RowEnd
        {
            get
            {
                return this.row2;
            }
            set
            {
                this.row2 = value;
            }
        }
        [XmlElement(ElementName = "ColumnEnd")]
        public double ColumnEnd
        {
            get
            {
                return this.col2;
            }
            set
            {
                this.col2 = value;
            }
        }
        public ROILine()
        {
            this.NumHandles = 3;
            this.activeHandleIdx = 2;
            this.arrowHandleXLD = new HXLDCont();
            this.arrowHandleXLD.GenEmptyObj();
        }
        public ROILine(double beginRow, double beginCol, double endRow, double endCol)
        {
            this.createLine(beginRow, beginCol, endRow, endCol);
        }
        public override void createLine(double beginRow, double beginCol, double endRow, double endCol)
        {
            base.createLine(beginRow, beginCol, endRow, endCol);
            this.row1 = beginRow;
            this.col1 = beginCol;
            this.row2 = endRow;
            this.col2 = endCol;
            this.midR = (this.row1 + this.row2) / 2.0;
            this.midC = (this.col1 + this.col2) / 2.0;
            this.updateArrowHandle();
        }
        public override void createROI(double midX, double midY)
        {
            this.midR = midY;
            this.midC = midX;
            this.row1 = this.midR;
            this.col1 = this.midC - 50.0;
            this.row2 = this.midR;
            this.col2 = this.midC + 50.0;
            this.updateArrowHandle();
        }
        public override void draw(HWindow window)
        {
            window.DispLine(this.row1, this.col1, this.row2, this.col2);
            window.DispRectangle2(this.row1, this.col1, 0.0, 25.0, 25.0);
            window.DispObj(this.arrowHandleXLD);
            window.DispRectangle2(this.midR, this.midC, 0.0, 25.0, 25.0);
        }
        public override double distToClosestHandle(double x, double y)
        {
            double num = 10000.0;
            double[] array = new double[this.NumHandles];
            array[0] = HMisc.DistancePp(y, x, this.row1, this.col1);
            array[1] = HMisc.DistancePp(y, x, this.row2, this.col2);
            array[2] = HMisc.DistancePp(y, x, this.midR, this.midC);
            for (int i = 0; i < this.NumHandles; i++)
            {
                if (array[i] < num)
                {
                    num = array[i];
                    this.activeHandleIdx = i;
                }
            }
            return array[this.activeHandleIdx];
        }
        public override void displayActive(HWindow window)
        {
            switch (this.activeHandleIdx)
            {
                case 0:
                    window.DispRectangle2(this.row1, this.col1, 0.0, 25.0, 25.0);
                    break;
                case 1:
                    window.DispObj(this.arrowHandleXLD);
                    break;
                case 2:
                    window.DispRectangle2(this.midR, this.midC, 0.0, 25.0, 25.0);
                    break;
            }
        }
        public override HRegion getRegion()
        {
            HRegion hRegion = new HRegion();
            hRegion.GenRegionLine(this.row1, this.col1, this.row2, this.col2);
            return hRegion;
        }
        public override double getDistanceFromStartPoint(double row, double col)
        {
            return HMisc.DistancePp(row, col, this.row1, this.col1);
        }
        public override HTuple getModelData()
        {
            return new HTuple(new double[]
            {
                this.row1,
                this.col1,
                this.row2,
                this.col2
            });
        }
        public override void moveByHandle(double newX, double newY)
        {
            switch (this.activeHandleIdx)
            {
                case 0:
                    this.row1 = newY;
                    this.col1 = newX;
                    this.midR = (this.row1 + this.row2) / 2.0;
                    this.midC = (this.col1 + this.col2) / 2.0;
                    break;
                case 1:
                    this.row2 = newY;
                    this.col2 = newX;
                    this.midR = (this.row1 + this.row2) / 2.0;
                    this.midC = (this.col1 + this.col2) / 2.0;
                    break;
                case 2:
                    {
                        double num = this.row1 - this.midR;
                        double num2 = this.col1 - this.midC;
                        this.midR = newY;
                        this.midC = newX;
                        this.row1 = this.midR + num;
                        this.col1 = this.midC + num2;
                        this.row2 = this.midR - num;
                        this.col2 = this.midC - num2;
                        break;
                    }
            }
            this.updateArrowHandle();
        }
        private void updateArrowHandle()
        {
            double num = 35.0;
            double num2 = 35.0;
            this.arrowHandleXLD.Dispose();
            this.arrowHandleXLD.GenEmptyObj();
            double num3 = this.row1 + (this.row2 - this.row1) * 0.9;
            double num4 = this.col1 + (this.col2 - this.col1) * 0.9;
            double num5 = HMisc.DistancePp(num3, num4, this.row2, this.col2);
            if (num5 == 0.0)
            {
                num5 = -1.0;
            }
            double num6 = (this.row2 - num3) / num5;
            double num7 = (this.col2 - num4) / num5;
            double num8 = num2 / 2.0;
            double num9 = num3 + (num5 - num) * num6 + num8 * num7;
            double num10 = num3 + (num5 - num) * num6 - num8 * num7;
            double num11 = num4 + (num5 - num) * num7 - num8 * num6;
            double num12 = num4 + (num5 - num) * num7 + num8 * num6;
            if (num5 == -1.0)
            {
                HOperatorSet.GenContourPolygonXld(out this.arrowHandleXLD, num3, num4);
            }
            else
            {
                HOperatorSet.GenContourPolygonXld(out  this.arrowHandleXLD, new HTuple(new double[]
                {
                    num3,
                    this.row2,
                    num9,
                    this.row2,
                    num10,
                    this.row2
                }), new HTuple(new double[]
                {
                    num4,
                    this.col2,
                    num11,
                    this.col2,
                    num12,
                    this.col2
                }));
            }
        }
    }
}
