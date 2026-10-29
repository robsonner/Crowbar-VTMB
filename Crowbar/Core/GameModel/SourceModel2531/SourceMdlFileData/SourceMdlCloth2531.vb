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
	Public fixedBoneName As String
	Public theNodeMeshVertexIndices As List(Of UShort)
	Public theSprings As List(Of SourceMdlClothSpring2531)
	' Role IDs are helper-skeleton IDs, never game bone indices.
	Public Shared ReadOnly RoleNames As String() = {"cloth_rigid", "cloth_anchor", "cloth_pinned", "cloth_free"}
	Public theVertexRoles As Dictionary(Of Integer, Integer)
	Public rolesWarning As String

	Public Shared Function RolesFileName(ByVal referenceFileName As String) As String
		Return System.IO.Path.Combine(If(System.IO.Path.GetDirectoryName(referenceFileName), ""), System.IO.Path.GetFileNameWithoutExtension(referenceFileName) + "_CLOTH.smd")
	End Function
End Class
