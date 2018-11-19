using HalconDotNet;
using System;
using System.Xml.Serialization;
namespace ViewWindows.Model
{
    [Serializable]
    public class ROIRectangle1 : ROI
    {
        private string color = "yellow";
        private double row1;
        private double col1;
        private double row2;
        private double col2;
        private double midR;
        private double midC;
        [XmlElement(ElementName = "Row1")]
        public double Row1
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
        [XmlElement(ElementName = "Column1")]
        public double Column1
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
        [XmlElement(ElementName = "Row2")]
        public double Row2
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
        [XmlElement(ElementName = "Column2")]
        public double Column2
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
        public ROIRectangle1()
        {
            this.NumHandles = 5;
            this.activeHandleIdx = 4;
        }
        public ROIRectangle1(double row1, double col1, double row2, double col2)
        {
            this.createRectangle1(row1, col1, row2, col2);
        }
        public override void createRectangle1(double row1, double col1, double row2, double col2)
        {
            base.createRectangle1(row1, col1, row2, col2);
            this.row1 = row1;
            this.col1 = col1;
            this.row2 = row2;
            this.col2 = col2;
            this.midR = (this.row1 + this.row2) / 2.0;
            this.midC = (this.col1 + this.col2) / 2.0;
        }
        public override void createROI(double midX, double midY)
        {
            this.midR = midY;
            this.midC = midX;
            this.row1 = this.midR - 25.0;
            this.col1 = this.midC - 25.0;
            this.row2 = this.midR + 25.0;
            this.col2 = this.midC + 25.0;
        }
        public override void draw(HWindow window)
        {
            window.DispRectangle1(this.row1, this.col1, this.row2, this.col2);
            window.DispRectangle2(this.row1, this.col1, 0.0, 25.0, 25.0);
            window.DispRectangle2(this.row1, this.col2, 0.0, 25.0, 25.0);
            window.DispRectangle2(this.row2, this.col2, 0.0, 25.0, 25.0);
            window.DispRectangle2(this.row2, this.col1, 0.0, 25.0, 25.0);
            window.DispRectangle2(this.midR, this.midC, 0.0, 25.0, 25.0);
        }
        public override double distToClosestHandle(double x, double y)
        {
            double num = 10000.0;
            double[] array = new double[this.NumHandles];
            this.midR = (this.row2 - this.row1) / 2.0 + this.row1;
            this.midC = (this.col2 - this.col1) / 2.0 + this.col1;
            array[0] = HMisc.DistancePp(y, x, this.row1, this.col1);
            array[1] = HMisc.DistancePp(y, x, this.row1, this.col2);
            array[2] = HMisc.DistancePp(y, x, this.row2, this.col2);
            array[3] = HMisc.DistancePp(y, x, this.row2, this.col1);
            array[4] = HMisc.DistancePp(y, x, this.midR, this.midC);
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
                    window.DispRectangle2(this.row1, this.col2, 0.0, 25.0, 25.0);
                    break;
                case 2:
                    window.DispRectangle2(this.row2, this.col2, 0.0, 25.0, 25.0);
                    break;
                case 3:
                    window.DispRectangle2(this.row2, this.col1, 0.0, 25.0, 25.0);
                    break;
                case 4:
                    window.DispRectangle2(this.midR, this.midC, 0.0, 25.0, 25.0);
                    break;
            }
        }
        public override HRegion getRegion()
        {
            HRegion hRegion = new HRegion();
            hRegion.GenRectangle1(this.row1, this.col1, this.row2, this.col2);
            return hRegion;
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
                    break;
                case 1:
                    this.row1 = newY;
                    this.col2 = newX;
                    break;
                case 2:
                    this.row2 = newY;
                    this.col2 = newX;
                    break;
                case 3:
                    this.row2 = newY;
                    this.col1 = newX;
                    break;
                case 4:
                    {
                        double num = (this.row2 - this.row1) / 2.0;
                        double num2 = (this.col2 - this.col1) / 2.0;
                        this.row1 = newY - num;
                        this.row2 = newY + num;
                        this.col1 = newX - num2;
                        this.col2 = newX + num2;
                        break;
                    }
            }
            if (this.row2 <= this.row1)
            {
                double num3 = this.row1;
                this.row1 = this.row2;
                this.row2 = num3;
            }
            if (this.col2 <= this.col1)
            {
                double num3 = this.col1;
                this.col1 = this.col2;
                this.col2 = num3;
            }
            this.midR = (this.row2 - this.row1) / 2.0 + this.row1;
            this.midC = (this.col2 - this.col1) / 2.0 + this.col1;
        }
    }
}
