Public Class AboutUserControl

	Private WithEvents LicenseLinkLabel As LinkLabel

#Region "Creation and Destruction"

	Public Sub New()
		' This call is required by the Windows Form Designer.
		InitializeComponent()
	End Sub

#End Region

#Region "Init and Free"

	Protected Overrides Sub Init()
		'NOTE: Customize the application's assembly information in the "Application" pane of the project 
		'    properties dialog (under the "Project" menu).

		Me.ProductNameLinkLabel.Text = My.Application.Info.ProductName
		Me.ProductNameLinkLabel.Links.Clear()
		Me.ProductNameLinkLabel.Links.Add(0, My.Application.Info.ProductName.Length(), My.Resources.About_ProductLink)

		Me.GotoSteamGroupLinkLabel.Visible = False
		Me.GotoSteamGroupLinkLabel.Links.Clear()

		Dim infoText As New System.Text.StringBuilder()
		infoText.AppendLine("Application: Crowbar-VTMB")
		infoText.AppendLine("Version: " + My.Application.Info.Version.ToString(2) + " (v" + My.Application.Info.Version.ToString() + ")")
		infoText.AppendLine("Original author: ZeqMacaw")
		infoText.AppendLine("Modified by: robsonner")
		infoText.AppendLine("License: Creative Commons Attribution-ShareAlike 3.0 Unported (CC BY-SA 3.0)")
		infoText.AppendLine(My.Application.Info.Copyright)
		Me.ProductInfoTextBox.Text = infoText.ToString()

		Dim oldBottom As Integer = Me.ProductInfoTextBox.Bottom
		If Me.ProductDescriptionTextBox IsNot Nothing AndAlso Me.ProductDescriptionTextBox.Left > Me.ProductInfoTextBox.Left Then
			Dim maxWidth As Integer = Me.ProductDescriptionTextBox.Left - Me.ProductInfoTextBox.Left - 16
			Me.ProductInfoTextBox.Width = Math.Max(Me.ProductInfoTextBox.Width, Math.Min(450, maxWidth))
		Else
			Me.ProductInfoTextBox.Width = Math.Max(Me.ProductInfoTextBox.Width, 450)
		End If
		Me.ProductInfoTextBox.Height = Math.Max(Me.ProductInfoTextBox.Height, 125)
		Dim deltaY As Integer = Me.ProductInfoTextBox.Bottom - oldBottom

		If deltaY > 0 Then
			If Me.AuthorLinkLabel.Top >= oldBottom Then
				Me.AuthorLinkLabel.Top += deltaY
			End If
			If Me.GotoSteamProfileLinkLabel.Top >= oldBottom Then
				Me.GotoSteamProfileLinkLabel.Top += deltaY
			End If
			If Me.AuthorIconButton.Top >= oldBottom Then
				Me.AuthorIconButton.Top += deltaY
			End If
			If Me.PayPalPictureBox.Top >= oldBottom Then
				Me.PayPalPictureBox.Top += deltaY
			End If
		End If

		Me.AuthorLinkLabel.Text = "Original Author: " + My.Application.Info.CompanyName
		Me.AuthorLinkLabel.Links.Clear()
		Me.AuthorLinkLabel.Links.Add(0, Me.AuthorLinkLabel.Text.Length(), My.Resources.About_AuthorLink)

		Me.GotoSteamProfileLinkLabel.Text = My.Resources.About_GotoSteamProfileText
		Me.GotoSteamProfileLinkLabel.Links.Clear()
		Me.GotoSteamProfileLinkLabel.Links.Add(0, My.Resources.About_GotoSteamProfileText.Length(), My.Resources.About_AuthorLink)

		If Me.LicenseLinkLabel Is Nothing Then
			Me.LicenseLinkLabel = New LinkLabel()
			Me.LicenseLinkLabel.Name = "LicenseLinkLabel"
			Me.LicenseLinkLabel.AutoSize = True
			Dim licenseUrl As String = "https://creativecommons.org/licenses/by-sa/3.0/deed.en_US"
			Me.LicenseLinkLabel.Text = "License: " + licenseUrl
			Me.LicenseLinkLabel.Links.Clear()
			Me.LicenseLinkLabel.Links.Add(9, licenseUrl.Length, licenseUrl)

			If Me.ProductInfoTextBox.Parent IsNot Nothing Then
				Me.ProductInfoTextBox.Parent.Controls.Add(Me.LicenseLinkLabel)
			Else
				Me.Controls.Add(Me.LicenseLinkLabel)
			End If
		End If

		Dim targetTop As Integer = Math.Max(Me.ProductInfoTextBox.Bottom, Math.Max(Me.GotoSteamProfileLinkLabel.Bottom, Me.PayPalPictureBox.Bottom)) + 8
		Me.LicenseLinkLabel.Location = New Point(Me.ProductInfoTextBox.Left, targetTop)
		Me.LicenseLinkLabel.BringToFront()

		Me.ProductDescriptionTextBox.Text = My.Resources.About_ProductDescription

		'Me.Panel1.DataBindings.Add("BackColor", TheApp.Settings, "AboutTabBackgroundColor", False, DataSourceUpdateMode.OnPropertyChanged)
	End Sub

	'Protected Overrides Sub Free()

	'End Sub

#End Region

#Region "Properties"

#End Region

#Region "Widget Event Handlers"

#End Region

#Region "Child Widget Event Handlers"

	Private Sub ProductLogoButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProductLogoButton.Click
		System.Diagnostics.Process.Start(My.Resources.About_ProductLink)
	End Sub

	Private Sub AuthorIconButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AuthorIconButton.Click
		System.Diagnostics.Process.Start(My.Resources.About_AuthorLink)
	End Sub

	Private Sub LinkLabel_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles ProductNameLinkLabel.LinkClicked, AuthorLinkLabel.LinkClicked, GotoSteamGroupLinkLabel.LinkClicked, GotoSteamProfileLinkLabel.LinkClicked, LicenseLinkLabel.LinkClicked
		Dim aLinkLabel As LinkLabel
		aLinkLabel = CType(sender, LinkLabel)

		If e.Button = Windows.Forms.MouseButtons.Left Then
			aLinkLabel.LinkVisited = True
			Dim target As String = CType(e.Link.LinkData, String)
			If Not String.IsNullOrEmpty(target) Then
				System.Diagnostics.Process.Start(target)
			End If
		ElseIf e.Button = Windows.Forms.MouseButtons.Right Then
			'TODO: Show context menu with: Copy Link, Copy Text
		End If
	End Sub

	Private Sub PayPalPictureBox_Click(sender As Object, e As EventArgs) Handles PayPalPictureBox.Click
		System.Diagnostics.Process.Start(My.Resources.About_PayPalLink)
	End Sub

#End Region

#Region "Core Event Handlers"

#End Region

#Region "Private Methods"

#End Region

#Region "Data"

#End Region

End Class
