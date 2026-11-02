Public Class SourceMdlBoneController2531

	' MDL v2531 bone controller, 24 bytes. The game steps through the array
	' at this size; unlike later Source versions there is no padding.
	'	int		bone;
	'	int		type;		// X, Y, Z, XR, YR, ZR, M
	'	float	start;
	'	float	end;
	'	int		rest;
	'	int		inputfield;	// 0-3 user set controller, 4 mouth

	Public boneIndex As Integer
	Public type As Integer
	Public startAngleDegrees As Double
	Public endAngleDegrees As Double
	Public restIndex As Integer
	Public inputField As Integer


End Class
