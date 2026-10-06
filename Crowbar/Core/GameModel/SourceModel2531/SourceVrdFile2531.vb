Imports System.IO

Public Class SourceVrdFile2531

#Region "Creation and Destruction"

	Public Sub New(ByVal outputFileStream As StreamWriter, ByVal mdlFileData As SourceMdlFileData2531)
		Me.theOutputFileStreamWriter = outputFileStream
		Me.theMdlFileData = mdlFileData
	End Sub

#End Region

#Region "Methods"

	Public Sub WriteCommands()
		If Me.theMdlFileData.theBones IsNot Nothing Then
			Me.theOutputFileStreamWriter.WriteLine("proceduralbones")

			Dim aBone As SourceMdlBone2531
			Dim aControlBone As SourceMdlBone2531
			Dim controlBoneName As String
			Dim line As String

			For i As Integer = 0 To Me.theMdlFileData.theBones.Count - 1
				aBone = Me.theMdlFileData.theBones(i)

				If aBone.theAxisInterpBone IsNot Nothing Then
					If aBone.theAxisInterpBone.controlBoneIndex >= 0 AndAlso aBone.theAxisInterpBone.controlBoneIndex < Me.theMdlFileData.theBones.Count Then
						aControlBone = Me.theMdlFileData.theBones(aBone.theAxisInterpBone.controlBoneIndex)
						controlBoneName = aControlBone.theName
					Else
						controlBoneName = "none"
					End If

					line = "bone """ + aBone.theName + """ ""none"" """ + controlBoneName + """ ""none"" " + (aBone.theAxisInterpBone.axis + 1).ToString()
					Me.theOutputFileStreamWriter.WriteLine(line)

					Me.theOutputFileStreamWriter.WriteLine("basepos 0.000000 0.000000 0.000000")

					For j As Integer = 0 To aBone.theAxisInterpBone.pos.Length - 1
						line = "axisquat " + j.ToString()
						line += " " + aBone.theAxisInterpBone.pos(j).x.ToString("0.000000", TheApp.InternalNumberFormat)
						line += " " + aBone.theAxisInterpBone.pos(j).y.ToString("0.000000", TheApp.InternalNumberFormat)
						line += " " + aBone.theAxisInterpBone.pos(j).z.ToString("0.000000", TheApp.InternalNumberFormat)
						line += " " + aBone.theAxisInterpBone.quat(j).x.ToString("0.000000", TheApp.InternalNumberFormat)
						line += " " + aBone.theAxisInterpBone.quat(j).y.ToString("0.000000", TheApp.InternalNumberFormat)
						line += " " + aBone.theAxisInterpBone.quat(j).z.ToString("0.000000", TheApp.InternalNumberFormat)
						line += " " + aBone.theAxisInterpBone.quat(j).w.ToString("0.000000", TheApp.InternalNumberFormat)
						Me.theOutputFileStreamWriter.WriteLine(line)
					Next
				End If
			Next

			Me.theOutputFileStreamWriter.WriteLine("end")
		End If
	End Sub

#End Region

#Region "Data"

	Private theOutputFileStreamWriter As StreamWriter
	Private theMdlFileData As SourceMdlFileData2531

#End Region

End Class
