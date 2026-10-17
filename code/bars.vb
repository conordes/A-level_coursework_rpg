Public Class bar
    Inherits base_block

    Private fullness As Double = 1
    Public topbox As New PictureBox

    Public bottom_col As Color
    Public top_col As Color

    Public border As Integer

    Public stat_text As text

    Public txt_alive As Boolean = False
    Public txt_size As Integer = -1

    Public bar_alive As Boolean = False

    Public Sub New(ByVal game As Form, ByVal width As Integer, ByVal height As Integer, ByVal x As Integer, ByVal y As Integer, ByVal bottom_cl As Color, ByVal top_cl As Color, Optional ByVal border As Integer = -1, Optional ByVal txt As Boolean = False, Optional ByVal txt_size As Integer = -1)
        MyBase.New(game, height, width, x, y)


        Me.bottom_col = bottom_cl
        Me.top_col = top_cl

        Me.box.BackColor = Me.bottom_col
        Me.box.Left = x
        Me.box.Top = y

        Me.border = border
        Me.txt_alive = txt
        Me.txt_size = txt_size
    End Sub

    Public Sub load_all(Optional ByVal num As Integer = 1)
        'create the picture boxes, load them in to the game
        Me.box.Size = New Size(Me.width, Me.height)

        Me.box.Top = Me.y
        Me.box.Left = Me.x

        If Me.txt_size <> -1 Then
            Me.stat_text = New text(Me.game, Me.x, Me.y + (Me.height - Me.width / 12), Me.txt_size, "", Color.Black)
        Else
            Me.stat_text = New text(Me.game, Me.x, Me.y + (Me.height - Me.width / 12), Me.width / 12, "", Color.Black)
        End If
        '------------------------------------------------

        'if a border is wanted (border=1) it will make the back box larger
        If Me.border = -1 Then
            Me.topbox.Size = New Size(Me.width, Me.height)
            Me.topbox.Top = Me.y
            Me.topbox.Left = Me.x
        Else
            Me.topbox.Size = New Size(Me.width - (Me.width * 0.16), Me.height - (Me.height * 0.4))
            Me.topbox.Top = Me.y + (Me.height * 0.2)
            Me.topbox.Left = Me.x + (Me.width * 0.08)
        End If
        '---------------------------------------------------------------
        Me.box.BringToFront()
        Me.topbox.BringToFront()

        If Me.txt_alive Then
            Me.stat_text.box.BackColor = Me.top_col
            Me.stat_text.load()
            Me.stat_text.box.Text = num
        End If

        Me.topbox.BackColor = top_col

        Me.game.Controls.Add(Me.topbox)
        Me.game.Controls.Add(Me.box)


    End Sub

    Public Sub destroy()
        ' remove the top picturebox and bottom picturebox from graphical mamory, keeps the object in memory to be used for later
        Me.game.Controls.Remove(Me.topbox)
        Me.game.Controls.Remove(Me.box)
        '------------------------------------------------

        If Me.txt_alive Then
            Me.stat_text.destroy()
        End If

    End Sub

    Public Sub reload_all(Optional ByVal num As Integer = 1)

        'reloads the object if its just been removed from graphical memory but is still stored in memory
        Me.game.Controls.Add(Me.box)
        Me.game.Controls.Add(Me.topbox)

        Me.box.BringToFront()
        Me.topbox.BringToFront()

        If Me.txt_alive Then

            Me.stat_text.box.BackColor = Me.top_col
            Me.stat_text.box.Text = num

            Me.stat_text.load()

            Me.stat_text.box.BringToFront()
        End If
        '-----------------------------------------------------------------------------------------

    End Sub

    Public Sub new_size(ByVal num As Double)
        'change the size of the topbox to make it seem as if the bar is reducing
        Me.fullness = num
        Me.topbox.Size = New Size(Me.width * (Me.fullness), Me.height)

        Me.box.BringToFront()
        Me.topbox.BringToFront()

        If Me.txt_alive Then
            If num < 0 Then num = 0

            'gets rid of long doubles
            Me.stat_text.box.Text = CInt(num * 100) / 100

            Me.stat_text.box.BringToFront()
        End If

        '-----------------------------------------------------------------------
    End Sub

    Public Sub new_loc(ByVal new_x As Integer, ByVal new_y As Integer)

        Me.x = new_x
        Me.y = new_y

        Me.stat_text.box.Location = New Point(new_x, new_y + 20)

        Me.box.Location = New Point(new_x, new_y)
        Me.topbox.Location = New Point(new_x, new_y)

    End Sub
End Class
