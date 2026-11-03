Imports System.IO

' Direction tables that MDL v2531 compressed vertices and flexes index into.
' The engine keeps them as constant tables; both are rings of unit vectors at
' equal polar steps from -Z to +Z, and are rebuilt here from that rule. Ring k
' of n holds 4 * Int(n / 2 * Sin(k * Pi / n) + 0.9) azimuth steps (one at the
' poles). The fine table (n = 64, 5314 directions) is addressed by byte offset
' with a 12-byte stride; the coarse table (n = 12, 202 directions) by index.
' Rebuilt values agree with the shipped tables to within 3E-8.
Public Module SourceNormalTables2531

	Public Const FineTableStride As Integer = 12

	Public Function FineDirectionFromByteOffset(ByVal byteOffset As Integer) As SourceVector
		If byteOffset < 0 OrElse byteOffset Mod FineTableStride <> 0 OrElse byteOffset \ FineTableStride >= theFineTable.Length Then
			Throw New InvalidDataException("Direction offset " + byteOffset.ToString() + " is outside the v2531 direction table.")
		End If
		Return theFineTable(byteOffset \ FineTableStride)
	End Function

	Public Function CoarseDirection(ByVal index As Integer) As SourceVector
		If index < 0 OrElse index >= theCoarseTable.Length Then
			Throw New InvalidDataException("Normal index " + index.ToString() + " is outside the v2531 compressed normal table.")
		End If
		Return theCoarseTable(index)
	End Function

	Public ReadOnly Property FineTableCount As Integer
		Get
			Return theFineTable.Length
		End Get
	End Property

	Public ReadOnly Property CoarseTableCount As Integer
		Get
			Return theCoarseTable.Length
		End Get
	End Property

	Private Function BuildTable(ByVal ringSteps As Integer) As SourceVector()
		Dim directions As New List(Of SourceVector)()
		For ring As Integer = 0 To ringSteps
			Dim polar As Double = ring * Math.PI / ringSteps
			Dim sinPolar As Double = Math.Sin(polar)
			Dim z As Double = -Math.Cos(polar)
			Dim count As Integer
			If ring = 0 OrElse ring = ringSteps Then
				count = 1
			Else
				count = 4 * CInt(Math.Floor(ringSteps / 2.0 * sinPolar + 0.9))
			End If
			For stepIndex As Integer = 0 To count - 1
				Dim azimuth As Double = stepIndex * 2.0 * Math.PI / count
				directions.Add(New SourceVector(sinPolar * Math.Cos(azimuth), sinPolar * Math.Sin(azimuth), z))
			Next
		Next
		Return directions.ToArray()
	End Function

	Private ReadOnly theFineTable As SourceVector() = BuildTable(64)
	Private ReadOnly theCoarseTable As SourceVector() = BuildTable(12)

End Module
