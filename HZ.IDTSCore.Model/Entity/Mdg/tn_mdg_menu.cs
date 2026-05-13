using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Basic
{
	public class MenuItem
	{
		public string name { get; set; }
		public string url { get; set; }
	}

	public class tn_mdg_menu
	{
		public List<MenuItem> UserMenuList = new List<MenuItem>();
	}
}
