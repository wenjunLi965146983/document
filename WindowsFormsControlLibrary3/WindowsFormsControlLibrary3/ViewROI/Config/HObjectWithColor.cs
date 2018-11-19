using HalconDotNet;
using System;
namespace ViewROI.Config
{
    internal class HObjectWithColor
    {
        private HObject hObject;
        private string color;
        public HObject HObject
        {
            get
            {
                return this.hObject;
            }
            set
            {
                this.hObject = value;
            }
        }
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
        public HObjectWithColor(HObject _hbj, string _color)
        {
            this.hObject = _hbj;
            this.color = _color;
        }
    }
}
