Public Class text
    Private game As Form

    Public box As New Label
    Public textbox As New TextBox

    Protected Friend x As Integer
    Protected Friend y As Integer

    Protected Friend my_text As String
    Private _size As Integer
    Public ReadOnly Property size
        Get
            Return _size
        End Get
    End Property

    Protected Friend colour As Color
    Protected Friend largetext As Boolean
    Protected Friend lines As Integer

    Public button_clicked As Boolean = False

    Private _border As Boolean = False

    Public Property border As Boolean
        Get
            Return _border
        End Get
        Set(ByVal value As Boolean)
            _border = value
        End Set
    End Property

    Public Sub New(ByVal game As Form, ByVal x As Integer, ByVal y As Integer, ByVal size As Integer, ByVal txt As String, ByVal colr As Color, Optional ByVal largetext As Boolean = False, Optional ByVal lns As Integer = 0)
        Me.game = game
        Me.x = x
        Me.y = y

        Me._size = size
        Me.my_text = txt

        'has a textbox and many words in it
        Me.largetext = largetext

        'how many lines in the textbox
        Me.lines = lns

        Me.colour = colr
    End Sub

    Public Sub load()

        If Not Me.largetext Then
            box.Text = Me.my_text
            box.Font = New Font("Arial", size, FontStyle.Bold)
            box.ForeColor = Me.colour
            box.AutoSize = True
            box.Location = New Point(Me.x, Me.y) ' Adjust position as needed
            Me.game.Controls.Add(box)


            box.BringToFront()
        Else
            Me.textbox.Text = Me.my_text
            Me.textbox.Font = New Font("Arial", size, FontStyle.Bold)
            Me.textbox.Location = New Point(Me.x, Me.y)
            Me.textbox.Size = New Size(780, Me.lines * 20)
            If Me.lines > 0 Then
                Me.textbox.ReadOnly = True
                Me.textbox.Size = New Size(780, Me.lines * 20)
            Else
                Me.textbox.Size = New Size(780, 40)
            End If

            Me.game.Controls.Add(Me.textbox)

            Me.textbox.Multiline = True

            Me.textbox.ScrollBars = ScrollBars.Vertical
            Me.textbox.WordWrap = True
            Me.textbox.BringToFront()
        End If
        Me.draw_border()
    End Sub

    Protected Friend Sub draw_border()
        'if a border is wanted, it draw a rectangle around the
        If Me.border Then

            Dim bit_map As New Bitmap(box.Width, box.Height)
            Dim g As Graphics = Graphics.FromImage(bit_map)
            Dim pen As New Pen(Color.LightGray, 5)

            g.DrawLine(pen, 0, 0, box.Width, 0)
            g.DrawLine(pen, 0, box.Height, box.Width, box.Height)
            g.DrawLine(pen, 0, 0, 0, box.Height)
            g.DrawLine(pen, box.Width, 0, box.Width, box.Height)

            box.Image = bit_map

        End If
        '----------------------------------------------------

    End Sub

    Protected Friend Sub click_handler(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        'if the text can be clicked this is where it is handled
        If e.Button = MouseButtons.Left And Not Me.button_clicked Then
            Me.button_clicked = True
        End If

    End Sub

    Protected Friend Sub destroy()

        'if it is text that can be scrolled(in the instructions menu) it is destroyed differently
        If Not Me.largetext Then
            Me.game.Controls.Remove(Me.box)
        Else
            Me.game.Controls.Remove(Me.textbox)
        End If

    End Sub

    Public Sub update(ByVal txt)
        Me.my_text = txt

        box.ForeColor = Me.colour
        box.Text = Me.my_text
    End Sub
End Class
