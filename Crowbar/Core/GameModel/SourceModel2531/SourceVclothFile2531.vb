Imports System.Collections
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Web.Script.Serialization

' Exact REF triangle provenance; no additional SMD or helper skeleton is emitted.
Public Class SourceVclothReferenceTriangle2531
	Public material As String
	Public vertices As Integer()
	Public smdCorners As String()
End Class

Public Class SourceVclothFile2531
	Public Shared Function IsFinite(ByVal value As Double) As Boolean
		Return Not Double.IsNaN(value) AndAlso Not Double.IsInfinity(value)
	End Function

	Private Shared Function Vec(ByVal value As SourceVector) As Double()
		Return New Double() {value.x, value.y, value.z}
	End Function

	Private Shared Function Skin(ByVal vertex As SourceMdlType0Vertex2531, ByVal data As SourceMdlFileData2531) As List(Of Object)
		Dim result As New List(Of Object)()
		For i As Integer = 0 To vertex.weight.Length - 1
			If vertex.weight(i) = 0 Then Continue For
			Dim bone As Integer = vertex.boneIndex(i)
			If bone < 0 OrElse bone >= data.theBones.Count Then Throw New InvalidDataException("VCLOTH vertex has an invalid skinning bone.")
			result.Add(New Dictionary(Of String, Object) From {{"bone", bone}, {"weight_byte", CInt(vertex.weight(i))}})
		Next
		Return result
	End Function

	Private Shared Function ExportedCorner(ByVal line As String) As Object
		Dim values As String() = line.Split(New Char() {" "c, ChrW(9)}, StringSplitOptions.RemoveEmptyEntries)
		If values.Length < 10 Then Throw New InvalidDataException("Incomplete VCLOTH REF corner.")
		Dim links As Integer = Integer.Parse(values(9), CultureInfo.InvariantCulture)
		If links < 0 OrElse values.Length <> 10 + links * 2 Then Throw New InvalidDataException("Invalid VCLOTH REF skinning fields.")
		Dim geometry(7) As Double
		For i As Integer = 0 To geometry.Length - 1
			geometry(i) = Double.Parse(values(i + 1), CultureInfo.InvariantCulture)
		Next
		Dim skinning As New List(Of Object)()
		For i As Integer = 0 To links - 1
			skinning.Add(New Object() {Integer.Parse(values(10 + i * 2), CultureInfo.InvariantCulture), Double.Parse(values(11 + i * 2), CultureInfo.InvariantCulture)})
		Next
		Return New Dictionary(Of String, Object) From {{"parent_bone", Integer.Parse(values(0), CultureInfo.InvariantCulture)}, {"position", New Double() {geometry(0), geometry(1), geometry(2)}}, {"normal", New Double() {geometry(3), geometry(4), geometry(5)}}, {"uv", New Double() {geometry(6), geometry(7)}}, {"skinning", skinning}}
	End Function

	Public Shared Function Serialize(ByVal model As SourceMdlModel2531, ByVal data As SourceMdlFileData2531, ByVal referenceFile As String, ByVal referenceTriangles As List(Of SourceVclothReferenceTriangle2531)) As String
		Dim cloth As SourceMdlClothMesh2531 = model.theClothMesh
		If cloth Is Nothing OrElse Not cloth.isDataReady Then Throw New InvalidDataException("VCLOTH data is unavailable or invalid.")
		If referenceTriangles Is Nothing OrElse referenceTriangles.Count = 0 Then Throw New InvalidDataException("VCLOTH requires exported REF triangles.")
		Dim corners As New Dictionary(Of Integer, String)()
		Dim faces As New List(Of Object)()
		For Each triangle As SourceVclothReferenceTriangle2531 In referenceTriangles
			For i As Integer = 0 To 2
				Dim vertex As Integer = triangle.vertices(i)
				If vertex < 0 OrElse vertex >= model.vertexCount Then Throw New InvalidDataException("VCLOTH REF vertex is outside the model.")
				If corners.ContainsKey(vertex) AndAlso corners(vertex) <> triangle.smdCorners(i) Then Throw New InvalidDataException("VCLOTH REF vertex has inconsistent exported attributes.")
				corners(vertex) = triangle.smdCorners(i)
			Next
			faces.Add(New Dictionary(Of String, Object) From {{"material", triangle.material}, {"vertices", triangle.vertices}})
		Next
		Dim vertices As New List(Of Object)()
		For index As Integer = 0 To model.vertexCount - 1
			Dim vertex As SourceMdlType0Vertex2531 = model.theVertexesType0(index)
			Dim exported As Object = Nothing
			Dim corner As String = Nothing
			If corners.TryGetValue(index, corner) Then exported = ExportedCorner(corner)
			vertices.Add(New Dictionary(Of String, Object) From {{"id", index}, {"source_position", Vec(vertex.position)}, {"source_normal", Vec(vertex.normal)}, {"source_uv", New Double() {vertex.texCoordU, vertex.texCoordV}}, {"skinning", Skin(vertex, data)}, {"exported", exported}})
		Next
		Dim bones As New List(Of Object)()
		For index As Integer = 0 To data.theBones.Count - 1
			Dim bone As SourceMdlBone2531 = data.theBones(index)
			bones.Add(New Dictionary(Of String, Object) From {{"id", index}, {"name", bone.theName}, {"parent", bone.parentBoneIndex}, {"position", Vec(bone.position)}, {"rotation_xyzw", New Double() {bone.rotation.x, bone.rotation.y, bone.rotation.z, bone.rotation.w}}})
		Next
		Dim nodes As New List(Of Object)()
		For node As Integer = 0 To cloth.header.numNodes - 1
			Dim vertex As Integer = cloth.theNodeMeshVertexIndices(node)
			nodes.Add(New Dictionary(Of String, Object) From {{"id", node}, {"fixed", node < cloth.header.numFixedNodes}, {"reference_vertex", vertex}, {"position", Vec(model.theVertexesType0(vertex).position)}})
		Next
		Dim meshes As New List(Of Object)()
		For meshIndex As Integer = 0 To model.theMeshes.Count - 1
			Dim mesh As SourceMdlMesh2531 = model.theMeshes(meshIndex)
			Dim bindings As New List(Of Object)()
			If mesh.theClothInstances IsNot Nothing Then
				For v As Integer = 0 To mesh.vertexCount - 1
					If mesh.theClothInstances(v) = -1 Then Continue For
					Dim rawNode As Integer = mesh.theClothNodes(v)
					bindings.Add(New Dictionary(Of String, Object) From {{"vertex", mesh.vertexIndexStart + v}, {"node", rawNode And &H7FFF}, {"node_flags", rawNode And &H8000}, {"tangent_index", CInt(mesh.theClothTangents(v))}})
				Next
			End If
			If mesh.materialIndex < 0 OrElse mesh.materialIndex >= data.theTextures.Count Then Throw New InvalidDataException("Invalid VCLOTH mesh material.")
			meshes.Add(New Dictionary(Of String, Object) From {{"id", meshIndex}, {"material", data.theTextures(mesh.materialIndex).theFileName}, {"vertex_start", mesh.vertexIndexStart}, {"vertex_count", mesh.vertexCount}, {"bindings", bindings}})
		Next
		Dim springs As New List(Of Object)()
		For index As Integer = 0 To cloth.theSprings.Count - 1
			Dim spring As SourceMdlClothSpring2531 = cloth.theSprings(index)
			springs.Add(New Dictionary(Of String, Object) From {{"nodes", New Integer() {spring.nodeA, spring.nodeB}}, {"family", If(index < cloth.header.numStructuralSprings, "structural", "bending")}, {"weight_a", spring.weightA}, {"weight_b", spring.weightB}, {"rest_length_squared", spring.restLengthSq}})
		Next
		Dim tangents As New List(Of Object)()
		For Each tangent As SourceMdlClothTangent2531 In cloth.theTangents
			tangents.Add(New Dictionary(Of String, Object) From {{"edges", New Integer() {tangent.edge0, tangent.edge1}}, {"coefficients", New Single() {tangent.coefficient0, tangent.coefficient1}}})
		Next
		Dim document As New Dictionary(Of String, Object) From {
			{"format", "vtmb-cloth"}, {"version", 1}, {"reference_smd", referenceFile}, {"lod", 0},
			{"coordinate_space", "reference_model"}, {"source_model", data.theModelName},
			{"reference", New Dictionary(Of String, Object) From {{"bones", bones}, {"vertices", vertices}, {"triangles", faces}, {"meshes", meshes}}},
			{"simulation", New Dictionary(Of String, Object) From {{"time_step_scale", cloth.header.stiffness}, {"nodes", nodes}, {"springs", springs}, {"edges", cloth.theEdges}, {"normal_edge_prefix", cloth.normalEdgePrefix}, {"triangles", cloth.theTriangles}, {"tangents", tangents}, {"collision_neighbors", cloth.theCollisionNeighbors}, {"structural_batch_count", cloth.header.val2}, {"batch_sizes", cloth.theBatchSizes}}}}
		Dim result As New StringBuilder()
		WriteJson(result, document, 0)
		result.AppendLine()
		Return result.ToString()
	End Function

	' Round-trip float formatting preserves unknown solver coefficients without guessing meanings.
	Private Shared Sub WriteJson(ByVal output As StringBuilder, ByVal value As Object, ByVal depth As Integer)
		If value Is Nothing Then
			output.Append("null")
		ElseIf TypeOf value Is String Then
			output.Append(New JavaScriptSerializer().Serialize(CStr(value)))
		ElseIf TypeOf value Is Boolean Then
			output.Append(If(CBool(value), "true", "false"))
		ElseIf TypeOf value Is Single OrElse TypeOf value Is Double Then
			If Not IsFinite(CDbl(value)) Then Throw New InvalidDataException("VCLOTH cannot contain non-finite numbers.")
			output.Append(If(TypeOf value Is Single, CSng(value).ToString("R", CultureInfo.InvariantCulture), CDbl(value).ToString("R", CultureInfo.InvariantCulture)))
		ElseIf TypeOf value Is IDictionary Then
			output.Append("{")
			Dim first As Boolean = True
			For Each pair As DictionaryEntry In DirectCast(value, IDictionary)
				If Not first Then output.Append(",")
				output.AppendLine()
				output.Append(New String(" "c, (depth + 1) * 2))
				WriteJson(output, CStr(pair.Key), depth + 1)
				output.Append(": ")
				WriteJson(output, pair.Value, depth + 1)
				first = False
			Next
			If Not first Then output.AppendLine().Append(New String(" "c, depth * 2))
			output.Append("}")
		ElseIf TypeOf value Is IEnumerable Then
			output.Append("[")
			Dim first As Boolean = True
			For Each element As Object In DirectCast(value, IEnumerable)
				If Not first Then output.Append(", ")
				WriteJson(output, element, depth + 1)
				first = False
			Next
			output.Append("]")
		ElseIf TypeOf value Is Byte OrElse TypeOf value Is Short OrElse TypeOf value Is UShort OrElse TypeOf value Is Integer OrElse TypeOf value Is Long Then
			output.Append(Convert.ToString(value, CultureInfo.InvariantCulture))
		Else
			Throw New InvalidDataException("Unsupported VCLOTH JSON value.")
		End If
	End Sub
End Class
