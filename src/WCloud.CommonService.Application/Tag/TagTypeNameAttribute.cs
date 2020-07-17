using System;

namespace WCloud.CommonService.Application.Tag
{
    public class TagTypeNameAttribute:Attribute
    {
        public string Name { get; private set; }

        public TagTypeNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}
