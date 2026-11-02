Public Class SourceMdlType1Vertex2531

	' Vertex format 1 (unskinned), 12 bytes:
	'	unsigned short position[3];	// * model position scale + model position offset
	'	unsigned short normalOffset;	// byte offset into the fine direction table
	'	unsigned short texCoord[2];	// / 65535

	Public positionX As UInt16
	Public positionY As UInt16
	Public positionZ As UInt16
	Public normalOffset As UInt16
	Public texCoordU As UInt16
	Public texCoordV As UInt16

End Class
