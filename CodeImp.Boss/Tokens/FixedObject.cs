namespace CodeImp.Boss.Tokens
{
	public class FixedObjectElement : BossElement
	{
		public List<BossElement> Elements { get; } = new List<BossElement>();

		public FixedObjectElement(string name) : base(name)
		{
			Type = (byte)BossElementTypes.FixedObject;
		}

		protected override void WriteTo(BossWriter writer)
		{
			writer.WriteVLQ(Elements.Count);
			foreach(BossElement e in Elements)
			{
				e.WriteToStream(writer);
			}
		}

		protected override void ReadFrom(BossReader reader)
		{
			int numelement = reader.ReadVLQ();
			Elements.Capacity = Math.Max(numelement, Elements.Capacity);
			for(int i = 0; i < numelement; i++)
			{
				BossElement e = reader.ReadNextElement();
				Elements.Add(e);
			}
		}
	}
}
