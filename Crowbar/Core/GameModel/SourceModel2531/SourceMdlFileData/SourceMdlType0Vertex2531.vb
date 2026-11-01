Public Class SourceMdlType0Vertex2531

	'struct mstudiovertex_t // NOTE: This is exactly 64 bytes, two cache lines
	'{
	'	mstudioboneweight_t	m_BoneWeights;
	'	Vector		m_vecPosition;
	'	Vector		m_vecNormal;
	'	Vector2D	m_vecTexCoord;
	'};
	'
	'struct mstudioboneweight_t
	'{
	'	float	weight[4];
	'	short	bone[4]; 
	'
	'	short	numbones;
	'	short	material;
	'
	'	short	firstref;
	'	short	lastref;
	'};

	' Example: "character\pc\female\brujah\armor0\brujah_female_armor_0.mdl" 
	'          seems to use struct size of 12 + 32 = 44 bytes.
	'NOTE: The weight and bone fields are smaller than above. VtMB must only accept max of 3 bone weights per vertex.
	'Public weight(1) As Double
	'Public boneIndex(1) As Short
	'Public weight As Double
	'Public boneIndex As Short
	' Weights total to 255.
	' Experimental v2531 layout: three stored weights and an implicit fourth.
	' Keep decoded arrays aligned for both REF SMD and VCLOTH exports.
	Public weight(3) As Byte
	Public influenceSelector As Byte
	Public boneIndex(3) As Short

	Public Sub DecodeWeights()
		Dim count As Integer = Math.Max(1, CInt(influenceSelector) Mod 5)
		If count = 4 Then
			Dim remainder As Integer = 255 - CInt(weight(0)) - CInt(weight(1)) - CInt(weight(2))
			If remainder < 0 Then Throw New System.IO.InvalidDataException("VTMB four-influence vertex weights exceed 255.")
			weight(3) = CByte(remainder)
		End If
		For i As Integer = count To 3
			weight(i) = 0
		Next
	End Sub
	'Public boneCount As Byte
	'Public materialIndex As Short
	'Public firstRef As Short
	'Public lastRef As Short

	Public position As New SourceVector()
	Public normal As New SourceVector()
	Public texCoordU As Double
	Public texCoordV As Double
	'Public test(7) As Double

End Class
