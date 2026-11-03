Public Class SourceMdlFlex2531

	' MDL v2531 flex, 32 bytes:
	'	int		flexdesc;
	'	float	target0, target1, target2, target3;
	'	short	numverts;
	'	short	unused16;		// zero in shipped models
	'	int		vertindex;		// from this flex
	'	short	vertanimtype;	// 0: 20-byte records, 1: 8-byte records
	'	short	unused1E;		// not read by the engine

	Public flexDescIndex As Integer

	Public target0 As Double
	Public target1 As Double
	Public target2 As Double
	Public target3 As Double

	Public vertCount As Integer
	Public unused16 As Short
	Public vertOffset As Integer
	Public vertAnimType As Short
	Public unused1E As Short

	Public theVertAnims As List(Of SourceMdlVertAnim2531)

	Public Const VERTANIM_TYPE_FULL As Short = 0
	Public Const VERTANIM_TYPE_PACKED As Short = 1

End Class
