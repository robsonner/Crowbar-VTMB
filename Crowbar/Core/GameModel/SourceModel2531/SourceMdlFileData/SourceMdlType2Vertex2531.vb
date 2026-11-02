Public Class SourceMdlType2Vertex2531

	' Vertex format 2 (compressed, unskinned), 8 bytes:
	'	unsigned char position[3];	// / 255 * model position scale + model position offset
	'	unsigned char normalIndex;	// index into the coarse normal table
	'	unsigned short texCoord[2];	// / 65535

	Public positionX As Byte
	Public positionY As Byte
	Public positionZ As Byte
	Public normalIndex As Byte
	Public texCoordU As UInt16
	Public texCoordV As UInt16

End Class
