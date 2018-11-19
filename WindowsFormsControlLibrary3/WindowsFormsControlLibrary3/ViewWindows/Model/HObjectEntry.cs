using HalconDotNet;
using System;
using System.Collections;
namespace ViewWindows.Model
{
    public class HObjectEntry
    {
        public Hashtable gContext;
        public HObject HObj;
        public HObjectEntry(HObject obj, Hashtable gc)
        {
            this.gContext = gc;
            this.HObj = obj;
        }
        public void clear()
        {
            this.gContext.Clear();
            this.HObj.Dispose();
        }
    }
}
