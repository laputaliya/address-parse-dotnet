using Newtonsoft.Json.Linq;
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
    public class AreaDataUtil
    {
        /// <summary>
        /// 加载数据没有包含街道
        /// </summary>
        /// <returns></returns>
        public static (List<Area> Province, List<Area> City, List<Area> Region, List<Area> Street) LoadDataNoStreet()
        {
            using var resource = System.Reflection.Assembly.GetAssembly(typeof(AreaDataUtil)).GetManifestResourceStream("AddressParse.Lib.area.json");// System.IO.File.ReadAllText("area.json");
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

            return (Province, City, Region, new List<Area>());
        }

        /// <summary>
        /// 加入街道的数据源信息
        /// </summary>
        /// <returns></returns>
        public static (List<Area> Province, List<Area> City, List<Area> Region, List<Area> Street) LoadDataWithStreet()
        {
            List<Area> Province = new List<Area>();
            List<Area> City = new List<Area>();
            List<Area> Region = new List<Area>();
            List<Area> Street = new List<Area>();
            using var resource = System.Reflection.Assembly.GetAssembly(typeof(AreaDataUtil)).GetManifestResourceStream("AddressParse.Lib.area2.json");// System.IO.File.ReadAllText("area.json");
            using var reader = new StreamReader(resource);
            var fileContent = reader.ReadToEnd();
        
            var areaObj = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(fileContent);

            List<List<Area>> alldata = new List<List<Area>>() { new List<Area>(), new List<Area>(), new List<Area>(), new List<Area>() };

            ParseJson(alldata, areaObj, "86", 0);


            return (alldata[0], alldata[1], alldata[2], alldata[3]);
        }

        static void ParseJson(List<List<Area>> alldata, JObject areaObj, string path, int level)
        {
            var top = areaObj.SelectToken(path);
            if (top != null)
            {
                var s = top.Children();
                List<Area> a = new List<Area>();
                foreach (var item in s)
                {
                    var si = (JProperty)item;
                    var name = si.Name;
                    var value = si.Value.ToString();
                    a.Add(new Area() { Name = value, Id = int.Parse(name),Pid = int.Parse(path) });
                    ParseJson(alldata, areaObj, name, level + 1);
                }
                alldata[level].AddRange(a);
            }
           
        }

    }

}
