using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressParse.Lib
{

    /// <summary>
    /// 地区源地址处理工具
    /// </summary>
    internal class AreaDataUtil
    {
        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns></returns>
        public static (List<Area> Province, List<Area> City, List<Area> Region) LoadData()
        {
            using var resource = System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream("AddressParse.Lib.area.json");// System.IO.File.ReadAllText("area.json");
            using var reader = new StreamReader(resource);
            var fileContent = reader.ReadToEnd();
            var areaObj = Newtonsoft.Json.JsonConvert.DeserializeObject<FullAreaData>(fileContent);
            List<Area> Province = new List<Area>();
            List<Area> City = new List<Area>();
            List<Area> Region = new List<Area>();

            var chiled = areaObj.children;
            foreach (var item in chiled)
            {
                Province.Add(new Area() { Id = item.id, Pid = item.parent_id, Name = item.name, Zip = item.zip });
                item.children.ForEach(c =>
                {
                    City.Add(new Area() { Id = c.id, Pid = c.parent_id, Name = c.name, Zip = c.zip });

                    c.children.ForEach(d =>
                    {
                        Region.Add(new Area() { Id = d.id, Pid = d.parent_id, Name = d.name, Zip = d.zip });
                    });
                });
            }

            return (Province, City, Region);
        }

    }

}
