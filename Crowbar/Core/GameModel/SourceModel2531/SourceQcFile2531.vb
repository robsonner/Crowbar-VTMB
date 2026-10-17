Imports System.IO
Imports System.Text

Public Class SourceQcFile2531
	Inherits SourceQcFile

#Region "Creation and Destruction"

	Public Sub New(ByVal outputFileStream As StreamWriter, ByVal outputPathFileName As String, ByVal mdlFileData As SourceMdlFileData2531, ByVal vtxFileData As SourceVtxFileData107, ByVal phyFileData As SourcePhyFileData, ByVal modelName As String)
		Me.theOutputFileStreamWriter = outputFileStream
		Me.theMdlFileData = mdlFileData
		Me.thePhyFileData = phyFileData
		Me.theVtxFileData = vtxFileData
		Me.theModelName = modelName

		Me.theOutputPath = FileManager.GetPath(outputPathFileName)
		Me.theOutputFileNameWithoutExtension = Path.GetFileNameWithoutExtension(outputPathFileName)
	End Sub

#End Region

#Region "Methods"

	Public Sub WriteHeaderComment()
		Common.WriteHeaderComment(Me.theOutputFileStreamWriter)
	End Sub

	Public Sub WriteAttachmentCommand()
		Dim line As String = ""

		If Me.theMdlFileData.theAttachments IsNot Nothing Then
			Try
				line = ""
				Me.theOutputFileStreamWriter.WriteLine(line)

				Dim anAttachment As SourceMdlAttachment2531
				For i As Integer = 0 To Me.theMdlFileData.theAttachments.Count - 1
					anAttachment = Me.theMdlFileData.theAttachments(i)

					If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
						line = "$Attachment "
					Else
						line = "$attachment "
					End If
					If anAttachment.theName = "" Then
						line += i.ToString(TheApp.InternalNumberFormat)
					Else
						line += """"
						line += anAttachment.theName
						line += """"
					End If
					line += " """
					line += Me.theMdlFileData.theBones(anAttachment.boneIndex).theName
					line += """"
					line += " "

					line += anAttachment.posX.ToString("0.######", TheApp.InternalNumberFormat)
					line += " "
					line += anAttachment.posY.ToString("0.######", TheApp.InternalNumberFormat)
					line += " "
					line += anAttachment.posZ.ToString("0.######", TheApp.InternalNumberFormat)

					Me.theOutputFileStreamWriter.WriteLine(line)
				Next
			Catch ex As Exception
				Dim debug As Integer = 4242
			End Try
		End If
	End Sub

	Public Sub WriteBBoxCommand()
		Dim line As String = ""
		Dim minX As Double
		Dim minY As Double
		Dim minZ As Double
		Dim maxX As Double
		Dim maxY As Double
		Dim maxZ As Double

		Me.theOutputFileStreamWriter.WriteLine()

		If TheApp.Settings.DecompileDebugInfoFilesIsChecked Then
			line = "// Bounding box or hull. Used for collision with a world object."
			Me.theOutputFileStreamWriter.WriteLine(line)
		End If

		'FROM: VDC wiki: 
		'$bbox (min x) (min y) (min z) (max x) (max y) (max z)
		minX = Math.Round(Me.theMdlFileData.hullMinPosition.x, 3)
		minY = Math.Round(Me.theMdlFileData.hullMinPosition.y, 3)
		minZ = Math.Round(Me.theMdlFileData.hullMinPosition.z, 3)
		maxX = Math.Round(Me.theMdlFileData.hullMaxPosition.x, 3)
		maxY = Math.Round(Me.theMdlFileData.hullMaxPosition.y, 3)
		maxZ = Math.Round(Me.theMdlFileData.hullMaxPosition.z, 3)

		line = ""
		If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
			line += "$BBox "
		Else
			line += "$bbox "
		End If
		line += minX.ToString("0.######", TheApp.InternalNumberFormat)
		line += " "
		line += minY.ToString("0.######", TheApp.InternalNumberFormat)
		line += " "
		line += minZ.ToString("0.######", TheApp.InternalNumberFormat)
		line += " "
		line += maxX.ToString("0.######", TheApp.InternalNumberFormat)
		line += " "
		line += maxY.ToString("0.######", TheApp.InternalNumberFormat)
		line += " "
		line += maxZ.ToString("0.######", TheApp.InternalNumberFormat)

		Me.theOutputFileStreamWriter.WriteLine(line)
	End Sub

	Public Sub WriteBodyGroupCommand()
		Dim line As String = ""
		Dim aBodyPart As SourceMdlBodyPart2531
		Dim aBodyModel As SourceMdlModel2531

		If Me.theMdlFileData.theBodyParts IsNot Nothing AndAlso Me.theMdlFileData.theBodyParts.Count > 0 Then
			Me.theOutputFileStreamWriter.WriteLine()

			For bodyPartIndex As Integer = 0 To Me.theMdlFileData.theBodyParts.Count - 1
				aBodyPart = Me.theMdlFileData.theBodyParts(bodyPartIndex)

				If aBodyPart.theModelCommandIsUsed Then
					Me.WriteModelCommand(aBodyPart)
					Continue For
				End If

				If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
					line = "$BodyGroup "
				Else
					line = "$bodygroup "
				End If
				line += """"
				line += aBodyPart.theName
				line += """"
				Me.theOutputFileStreamWriter.WriteLine(line)

				line = "{"
				Me.theOutputFileStreamWriter.WriteLine(line)

				If aBodyPart.theModels IsNot Nothing AndAlso aBodyPart.theModels.Count > 0 Then
					For modelIndex As Integer = 0 To aBodyPart.theModels.Count - 1
						aBodyModel = aBodyPart.theModels(modelIndex)

						line = vbTab
						If aBodyModel.theName = "blank" Then
							line += "blank"
						Else
							aBodyModel.theSmdFileNames(0) = SourceFileNamesModule.CreateBodyGroupSmdFileName(aBodyModel.theSmdFileNames(0), bodyPartIndex, modelIndex, 0, Me.theModelName, Me.theMdlFileData.theBodyParts(bodyPartIndex).theModels(modelIndex).theName)
							line += "studio "
							line += """"
							line += FileManager.GetPathFileNameWithoutExtension(aBodyModel.theSmdFileNames(0))
							line += """"
						End If
						Me.theOutputFileStreamWriter.WriteLine(line)
					Next
				End If

				line = "}"
				Me.theOutputFileStreamWriter.WriteLine(line)
			Next
		End If
	End Sub

	Public Delegate Sub WriteGroupDelegate()

	Public Sub WriteGroup(ByVal qciGroupName As String, ByVal writeGroupAction As WriteGroupDelegate, ByVal includeLineIsCommented As Boolean, ByVal includeLineIsIndented As Boolean)
		If TheApp.Settings.DecompileGroupIntoQciFilesIsChecked Then
			Dim qciFileName As String
			Dim qciPathFileName As String
			Dim mainOutputFileStream As StreamWriter

			mainOutputFileStream = Me.theOutputFileStreamWriter

			Try
				qciFileName = Me.theOutputFileNameWithoutExtension + "_" + qciGroupName + ".qci"
				qciPathFileName = Path.Combine(Me.theOutputPath, qciFileName)

				Me.theOutputFileStreamWriter = File.CreateText(qciPathFileName)

				writeGroupAction.Invoke()
			Catch ex As Exception
				Throw
			Finally
				If Me.theOutputFileStreamWriter IsNot Nothing Then
					Me.theOutputFileStreamWriter.Flush()
					Me.theOutputFileStreamWriter.Close()

					Me.theOutputFileStreamWriter = mainOutputFileStream
				End If
			End Try

			Try
				If File.Exists(qciPathFileName) Then
					Dim qciFileInfo As New FileInfo(qciPathFileName)
					If qciFileInfo.Length > 0 Then
						Dim line As String = ""

						Me.theOutputFileStreamWriter.WriteLine()

						If includeLineIsCommented Then
							line += "// "
						End If
						If includeLineIsIndented Then
							line += vbTab
						End If
						If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
							line += "$Include "
						Else
							line += "$include "
						End If
						line += """"
						line += qciFileName
						line += """"
						Me.theOutputFileStreamWriter.WriteLine(line)
					End If
				End If
			Catch ex As Exception
				Throw
			End Try
		Else
			writeGroupAction.Invoke()
		End If
	End Sub

	Public Sub WriteModelCommand(ByVal aBodyPart As SourceMdlBodyPart2531)
		Dim line As String = ""
		Dim bodyPartIndex As Integer
		Dim aBodyModel As SourceMdlModel2531
		Dim eyeballNames As List(Of String)

		If aBodyPart.theModels Is Nothing OrElse aBodyPart.theModels.Count = 0 Then
			Exit Sub
		End If

		eyeballNames = New List(Of String)()
		aBodyModel = aBodyPart.theModels(0)
		bodyPartIndex = Me.theMdlFileData.theBodyParts.IndexOf(aBodyPart)
		aBodyModel.theSmdFileNames(0) = SourceFileNamesModule.CreateBodyGroupSmdFileName(aBodyModel.theSmdFileNames(0), bodyPartIndex, 0, 0, Me.theModelName, aBodyModel.theName)

		If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
			line = "$Model "
		Else
			line = "$model "
		End If
		line += """"
		line += aBodyPart.theName
		line += """ """
		line += aBodyModel.theSmdFileNames(0)
		line += """"
		line += " {"
		Me.theOutputFileStreamWriter.WriteLine(line)

		If aBodyModel.theEyeballs IsNot Nothing AndAlso aBodyModel.theEyeballs.Count > 0 Then
			Me.WriteEyeballLines(aBodyModel, eyeballNames)
			Me.WriteEyelidLines(aBodyPart, aBodyModel, eyeballNames)
		End If

		If bodyPartIndex = 0 Then
			Me.WriteMouthLines()
		End If

		Me.theBodyPartForFlexWriting = aBodyPart
		Me.WriteGroup("flex", AddressOf WriteGroupFlex, False, True)

		line = "}"
		Me.theOutputFileStreamWriter.WriteLine(line)
	End Sub

	Private Sub WriteEyeballLines(ByVal aBodyModel As SourceMdlModel2531, ByRef eyeballNames As List(Of String))
		Dim line As String = ""
		Dim anEyeball As SourceMdlEyeball2531
		Dim eyeballTextureName As String
		Dim irisTextureName As String
		Dim diameter As Double
		Dim angle As Double
		Dim irisScale As Double
		Dim eyeballPosition As SourceVector

		For eyeballIndex As Integer = 0 To aBodyModel.theEyeballs.Count - 1
			anEyeball = aBodyModel.theEyeballs(eyeballIndex)

			Dim aBone As SourceMdlBone2531 = Nothing
			If anEyeball.boneIndex >= 0 AndAlso anEyeball.boneIndex < Me.theMdlFileData.theBones.Count Then
				aBone = Me.theMdlFileData.theBones(anEyeball.boneIndex)
			End If

			If aBone IsNot Nothing Then
				eyeballPosition = MathModule.VectorITransform(anEyeball.org, aBone.poseToBoneColumn0, aBone.poseToBoneColumn1, aBone.poseToBoneColumn2, aBone.poseToBoneColumn3)
			Else
				eyeballPosition = anEyeball.org
			End If

			diameter = anEyeball.radius * 2
			angle = Math.Round(MathModule.RadiansToDegrees(Math.Atan(anEyeball.zOffset)), 6)
			If anEyeball.iris_scale <> 0 Then
				irisScale = 1 / anEyeball.iris_scale
			Else
				irisScale = 1
			End If

			Dim eyeballName As String
			If Not String.IsNullOrEmpty(anEyeball.theName) Then
				eyeballName = anEyeball.theName
			ElseIf eyeballIndex = 0 AndAlso angle > 0 Then
				eyeballName = "righteye"
			ElseIf eyeballIndex = 1 AndAlso angle < 0 Then
				eyeballName = "lefteye"
			ElseIf eyeballIndex = 0 Then
				eyeballName = "righteye"
			ElseIf eyeballIndex = 1 Then
				eyeballName = "lefteye"
			Else
				eyeballName = "eye_" + eyeballIndex.ToString(TheApp.InternalNumberFormat)
			End If
			eyeballNames.Add(eyeballName)

			' Resolve textures
			eyeballTextureName = ""
			irisTextureName = ""

			' 1. Match dedicated eyeball mesh texture from model textures (e.g. Eyeball_r / Eyeball_l)
			If Me.theMdlFileData.theTextures IsNot Nothing Then
				For Each aTexture As SourceMdlTexture2531 In Me.theMdlFileData.theTextures
					Dim texLower As String = aTexture.theFileName.ToLower()
					If (eyeballIndex = 0 OrElse eyeballName.ToLower().Contains("right")) AndAlso (texLower.Contains("eyeball_r") OrElse (texLower.EndsWith("_r") AndAlso texLower.Contains("eye"))) Then
						eyeballTextureName = aTexture.theFileName
						Exit For
					ElseIf (eyeballIndex = 1 OrElse eyeballName.ToLower().Contains("left")) AndAlso (texLower.Contains("eyeball_l") OrElse (texLower.EndsWith("_l") AndAlso texLower.Contains("eye"))) Then
						eyeballTextureName = aTexture.theFileName
						Exit For
					End If
				Next
				If String.IsNullOrEmpty(eyeballTextureName) Then
					For Each aTexture As SourceMdlTexture2531 In Me.theMdlFileData.theTextures
						Dim texLower As String = aTexture.theFileName.ToLower()
						If texLower.Contains("eyeball") Then
							eyeballTextureName = aTexture.theFileName
							Exit For
						End If
					Next
				End If
			End If

			' 2. Fallback to anEyeball.texture if valid, or default name
			Dim eyeballTextureIsFallback As Boolean = False
			If String.IsNullOrEmpty(eyeballTextureName) Then
				eyeballTextureIsFallback = True
				If anEyeball.texture > 0 AndAlso anEyeball.texture < Me.theMdlFileData.theTextures.Count Then
					eyeballTextureName = Me.theMdlFileData.theTextures(anEyeball.texture).theFileName
				ElseIf eyeballIndex = 0 Then
					eyeballTextureName = "Eyeball_r"
				Else
					eyeballTextureName = "Eyeball_l"
				End If
			End If

			If anEyeball.iris_material >= 0 AndAlso anEyeball.iris_material < Me.theMdlFileData.theTextures.Count Then
				irisTextureName = Me.theMdlFileData.theTextures(anEyeball.iris_material).theFileName
			End If

			If String.IsNullOrEmpty(irisTextureName) OrElse irisTextureName = "default" Then
				If eyeballIndex = 0 Then
					irisTextureName = "Pupil_r"
				Else
					irisTextureName = "Pupil_l"
				End If
				If Me.theMdlFileData.theTextures IsNot Nothing Then
					For Each aTexture As SourceMdlTexture2531 In Me.theMdlFileData.theTextures
						If aTexture.theFileName.ToLower().Contains("pupil_r") AndAlso eyeballIndex = 0 Then
							irisTextureName = aTexture.theFileName
							Exit For
						ElseIf aTexture.theFileName.ToLower().Contains("pupil_l") AndAlso eyeballIndex = 1 Then
							irisTextureName = aTexture.theFileName
							Exit For
						ElseIf aTexture.theFileName.ToLower().Contains("pupil") Then
							irisTextureName = aTexture.theFileName
						End If
					Next
				End If
			End If

			Dim boneName As String = "Bip01 Head"
			If aBone IsNot Nothing Then
				boneName = aBone.theName
			End If

			If eyeballTextureIsFallback Then
				line = vbTab + "// WARNING: Eyeball texture not matched in model texture list; fallback used: """ + eyeballTextureName + """"
				Me.theOutputFileStreamWriter.WriteLine(line)
			End If

			line = vbTab
			line += "eyeball """
			line += eyeballName
			line += """ """
			line += boneName
			line += """ "
			line += eyeballPosition.x.ToString("0.000000", TheApp.InternalNumberFormat)
			line += " "
			line += eyeballPosition.y.ToString("0.000000", TheApp.InternalNumberFormat)
			line += " "
			line += eyeballPosition.z.ToString("0.000000", TheApp.InternalNumberFormat)
			line += " """
			line += eyeballTextureName
			line += """ "
			line += diameter.ToString("0.######", TheApp.InternalNumberFormat)
			line += " "
			line += angle.ToString("0.######", TheApp.InternalNumberFormat)
			line += " """
			line += irisTextureName
			line += """ "
			line += Math.Round(irisScale, 6).ToString("0.######", TheApp.InternalNumberFormat)
			Me.theOutputFileStreamWriter.WriteLine(line)

			If anEyeball.upperLidFlexDesc >= 0 AndAlso anEyeball.upperLidFlexDesc < Me.theMdlFileData.theFlexDescs.Count Then
				Me.theMdlFileData.theFlexDescs(anEyeball.upperLidFlexDesc).theDescIsUsedByEyelid = True
			End If
			If anEyeball.lowerLidFlexDesc >= 0 AndAlso anEyeball.lowerLidFlexDesc < Me.theMdlFileData.theFlexDescs.Count Then
				Me.theMdlFileData.theFlexDescs(anEyeball.lowerLidFlexDesc).theDescIsUsedByEyelid = True
			End If
		Next
	End Sub

	Private Sub WriteEyelidLines(ByVal aBodyPart As SourceMdlBodyPart2531, ByVal aModel As SourceMdlModel2531, ByVal eyeballNames As List(Of String))
		Dim line As String = ""
		Dim bodyPartIndex As Integer
		Dim anEyeball As SourceMdlEyeball2531
		Dim frameIndex As Integer
		Dim eyelidName As String
		Dim aFlexFrame As FlexFrame2531

		Try
			bodyPartIndex = Me.theMdlFileData.theBodyParts.IndexOf(aBodyPart)
			If aModel.theEyeballs IsNot Nothing AndAlso aModel.theEyeballs.Count > 0 AndAlso Me.theMdlFileData.theFlexFrames IsNot Nothing AndAlso Me.theMdlFileData.theFlexFrames.Count > 1 Then
				line = ""
				Me.theOutputFileStreamWriter.WriteLine(line)

				For eyeballIndex As Integer = 0 To aModel.theEyeballs.Count - 1
					anEyeball = aModel.theEyeballs(eyeballIndex)

					If anEyeball.upperLidFlexDesc >= 0 AndAlso anEyeball.upperLidFlexDesc < Me.theMdlFileData.theFlexDescs.Count Then
						eyelidName = Me.theMdlFileData.theFlexDescs(anEyeball.upperLidFlexDesc).theName

						line = vbTab
						line += "eyelid "
						line += eyelidName
						line += " """
						line += SourceFileNamesModule.GetVtaFileName(Me.theModelName, bodyPartIndex)
						line += """ "
						line += "lowerer "

						frameIndex = 0
						For flexFrameIndex As Integer = 1 To Me.theMdlFileData.theFlexFrames.Count - 1
							aFlexFrame = Me.theMdlFileData.theFlexFrames(flexFrameIndex)
							If aFlexFrame.flexName = eyelidName Then
								If aFlexFrame.flexes(0).target0 = -11 Then
									frameIndex = flexFrameIndex
									Exit For
								End If
							End If
						Next
						line += frameIndex.ToString(TheApp.InternalNumberFormat)
						line += " "
						line += anEyeball.upperTarget(0).ToString("0.##", TheApp.InternalNumberFormat)
						line += " neutral 0 "
						line += anEyeball.upperTarget(1).ToString("0.##", TheApp.InternalNumberFormat)
						line += " raiser "

						frameIndex = 0
						For flexFrameIndex As Integer = 1 To Me.theMdlFileData.theFlexFrames.Count - 1
							aFlexFrame = Me.theMdlFileData.theFlexFrames(flexFrameIndex)
							If aFlexFrame.flexName = eyelidName Then
								If aFlexFrame.flexes(0).target3 = 11 Then
									frameIndex = flexFrameIndex
									Exit For
								End If
							End If
						Next
						line += frameIndex.ToString(TheApp.InternalNumberFormat)
						line += " "
						line += anEyeball.upperTarget(2).ToString("0.##", TheApp.InternalNumberFormat)
						line += " split "
						line += Me.GetSplitNumber(eyelidName)
						line += " eyeball """
						If eyeballIndex < eyeballNames.Count Then
							line += eyeballNames(eyeballIndex)
						Else
							line += "eye_" + eyeballIndex.ToString(TheApp.InternalNumberFormat)
						End If
						line += """"
						Me.theOutputFileStreamWriter.WriteLine(line)

						Me.theMdlFileData.theFlexDescs(anEyeball.upperLidFlexDesc).theDescIsUsedByFlex = True
						If anEyeball.upperFlexDesc(0) >= 0 AndAlso anEyeball.upperFlexDesc(0) < Me.theMdlFileData.theFlexDescs.Count Then
							Me.theMdlFileData.theFlexDescs(anEyeball.upperFlexDesc(0)).theDescIsUsedByFlex = True
						End If
						If anEyeball.upperFlexDesc(1) >= 0 AndAlso anEyeball.upperFlexDesc(1) < Me.theMdlFileData.theFlexDescs.Count Then
							Me.theMdlFileData.theFlexDescs(anEyeball.upperFlexDesc(1)).theDescIsUsedByFlex = True
						End If
						If anEyeball.upperFlexDesc(2) >= 0 AndAlso anEyeball.upperFlexDesc(2) < Me.theMdlFileData.theFlexDescs.Count Then
							Me.theMdlFileData.theFlexDescs(anEyeball.upperFlexDesc(2)).theDescIsUsedByFlex = True
						End If
					End If

					If anEyeball.lowerLidFlexDesc >= 0 AndAlso anEyeball.lowerLidFlexDesc < Me.theMdlFileData.theFlexDescs.Count Then
						eyelidName = Me.theMdlFileData.theFlexDescs(anEyeball.lowerLidFlexDesc).theName

						line = vbTab
						line += "eyelid "
						line += eyelidName
						line += " """
						line += SourceFileNamesModule.GetVtaFileName(Me.theModelName, bodyPartIndex)
						line += """ "
						line += "lowerer "

						frameIndex = 0
						For flexFrameIndex As Integer = 1 To Me.theMdlFileData.theFlexFrames.Count - 1
							aFlexFrame = Me.theMdlFileData.theFlexFrames(flexFrameIndex)
							If aFlexFrame.flexName = eyelidName Then
								If aFlexFrame.flexes(0).target0 = -11 Then
									frameIndex = flexFrameIndex
									Exit For
								End If
							End If
						Next
						line += frameIndex.ToString(TheApp.InternalNumberFormat)
						line += " "
						line += anEyeball.lowerTarget(0).ToString("0.##", TheApp.InternalNumberFormat)
						line += " neutral 0 "
						line += anEyeball.lowerTarget(1).ToString("0.##", TheApp.InternalNumberFormat)
						line += " raiser "

						frameIndex = 0
						For flexFrameIndex As Integer = 1 To Me.theMdlFileData.theFlexFrames.Count - 1
							aFlexFrame = Me.theMdlFileData.theFlexFrames(flexFrameIndex)
							If aFlexFrame.flexName = eyelidName Then
								If aFlexFrame.flexes(0).target3 = 11 Then
									frameIndex = flexFrameIndex
									Exit For
								End If
							End If
						Next
						line += frameIndex.ToString(TheApp.InternalNumberFormat)
						line += " "
						line += anEyeball.lowerTarget(2).ToString("0.##", TheApp.InternalNumberFormat)
						line += " split "
						line += Me.GetSplitNumber(eyelidName)
						line += " eyeball """
						If eyeballIndex < eyeballNames.Count Then
							line += eyeballNames(eyeballIndex)
						Else
							line += "eye_" + eyeballIndex.ToString(TheApp.InternalNumberFormat)
						End If
						line += """"
						Me.theOutputFileStreamWriter.WriteLine(line)

						Me.theMdlFileData.theFlexDescs(anEyeball.lowerLidFlexDesc).theDescIsUsedByFlex = True
						If anEyeball.lowerFlexDesc(0) >= 0 AndAlso anEyeball.lowerFlexDesc(0) < Me.theMdlFileData.theFlexDescs.Count Then
							Me.theMdlFileData.theFlexDescs(anEyeball.lowerFlexDesc(0)).theDescIsUsedByFlex = True
						End If
						If anEyeball.lowerFlexDesc(1) >= 0 AndAlso anEyeball.lowerFlexDesc(1) < Me.theMdlFileData.theFlexDescs.Count Then
							Me.theMdlFileData.theFlexDescs(anEyeball.lowerFlexDesc(1)).theDescIsUsedByFlex = True
						End If
						If anEyeball.lowerFlexDesc(2) >= 0 AndAlso anEyeball.lowerFlexDesc(2) < Me.theMdlFileData.theFlexDescs.Count Then
							Me.theMdlFileData.theFlexDescs(anEyeball.lowerFlexDesc(2)).theDescIsUsedByFlex = True
						End If
					End If
				Next
			End If
		Catch ex As Exception
			Dim debug As Integer = 4242
		End Try
	End Sub

	Private Function GetSplitNumber(ByVal eyelidName As String) As String
		If eyelidName.Contains("right") Then
			Return "1"
		ElseIf eyelidName.Contains("left") Then
			Return "-1"
		Else
			Return "0"
		End If
	End Function

	Private Sub WriteMouthLines()
		Dim line As String = ""
		Dim offsetX As Double
		Dim offsetY As Double
		Dim offsetZ As Double

		If Me.theMdlFileData.theMouths IsNot Nothing AndAlso Me.theMdlFileData.theMouths.Count > 0 Then
			line = ""
			Me.theOutputFileStreamWriter.WriteLine(line)

			For i As Integer = 0 To Me.theMdlFileData.theMouths.Count - 1
				Dim aMouth As SourceMdlMouth
				aMouth = Me.theMdlFileData.theMouths(i)
				offsetX = Math.Round(aMouth.forward.x, 3)
				offsetY = Math.Round(aMouth.forward.y, 3)
				offsetZ = Math.Round(aMouth.forward.z, 3)

				line = vbTab
				line += "mouth "
				line += i.ToString(TheApp.InternalNumberFormat)
				line += " """
				If aMouth.flexDescIndex >= 0 AndAlso aMouth.flexDescIndex < Me.theMdlFileData.theFlexDescs.Count Then
					line += Me.theMdlFileData.theFlexDescs(aMouth.flexDescIndex).theName
				Else
					line += "mouth"
				End If
				line += """ """
				If aMouth.boneIndex >= 0 AndAlso aMouth.boneIndex < Me.theMdlFileData.theBones.Count Then
					line += Me.theMdlFileData.theBones(aMouth.boneIndex).theName
				Else
					line += "head"
				End If
				line += """ "
				line += offsetX.ToString("0.######", TheApp.InternalNumberFormat)
				line += " "
				line += offsetY.ToString("0.######", TheApp.InternalNumberFormat)
				line += " "
				line += offsetZ.ToString("0.######", TheApp.InternalNumberFormat)
				Me.theOutputFileStreamWriter.WriteLine(line)

				If aMouth.flexDescIndex >= 0 AndAlso aMouth.flexDescIndex < Me.theMdlFileData.theFlexDescs.Count Then
					Me.theMdlFileData.theFlexDescs(aMouth.flexDescIndex).theDescIsUsedByFlex = True
				End If
			Next
		End If
	End Sub

	Private Sub WriteGroupFlex()
		If Me.theMdlFileData.theFlexFrames IsNot Nothing AndAlso Me.theMdlFileData.theFlexFrames.Count > 1 Then
			Me.WriteFlexLines()
			Me.WriteFlexControllerLines()
			Me.WriteFlexRuleLines()
		End If
	End Sub

	Private Sub WriteFlexLines()
		Dim line As String = ""
		Dim bodyPartIndex As Integer
		bodyPartIndex = Me.theMdlFileData.theBodyParts.IndexOf(Me.theBodyPartForFlexWriting)
		If bodyPartIndex < 0 Then
			bodyPartIndex = 0
		End If

		line = ""
		Me.theOutputFileStreamWriter.WriteLine(line)

		line = vbTab
		line += "flexfile"
		line += " """
		line += SourceFileNamesModule.GetVtaFileName(Me.theModelName, bodyPartIndex)
		line += """ "
		Me.theOutputFileStreamWriter.WriteLine(line)

		line = vbTab
		line += "{"
		Me.theOutputFileStreamWriter.WriteLine(line)

		line = vbTab
		line += vbTab
		line += "defaultflex frame 0"
		Me.theOutputFileStreamWriter.WriteLine(line)

		Dim aFlexFrame As FlexFrame2531
		For frameIndex As Integer = 1 To Me.theMdlFileData.theFlexFrames.Count - 1
			aFlexFrame = Me.theMdlFileData.theFlexFrames(frameIndex)
			line = vbTab
			line += vbTab
			If Me.theMdlFileData.theFlexDescs(aFlexFrame.flexes(0).flexDescIndex).theDescIsUsedByEyelid Then
				line += "// Already in eyelid lines: "
			End If
			line += "flex """
			line += aFlexFrame.flexName
			line += """"
			line += " frame "
			line += CStr(frameIndex)
			Me.theOutputFileStreamWriter.WriteLine(line)
		Next

		line = vbTab
		line += "}"
		Me.theOutputFileStreamWriter.WriteLine(line)
	End Sub

	Private Sub WriteFlexControllerLines()
		Dim line As String = ""

		If Me.theMdlFileData.theFlexControllers IsNot Nothing AndAlso Me.theMdlFileData.theFlexControllers.Count > 0 Then
			Dim aFlexController As SourceMdlFlexController

			line = ""
			Me.theOutputFileStreamWriter.WriteLine(line)

			For i As Integer = 0 To Me.theMdlFileData.theFlexControllers.Count - 1
				aFlexController = Me.theMdlFileData.theFlexControllers(i)

				line = vbTab
				line += "flexcontroller "
				line += aFlexController.theType
				line += " "
				line += "range "
				line += aFlexController.min.ToString("0.######", TheApp.InternalNumberFormat)
				line += " "
				line += aFlexController.max.ToString("0.######", TheApp.InternalNumberFormat)
				line += " """
				line += aFlexController.theName
				line += """"
				Me.theOutputFileStreamWriter.WriteLine(line)
			Next
		End If
	End Sub

	Private Sub WriteFlexRuleLines()
		Dim line As String = ""

		If Me.theMdlFileData.theFlexRules IsNot Nothing AndAlso Me.theMdlFileData.theFlexRules.Count > 0 Then
			Dim aFlexRule As SourceMdlFlexRule

			line = ""
			Me.theOutputFileStreamWriter.WriteLine(line)

			For i As Integer = 0 To Me.theMdlFileData.theFlexDescs.Count - 1
				Dim flexDesc As SourceMdlFlexDesc
				flexDesc = Me.theMdlFileData.theFlexDescs(i)

				If Not flexDesc.theDescIsUsedByFlex AndAlso flexDesc.theDescIsUsedByFlexRule Then
					line = vbTab
					line += "localvar "
					line += flexDesc.theName
					Me.theOutputFileStreamWriter.WriteLine(line)
				End If
			Next

			For i As Integer = 0 To Me.theMdlFileData.theFlexRules.Count - 1
				aFlexRule = Me.theMdlFileData.theFlexRules(i)
				line = Common.GetFlexRule(Me.theMdlFileData.theFlexDescs, Me.theMdlFileData.theFlexControllers, aFlexRule)
				Me.theOutputFileStreamWriter.WriteLine(line)
			Next
		End If
	End Sub

	Public Sub WriteCBoxCommand()
		Dim line As String = ""
		Dim minX As Double
		Dim minY As Double
		Dim minZ As Double
		Dim maxX As Double
		Dim maxY As Double
		Dim maxZ As Double

		Me.theOutputFileStreamWriter.WriteLine()

		If TheApp.Settings.DecompileDebugInfoFilesIsChecked Then
			line = "// Clipping box or view bounding box."
			Me.theOutputFileStreamWriter.WriteLine(line)
		End If

		'FROM: VDC wiki: 
		'$cbox <float|minx> <float|miny> <float|minz> <float|maxx> <float|maxy> <float|maxz> 
		minX = Math.Round(Me.theMdlFileData.viewBoundingBoxMinPosition.x, 3)
		minY = Math.Round(Me.theMdlFileData.viewBoundingBoxMinPosition.y, 3)
		minZ = Math.Round(Me.theMdlFileData.viewBoundingBoxMinPosition.z, 3)
		maxX = Math.Round(Me.theMdlFileData.viewBoundingBoxMaxPosition.x, 3)
		maxY = Math.Round(Me.theMdlFileData.viewBoundingBoxMaxPosition.y, 3)
		maxZ = Math.Round(Me.theMdlFileData.viewBoundingBoxMaxPosition.z, 3)

		If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
			line = "$CBox "
		Else
			line = "$cbox "
		End If
		line += minX.ToString("0.######", TheApp.InternalNumberFormat)
		line += " "
		line += minY.ToString("0.######", TheApp.InternalNumberFormat)
		line += " "
		line += minZ.ToString("0.######", TheApp.InternalNumberFormat)
		line += " "
		line += maxX.ToString("0.######", TheApp.InternalNumberFormat)
		line += " "
		line += maxY.ToString("0.######", TheApp.InternalNumberFormat)
		line += " "
		line += maxZ.ToString("0.######", TheApp.InternalNumberFormat)

		Me.theOutputFileStreamWriter.WriteLine(line)
	End Sub

	Public Sub WriteCdMaterialsCommand()
		Dim line As String = ""

		If Me.theMdlFileData.theTexturePaths IsNot Nothing Then
			line = ""
			Me.theOutputFileStreamWriter.WriteLine(line)

			For i As Integer = 0 To Me.theMdlFileData.theTexturePaths.Count - 1
				Dim aTexturePath As String
				aTexturePath = Me.theMdlFileData.theTexturePaths(i)
				'NOTE: Write out null or empty strings, because Crowbar should show what was stored.
				If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
					line = "$CDMaterials "
				Else
					line = "$cdmaterials "
				End If
				line += """"
				line += aTexturePath
				line += """"
				Me.theOutputFileStreamWriter.WriteLine(line)
			Next
		End If
	End Sub

	Public Sub WriteControllerCommand()
		'$controller mouth "jaw" X 0 20
		'$controller 0 "tracker" LYR -1 1
		If Me.theMdlFileData.theBoneControllers IsNot Nothing Then
			Try
				If Me.theMdlFileData.theBoneControllers.Count > 0 Then
					Me.theOutputFileStreamWriter.WriteLine()
				End If

				Dim boneController As SourceMdlBoneController2531
				Dim line As String = ""
				For i As Integer = 0 To Me.theMdlFileData.theBoneControllers.Count - 1
					boneController = Me.theMdlFileData.theBoneControllers(i)

					If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
						line = "$Controller "
					Else
						line = "$controller "
					End If
					If boneController.inputField = 4 Then
						line += "Mouth"
					Else
						line += boneController.inputField.ToString(TheApp.InternalNumberFormat)
					End If
					line += " """
					line += Me.theMdlFileData.theBones(boneController.boneIndex).theName
					line += """ "
					line += SourceModule2531.GetControlText(boneController.type)
					line += " "
					line += boneController.startAngleDegrees.ToString("0.######", TheApp.InternalNumberFormat)
					line += " "
					line += boneController.endAngleDegrees.ToString("0.######", TheApp.InternalNumberFormat)
					Me.theOutputFileStreamWriter.WriteLine(line)
				Next
			Catch ex As Exception
				Dim debug As Integer = 4242
			End Try
		End If
	End Sub

	Public Sub WriteEyePositionCommand()
		Dim line As String = ""
		Dim offsetX As Double
		Dim offsetY As Double
		Dim offsetZ As Double

		offsetX = Math.Round(Me.theMdlFileData.eyePosition.y, 3)
		offsetY = -Math.Round(Me.theMdlFileData.eyePosition.x, 3)
		offsetZ = Math.Round(Me.theMdlFileData.eyePosition.z, 3)

		If offsetX = 0 AndAlso offsetY = 0 AndAlso offsetZ = 0 Then
			Exit Sub
		End If

		line = ""
		Me.theOutputFileStreamWriter.WriteLine(line)

		If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
			line = "$EyePosition "
		Else
			line = "$eyeposition "
		End If
		line += offsetX.ToString("0.######", TheApp.InternalNumberFormat)
		line += " "
		line += offsetY.ToString("0.######", TheApp.InternalNumberFormat)
		line += " "
		line += offsetZ.ToString("0.######", TheApp.InternalNumberFormat)
		Me.theOutputFileStreamWriter.WriteLine(line)
	End Sub

	Public Sub WriteIllumPositionCommand()
		Dim line As String
		Dim offsetX As Double
		Dim offsetY As Double
		Dim offsetZ As Double

		offsetX = Math.Round(Me.theMdlFileData.illuminationPosition.y, 3)
		offsetY = -Math.Round(Me.theMdlFileData.illuminationPosition.x, 3)
		offsetZ = Math.Round(Me.theMdlFileData.illuminationPosition.z, 3)

		line = ""
		Me.theOutputFileStreamWriter.WriteLine(line)

		line = ""
		If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
			line += "$IllumPosition "
		Else
			line += "$illumposition "
		End If
		line += offsetX.ToString("0.######", TheApp.InternalNumberFormat)
		line += " "
		line += offsetY.ToString("0.######", TheApp.InternalNumberFormat)
		line += " "
		line += offsetZ.ToString("0.######", TheApp.InternalNumberFormat)
		Me.theOutputFileStreamWriter.WriteLine(line)
	End Sub

	'Public Sub WriteFlagsCommand()
	'	Dim line As String = ""

	'	Me.theOutputFileStreamWriter.WriteLine()

	'If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
	'	line = "$Flags "
	'Else
	'	line = "$flags "
	'End If
	'	line += Me.theMdlFileData.flags.ToString(TheApp.InternalNumberFormat)
	'	Me.theOutputFileStreamWriter.WriteLine(line)
	'End Sub

	Public Sub WriteHBoxRelatedCommands()
		Dim line As String = ""
		Dim commentTag As String = ""
		Dim hitBoxWasAutoGenerated As Boolean = False
		Dim skipBoneInBBoxCommandWasUsed As Boolean = False

		If Me.theMdlFileData.theHitboxSets.Count < 1 Then
			Exit Sub
		End If

		hitBoxWasAutoGenerated = (Me.theMdlFileData.flags And SourceMdlFileData.STUDIOHDR_FLAGS_AUTOGENERATED_HITBOX) > 0
		If hitBoxWasAutoGenerated AndAlso Not TheApp.Settings.DecompileDebugInfoFilesIsChecked Then
			Exit Sub
		End If

		Me.theOutputFileStreamWriter.WriteLine()

		If TheApp.Settings.DecompileDebugInfoFilesIsChecked Then
			line = "// Hitbox info. Used for damage-based collision."
			Me.theOutputFileStreamWriter.WriteLine(line)
		End If

		If hitBoxWasAutoGenerated Then
			line = "// The hitbox info below was automatically generated when compiled because no hitbox info was provided."
			Me.theOutputFileStreamWriter.WriteLine(line)

			'NOTE: Only comment-out the hbox lines if auto-generated.
			commentTag = "// "
		End If

		Dim aHitboxSet As SourceMdlHitboxSet2531
		For i As Integer = 0 To Me.theMdlFileData.theHitboxSets.Count - 1
			aHitboxSet = Me.theMdlFileData.theHitboxSets(i)

			If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
				line = "$HBoxSet "
			Else
				line = "$hboxset "
			End If
			line += """"
			line += aHitboxSet.theName
			line += """"
			Me.theOutputFileStreamWriter.WriteLine(commentTag + line)

			If aHitboxSet.theHitboxes Is Nothing Then
				Continue For
			End If

			Me.WriteHBoxCommands(aHitboxSet.theHitboxes, commentTag, aHitboxSet.theName, skipBoneInBBoxCommandWasUsed)
		Next
	End Sub

	Private Sub WriteHBoxCommands(ByVal theHitboxes As List(Of SourceMdlHitbox2531), ByVal commentTag As String, ByVal hitboxSetName As String, ByRef theSkipBoneInBBoxCommandWasUsed As Boolean)
		Dim line As String = ""
		Dim aHitbox As SourceMdlHitbox2531

		For j As Integer = 0 To theHitboxes.Count - 1
			aHitbox = theHitboxes(j)
			If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
				line = "$HBox "
			Else
				line = "$hbox "
			End If
			line += aHitbox.groupIndex.ToString(TheApp.InternalNumberFormat)
			line += " "
			line += """"
			line += Me.theMdlFileData.theBones(aHitbox.boneIndex).theName
			line += """"
			line += " "
			line += aHitbox.boundingBoxMin.x.ToString("0.######", TheApp.InternalNumberFormat)
			line += " "
			line += aHitbox.boundingBoxMin.y.ToString("0.######", TheApp.InternalNumberFormat)
			line += " "
			line += aHitbox.boundingBoxMin.z.ToString("0.######", TheApp.InternalNumberFormat)
			line += " "
			line += aHitbox.boundingBoxMax.x.ToString("0.######", TheApp.InternalNumberFormat)
			line += " "
			line += aHitbox.boundingBoxMax.y.ToString("0.######", TheApp.InternalNumberFormat)
			line += " "
			line += aHitbox.boundingBoxMax.z.ToString("0.######", TheApp.InternalNumberFormat)
			Me.theOutputFileStreamWriter.WriteLine(commentTag + line)

			If Not theSkipBoneInBBoxCommandWasUsed Then
				If aHitbox.boundingBoxMin.x > 0 _
				 OrElse aHitbox.boundingBoxMin.y > 0 _
				 OrElse aHitbox.boundingBoxMin.z > 0 _
				 OrElse aHitbox.boundingBoxMax.x < 0 _
				 OrElse aHitbox.boundingBoxMax.y < 0 _
				 OrElse aHitbox.boundingBoxMax.z < 0 _
				 Then
					theSkipBoneInBBoxCommandWasUsed = True
				End If
			End If
		Next
	End Sub

	Public Sub WriteIncludeModelCommands()
		Dim line As String = ""

		If Me.theMdlFileData.theIncludeModels IsNot Nothing Then
			line = ""
			Me.theOutputFileStreamWriter.WriteLine(line)

			For i As Integer = 0 To Me.theMdlFileData.theIncludeModels.Count - 1
				Dim anIncludeModel As SourceMdlIncludeModel2531
				anIncludeModel = Me.theMdlFileData.theIncludeModels(i)

				If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
					line = "$IncludeModel "
				Else
					line = "$includemodel "
				End If
				line += """"
				If anIncludeModel.theFileName.StartsWith("models/") Then
					line += anIncludeModel.theFileName.Substring(7)
				Else
					line += anIncludeModel.theFileName
				End If
				line += """"
				Me.theOutputFileStreamWriter.WriteLine(line)
			Next
		End If
	End Sub

	Public Sub WriteLodCommand()
		Dim line As String = ""

		'NOTE: Data is from VTX file.
		If Me.theVtxFileData IsNot Nothing AndAlso Me.theMdlFileData.theBodyParts IsNot Nothing Then
			If Me.theVtxFileData.theVtxBodyParts Is Nothing Then
				Return
			End If
			If Me.theVtxFileData.theVtxBodyParts(0).theVtxModels Is Nothing Then
				Return
			End If
			If Me.theVtxFileData.theVtxBodyParts(0).theVtxModels(0).theVtxModelLods Is Nothing Then
				Return
			End If

			Dim aBodyPart As SourceVtxBodyPart107
			Dim aVtxModel As SourceVtxModel107
			Dim aBodyModel As SourceMdlModel2531
			Dim aLodQcInfo As LodQcInfo
			Dim aLodQcInfoList As List(Of LodQcInfo)
			Dim aLodList As SortedList(Of Double, List(Of LodQcInfo))
			Dim switchPoint As Double

			aLodList = New SortedList(Of Double, List(Of LodQcInfo))()
			For bodyPartIndex As Integer = 0 To Me.theVtxFileData.theVtxBodyParts.Count - 1
				aBodyPart = Me.theVtxFileData.theVtxBodyParts(bodyPartIndex)

				If aBodyPart.theVtxModels IsNot Nothing Then
					For modelIndex As Integer = 0 To aBodyPart.theVtxModels.Count - 1
						aVtxModel = aBodyPart.theVtxModels(modelIndex)

						If aVtxModel.theVtxModelLods IsNot Nothing Then
							aBodyModel = Me.theMdlFileData.theBodyParts(bodyPartIndex).theModels(modelIndex)
							'If aModel.name(0) = ChrW(0) Then
							'	Continue For
							'End If

							'NOTE: Start loop at 1 to skip first LOD, which isn't needed for the $lod command.
							For lodIndex As Integer = 1 To Me.theVtxFileData.lodCount - 1
								'TODO: Why would this count be different than the file header count?
								If lodIndex >= aVtxModel.theVtxModelLods.Count Then
									Exit For
								End If

								switchPoint = aVtxModel.theVtxModelLods(lodIndex).switchPoint
								If Not aLodList.ContainsKey(switchPoint) Then
									aLodQcInfoList = New List(Of LodQcInfo)()
									aLodList.Add(switchPoint, aLodQcInfoList)
								Else
									aLodQcInfoList = aLodList(switchPoint)
								End If

								aBodyModel.theSmdFileNames(0) = SourceFileNamesModule.CreateBodyGroupSmdFileName(aBodyModel.theSmdFileNames(0), bodyPartIndex, modelIndex, 0, Me.theModelName, Me.theMdlFileData.theBodyParts(bodyPartIndex).theModels(modelIndex).name)
								aBodyModel.theSmdFileNames(lodIndex) = SourceFileNamesModule.CreateBodyGroupSmdFileName(aBodyModel.theSmdFileNames(lodIndex), bodyPartIndex, modelIndex, lodIndex, Me.theModelName, Me.theMdlFileData.theBodyParts(bodyPartIndex).theModels(modelIndex).name)
								aLodQcInfo = New LodQcInfo()
								aLodQcInfo.referenceFileName = aBodyModel.theSmdFileNames(0)
								aLodQcInfo.lodFileName = aBodyModel.theSmdFileNames(lodIndex)
								aLodQcInfoList.Add(aLodQcInfo)
							Next
						End If
					Next
				End If
			Next

			line = ""
			Me.theOutputFileStreamWriter.WriteLine(line)

			Dim lodQcInfoListOfShadowLod As List(Of LodQcInfo)
			lodQcInfoListOfShadowLod = Nothing

			For lodListIndex As Integer = 0 To aLodList.Count - 1
				switchPoint = aLodList.Keys(lodListIndex)
				If switchPoint = -1 Then
					' Skip writing $shadowlod. Write it last after this loop.
					lodQcInfoListOfShadowLod = aLodList.Values(lodListIndex)
					Continue For
				End If

				aLodQcInfoList = aLodList.Values(lodListIndex)

				If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
					line = "$LOD "
				Else
					line = "$lod "
				End If
				line += switchPoint.ToString("0.######", TheApp.InternalNumberFormat)
				Me.theOutputFileStreamWriter.WriteLine(line)

				line = "{"
				Me.theOutputFileStreamWriter.WriteLine(line)

				For i As Integer = 0 To aLodQcInfoList.Count - 1
					aLodQcInfo = aLodQcInfoList(i)

					line = vbTab
					line += "replacemodel "
					line += """"
					line += aLodQcInfo.referenceFileName
					line += """ """
					line += aLodQcInfo.lodFileName
					line += """"
					Me.theOutputFileStreamWriter.WriteLine(line)
				Next

				line = "}"
				Me.theOutputFileStreamWriter.WriteLine(line)
			Next

			'NOTE: As a requirement for the compiler, write $shadowlod last.
			If lodQcInfoListOfShadowLod IsNot Nothing Then
				'// Shadow lod reserves -1 as switch value
				'// which uniquely identifies a shadow lod
				'newLOD.switchValue = -1.0f;
				If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
					line = "$ShadowLOD"
				Else
					line = "$shadowlod"
				End If
				Me.theOutputFileStreamWriter.WriteLine(line)

				line = "{"
				Me.theOutputFileStreamWriter.WriteLine(line)

				For i As Integer = 0 To lodQcInfoListOfShadowLod.Count - 1
					aLodQcInfo = lodQcInfoListOfShadowLod(i)

					line = vbTab
					line += "replacemodel "
					line += """"
					line += aLodQcInfo.referenceFileName
					line += """ """
					line += aLodQcInfo.lodFileName
					line += """"
					Me.theOutputFileStreamWriter.WriteLine(line)
				Next

				line = "}"
				Me.theOutputFileStreamWriter.WriteLine(line)
			End If
		End If
	End Sub

	Public Sub WriteProceduralBonesCommand()
		If Me.theMdlFileData.theProceduralBonesCommandIsUsed Then
			Me.theOutputFileStreamWriter.WriteLine()

			Dim line As String = ""
			If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
				line = "$ProceduralBones "
			Else
				line = "$proceduralbones "
			End If
			line += """"
			line += SourceFileNamesModule.GetVrdFileName(Me.theModelName, 2531)
			line += """"
			Me.theOutputFileStreamWriter.WriteLine(line)
		End If
	End Sub

	Public Sub WriteModelNameCommand()
		Dim line As String = ""
		Dim modelPathFileName As String

		modelPathFileName = Me.theMdlFileData.theModelName

		Me.theOutputFileStreamWriter.WriteLine()

		If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
			line = "$ModelName "
		Else
			line = "$modelname "
		End If
		line += """"
		line += modelPathFileName
		line += """"
		Me.theOutputFileStreamWriter.WriteLine(line)
	End Sub

	'Public Sub WriteSequenceGroupCommands()
	'	Dim line As String = ""
	'	Dim aSequenceGroup As SourceMdlSequenceGroup2531

	'	If Me.theMdlFileData.theSequenceGroups IsNot Nothing AndAlso Me.theMdlFileData.theSequenceGroups.Count > 1 Then
	'		Me.theOutputFileStreamWriter.WriteLine()

	'		For sequenceGroupIndex As Integer = 0 To Me.theMdlFileData.theSequenceGroups.Count - 1
	'			aSequenceGroup = Me.theMdlFileData.theSequenceGroups(sequenceGroupIndex)

	'			If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
	'				line = "$SequenceGroup "
	'			Else
	'				line = "$sequencegroup "
	'			End If
	'			line += """"
	'			line += aSequenceGroup.theName
	'			line += """"

	'			Me.theOutputFileStreamWriter.WriteLine(line)
	'		Next
	'	End If
	'End Sub

	Public Sub WriteSequenceCommands()
		Dim line As String = ""
		Dim aSequence As SourceMdlSequenceDesc2531

		If Me.theMdlFileData.theSequences IsNot Nothing AndAlso Me.theMdlFileData.theSequences.Count > 0 Then
			Me.theOutputFileStreamWriter.WriteLine()

			For sequenceIndex As Integer = 0 To Me.theMdlFileData.theSequences.Count - 1
				aSequence = Me.theMdlFileData.theSequences(sequenceIndex)

				If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
					line = "$Sequence "
				Else
					line = "$sequence "
				End If
				line += """"
				line += aSequence.theName
				line += """"
				'NOTE: Opening brace must be on same line as the command.
				line += " {"
				Me.theOutputFileStreamWriter.WriteLine(line)

				Me.WriteSequenceOptions(aSequence)

				line = "}"
				Me.theOutputFileStreamWriter.WriteLine(line)
			Next
		End If
	End Sub

	Public Sub WriteStaticPropCommand()
		Dim line As String = ""

		'$staticprop
		If (Me.theMdlFileData.flags And SourceMdlFileData.STUDIOHDR_FLAGS_STATIC_PROP) > 0 Then
			Me.theOutputFileStreamWriter.WriteLine()

			If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
				line = "$StaticProp"
			Else
				line = "$staticprop"
			End If
			Me.theOutputFileStreamWriter.WriteLine(line)
		End If
	End Sub

	Public Sub WriteSurfacePropCommand()
		Dim line As String = ""

		If Me.theMdlFileData.theSurfacePropName <> "" Then
			line = ""
			Me.theOutputFileStreamWriter.WriteLine(line)

			'$surfaceprop "flesh"
			If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
				line = "$SurfaceProp "
			Else
				line = "$surfaceprop "
			End If
			line += """"
			line += Me.theMdlFileData.theSurfacePropName
			line += """"
			Me.theOutputFileStreamWriter.WriteLine(line)
		End If
	End Sub

	Public Sub WriteTextureGroupCommand()
		Dim line As String = ""

		If Me.theMdlFileData.theSkinFamilies IsNot Nothing AndAlso Me.theMdlFileData.theSkinFamilies.Count > 0 AndAlso Me.theMdlFileData.theTextures IsNot Nothing AndAlso Me.theMdlFileData.theTextures.Count > 0 AndAlso Me.theMdlFileData.skinReferenceCount > 0 Then
			Dim processedSkinFamilies As List(Of List(Of Short))
			If TheApp.Settings.DecompileQcOnlyChangedMaterialsInTextureGroupLinesIsChecked Then
				processedSkinFamilies = Me.GetSkinFamiliesOfChangedMaterials(Me.theMdlFileData.theSkinFamilies)
			Else
				processedSkinFamilies = Me.theMdlFileData.theSkinFamilies
			End If

			Dim skinFamiliesOfTextureFileNames As List(Of List(Of String))
			skinFamiliesOfTextureFileNames = New List(Of List(Of String))(processedSkinFamilies.Count)
			Dim skinReferenceCount As Integer
			skinReferenceCount = processedSkinFamilies(0).Count
			For i As Integer = 0 To processedSkinFamilies.Count - 1
				Dim aSkinFamily As List(Of Short)
				aSkinFamily = processedSkinFamilies(i)

				Dim textureFileNames As New List(Of String)(skinReferenceCount)
				For j As Integer = 0 To skinReferenceCount - 1
					Dim aTexture As SourceMdlTexture2531
					aTexture = Me.theMdlFileData.theTextures(aSkinFamily(j))

					textureFileNames.Add(aTexture.theFileName)
				Next

				skinFamiliesOfTextureFileNames.Add(textureFileNames)
			Next

			If (Not TheApp.Settings.DecompileQcOnlyChangedMaterialsInTextureGroupLinesIsChecked) OrElse (skinFamiliesOfTextureFileNames.Count > 1) Then
				Me.theOutputFileStreamWriter.WriteLine()

				If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
					line = "$TextureGroup ""skinfamilies"""
				Else
					line = "$texturegroup ""skinfamilies"""
				End If
				Me.theOutputFileStreamWriter.WriteLine(line)
				line = "{"
				Me.theOutputFileStreamWriter.WriteLine(line)

				Dim skinFamilyLines As List(Of String)
				skinFamilyLines = Me.GetTextureGroupSkinFamilyLines(skinFamiliesOfTextureFileNames)
				For skinFamilyLineIndex As Integer = 0 To skinFamilyLines.Count - 1
					Me.theOutputFileStreamWriter.WriteLine(skinFamilyLines(skinFamilyLineIndex))
				Next

				line = "}"
				Me.theOutputFileStreamWriter.WriteLine(line)
			End If
		End If
	End Sub

	Public Sub WriteCollisionModelOrCollisionJointsCommand()
		Dim line As String = ""

		'NOTE: Data is from PHY file.
		If Me.thePhyFileData IsNot Nothing AndAlso Me.thePhyFileData.solidCount > 0 Then
			Me.theOutputFileStreamWriter.WriteLine(line)

			If Me.thePhyFileData.theSourcePhyCollisionDatas.Count = 1 Then
				If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
					line = "$CollisionModel "
				Else
					line = "$collisionmodel "
				End If
			Else
				If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
					line = "$CollisionJoints "
				Else
					line = "$collisionjoints "
				End If
			End If
			line += """"
			Me.thePhyFileData.thePhysicsMeshSmdFileName = SourceFileNamesModule.CreatePhysicsSmdFileName(Me.thePhyFileData.thePhysicsMeshSmdFileName, Me.theModelName)
			line += Me.thePhyFileData.thePhysicsMeshSmdFileName
			line += """"
			Me.theOutputFileStreamWriter.WriteLine(line)
			line = "{"
			Me.theOutputFileStreamWriter.WriteLine(line)

			Me.WriteCollisionModelOrCollisionJointsOptions()

			line = "}"
			Me.theOutputFileStreamWriter.WriteLine(line)
		End If
	End Sub

	Private Sub WriteCollisionModelOrCollisionJointsOptions()
		Dim line As String = ""

		line = vbTab
		line += "$mass "
		line += Me.thePhyFileData.theSourcePhyEditParamsSection.totalMass.ToString("0.######", TheApp.InternalNumberFormat)
		Me.theOutputFileStreamWriter.WriteLine(line)
		line = vbTab
		line += "$inertia "
		line += Me.thePhyFileData.theSourcePhyPhysCollisionModelMostUsedValues.theInertia.ToString("0.######", TheApp.InternalNumberFormat)
		Me.theOutputFileStreamWriter.WriteLine(line)
		line = vbTab
		line += "$damping "
		line += Me.thePhyFileData.theSourcePhyPhysCollisionModelMostUsedValues.theDamping.ToString("0.######", TheApp.InternalNumberFormat)
		Me.theOutputFileStreamWriter.WriteLine(line)
		line = vbTab
		line += "$rotdamping "
		line += Me.thePhyFileData.theSourcePhyPhysCollisionModelMostUsedValues.theRotDamping.ToString("0.######", TheApp.InternalNumberFormat)
		Me.theOutputFileStreamWriter.WriteLine(line)
		If Me.thePhyFileData.theSourcePhyEditParamsSection.rootName <> "" Then
			line = vbTab
			line += "$rootbone """
			line += Me.thePhyFileData.theSourcePhyEditParamsSection.rootName
			line += """"
			Me.theOutputFileStreamWriter.WriteLine(line)
		End If
		If Me.thePhyFileData.theSourcePhyEditParamsSection.concave = "1" Then
			line = vbTab
			line += "$concave"
			Me.theOutputFileStreamWriter.WriteLine(line)
			line = vbTab
			line += "$maxconvexpieces "
			line += Me.thePhyFileData.theSourcePhyMaxConvexPieces.ToString()
			Me.theOutputFileStreamWriter.WriteLine(line)
		End If
		If Me.thePhyFileData.theSourcePhyCollisionDatas.Count > 1 Then
			For Each collisionData As SourcePhyCollisionData In Me.thePhyFileData.theSourcePhyCollisionDatas
				If collisionData.theConvexMeshes.Count > 1 Then
					line = vbTab
					line += "$concaveperjoint"
					Me.theOutputFileStreamWriter.WriteLine(line)
					Exit For
				End If
			Next
		End If

		For i As Integer = 0 To Me.thePhyFileData.theSourcePhyPhysCollisionModels.Count - 1
			Dim aSourcePhysCollisionModel As SourcePhyPhysCollisionModel
			aSourcePhysCollisionModel = Me.thePhyFileData.theSourcePhyPhysCollisionModels(i)

			line = ""
			Me.theOutputFileStreamWriter.WriteLine(line)

			'If aSourcePhysCollisionModel.theDragCoefficientIsValid Then
			'End If

			If aSourcePhysCollisionModel.theMassBiasIsValid Then
				line = vbTab
				line += "$jointmassbias """
				line += aSourcePhysCollisionModel.theName
				line += """ "
				line += aSourcePhysCollisionModel.theMassBias.ToString("0.######", TheApp.InternalNumberFormat)
				Me.theOutputFileStreamWriter.WriteLine(line)
			End If

			If aSourcePhysCollisionModel.theDamping <> Me.thePhyFileData.theSourcePhyPhysCollisionModelMostUsedValues.theDamping Then
				line = vbTab
				line += "$jointdamping """
				line += aSourcePhysCollisionModel.theName
				line += """ "
				line += aSourcePhysCollisionModel.theDamping.ToString("0.######", TheApp.InternalNumberFormat)
				Me.theOutputFileStreamWriter.WriteLine(line)
			End If

			If aSourcePhysCollisionModel.theInertia <> Me.thePhyFileData.theSourcePhyPhysCollisionModelMostUsedValues.theInertia Then
				line = vbTab
				line += "$jointinertia """
				line += aSourcePhysCollisionModel.theName
				line += """ "
				line += aSourcePhysCollisionModel.theInertia.ToString("0.######", TheApp.InternalNumberFormat)
				Me.theOutputFileStreamWriter.WriteLine(line)
			End If

			If aSourcePhysCollisionModel.theRotDamping <> Me.thePhyFileData.theSourcePhyPhysCollisionModelMostUsedValues.theRotDamping Then
				line = vbTab
				line += "$jointrotdamping """
				line += aSourcePhysCollisionModel.theName
				line += """ "
				line += aSourcePhysCollisionModel.theRotDamping.ToString("0.######", TheApp.InternalNumberFormat)
				Me.theOutputFileStreamWriter.WriteLine(line)
			End If

			If Me.thePhyFileData.theSourcePhyRagdollConstraintDescs.ContainsKey(aSourcePhysCollisionModel.theIndex) Then
				Dim aConstraint As SourcePhyRagdollConstraint
				aConstraint = Me.thePhyFileData.theSourcePhyRagdollConstraintDescs(aSourcePhysCollisionModel.theIndex)
				line = vbTab
				line += "$jointconstrain """
				line += aSourcePhysCollisionModel.theName
				line += """ x limit "
				line += aConstraint.theXMin.ToString("0.######", TheApp.InternalNumberFormat)
				line += " "
				line += aConstraint.theXMax.ToString("0.######", TheApp.InternalNumberFormat)
				line += " "
				line += aConstraint.theXFriction.ToString("0.######", TheApp.InternalNumberFormat)
				Me.theOutputFileStreamWriter.WriteLine(line)
				line = vbTab
				line += "$jointconstrain """
				line += aSourcePhysCollisionModel.theName
				line += """ y limit "
				line += aConstraint.theYMin.ToString("0.######", TheApp.InternalNumberFormat)
				line += " "
				line += aConstraint.theYMax.ToString("0.######", TheApp.InternalNumberFormat)
				line += " "
				line += aConstraint.theYFriction.ToString("0.######", TheApp.InternalNumberFormat)
				Me.theOutputFileStreamWriter.WriteLine(line)
				line = vbTab
				line += "$jointconstrain """
				line += aSourcePhysCollisionModel.theName
				line += """ z limit "
				line += aConstraint.theZMin.ToString("0.######", TheApp.InternalNumberFormat)
				line += " "
				line += aConstraint.theZMax.ToString("0.######", TheApp.InternalNumberFormat)
				line += " "
				line += aConstraint.theZFriction.ToString("0.######", TheApp.InternalNumberFormat)
				Me.theOutputFileStreamWriter.WriteLine(line)
			End If
		Next

		If Not Me.thePhyFileData.theSourcePhySelfCollides Then
			line = vbTab
			line += "$noselfcollisions"
			Me.theOutputFileStreamWriter.WriteLine(line)
		Else
			For Each aSourcePhyCollisionPair As SourcePhyCollisionPair In Me.thePhyFileData.theSourcePhyCollisionPairs
				line = vbTab
				line += "$jointcollide"
				line += " "
				line += """"
				line += Me.thePhyFileData.theSourcePhyPhysCollisionModels(aSourcePhyCollisionPair.obj0).theName
				line += """"
				line += " "
				line += """"
				line += Me.thePhyFileData.theSourcePhyPhysCollisionModels(aSourcePhyCollisionPair.obj1).theName
				line += """"
				Me.theOutputFileStreamWriter.WriteLine(line)
			Next
		End If
	End Sub

	Public Sub WriteCollisionTextCommand()
		Dim line As String = ""

		Try
			If Me.thePhyFileData IsNot Nothing AndAlso Me.thePhyFileData.theSourcePhyCollisionText IsNot Nothing AndAlso Me.thePhyFileData.theSourcePhyCollisionText.Length > 0 Then
				line = ""
				Me.theOutputFileStreamWriter.WriteLine(line)

				If TheApp.Settings.DecompileQcUseMixedCaseForKeywordsIsChecked Then
					line = "$CollisionText"
				Else
					line = "$collisiontext"
				End If
				Me.theOutputFileStreamWriter.WriteLine(line)

				line = "{"
				Me.theOutputFileStreamWriter.WriteLine(line)

				Me.WriteTextLines(Me.thePhyFileData.theSourcePhyCollisionText, 1)

				line = "}"
				Me.theOutputFileStreamWriter.WriteLine(line)
			End If
		Catch ex As Exception

		End Try
	End Sub

	Private Sub WriteTextLines(ByVal text As String, ByVal indentCount As Integer)
		Dim line As String = ""
		Dim textChar As Char
		Dim startIndex As Integer
		Dim indentText As String
		Dim lineQuoteCount As Integer
		Dim lineWordCount As Integer
		Dim beforeCloseBraceText As String

		indentText = ""
		For j As Integer = 1 To indentCount
			indentText += vbTab
		Next

		startIndex = 0
		lineQuoteCount = 0
		lineWordCount = 0
		For i As Integer = 0 To text.Length - 1
			textChar = text(i)
			If textChar = "{" Then
				If i > startIndex Then
					line = indentText
					line += text.Substring(startIndex, i - startIndex)
					Me.theOutputFileStreamWriter.WriteLine(line)
				End If

				line = indentText
				line += "{"
				Me.theOutputFileStreamWriter.WriteLine(line)

				indentCount += 1
				indentText = ""
				For j As Integer = 1 To indentCount
					indentText += vbTab
				Next

				startIndex = i + 1
				lineQuoteCount = 0
			ElseIf textChar = "}" Then
				If i > startIndex Then
					beforeCloseBraceText = text.Substring(startIndex, i - startIndex).Trim()
					If beforeCloseBraceText <> "" Then
						line = indentText
						line += beforeCloseBraceText
						Me.theOutputFileStreamWriter.WriteLine(line)
					End If
				End If

				indentCount -= 1
				indentText = ""
				For j As Integer = 1 To indentCount
					indentText += vbTab
				Next

				line = indentText
				line += "}"
				Me.theOutputFileStreamWriter.WriteLine(line)

				startIndex = i + 1
				lineQuoteCount = 0
			ElseIf textChar = """" Then
				lineQuoteCount += 1
				If lineQuoteCount = 4 Then
					If i > startIndex Then
						line = indentText
						line += text.Substring(startIndex, i - startIndex + 1).Trim()
						Me.theOutputFileStreamWriter.WriteLine(line)
					End If
					startIndex = i + 1
					lineQuoteCount = 0
				End If
				'If lineQuoteCount = 2 OrElse lineQuoteCount = 4 Then
				'	lineWordCount += 1
				'End If
			ElseIf textChar = vbLf Then
				startIndex = i + 1
				lineQuoteCount = 0
			End If
		Next
	End Sub

