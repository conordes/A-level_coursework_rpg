Public Class buttono
    Private game As Form
    Public mybutton As New Button()

    Public x As Integer
    Public y As Integer

    Public width As Integer
    Public height As Integer

    'text in the button and wether it has been clicked
    Protected Friend text As String
    Protected Friend been_clicked As Boolean = False

    Public Sub New(ByVal game, ByVal x, ByVal y, ByVal width, ByVal height, ByVal txt)

        Me.x = x
        Me.y = y

        Me.width = width
        Me.height = height

        Me.text = txt
        Me.game = game


    End Sub

    Public Sub destroy()
        Me.game.Controls.Remove(Me.mybutton)
    End Sub

    Public Sub Me_OnClick(ByVal sender As Object, ByVal e As EventArgs)
        'uses functionality to check for clicks in the button picturebox
        Me.been_clicked = True
    End Sub

    Protected Friend Sub load()
        'adds the text to the button
        mybutton.Text = Me.text
        mybutton.Font = New Font("Microsoft Sans Serif", 20)

        mybutton.Size = New Size(Me.width, Me.height)
        mybutton.Location = New Point(Me.x, Me.y)

        ' Add to the form
        Me.game.Controls.Add(mybutton)
    End Sub

End Class
