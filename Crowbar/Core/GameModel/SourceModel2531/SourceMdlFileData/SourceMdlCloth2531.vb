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

Public Class SourceMdlClothMesh2531
	Public theMeshName As String
	Public rootBoneName As String
	Public numVertices As Integer
	Public numFixedVertices As Integer
	Public numFreeVertices As Integer
	Public numSprings As Integer
	Public damping As Single
	Public stiffness As Single
	Public gravity As Single
End Class