#End Region

#Region "Private Delegates"

#End Region

#Region "Private Methods"

	Private Sub WriteSequenceOptions(ByVal aSequenceDesc As SourceMdlSequenceDesc2531)
		Dim line As String = ""
		Dim anAnimDescIndex As Integer
		Dim anAnimationDesc As SourceMdlAnimationDesc2531

		For blendIndex As Integer = 0 To aSequenceDesc.blendCount - 1
			anAnimDescIndex = aSequenceDesc.anim(blendIndex)(0)
			If anAnimDescIndex >= Me.theMdlFileData.theAnimationDescs.Count Then
				anAnimDescIndex = Me.theMdlFileData.theAnimationDescs.Count - 1
			End If
			anAnimationDesc = Me.theMdlFileData.theAnimationDescs(anAnimDescIndex)

			anAnimationDesc.theSmdRelativePathFileName = SourceFileNamesModule.CreateAnimationSmdRelativePathFileName(anAnimationDesc.theSmdRelativePathFileName, Me.theModelName, anAnimationDesc.theName)
			line = vbTab
			line += """"
			line += anAnimationDesc.theSmdRelativePathFileName
			line += """"
			Me.theOutputFileStreamWriter.WriteLine(line)
		Next

		'If aSequenceDesc.activityId > 0 Then
		'	line = vbTab
		'	line += SourceModule2531.activityMap(aSequenceDesc.activityId)
		'	line += " "
		'	line += aSequenceDesc.activityWeight.ToString(TheApp.InternalNumberFormat)
		'	Me.theOutputFileStreamWriter.WriteLine(line)
		'End If
		If Not String.IsNullOrEmpty(aSequenceDesc.theActivityName) Then
			line = vbTab
			line += "activity "
			line += """"
			line += aSequenceDesc.theActivityName
			line += """"
			line += " "
			line += aSequenceDesc.activityWeight.ToString(TheApp.InternalNumberFormat)
			Me.theOutputFileStreamWriter.WriteLine(line)
		End If

		'For i As Integer = 0 To 1
		'	If aSequenceDesc.blendType(i) <> 0 Then
		'		line = vbTab
		'		line += "blend "
		'		line += """"
		'		line += SourceModule2531.GetControlText(aSequenceDesc.blendType(i))
		'		line += """"
		'		line += " "
		'		line += aSequenceDesc.blendStart(i).ToString("0.######", TheApp.InternalNumberFormat)
		'		line += " "
		'		line += aSequenceDesc.blendEnd(i).ToString("0.######", TheApp.InternalNumberFormat)
		'		Me.theOutputFileStreamWriter.WriteLine(line)
		'	End If
		'Next

		'If aSequenceDesc.theEvents IsNot Nothing Then
		'	Dim frameIndex As Integer
		'	For j As Integer = 0 To aSequenceDesc.theEvents.Count - 1
		'		If aSequenceDesc.frameCount <= 1 Then
		'			frameIndex = 0
		'		Else
		'			frameIndex = aSequenceDesc.theEvents(j).frameIndex
		'		End If
		'		line = vbTab
		'		line += "{ "
		'		line += "event "
		'		line += aSequenceDesc.theEvents(j).eventIndex.ToString(TheApp.InternalNumberFormat)
		'		line += " "
		'		line += frameIndex.ToString(TheApp.InternalNumberFormat)
		'		If aSequenceDesc.theEvents(j).theOptions <> "" Then
		'			line += " """
		'			line += aSequenceDesc.theEvents(j).theOptions
		'			line += """"
		'		End If
		'		line += " }"
		'		Me.theOutputFileStreamWriter.WriteLine(line)
		'	Next
		'End If

		line = vbTab
		line += "fps "
		'line += Me.theMdlFileData.theAnimationDescs(aSequenceDesc.theAnimDescIndexes(0)).fps.ToString("0.######", TheApp.InternalNumberFormat)
		'NOTE: Not sure why VtMB model "character/monster/manbat/Throw_Objects/ThrowTaxi.mdl" has aSequenceDesc.anim(0) = 1 when there is only 1 animDesc.
		'      So, use this "if" block to handle the situation.
		anAnimDescIndex = aSequenceDesc.anim(0)(0)
		If anAnimDescIndex >= Me.theMdlFileData.theAnimationDescs.Count Then
			anAnimDescIndex = Me.theMdlFileData.theAnimationDescs.Count - 1
		End If
		line += Me.theMdlFileData.theAnimationDescs(anAnimDescIndex).fps.ToString("0.######", TheApp.InternalNumberFormat)
		Me.theOutputFileStreamWriter.WriteLine(line)

		'If (aSequenceDesc.flags And SourceMdlSequenceDesc2531.STUDIO_LOOPING) > 0 Then
		'	line = vbTab
		'	line += "loop"
		'	Me.theOutputFileStreamWriter.WriteLine(line)
		'End If

		'If aSequenceDesc.motiontype > 0 Then
		'	line = vbTab
		'	line += SourceModule2531.GetMultipleControlText(aSequenceDesc.motiontype)
		'	Me.theOutputFileStreamWriter.WriteLine(line)
		'End If
	End Sub

#End Region

#Region "Constants"

#End Region

#Region "Data"

	Private theOutputFileStreamWriter As StreamWriter
	Private theMdlFileData As SourceMdlFileData2531
	Private thePhyFileData As SourcePhyFileData
	Private theVtxFileData As SourceVtxFileData107
	Private theModelName As String

	Private theOutputPath As String
	Private theOutputFileNameWithoutExtension As String

	Private theBodyPartForFlexWriting As SourceMdlBodyPart2531

#End Region

End Class
