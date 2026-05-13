using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HZ.IDTS.SimulateService
{
    public  class CarModel
    {

        public int id { set; get; }
        ////carName
        public string n { set; get; } 
        public float x { set; get; }
        public float y { set; get; }
        //角度
        public float th { set; get; }
        //锁定节点
        public List<int> l { set; get; }
        //车辆状态
        public short s { get; set; }  //statusWold 
        public string t { get; set; } //carType  1.三指点 2差速轮
        public int b { set; get; } = 0; //blockCar
        public int h { set; get; } = 0;//listPos
      //速度
        public float v { set; get; }

        /// <summary>
        /// 电池电量
        /// </summary>
        public int soc { get; set; }
    }
}
