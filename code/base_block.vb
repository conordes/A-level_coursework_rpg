
Public Class base_block

    Public box As New PictureBox

    Protected game As Form

    Public x As Integer = 0
    Public y As Integer = 0

    Public xvel As Integer = 0
    Public yvel As Integer = 0

    Public width As Integer
    Public height As Integer

    Private wall_img_pth As String

    Public Sub New(ByVal game As Form, ByVal height As Integer, ByVal width As Integer, ByVal x As Integer, ByVal y As Integer)
        Me.game = game
        Me.x = x
        Me.y = y

        Me.width = width
        Me.height = height

    End Sub

    Public Overridable Sub load()
        'used by many calsses to load the image of the entitiy to the screen
        Me.box.Size = New Size(Me.width, Me.height)

        Me.box.Top = Me.y
        Me.box.Left = Me.x

        Me.game.Controls.Add(Me.box)
        '-------------------------------------------------------------------
    End Sub

End Class
