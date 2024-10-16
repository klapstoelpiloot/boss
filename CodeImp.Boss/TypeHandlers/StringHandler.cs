using System;

namespace CodeImp.Boss.TypeHandlers
{
    public class StringHandler : BossTypeHandler
    {
        public override byte BossType => (byte)BossTypeCode.String;
        public override Type ClassType => typeof(string);

        public override void WriteTo(BossSerializer serializer, BossWriter writer, object value)
        {
            writer.Write(value?.ToString());
        }

        public override object ReadFrom(BossSerializer serializer, BossReader reader, Type basetype)
        {
            string str = reader.ReadString();
            if (basetype.IsEnum && (str != null))
                return Enum.Parse(basetype, str);
            else
                return str;
        }
    }
}
