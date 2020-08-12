using System;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.MongoDB.IdGenerator
{
    public class IdGeneratorEntityBase : EntityBase
    {
        public string Category { get; set; }
        public int MaxID { get; set; }
    }
}
