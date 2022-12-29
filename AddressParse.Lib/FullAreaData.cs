namespace AddressParse.Lib
{
    /// <summary>
    /// 完整的地址源数据
    /// </summary>
    public class FullAreaData
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 父级
        /// </summary>
        public int parent_id { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 邮政编码
        /// </summary>
        public string zip { get; set; }
        /// <summary>
        /// 子集
        /// </summary>
        public List<FullAreaData> children { get; set; }
    }

}
