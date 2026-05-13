using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Model.Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class TestController
    {
        [HttpGet]
        public IActionResult LocationStateInfo(string stockCode, string areaCode)
        {
            List<LocationStateInfo> locationStateInfoList = new List<LocationStateInfo>();
            LocationStateInfo locationStateInfoOne = new LocationStateInfo();
            locationStateInfoOne.stockCode = stockCode;
            locationStateInfoOne.areaCode = areaCode;
            locationStateInfoOne.structMode = "移动料架";
            locationStateInfoOne.fixLocationCode = "P1001";
            locationStateInfoOne.locationState = "正常";
            locationStateInfoOne.useState = "满";
            MoveLocationStateInfo moveLocationOne = new MoveLocationStateInfo();
            moveLocationOne.rackCode = "YDLJ001";
            moveLocationOne.face = "180";
            List<Container> containerListOne = new List<Container>();
            Container containerOneOne = new Container();
            containerOneOne.moveLocationCode = "YDLJ001-180-1-1";
            containerOneOne.trayCode = "LX001";
            containerOneOne.locationState = "正常";
            containerOneOne.useState = "满";
            containerListOne.Add(containerOneOne);
            Container containerOneTwo = new Container();
            containerOneTwo.moveLocationCode = "YDLJ001-180-1-2";
            containerOneTwo.trayCode = "LX002";
            containerOneTwo.locationState = "正常";
            containerOneTwo.useState = "空";
            containerListOne.Add(containerOneTwo);
            moveLocationOne.containers = containerListOne;
            locationStateInfoOne.moveLocations = moveLocationOne;
            locationStateInfoList.Add(locationStateInfoOne);
            LocationStateInfo locationStateInfoTwo = new LocationStateInfo();
            locationStateInfoTwo.stockCode = stockCode;
            locationStateInfoTwo.areaCode = areaCode;
            locationStateInfoTwo.structMode = "移动料架";
            locationStateInfoTwo.fixLocationCode = "P1002";
            locationStateInfoTwo.locationState = "正常";
            locationStateInfoTwo.useState = "满";
            MoveLocationStateInfo moveLocationTwo = new MoveLocationStateInfo();
            moveLocationTwo.rackCode = "YDLJ002";
            moveLocationTwo.face = "0";
            List<Container> containerListTwo = new List<Container>();
            Container containerTwoOne = new Container();
            containerTwoOne.moveLocationCode = "YDLJ002-0-1-1";
            containerTwoOne.trayCode = "LX003";
            containerTwoOne.locationState = "正常";
            containerTwoOne.useState = "满";
            containerListTwo.Add(containerTwoOne);
            Container containerTwoTwo = new Container();
            containerTwoTwo.moveLocationCode = "YDLJ002-0-1-2";
            containerTwoTwo.trayCode = "LX004";
            containerTwoTwo.locationState = "正常";
            containerTwoTwo.useState = "空";
            containerListTwo.Add(containerTwoTwo);
            moveLocationTwo.containers = containerListTwo;
            locationStateInfoTwo.moveLocations = moveLocationTwo;
            locationStateInfoList.Add(locationStateInfoTwo);
            return new JsonResult(locationStateInfoList);
        }

        [HttpGet]
        public IActionResult DeviceStateInfo()
        {
            string json = @"{  
    ""statusCode"": 200,  
    ""isSuccess"": true,  
    ""message"": """",  
    ""errCode"": """",  
    ""data"": {  
        ""line"": [  
            {""deviceCode"": ""1001"", ""deviceName"": """", ""errCode"": null, ""errMsg"": """", ""state"": null, ""signal"": false, ""taskNo"": null, ""trayCode"": null, ""trayUsed"": false},  
            {""deviceCode"": ""11001"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": true, ""taskNo"": ""0"", ""trayCode"": ""E00001"", ""trayUsed"": false},  
            {""deviceCode"": ""11002"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""11003"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": true, ""taskNo"": ""0"", ""trayCode"": ""E00002"", ""trayUsed"": false},  
            {""deviceCode"": ""11004"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""11005"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""11006"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""11007"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""11008"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""11009"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""11010"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""11011"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""12001"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""12002"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""12003"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""12004"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""12005"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""12006"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""12007"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""12008"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""12009"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""12010"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""12011"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""12012"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""12013"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""1"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""0"", ""deviceName"": """", ""errCode"": ""0"", ""errMsg"": """", ""state"": ""0"", ""signal"": false, ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false},  
            {""deviceCode"": ""1001"", ""deviceName"": """", ""errCode"": null, ""errMsg"": """", ""state"": null, ""signal"": false, ""taskNo"": null, ""trayCode"": null, ""trayUsed"": false}, 
        ],  
        ""hoister"": [  
            {""deviceCode"": ""Hoister1"", ""deviceName"": ""提升机1"", ""errCode"": ""0"", ""errMsg"": null, ""state"": ""1"", ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false, ""floor"": ""1"", ""runHeight"": -45},  
            {""deviceCode"": ""Hoister2"", ""deviceName"": ""提升机2"", ""errCode"": ""0"", ""errMsg"": null, ""state"": ""1"", ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false, ""floor"": ""2"", ""runHeight"": 557},  
            {""deviceCode"": ""Hoister3"", ""deviceName"": ""提升机3"", ""errCode"": ""0"", ""errMsg"": null, ""state"": ""0"", ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false, ""floor"": ""0"", ""runHeight"": 0},  
            {""deviceCode"": ""Hoister4"", ""deviceName"": ""提升机4"", ""errCode"": ""0"", ""errMsg"": null, ""state"": ""0"", ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false, ""floor"": ""0"", ""runHeight"": 0},  
            {""deviceCode"": ""Hoister5"", ""deviceName"": ""提升机5"", ""errCode"": ""0"", ""errMsg"": null, ""state"": ""1"", ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false, ""floor"": ""2"", ""runHeight"": 292},
            {""deviceCode"": ""Hoister6"", ""deviceName"": ""提升机6"", ""errCode"": ""0"", ""errMsg"": null, ""state"": ""1"", ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false, ""floor"": ""4"", ""runHeight"": 3367},  
            {""deviceCode"": ""Hoister7"", ""deviceName"": ""提升机7"", ""errCode"": ""0"", ""errMsg"": null, ""state"": ""1"", ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false, ""floor"": ""1"", ""runHeight"": -508},  
            {""deviceCode"": ""Hoister8"", ""deviceName"": ""提升机8"", ""errCode"": ""0"", ""errMsg"": null, ""state"": ""2"", ""taskNo"": ""0"", ""trayCode"": """", ""trayUsed"": false, ""floor"": ""612"", ""runHeight"": 612}  
        ],  
        ""stacker"": [  
            {""deviceCode"": ""Stacker1"", ""deviceName"": ""堆垛机1"", ""errCode"": ""0"", ""errMsg"": null, ""state"": null, ""taskNo"": ""0"", ""toStation"": null, ""trayCode"": """", ""trayUsed"": false, ""posX"": 1983, ""posY"": 947, ""posZ"": 0},  
            {""deviceCode"": ""Stacker2"", ""deviceName"": ""堆垛机2"", ""errCode"": ""0"", ""errMsg"": null, ""state"": ""1"", ""taskNo"": null, ""toStation"": null, ""trayCode"": """", ""trayUsed"": false, ""posX"": 1323, ""posY"": 472, ""posZ"": 0},  
            {""deviceCode"": ""Elevator1"", ""deviceName"": ""电梯1"", ""errCode"": null, ""errMsg"": null, ""state"": null, ""taskNo"": null, ""toStation"": null, ""trayCode"": null, ""trayUsed"": false, ""posX"": 0, ""posY"": 0, ""posZ"": 0},  
            {""deviceCode"": ""SafetyDoor1"", ""deviceName"": ""安全门1"", ""errCode"": null, ""errMsg"": null, ""state"": null, ""taskNo"": null, ""toStation"": null, ""trayCode"": null, ""trayUsed"": false, ""posX"": 0, ""posY"": 0, ""posZ"": 0}  
        ]  
    },  
    ""timestamp"": ""1720749579""  
}";
            var obj = JsonConvert.DeserializeObject(json);
            return new JsonResult(obj);
            //List<Line> lineList = new List<Line>();
            //Line lineOne = new Line
            //{
            //    deviceCode = "21001",
            //    deviceName = "线体21001",
            //    errCode = "002",
            //    errMsg = "异常",
            //    state = "故障",
            //    signal = true,
            //    taskNo = "task001",
            //    trayCode = "LX001",
            //    trayUsed = "满"
            //};
            //Line lineTwo = new Line
            //{
            //    deviceCode = "21002",
            //    deviceName = "线体21002",
            //    errCode = "002",
            //    errMsg = "异常",
            //    state = "故障",
            //    signal = true,
            //    taskNo = "task002",
            //    trayCode = "LX002",
            //    trayUsed = "满"
            //};
            //lineList.Add(lineOne);
            //lineList.Add(lineTwo);
            //deviceStateInfo.line = lineList;
            //List<Hoister> hoisterList = new List<Hoister>();
            //Hoister hoisterOne = new Hoister
            //{
            //    deviceCode = "23001",
            //    deviceName = "一号提升机",
            //    errCode = "002",
            //    errMsg = "异常",
            //    state = "故障",
            //    taskNo = "task003",
            //    trayCode = "LX003",
            //    trayUsed = "满",
            //    floor = "1",
            //    runHeight = 1000
            //};
            //Hoister hoisterTwo = new Hoister
            //{
            //    deviceCode = "23002",
            //    deviceName = "二号提升机",
            //    errCode = "002",
            //    errMsg = "异常",
            //    state = "故障",
            //    taskNo = "task004",
            //    trayCode = "LX004",
            //    trayUsed = "满",
            //    floor = "2",
            //    runHeight = 20
            //};
            //hoisterList.Add(hoisterOne);
            //hoisterList.Add(hoisterTwo);
            //deviceStateInfo.hoister = hoisterList;
            //List<Stacker> stackerList = new List<Stacker>();
            //Stacker stackerOne = new Stacker
            //{
            //    deviceCode = "24001",
            //    deviceName = "一号堆垛车",
            //    errCode = "002",
            //    errMsg = "异常",
            //    state = "故障",
            //    taskNo = "task005",
            //    toStation = "21001",
            //    trayCode = "LX005",
            //    trayUsed = "满",
            //    posX = 1000,
            //    posY = 2000,
            //    posZ = 3000
            //};
            //Stacker stackerTwo = new Stacker
            //{
            //    deviceCode = "24002",
            //    deviceName = "二号堆垛车",
            //    errCode = "002",
            //    errMsg = "异常",
            //    state = "故障",
            //    taskNo = "task006",
            //    toStation = "21002",
            //    trayCode = "LX006",
            //    trayUsed = "满",
            //    posX = 1000,
            //    posY = 2000,
            //    posZ = 3000
            //};
            //stackerList.Add(stackerOne);
            //stackerList.Add(stackerTwo);
            //deviceStateInfo.stacker = stackerList;
            //return new JsonResult(deviceStateInfo);
        }

        [HttpGet]
        public IActionResult TrayItemInfo(string locationCode, string trayCode)
        {
            List<TrayItemInfo> trayItemInfoList = new List<TrayItemInfo>();
            TrayItemInfo trayItemInfo = new TrayItemInfo();
            trayItemInfo.trayCode = trayCode;
            #region 料箱
            //trayItemInfo.trayClass = "料箱";
            //List<Item> itemList = new List<Item>();
            //Item itemOne = new Item()
            //{
            //    itemCode = "WL001",
            //    itemName = "物料1",
            //    itemModel = "规格",
            //    qty = 10,
            //    unit = "个",
            //    lotCode = "批次一"
            //};
            //itemList.Add(itemOne);
            //Item itemTwo = new Item()
            //{
            //    itemCode = "WL002",
            //    itemName = "物料2",
            //    itemModel = "规格",
            //    qty = 10,
            //    unit = "个",
            //    lotCode = "批次一"
            //};
            //itemList.Add(itemTwo);
            //trayItemInfo.items = itemList;
            #endregion
            #region 料架
            trayItemInfo.trayClass = "移动料架";
            List<MoveLocationTrayItem> moveLocationList = new List<MoveLocationTrayItem>();
            MoveLocationTrayItem moveLocationTrayOne = new MoveLocationTrayItem();
            moveLocationTrayOne.trayCode = "LX001";
            moveLocationTrayOne.trayClass = "料箱";
            List<Item> itemListOne = new List<Item>();
            Item itemOneOne = new Item
            {
                itemCode = "WL001",
                itemName = "物料1",
                itemModel = "规格",
                qty = 10,
                unit = "个",
                lotCode = "批次一"
            };
            itemListOne.Add(itemOneOne);
            Item itemOneTwo = new Item
            {
                itemCode = "WL002",
                itemName = "物料2",
                itemModel = "规格",
                qty = 10,
                unit = "个",
                lotCode = "批次一"
            };
            itemListOne.Add(itemOneTwo);
            moveLocationTrayOne.items = itemListOne;
            moveLocationList.Add(moveLocationTrayOne);
            MoveLocationTrayItem moveLocationTrayTwo = new MoveLocationTrayItem();
            moveLocationTrayTwo.trayCode = "LX002";
            moveLocationTrayTwo.trayClass = "料箱";
            List<Item> itemListTwo = new List<Item>();
            Item itemTwoOne = new Item
            {
                itemCode = "WL001",
                itemName = "物料1",
                itemModel = "规格",
                qty = 10,
                unit = "个",
                lotCode = "批次一"
            };
            itemListTwo.Add(itemTwoOne);
            Item itemTwoTwo = new Item
            {
                itemCode = "WL002",
                itemName = "物料2",
                itemModel = "规格",
                qty = 10,
                unit = "个",
                lotCode = "批次一"
            };
            itemListTwo.Add(itemTwoTwo);
            moveLocationTrayTwo.items = itemListTwo;
            moveLocationList.Add(moveLocationTrayTwo);
            trayItemInfo.moveLocations = moveLocationList;
            #endregion
            trayItemInfoList.Add(trayItemInfo);
            return new JsonResult(trayItemInfoList);
        }
    }
}
