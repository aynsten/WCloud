using System;

namespace WCloud.Framework.Database.MongoDB.IdGenerator
{
    public class IdGeneratorEntityBase : MongoEntityBase
    {
        public string Category { get; set; }
        public int MaxID { get; set; }
    }
}
