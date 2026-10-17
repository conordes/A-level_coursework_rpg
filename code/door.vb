Public Class door
    Inherits base_block

    'a simple class that just stores where the door leads to

    Private _goes_to As String

    Public ReadOnly Property goes_to
        Get
            Return Me._goes_to
        End Get
    End Property

    Private Const image_path As String = "\\cc4sjd8\2024 Intake\24026166\Visual Studio 2010\Projects\practice_courswork_platformer\door_img.png"

    Public Sub New(ByVal game As Form, ByVal height As Integer, ByVal width As Integer, ByVal x As Integer, ByVal y As Integer, ByVal goes_to As String)

        MyBase.New(game, height, width, x, y)

        Dim originalImage As New Bitmap(door.image_path)

        Me.box.Image = originalImage
        Me.box.BackColor = Color.Transparent
        Me.box.SizeMode = PictureBoxSizeMode.StretchImage

        Me._goes_to = goes_to

    End Sub
End Class
