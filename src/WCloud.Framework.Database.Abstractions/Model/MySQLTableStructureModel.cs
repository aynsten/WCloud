namespace WCloud.Framework.Database.Abstractions.Model
{
    /*
     SELECT * FROM INFORMATION_SCHEMA.COLUMNS limit 50000
         */

    /// <summary>
    /// 可以使用官方的mysqldiff工具对比，python写的
    /// </summary>
    public class MySQLTableStructureModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string TABLE_CATALOG { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TABLE_SCHEMA { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TABLE_NAME { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string COLUMN_NAME { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ORDINAL_POSITION { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string COLUMN_DEFAULT { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IS_NULLABLE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DATA_TYPE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CHARACTER_MAXIMUM_LENGTH { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CHARACTER_OCTET_LENGTH { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NUMERIC_PRECISION { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string NUMERIC_SCALE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DATETIME_PRECISION { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CHARACTER_SET_NAME { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string COLLATION_NAME { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string COLUMN_TYPE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string COLUMN_KEY { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EXTRA { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PRIVILEGES { get; set; }

        /// <summary>
        /// 设置名称
        /// </summary>
        public string COLUMN_COMMENT { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GENERATION_EXPRESSION { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SRS_ID { get; set; }
    }
}
