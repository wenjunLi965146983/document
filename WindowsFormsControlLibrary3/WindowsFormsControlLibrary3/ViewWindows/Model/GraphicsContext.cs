using HalconDotNet;
using System;
using System.Collections;
namespace ViewWindows.Model
{
    
    public class GraphicsContext
    {
        public delegate void GCDelegate(String s);
        public const string GC_COLOR = "Color";
        public const string GC_COLORED = "Colored";
        public const string GC_LINEWIDTH = "LineWidth";
        public const string GC_DRAWMODE = "DrawMode";
        public const string GC_SHAPE = "Shape";
        public const string GC_LUT = "Lut";
        public const string GC_PAINT = "Paint";
        public const string GC_LINESTYLE = "LineStyle";
        private Hashtable graphicalSettings;
        public Hashtable stateOfSettings;
        private IEnumerator iterator;
        public GCDelegate gcNotification;
        public GraphicsContext()
        {
            this.graphicalSettings = new Hashtable(10, 0.2f);
            this.gcNotification = new GCDelegate(this.dummy);
            this.stateOfSettings = new Hashtable(10, 0.2f);
        }
        public GraphicsContext(Hashtable settings)
        {
            this.graphicalSettings = settings;
            this.gcNotification = new GCDelegate(this.dummy);
            this.stateOfSettings = new Hashtable(10, 0.2f);
        }
        public void applyContext(HWindow window, Hashtable cContext)
        {
            string text = "";
            int num = -1;
            HTuple hTuple = null;
            this.iterator = cContext.Keys.GetEnumerator();
            try
            {
                while (this.iterator.MoveNext())
                {
                    string text2 = (string)this.iterator.Current;
                    if (!this.stateOfSettings.Contains(text2) || this.stateOfSettings[text2] != cContext[text2])
                    {
                        string text3 = text2;
                        switch (text3)
                        {
                            case "Color":
                                text = (string)cContext[text2];
                                window.SetColor(text);
                                if (this.stateOfSettings.Contains("Colored"))
                                {
                                    this.stateOfSettings.Remove("Colored");
                                }
                                break;
                            case "Colored":
                                num = (int)cContext[text2];
                                window.SetColored(num);
                                if (this.stateOfSettings.Contains("Color"))
                                {
                                    this.stateOfSettings.Remove("Color");
                                }
                                break;
                            case "DrawMode":
                                text = (string)cContext[text2];
                                window.SetDraw(text);
                                break;
                            case "LineWidth":
                                num = (int)cContext[text2];
                                window.SetLineWidth(num);
                                break;
                            case "Lut":
                                text = (string)cContext[text2];
                                window.SetLut(text);
                                break;
                            case "Paint":
                                text = (string)cContext[text2];
                                window.SetPaint(text);
                                break;
                            case "Shape":
                                text = (string)cContext[text2];
                                window.SetShape(text);
                                break;
                            case "LineStyle":
                                hTuple = (HTuple)cContext[text2];
                                window.SetLineStyle(hTuple);
                                break;
                        }
                        IL_249:
                        if (num != -1)
                        {
                            if (this.stateOfSettings.Contains(text2))
                            {
                                this.stateOfSettings[text2] = num;
                            }
                            else
                            {
                                this.stateOfSettings.Add(text2, num);
                            }
                            num = -1;
                        }
                        else
                        {
                            if (text != "")
                            {
                                if (this.stateOfSettings.Contains(text2))
                                {
                                    this.stateOfSettings[text2] = num;
                                }
                                else
                                {
                                    this.stateOfSettings.Add(text2, num);
                                }
                                text = "";
                            }
                            else
                            {
                                if (hTuple != null)
                                {
                                    if (this.stateOfSettings.Contains(text2))
                                    {
                                        this.stateOfSettings[text2] = num;
                                    }
                                    else
                                    {
                                        this.stateOfSettings.Add(text2, num);
                                    }
                                    hTuple = null;
                                }
                            }
                        }
                        continue;
                        goto IL_249;
                    }
                }
            }
            catch (HOperatorException ex)
            {
                this.gcNotification(ex.Message);
            }
        }
        public void setColorAttribute(string val)
        {
            if (this.graphicalSettings.ContainsKey("Colored"))
            {
                this.graphicalSettings.Remove("Colored");
            }
            this.addValue("Color", val);
        }
        public void setColoredAttribute(int val)
        {
            if (this.graphicalSettings.ContainsKey("Color"))
            {
                this.graphicalSettings.Remove("Color");
            }
            this.addValue("Colored", val);
        }
        public void setDrawModeAttribute(string val)
        {
            this.addValue("DrawMode", val);
        }
        public void setLineWidthAttribute(int val)
        {
            this.addValue("LineWidth", val);
        }
        public void setLutAttribute(string val)
        {
            this.addValue("Lut", val);
        }
        public void setPaintAttribute(string val)
        {
            this.addValue("Paint", val);
        }
        public void setShapeAttribute(string val)
        {
            this.addValue("Shape", val);
        }
        public void setLineStyleAttribute(HTuple val)
        {
            this.addValue("LineStyle", val);
        }
        private void addValue(string key, int val)
        {
            if (this.graphicalSettings.ContainsKey(key))
            {
                this.graphicalSettings[key] = val;
            }
            else
            {
                this.graphicalSettings.Add(key, val);
            }
        }
        private void addValue(string key, string val)
        {
            if (this.graphicalSettings.ContainsKey(key))
            {
                this.graphicalSettings[key] = val;
            }
            else
            {
                this.graphicalSettings.Add(key, val);
            }
        }
        private void addValue(string key, HTuple val)
        {
            if (this.graphicalSettings.ContainsKey(key))
            {
                this.graphicalSettings[key] = val;
            }
            else
            {
                this.graphicalSettings.Add(key, val);
            }
        }
        public void clear()
        {
            this.graphicalSettings.Clear();
        }
        public GraphicsContext copy()
        {
            return new GraphicsContext((Hashtable)this.graphicalSettings.Clone());
        }
        public object getGraphicsAttribute(string key)
        {
            object result;
            if (this.graphicalSettings.ContainsKey(key))
            {
                result = this.graphicalSettings[key];
            }
            else
            {
                result = null;
            }
            return result;
        }
        public Hashtable copyContextList()
        {
            return (Hashtable)this.graphicalSettings.Clone();
        }
        public void dummy(string val)
        {
        }
    }
}
