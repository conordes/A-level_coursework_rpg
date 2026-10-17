Public Class item
    Inherits base_block

    Private _name As String

    Public Property name As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Public touched As Boolean = False

    Public img_path As String
    Public image As Bitmap

    Public amount As Integer = 1
    Public amount_text As text
    Public loaded_text As Boolean = False

    Public take_away As Boolean = False

    Public Sub New(ByVal game As Form, ByVal height As Integer, ByVal width As Integer, ByVal x As Integer, ByVal y As Integer, ByVal name As String)
        MyBase.New(game, height, width, x, y)

        Me.name = name
        Me.img_path = "\\cc4sjd8\2024 Intake\24026166\Visual Studio 2010\Projects\practice_courswork_platformer\invent_imgs\" & name & ".png"
    End Sub

    Public Sub make_image()

        'makes the items on the floor and images
        Me.box.BackColor = Color.FromArgb(99, 107, 118)

        Dim orig_img As New Bitmap(Me.img_path)
        orig_img.MakeTransparent()

        Me.image = orig_img
        Me.box.Image = orig_img
        Me.box.SizeMode = PictureBoxSizeMode.StretchImage
        '---------------------------------------

    End Sub

    Public Sub show_amount()
        amount_text = New text(Me.game, 0, 0, 10, 0.ToString(), Color.White)
        amount_text.load()
    End Sub

    Public Sub destroy()
        Me.game.Controls.Remove(Me.box)
        If Me.loaded_text Then
            Me.amount_text.destroy()
        End If
    End Sub

    Public Sub item_MouseClick(ByVal sender As Object, ByVal e As EventArgs)

        'check if they've been clicked
        Me.touched = True

    End Sub

    Public Sub update()

        'check if there is text on the item and update it
        'e.g. floor items dont have text

        If Not (Me.amount_text Is Nothing) Then

            amount_text.my_text = amount.ToString()
            amount_text.box.Text = amount_text.my_text

            amount_text.box.Left = Me.x
            amount_text.box.Top = Me.y + 30


        End If
        '-----------------------------------------------

    End Sub

End Class
