using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public static class Constants
    {

        public static int BoardSize = 250;
        public static double M_SQRT2 = 0.41421356237309504880;
        public static int Obsticle_Size = 20;
        public const string PortName = "COM3";
        public const int PortBaudRate = 115200;
        public static double CenterRange = 20;
        public static double YoloSize = 304;
        public static int Delay = 525; // seconds
        public static int Blocks = 1;
        #region Yolo constants
        //YOLOv3
        //https://github.com/pjreddie/darknet/blob/master/cfg/yolov3.cfg
        public const string Cfg = "tiny.cfg";

        //https://pjreddie.com/media/files/yolov3.weights
        public const string Weight = "tiny_last.weights";

        //https://github.com/pjreddie/darknet/blob/master/data/coco.names
        public const string Names = "obj.names";

        //file location
        public const string Location = "./cfg/";
        #endregion
    }
}
