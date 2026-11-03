Public Class SourceMdlVertAnim2531

	' MDL v2531 stores a flex's per-vertex deltas in one of two records,
	' selected by mstudioflex_t.vertanimtype. Directions index the fine
	' direction table by byte offset; magnitudes are byte / 255.
	'
	' Type 0, 20 bytes:
	'	short	index;
	'	short	unused02;
	'	float	delta[3];				// position delta
	'	ushort	normalDeltaDirection;	// direction table byte offset
	'	byte	normalDeltaLength;		// * 2 / 255
	'	byte	unused13;
	'
	' Type 1, 8 bytes:
	'	short	index;
	'	ushort	positionDeltaDirection;	// direction table byte offset
	'	ushort	normalDeltaDirection;	// direction table byte offset
	'	byte	positionDeltaLength;	// * 8 / 255
	'	byte	normalDeltaLength;		// * 2 / 255

	Public Const PositionDeltaRange As Double = 8.0R
	Public Const NormalDeltaRange As Double = 2.0R

	Public index As UShort

	Public positionDeltaDirection As UShort
	Public positionDeltaLength As Byte
	Public normalDeltaDirection As UShort
	Public normalDeltaLength As Byte

	' Decoded deltas for a flex weight of 1.
	Public delta As New SourceVector()
	Public nDelta As New SourceVector()

End Class
