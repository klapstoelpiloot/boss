namespace CodeImp.Boss.Tokens
{
	public abstract class BossElement
	{
		public byte Type { get; protected set; }

		public string Name { get; private set; }

		// Constructor
		protected BossElement(string name)
		{
			this.Name = name;
		}

		protected abstract void WriteTo(BossWriter writer);
		protected abstract void ReadFrom(BossReader reader);

		public void WriteToStream(BossWriter writer, bool includetypeandname = true)
		{
			if(includetypeandname)
			{
				writer.Write(Type);
				writer.Write(Name);
			}
			WriteTo(writer);
		}

		public void ReadFromStream(BossReader reader)
		{
			ReadFrom(reader);
		}
	}
}
