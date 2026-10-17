Public Class Npc
    Inherits base_enemy

    Private plr1 As player

    Public textbox_open As Boolean = False
    Public speach_mission_txt As text


    Public mission_txt_list As List(Of String)
    Public mission_stats As List(Of Integer)
    Public mission_txt As text

    Private _already_clicked As Boolean = False
    Public ReadOnly Property already_clicked As Boolean
        Get
            Return _already_clicked
        End Get
    End Property

    Private _done_mis As Boolean = False


    Public Property done_mis
        Get
            Return _done_mis
        End Get
        Set(ByVal value)
            _done_mis = value
        End Set
    End Property



    Public Sub New(ByVal game As Form, ByVal plr As player, ByVal x As Integer, ByVal y As Integer, ByVal wd As Integer, ByVal ht As Integer, ByVal mstxt As List(Of String), ByVal util As utilities)
        MyBase.new(game, ht, wd, x, y, 0, 0, util)

        Me.game = game
        Me.plr1 = plr

        Me.mission_txt_list = mstxt
        Me.box.BackColor = plr.box.BackColor

        Me.speach_mission_txt = New text(Me.game, Me.x + 20, Me.y - 20, 20, Me.mission_txt_list(0), Color.Black)
        Me.speach_mission_txt.border = True

        Me.speach_mission_txt.box.Width = Me.mission_txt_list(0).Count() * 20

    End Sub

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

    Public Function player_near()
        'player in distance
        If Math.Sqrt((Me.x - plr1.x) ^ 2 + (Me.y - plr1.y) ^ 2) <= 200 Then
            Return True
        End If

        Return False
    End Function

    Public Sub creat_text_box()
        'make sure the text box hasnt been deleted, so can still be loaded from memory
        If Not Me.textbox_open And Not Me.dead Then
            Me.speach_mission_txt.load()
        End If

        Me.textbox_open = True
    End Sub

    Public Sub click_handler(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        If e.Button = MouseButtons.Left And Not already_clicked And Me.textbox_open Then
            Me._already_clicked = True
        End If

    End Sub

    Public Sub destroy()

        Me.game.Controls.Remove(Me.box)
        Me.game.Controls.Remove(Me.eyebox)
        Me.game.Controls.Remove(Me.eye)

        For Each limb In Me.all_limbs
            Me.game.Controls.Remove(limb)
        Next

        Me.speach_mission_txt.destroy()

        Me.dead = True
    End Sub

    Public Sub del_text_box()
        'delete the text box 
        If Me.textbox_open Then

            Me.speach_mission_txt.destroy()

            Me.textbox_open = False

        End If

    End Sub

    Protected Friend Overloads Sub update()

        'checks wether the player is near and what to do about it
        If Me.player_near() And Not Me.dead Then
            Me.creat_text_box()

        ElseIf Not Me.dead Then
            Me.del_text_box()

        End If
        '--------------------------------------------------------

    End Sub

End Class
