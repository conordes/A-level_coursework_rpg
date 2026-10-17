Public Class sheild
    Inherits base_block

    Private plr As player

    Private _sheild_hp_bar As bar
    Public WriteOnly Property sheild_hp_bar As bar
        Set(ByVal value As bar)
            _sheild_hp_bar = value
        End Set
    End Property

    Public sheild_use As Boolean = False
    Public loaded As Boolean = False

    Private sheild_maxhp As Integer = 0
    Private sheild_nowhp As Integer = 0

    Public alive As Boolean = False

    Public sheild_img_path As String

    Public Sub New(ByVal game As Form, ByVal playr As player, ByVal height As Integer, ByVal width As Integer, ByVal x As Integer, ByVal y As Integer, ByVal maxhp As Integer)
        MyBase.New(game, height, width, x, y)

        Me.game = game
        Me.plr = playr

        Me.sheild_maxhp = maxhp

        Me.sheild_nowhp = maxhp

    End Sub

    Public Sub add_img()

        Me.sheild_img_path = "\\cc4sjd8\2024 Intake\24026166\Visual Studio 2010\Projects\practice_courswork_platformer\invent_imgs\" & Me.plr.equipped(6) & ".png"

        Me.box.Image = New Bitmap(Me.sheild_img_path)

        Me.box.SizeMode = PictureBoxSizeMode.StretchImage

    End Sub

    Public Function collision(ByVal hit_taken As Integer, ByVal target As Object)
        'checks for a collision with the sheild to reduce hp
        If target.box.Bounds.IntersectsWith(Me.box.Bounds) Then
            Me.sheild_nowhp += hit_taken
            Return True
        End If

        Return False
    End Function

    Public Sub check_sheild()
        'checks if the sheild needs to be re-loaded
        If Not Me.loaded And Me.alive Then

            If Me.sheild_use Then

                Me.load()

                Me.add_img()

                Me.game.Controls.Add(Me.box)

                Me._sheild_hp_bar.load_all()


            ElseIf Not Me.sheild_use Then

                Me.game.Controls.Remove(Me.box)

                Me._sheild_hp_bar.destroy()


            End If
            Me.loaded = True
        End If
        '--------------------------------------


        If Me.sheild_nowhp <= 0 Then

            Me.game.Controls.Remove(Me.box)

            Me._sheild_hp_bar.destroy()

            Me.plr.equipped(6) = "0"

            Me.sheild_use = False
            Me.alive = False

        End If

        'if it is equipped it is alive
        If Me.plr.equipped(6) = "shield" Then

            Me.alive = True

        End If


    End Sub


    Public Sub update()

        Me.check_sheild()

        'move the sheild to follow te players arms and the mouse
        If Me.plr.mousex > Me.plr.x Then
            Me.x = Me.plr.rightArm.Left
            Me.y = Me.plr.rightArm.Top
        Else
            Me.x = Me.plr.leftArm.Left
            Me.y = Me.plr.leftArm.Top
        End If
        '-----------------------------------------

        Me._sheild_hp_bar.new_size(Me.sheild_nowhp / Me.sheild_maxhp)

        Me.box.Left = Me.x - 25
        Me.box.Top = Me.y


    End Sub

End Class
