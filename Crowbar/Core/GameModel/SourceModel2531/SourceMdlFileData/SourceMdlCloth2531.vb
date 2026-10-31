Public Class SourceMdlClothCollisionCapsule2531
	' Size: 36 bytes (9 * 4)
	Public boneIndex1 As Integer
	Public boneIndex2 As Integer
	Public radius As Single
	Public pt1 As SourceVector
	Public pt2 As SourceVector

	Public theBoneName As String
	Public theBone2Name As String
End Class

Public Class SourceMdlClothConstraintCapsule2531
	' Size: 20 bytes (5 * 4)
	Public boneIndex As Integer
	Public radius As Single
	Public position As SourceVector

	Public theBoneName As String
End Class

Public Class SourceMdlClothHeader2531
	' Size: 64 bytes (16 * 4) at start of unknown03
	Public ptr0 As Integer
	Public stiffness As Single
	Public numNodes As Integer
	Public numFixedNodes As Integer
	Public numFreeNodes As Integer
	Public nodeTableOffset As Integer
	Public numSprings As Integer
	Public numStructuralSprings As Integer
	Public numShearBendSprings As Integer
	Public val1 As Integer
	Public val2 As Integer
	Public val3 As Integer
	Public subTable1Offset As Integer
	Public subTable1Count As Integer
	Public subTable2Offset As Integer
	Public subTable2Count As Integer
End Class

Public Class SourceMdlClothSpring2531
	' Size: 16 bytes.
	Public nodeA As UShort
	Public nodeB As UShort
	Public weightA As Single
	Public weightB As Single
	Public restLengthSq As Single
End Class

Public Class SourceMdlClothMesh2531
	Public header As SourceMdlClothHeader2531
	Public theMeshIndex As Integer
	Public theMeshMaterialName As String
	Public theNodeMeshVertexIndices As List(Of UShort)
	Public theSprings As List(Of SourceMdlClothSpring2531)
	Public exportWarning As String
	Public isDataReady As Boolean
	Public theEdges As List(Of UShort())
	Public theTriangles As List(Of UShort())
	Public theTangents As List(Of SourceMdlClothTangent2531)
	Public theCollisionNeighbors As List(Of UShort())
	Public theBatchSizes As List(Of Byte)
	Public normalEdgePrefix As Integer
	Public hasLodInterpolation As Boolean

	Public Shared Function DataFileName(ByVal referenceFileName As String) As String
		Return System.IO.Path.ChangeExtension(referenceFileName, ".vcloth")
	End Function
End Class

Public Class SourceMdlClothTangent2531
	Public edge0 As UShort
	Public edge1 As UShort
	Public coefficient0 As Single
	Public coefficient1 As Single
End Class
