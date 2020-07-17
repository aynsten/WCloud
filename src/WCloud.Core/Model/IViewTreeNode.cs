using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WCloud.Core.Model
{
    /// <summary>
    /// https://www.iviewui.com/components/tree
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public class IViewTreeNode
    {
        [JsonProperty(Required = Required.Always)]
        public virtual string id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual string pId { get; set; }

        public virtual string title { get; set; }

        public virtual bool expand { get; set; } = false;

        public virtual bool disabled { get; set; } = false;

        public virtual bool disableCheckbox { get; set; } = false;

        public virtual bool selected { get; set; } = false;

        public virtual bool @checked { get; set; } = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual List<IViewTreeNode> children { get; set; }

        public virtual bool match { get; set; } = false;

        public virtual object raw_data { get; set; }
    }
}
