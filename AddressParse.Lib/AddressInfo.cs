namespace AddressParse.Lib
{
    /// <summary>
    /// 完整信息
    /// </summary>
    public class AddressInfo
    {
		/// <summary>
		/// 省
		/// </summary>
        public string Province { get; set; }
		/// <summary>
		/// 省代码
		/// </summary>
        public int ProvinceId { get; set; }
		/// <summary>
		/// 市
		/// </summary>
        public string City { get; set; }
		/// <summary>
		/// 市代码
		/// </summary>
        public int CityId { get; set; }
		/// <summary>
		/// 区
		/// </summary>
        public string Region { get; set; }
		/// <summary>
		/// 区代码
		/// </summary>
        public int RegionId { get; set; }
		/// <summary>
		/// 完整地址
		/// </summary>
        public string FullAddress { get; set; }
		/// <summary>
		/// 短地址
		/// </summary>
		public string ShortAddress { get; set; }
		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 电话
		/// </summary>
        public string Telphone { get; set; }
		/// <summary>
		/// 邮政编码
		/// </summary>
        public string Zipcode { get; set; }
		/// <summary>
		/// 身份证
		/// </summary>
        public string IdentityNumer { get; set; }
    }
}
