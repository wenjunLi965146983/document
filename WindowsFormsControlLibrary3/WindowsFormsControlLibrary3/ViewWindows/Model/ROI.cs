using HalconDotNet;
using System;
namespace ViewWindows.Model
{
    [Serializable]
    public class ROI
    {
        public const int POSITIVE_FLAG = 21;
        public const int NEGATIVE_FLAG = 22;
        public const int ROI_TYPE_LINE = 10;
        public const int ROI_TYPE_CIRCLE = 11;
        public const int ROI_TYPE_CIRCLEARC = 12;
        public const int ROI_TYPE_RECTANCLE1 = 13;
        public const int ROI_TYPE_RECTANGLE2 = 14;
        private string color = "yellow";
        private string _type;
        protected int NumHandles;
        protected int activeHandleIdx;
        protected int OperatorFlag;
        public HTuple flagLineStyle;
        protected HTuple posOperation = new HTuple();
        protected HTuple negOperation = new HTuple(new int[]
        {
            2,
            2
        });
        public string Color
        {
            get
            {
                return this.color;
            }
            set
            {
                this.color = value;
            }
        }
        public string Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }
        public virtual void createRectangle1(double row1, double col1, double row2, double col2)
        {
        }
        public virtual void createRectangle2(double row, double col, double phi, double length1, double length2)
        {
        }
        public virtual void createCircle(double row, double col, double radius)
        {
        }
        public virtual void createLine(double beginRow, double beginCol, double endRow, double endCol)
        {
        }
        public virtual void createROI(double midX, double midY)
        {
        }
        public virtual void draw(HWindow window)
        {
        }
        public virtual double distToClosestHandle(double x, double y)
        {
            return 0.0;
        }
        public virtual void displayActive(HWindow window)
        {
        }
        public virtual void moveByHandle(double x, double y)
        {
        }
        public virtual HRegion getRegion()
        {
            return null;
        }
        public virtual double getDistanceFromStartPoint(double row, double col)
        {
            return 0.0;
        }
        public virtual HTuple getModelData()
        {
            return null;
        }
        public int getNumHandles()
        {
            return this.NumHandles;
        }
        public int getActHandleIdx()
        {
            return this.activeHandleIdx;
        }
        public int getOperatorFlag()
        {
            return this.OperatorFlag;
        }
        public void setOperatorFlag(int flag)
        {
            this.OperatorFlag = flag;
            switch (this.OperatorFlag)
            {
                case 21:
                    this.flagLineStyle = this.posOperation;
                    break;
                case 22:
                    this.flagLineStyle = this.negOperation;
                    break;
                default:
                    this.flagLineStyle = this.posOperation;
                    break;
            }
        }
    }
}
