namespace AddressParse.Lib
{
    /// <summary>
    /// 地址库信息
    /// </summary>
    internal class Area
    {
		/// <summary>
		/// 地址ID
		/// </summary>
        public int Id { get; set; }
		/// <summary>
		/// 父级地址
		/// </summary>
        public int Pid { get; set; }
		/// <summary>
		/// 地址名称
		/// </summary>
        public string Name { get; set; }
		/// <summary>
		/// 邮政编码
		/// </summary>
        public int Zipcode { get; set; }
		/// <summary>
		/// 邮政编码
		/// </summary>
		public string Zip { get; set; }
    }
}
