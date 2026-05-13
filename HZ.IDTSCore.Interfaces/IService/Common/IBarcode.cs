using HZ.IDTSCore.Model;
using HZ.IDTSCore.Model.Entity.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Common
{
    public interface IBarcode
    {
        public BarcodeItem GetItem(string key, string type);
    }
}
