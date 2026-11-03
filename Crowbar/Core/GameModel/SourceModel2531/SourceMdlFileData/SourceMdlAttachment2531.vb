Public Class SourceMdlAttachment2531

	'FROM: Bloodlines SDK source 2015-06-16\sdk-src (16.06.2015)\src\public\studio.h
	'struct mstudioattachment_t
	'{
	'	int					sznameindex;
	'	inline char * const pszName( void ) const { return ((char *)this) + sznameindex; }
	'	int					type;
	'	int					bone;
	'	matrix3x4_t			local; // attachment point
	'};
	'------
	'FROM: [1999] HLStandardSDK\SourceCode\engine\studio.h
	'// attachment
	'typedef struct 
	'{
	'	char				name[32];
	'	int					type;
	'	int					bone;
	'	vec3_t				org;	// attachment point
	'	vec3_t				vectors[3];
	'} mstudioattachment_t;

	Public nameOffset As Integer
	Public type As Integer
	Public boneIndex As Integer

	'Public attachmentPointColumn0 As New SourceVector()
	'Public attachmentPointColumn1 As New SourceVector()
	'Public attachmentPointColumn2 As New SourceVector()
	'Public attachmentPointColumn3 As New SourceVector()
	'Public attachmentPoint As New SourceVector()
	'Public vector01 As New SourceVector()
	'Public vector02 As New SourceVector()
	'Public vector03 As New SourceVector()
	'------
	' MDL v2531 attachment, 60 bytes: name offset, type, bone, then the
	' full bone-relative matrix3x4_t (rows of rotation plus translation).
	' The game concatenates the whole matrix with the bone's transform.
	Public localM11 As Double '12
	Public localM12 As Double '16
	Public localM13 As Double '20
	Public posX As Double '24

	Public localM21 As Double '28
	Public localM22 As Double '32
	Public localM23 As Double '36
	Public posY As Double '40

	Public localM31 As Double '44
	Public localM32 As Double '48
	Public localM33 As Double '52
	Public posZ As Double '56

	Public Function HasRotation() As Boolean
		Const tolerance As Double = 0.0001
		Return Math.Abs(Me.localM11 - 1) > tolerance OrElse Math.Abs(Me.localM12) > tolerance OrElse Math.Abs(Me.localM13) > tolerance _
		 OrElse Math.Abs(Me.localM21) > tolerance OrElse Math.Abs(Me.localM22 - 1) > tolerance OrElse Math.Abs(Me.localM23) > tolerance _
		 OrElse Math.Abs(Me.localM31) > tolerance OrElse Math.Abs(Me.localM32) > tolerance OrElse Math.Abs(Me.localM33 - 1) > tolerance
	End Function

	Public theName As String

End Class
