﻿namespace IO
{
	public interface ISerializable
	{
		void Write(IExtendedBinaryWriter writer);
	}
}