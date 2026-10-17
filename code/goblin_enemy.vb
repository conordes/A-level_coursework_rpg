Public Class goblin_enemy
    Inherits base_enemy

    Protected Overrides Sub update_arms(ByVal plr1 As player)

        Dim centerX As Integer = x + width \ 2
        Dim centerY As Integer = y + height \ 2

        'the goblin arms follow the player
        Dim dx As Integer = plr1.x - centerX
        Dim dy As Integer = plr1.y - centerY

        Dim length As Double = Math.Sqrt(dx * dx + dy * dy)
        If length = 0 Then length = 1

        ' Normalized direction vector
        Dim nx As Double = dx / length
        Dim ny As Double = dy / length

        ' Perpendicular vector (for arm spacing)
        Dim perpX As Double = -ny
        Dim perpY As Double = nx

        Dim reach As Integer = 40
        Dim separation As Integer = 10

        ' Final positions for each arm
        Dim armBaseX As Integer = centerX + CInt(nx * reach)
        Dim armBaseY As Integer = centerY + CInt(ny * reach)

        ' Left arm
        leftArm.Left = armBaseX + CInt(perpX * separation) - leftArm.Width \ 2
        leftArm.Top = armBaseY + CInt(perpY * separation) - leftArm.Height \ 2

        ' Right arm
        rightArm.Left = armBaseX - CInt(perpX * separation) - rightArm.Width \ 2
        rightArm.Top = armBaseY - CInt(perpY * separation) - rightArm.Height \ 2

    End Sub

    Sub New(ByVal game As Form, ByVal height As Integer, ByVal width As Integer, ByVal x As Integer, ByVal y As Integer, ByVal id As Integer, ByVal hp As Integer, ByVal util As utilities)
        MyBase.New(game, height, width, x, y, id, hp, util)

        Me.type_enemy = "goblin"

        Me.box.BackColor = Color.Green
        Me.colour = Color.Green
    End Sub



End Class
