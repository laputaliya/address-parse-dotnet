using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressParse.Lib
{
    /// <summary>
    /// 地址解析
    /// </summary>
    public class AddressParser: IParser<AddressInfo>
    {
        /// <summary>
        /// 省份信息数据源
        /// </summary>
        readonly List<Area> ProvinceDataSource;
        /// <summary>
        /// 市信息数据源
        /// </summary>
        readonly List<Area> CityDataSource;
        /// <summary>
        /// 区信息数据源
        /// </summary>
        readonly List<Area> RegionDataSource;
        /// <summary>
        /// 区信息数据源
        /// </summary>
        readonly List<Area> StreetDataSource;

        public AddressParser()
        {
			var d = AreaDataUtil.LoadDataWithStreet();
			ProvinceDataSource = d.Province;
			CityDataSource = d.City;
			RegionDataSource = d.Region;
            StreetDataSource = d.Street;

		}
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public AddressInfo Parse(string data)
        {
			var addr = StripAddressInfo(data);
			var ssq = StripAddressDetail(addr.FullAddress);
            var finalPCR = MatchAddressDetail(ssq.Province, ssq.City, ssq.Region, ssq.Street);
			addr.Province = finalPCR.Province;
			addr.City = finalPCR.City;
			addr.Region = finalPCR.Region;
			addr.Street = finalPCR.Street;
            addr.StreetId = finalPCR.StreetId;
			addr.CityId = finalPCR.CityId;
			addr.ProvinceId = finalPCR.ProvinceId;
			addr.RegionId = finalPCR.RegionId;
            addr.ShortAddress = addr.FullAddress;
            if (!String.IsNullOrEmpty(addr.Street))
            {
                addr.ShortAddress = addr.ShortAddress.Replace(addr.Street,"");
                if (!string.IsNullOrWhiteSpace(ssq.Street))
                {
                    addr.ShortAddress = addr.ShortAddress.Replace(ssq.Street, "");
                }
            }
            if (!String.IsNullOrEmpty(addr.Region))
            {
                addr.ShortAddress = addr.ShortAddress.Replace(addr.Region, "");
                if (!string.IsNullOrWhiteSpace(ssq.Region))
                {
                    addr.ShortAddress = addr.ShortAddress.Replace(ssq.Region, "");
                }
            }
            if (!String.IsNullOrEmpty(addr.City))
            {
                addr.ShortAddress = addr.ShortAddress.Replace(addr.City, "");
                if (!string.IsNullOrWhiteSpace(ssq.City))
                {
                    addr.ShortAddress = addr.ShortAddress.Replace(ssq.City, "");
                }
            }
            if (!String.IsNullOrEmpty(addr.Province))
            {
                addr.ShortAddress = addr.ShortAddress.Replace(addr.Province, "");
                if (!string.IsNullOrWhiteSpace(ssq.Province))
                {
                    addr.ShortAddress = addr.ShortAddress.Replace(ssq.Province, "");
                }
            }
            return addr;
        }

		/// <summary>
		/// 剥离地址信息，分离成粗略的地址及姓名等信息
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
        private AddressInfo StripAddressInfo(string data)
        {
            var a = new AddressInfo();
            List<string> search = new List<string>
            {
                "收货地址",
                "详细地址",
                "地址",
                "收货人",
                "收件人",
                "收货",
                "所在地区",
                "邮编",
                "联系电话",
                "联系人",
                "电话",
                "手机号码",
                "身份证号码",
                "身份证号",
                "身份证",
                "：",
                ":",
                "；",
                ";",
                "，",
                ",",
                "。",
                "."
            };

            foreach (var item in search)
            {
                data = data.Replace(item, " ");
            }

            data = System.Text.RegularExpressions.Regex.Replace(data, "\\s{1,}", " ");

            var matchid = System.Text.RegularExpressions.Regex.Matches(data, "\\d{18}|\\d{17}X");
            if (matchid.Count > 0)
            {
                a.IdentityNumer = matchid[0].Value;
                data = data.Replace(a.IdentityNumer, " ");
            }
            var matchmobile = System.Text.RegularExpressions.Regex.Matches(data, "\\d{7,11}[-_]\\d{2,6}|\\d{7,11}|\\d{3,4}-\\d{6,8}|\\d{2,4}-\\d{6,11}|\\d{3}\\s\\d{4}\\s\\d{4}");
            if (matchmobile.Count > 0)
            {
                a.Telphone = matchmobile[0].Value;
                data = data.Replace(a.Telphone, " ");
				a.Telphone = a.Telphone.Replace(" ", "");
			}

            var matchpostcode = System.Text.RegularExpressions.Regex.Matches(data, "\\d{6}");
            if (matchpostcode.Count > 0)
            {
                a.Zipcode = matchpostcode[0].Value;
                data = data.Replace(a.Zipcode, " ");
            }
            data = System.Text.RegularExpressions.Regex.Replace(data, " {2,}", " ").Trim();

            var darr = data.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (darr.Count > 1)
            {
                a.Name = darr[0];
                var nameIndex = 0;
                for (int i = 0; i < darr.Count; i++)
                {
                    var item = darr[i];
                    if (item.Length < a.Name.Length)
                    {						
                        nameIndex = i;
                        a.Name = item;
                    }
                    //修正名称取到了省市区镇村组的
                    if (a.Name != item &&
                        (a.Name.EndsWith("省") || a.Name.EndsWith("市") || a.Name.EndsWith("区") || a.Name.EndsWith("镇") || a.Name.EndsWith("村") || a.Name.EndsWith("组"))
                        && !(item.EndsWith("省") || item.EndsWith("市") || item.EndsWith("区") || item.EndsWith("镇") || item.EndsWith("村") || item.EndsWith("组")) && item.Length <= 5)
                    {
                        nameIndex = i;
                        a.Name = item;
                    }
                }
                darr.RemoveAt(nameIndex);
                data = String.Join(" ", darr);
              
            }
			a.FullAddress = System.Text.RegularExpressions.Regex.Replace(data, "\\s{1,}", "");

            return a;
        }

		/// <summary>
		/// 分离解析地址 得到粗略的基本省市区信息
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
        private (string Province, string City, string Region, string Street,string Address) StripAddressDetail(string address)
        {
           // var address = a.FullAddress;
            var addr_origin = address;
            address = address.Replace(" ", "").Replace(",", "");
            address = address.Replace("自治区", "省");
            address = address.Replace("自治州", "州");
			//address = address.Replace("自治县", "县");
			address = address.Replace("地区", "市");			
			address = address.Replace("小区", "");
            address = address.Replace("校区", "");

            var province = "";
            var city = "";
            var region = "";
            var street = "";
            var addrlength = address.Length;
            if((address.Contains("县") && address.IndexOf("县")< (addrlength / 3 * 2))
                || (address.Contains("区") && address.IndexOf("区") < (addrlength / 3 * 2))
                || (address.Contains("旗") && address.IndexOf("旗") < (addrlength / 3 * 2)))
            {
                var pos = 0;
                if (address.Contains("旗"))
                {
                    pos = address.IndexOf("旗");
                    region = address.Substring(GetPos(pos - 1), GetLenth(pos - 1, 2));
                }


                if (address.Contains("区"))
                {
                    pos = address.IndexOf("区");

                    if (address.Contains("市"))
                    {
                        var city_pos = address.IndexOf("市");
                        var zone_pos = address.IndexOf("区");
                        region = address.Substring(city_pos + 1, zone_pos - city_pos);
                    }
                    else
                    {
                        region = address.Substring(GetPos(pos - 2), GetLenth(pos - 2, 3));
                    }

                }


                if (address.Contains("县"))
                {
                    pos = address.IndexOf("县");

                    if (address.Contains("市"))
                    {
                        var city_pos = address.IndexOf("市");
                        var zone_pos = address.IndexOf("县");
                        region = address.Substring(city_pos + 1, zone_pos - city_pos);
                        //说明不是一个正确的县字
						if (region.Length == 1) {
							var address2 = address.Substring(zone_pos+1);
                            if (address2.Contains("县"))
                            {
								zone_pos = address2.IndexOf("县");
								pos = pos + zone_pos + 1;
								region = address2.Substring(0, zone_pos + 1);
							}
						}
                    }
                    else
                    {
                        if (address.Contains("自治县"))
                        {
                            var va = new string[] { "省", "市", "州" };
                            region = address.Substring(GetPos(pos - 6), GetLenth(pos - 6, 7));
                            if (va.Contains(region.Substring(0, 1))){
                                region = region.Substring(1);
                            }
                        }
                        else
                        {
                            region = address.Substring(GetPos(pos - 2), GetLenth(pos - 2, 3));
                        }
                    }
                }
                street = address.Substring(pos + 1);
            }
            else if(address.Contains("市"))
            {
                var pos = address.IndexOf("市");
                if (pos == 1)
                {
                    region = address.Substring(GetPos(pos - 2), GetLenth(pos - 2, 3));
                    street = address.Substring(pos + 1);

                }else if (pos >= 2)
                {
                    region = address.Substring(GetPos(pos - 2), GetLenth(pos - 2, 3));
                    street = address.Substring(pos + 1);
                }
            }
            else
            {
                region = "";
                street = address;
            }

            if (address.Contains("市"))
            {
                city = address.Substring(GetPos(address.IndexOf("市") - 2), GetLenth(address.IndexOf("市") - 2, 3));
            }
            else if (address.Contains("盟"))
            {
                city = address.Substring(GetPos(address.IndexOf("盟") - 2), GetLenth(address.IndexOf("盟") - 2, 3));
            }
            else if (address.Contains("自治州"))
            {
                city = address.Substring(GetPos(address.IndexOf("自治州") - 4), GetLenth(address.IndexOf("自治州") - 4, 3));
            }
            else if (address.Contains("州"))
            {
                city = address.Substring(GetPos(address.IndexOf("州") - 2), GetLenth(address.IndexOf("州") - 2, 3));
            }
            else
            {
                city = "";
            }

            if (address.Contains("省") && address.IndexOf("省") < 5)
            {
                province = address.Substring(0, address.IndexOf("省") + 1);
			}


            //地级市
            if (street.Contains("市") && city == region)
			{
				var pos = street.IndexOf("市");
				region = street.Substring(GetPos(pos - 2), GetLenth(address.IndexOf("市") - 2, 3));
				street = street.Substring(pos + 1);
			}

            if (street.Contains("街道"))
            {
                street = street.Substring(0, street.IndexOf("街道") + 2);
            }
            else if (street.Contains("乡"))
            {
                street = street.Substring(0, street.IndexOf("乡") + 1);
            }
            else if (street.Contains("镇"))
            {
                street = street.Substring(0, street.IndexOf("镇") + 1);
            }
            if (!string.IsNullOrEmpty(street) && (street.Contains("街道")|| street.Contains("乡")|| street.Contains("镇")) )
            {
                address = address.Replace(street, "");
            }
            if (!string.IsNullOrEmpty(region))
            {
                address = address.Replace(region, "");
            }
            if (!string.IsNullOrEmpty(city))
            {
                address = address.Replace(city, "");
            }
            if (!string.IsNullOrEmpty(province))
            {
                address = address.Replace(province, "");
            }

            return (province, city, region, street, address);

        }

		/// <summary>
		/// 获取位置
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		private int GetPos(int pos)
        {
            if (pos < 0)
            {
				return 0;
            }
			return pos;
        }

        /// <summary>
        /// 获取取值的长度 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private int GetLenth(int pos, int len)
        {
            if (pos < 0)
            {
                var templen = len + pos;
                if (templen < 1)
                {
                    return len;
                }
                return templen;
            }
            return len;
        }


        /// <summary>
        /// 根据地址库匹配最终的省市区信息
        /// </summary>
        /// <param name="tempProvince"></param>
        /// <param name="tempCity"></param>
        /// <param name="tempRegion"></param>
        /// <returns></returns>
        private (string Province, string City, string Region,string Street, int ProvinceId, int CityId, int RegionId,int StreetId) MatchAddressDetail(string tempProvince, string tempCity, string tempRegion,string tempStreet)
        {
            string province = "";
            string city = "";
            string region = "";
            string street = "";
            int provinceid = 0;
            int cityid = 0;
			int regionid = 0;
            int streetid = 0;
            List<Area> matchStreetDataSource = new List<Area>();
            List<Area> matchRegionDataSource = new List<Area>();
            List<Area> matchCityDataSource = new List<Area>();
            List<Area> matchProvinceDataSource = new List<Area>();

            if (!string.IsNullOrWhiteSpace(tempStreet))
            {
                matchStreetDataSource = StreetDataSource.FindAll(a => a.Name.Contains(tempStreet));
            }

			if (!string.IsNullOrWhiteSpace(tempRegion))
            {
				matchRegionDataSource = RegionDataSource.FindAll(a => a.Name.Contains(tempRegion));
                if (tempRegion.EndsWith("区"))
                {
					//因为有些特殊的区，原来是县，但是地区库里面有内容还没有更新还是老的数据 以县为主
					//所以这里需要查询为县的
					if (matchRegionDataSource.Count == 0)
					{
						var resetRegion1 = $"{tempRegion.Remove(tempRegion.Length - 1)}县";
						matchRegionDataSource = RegionDataSource.FindAll(a => a.Name.Contains(resetRegion1));
					}
					
                    if (matchRegionDataSource.Count == 0)
                    {
						//可能改名为县级市
						var resetRegion1 = $"{tempRegion.Remove(tempRegion.Length - 1)}市";
						matchRegionDataSource = RegionDataSource.FindAll(a => a.Name.Contains(resetRegion1));
					}
                    //可能改名为新区级
                    var resetRegion = $"{tempRegion.Remove(tempRegion.Length - 1)}新区";
                    matchRegionDataSource.AddRange(RegionDataSource.FindAll(a => a.Name.Contains(resetRegion)));

                }

            }

            if (!string.IsNullOrWhiteSpace(tempCity))
			{
				matchCityDataSource = CityDataSource.FindAll(a => a.Name.Contains(tempCity));
			}

			if (!string.IsNullOrWhiteSpace(tempProvince))
			{
				matchProvinceDataSource = ProvinceDataSource.FindAll(a => a.Name.Contains(tempProvince));
			}

            if (matchRegionDataSource.Count > 0)
            {
                if (matchCityDataSource.Count > 0)
                {
                    var cityObj = matchCityDataSource.Find(a => matchRegionDataSource.Find(b => b.Pid == a.Id) != null);
					//根据上面的区域，没有找到市，那么就以市为准查找
                    if (cityObj == null)
                    {
                        if (matchProvinceDataSource.Count == 0)
                        {
							region = "";
							city = matchCityDataSource[0].Name;
							cityid = matchCityDataSource[0].Id;
							provinceid = matchCityDataSource[0].Pid;
							province = ProvinceDataSource.Find(a => a.Id == provinceid)?.Name;
                        }
                        else
                        {
							var provinceObj = matchProvinceDataSource.Find(a => matchCityDataSource.Find(b => b.Pid == a.Id) != null);
                            if (provinceObj != null)
							{
								var mcityObj = matchCityDataSource.Find(a => a.Pid == provinceObj.Id);
								region = "";
								city = mcityObj?.Name;
								cityid = mcityObj?.Id ?? 0;
								province = provinceObj.Name;
								provinceid = provinceObj.Id;

							}
                            else
                            {
								province = matchProvinceDataSource[0].Name;
								provinceid = matchProvinceDataSource[0].Id;
								city = "";
								region = "";
							}

						}
					
					}
                    else
                    {
						var regionObj = matchRegionDataSource.Find(a => a.Pid == cityObj.Id);
						region = regionObj.Name;
						regionid = regionObj.Id;
						city = cityObj.Name;
						cityid = cityObj.Id;
						provinceid = cityObj.Pid;
						province = ProvinceDataSource.Find(a => a.Id == provinceid)?.Name;
					}
                }
                //存在省
                else if (matchProvinceDataSource.Count > 0)
                {
					var cityMatchList = CityDataSource.FindAll(a => matchRegionDataSource.Find(b => b.Pid == a.Id) != null);
					var provinceMatch = matchProvinceDataSource.Find(a => cityMatchList.Find(b => b.Pid == a.Id) != null);
                    if (provinceMatch == null)
                    {
						region = "";
						city = "";
						province = matchProvinceDataSource[0].Name;
						provinceid = matchProvinceDataSource[0].Id;

					}
                    else
                    {
						var cityObj = cityMatchList.Find(a => a.Pid == provinceMatch.Id);
						var regionObj = matchRegionDataSource.Find(b => b.Pid == cityObj?.Id);
						city = cityObj?.Name;
						cityid = cityObj?.Id ?? 0;
						region = regionObj?.Name;
						regionid = regionObj?.Id ?? 0;
						province = provinceMatch.Name;
						provinceid = provinceMatch.Id;
					}
                }
                else
                {
					var cityObj = CityDataSource.Find(a => a.Id == matchRegionDataSource[0].Pid);
					var provinceObj = ProvinceDataSource.Find(b => b.Id == cityObj.Pid);

					region = matchRegionDataSource[0].Name;
					regionid = matchRegionDataSource[0].Id;
					city = cityObj.Name;
					cityid = cityObj.Id;
					province = provinceObj.Name;
					provinceid = provinceObj.Id;

				}
            }
            else
            {
                if (matchCityDataSource.Count > 0)
                {
                    if (matchProvinceDataSource.Count == 0)
                    {
                        region = "";
                        city = matchCityDataSource[0].Name;
						cityid = matchCityDataSource[0].Id;
						provinceid = matchCityDataSource[0].Pid;
                        var provinceObj = ProvinceDataSource.Find(a => a.Id == provinceid);
                        province = provinceObj?.Name;
                    }
                    else
                    {
                        var provinceObj = matchProvinceDataSource.Find(a => matchCityDataSource.Find(b => b.Pid == a.Id) != null);
                        if (provinceObj != null)
                        {
                            var mcityObj = matchCityDataSource.Find(a => a.Pid == provinceObj.Id);
                            region = "";
                            city = mcityObj?.Name;
                            cityid = mcityObj?.Id ?? 0;
                            province = provinceObj.Name;
                            provinceid = provinceObj?.Id ?? 0;

                        }
                        else
                        {
                            province = matchProvinceDataSource[0].Name;
							provinceid = matchProvinceDataSource[0].Id;

							city = "";
                            region = "";
                        }
                    }

                }
                //存在省
                else if (matchProvinceDataSource.Count > 0)
                {
                    region = "";
                    city = "";
                    province = matchProvinceDataSource[0].Name;
					provinceid = matchProvinceDataSource[0].Id;

				}
            }
            if (matchStreetDataSource.Count > 0)
            {
                var streetObj = matchStreetDataSource.Find(a => a.Pid == regionid);
                if (streetObj != null)
                {
                    street = streetObj?.Name;
                    streetid = streetObj?.Id ?? 0;
                }
            }
            return (province, city, region, street, provinceid, cityid, regionid, streetid);
        }

    }
}
