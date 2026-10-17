Imports System.Drawing.Drawing2D
Imports System.Drawing

Public MustInherit Class base_enemy
    Inherits base_block

    Protected util As utilities

    Public type_enemy As String
    Public type_magic As Boolean = False

    Public colour As Color

    Protected Const see_radius As Integer = 800
    Protected sees_player As Boolean = False
    Protected see_timer As Integer = 0
    Protected plr1_see As Boolean = False

    Public dead As Boolean = False

    Protected playerx As Integer = 0
    Protected playery As Integer = 0
    Protected enemyx As Integer = 0
    Protected enemyy As Integer = 0

    Protected max_hp As Integer = 2
    Public hp As Integer = 2

    Public attack_dict As New Dictionary(Of String, List(Of String))
    Public live_attacks As New List(Of base_attack)
    Public dead_attacks As New List(Of base_attack)
    Public cubes As New List(Of base_block)

    Protected angle As Double = 0

    Protected leftLeg As New PictureBox()
    Protected rightLeg As New PictureBox()
    Protected leftArm As New PictureBox()
    Protected rightArm As New PictureBox()

    Public eye As New PictureBox()
    Public eyebox As New PictureBox()

    Protected legWidth As Integer = width \ 3
    Protected legHeight As Integer = height \ 4
    Protected armWidth As Integer = width \ 4
    Protected armHeight As Integer = height \ 3

    Public all_limbs As New List(Of PictureBox)
    Public leg_move As String = "R"

    Protected _id As Integer
    Public ReadOnly Property id As Integer
        Get
            Return _id
        End Get
    End Property

    Public raycast_obj As ray_cast
    Public rand As New Random()

    Sub New(ByVal game As Form, ByVal height As Integer, ByVal width As Integer, ByVal x As Integer, ByVal y As Integer, ByVal id As Integer, ByVal hp As Integer, ByVal utility As utilities, Optional ByVal magic As Boolean = False)
        MyBase.New(game, height, width, x, y)
        Me._id = id

        Me.max_hp = hp
        Me.hp = hp


        Me.type_magic = magic
        Me.util = utility

    End Sub



    Public Overridable Sub collisions(ByVal wallz As List(Of wall), ByVal plr As player, ByVal tick As Integer)
        'wall collisions, push back so they dont get stuck, stop movement, re-checks if they can see the player
        For Each wal In wallz
            If wal.box.Bounds.IntersectsWith(Me.box.Bounds) Then
                ' Calculate enemy and wall boundaries
                Dim enemy_left As Integer = Me.x
                Dim enemy_right As Integer = Me.x + Me.width
                Dim enemy_top As Integer = Me.y
                Dim enemy_bottom As Integer = Me.y + Me.height

                Dim wall_left As Integer = wal.x
                Dim wall_right As Integer = wal.x + wal.width
                Dim wall_top As Integer = wal.y
                Dim wall_bottom As Integer = wal.y + wal.height

                ' Determine overlap distances
                Dim overlap_left As Integer = enemy_right - wall_left
                Dim overlap_right As Integer = wall_right - enemy_left
                Dim overlap_top As Integer = enemy_bottom - wall_top
                Dim overlap_bottom As Integer = wall_bottom - enemy_top


                Dim min_overlap As Integer = Math.Min(Math.Min(overlap_left, overlap_right), Math.Min(overlap_top, overlap_bottom))

                ' Push the enemy out depending on which side is smallest
                If min_overlap = overlap_left Then

                    Me.x -= overlap_left
                    Me.xvel = 0
                ElseIf min_overlap = overlap_right Then

                    Me.x += overlap_right
                    Me.xvel = 0
                ElseIf min_overlap = overlap_top Then

                    Me.y -= overlap_top
                    Me.yvel = 0
                ElseIf min_overlap = overlap_bottom Then

                    Me.y += overlap_bottom
                    Me.yvel = 0
                End If
                Me.see_timer = 0
            End If
        Next

        '---------------------------------------------------------------------------------------------------------

        'check collisions with attacks from the player
        For Each i In plr.live_attack

            If i.box.Bounds.IntersectsWith(Me.box.Bounds) Then

                Me.box.BackColor = Color.LightGray
                Me.hp -= i.damage

                If plr.attack_dict(i.typeofattack)(3) = 1 Then
                    i.dead = True

                End If

            End If

        Next
        '-----------------------------------------------

    End Sub

    Private Sub move_to_plr(ByVal plr As player, ByVal speed As Integer)
        'if the player can be seen move to them
        Dim dx As Integer = plr.x - Me.x
        Dim dy As Integer = plr.y - Me.y
        '--------------------------------------

        ' find distance
        Dim distance As Double = Math.Sqrt(dx * dx + dy * dy)
        If distance = 0 Then Exit Sub
        '-------------

        'get direction needed then multiply by speed
        Dim stepX As Integer = CInt((dx / distance) * speed)
        Dim stepY As Integer = CInt((dy / distance) * speed)
        '-------------------------------------------

        Me.x += stepX
        Me.y += stepY
    End Sub

    Protected Friend Sub limb_load()
        'load the limbs of the enemy
        'give the limb picturebox the desired attributes of each limb
        With Me.leftLeg

            .Width = legWidth
            .Height = legHeight

            .Left = x
            .Top = y + height

            .BackColor = Color.Black
        End With

        With Me.rightLeg

            .Width = legWidth
            .Height = legHeight

            .Left = x + (width - legWidth)
            .Top = y + height

            .BackColor = Color.Black
        End With

        With Me.leftArm

            .Width = armWidth
            .Height = armHeight

            .Left = x - armWidth
            .Top = y + armHeight \ 3

            .BackColor = Color.Black
        End With

        With Me.rightArm

            .Width = armWidth
            .Height = armHeight

            .Left = x + armWidth
            .Top = y + armHeight \ 3

            .BackColor = Color.Black
        End With


        'load in the eye image
        With eye

            .Width = 12
            .Height = 12

            .Left = x + width \ 2 - .Width \ 2
            .Top = y + height \ 4 - .Height \ 2

            'draw eyeball using graphics, paint to a picturebox
            Dim graphic As New GraphicsPath()

            graphic.AddEllipse(0, 0, .Width, .Height)

            .Region = New Region(graphic)
            '--------------------------------------------------

            .BackColor = Color.Black

        End With
        '------------------------

        'load in the eyebox
        With eyebox

            .Width = 20
            .Height = 20

            .Left = x + width \ 2 - .Width \ 2
            .Top = y + height \ 4 - .Height \ 2

            .BackColor = Color.White

        End With
        '---------------------

        all_limbs.Add(eye)
        all_limbs.Add(eyebox)

        If Me.type_enemy <> "spider" And Me.type_enemy <> "invisi" Then

            all_limbs.Add(leftLeg)
            all_limbs.Add(rightLeg)

            all_limbs.Add(leftArm)
            all_limbs.Add(rightArm)

            game.Controls.Add(leftLeg)
            game.Controls.Add(rightLeg)

            game.Controls.Add(leftArm)
            game.Controls.Add(rightArm)
        End If

        game.Controls.Add(eyebox)
        game.Controls.Add(eye)

        eyebox.BringToFront()
        eye.BringToFront()

    End Sub

    Protected Sub update_eye()

        'lets eye and eyebox follow the enemy x and y , to make it seem that it is on it/connected
        Dim centerx As Integer = Me.x + 5 + (Me.width / 2)
        Dim centery As Integer = Me.y + 2 + (Me.height / 2)

        eye.Left = centerx - eye.Width
        eye.Top = centery - eye.Height

        eyebox.Left = Me.x + Me.width \ 2 - eyebox.Width \ 2
        eyebox.Top = (Me.y + Me.height \ 4 - eyebox.Height \ 2) + 10

    End Sub

    Protected MustOverride Sub update_arms(ByVal plr1 As player)

    Protected Sub update_legs(ByVal tick As Integer)

        'move the left leg , then right leg, creates a timer between both to simulate walking
        If tick Mod 2 = 0 Then

            If leg_move = "R" Then

                rightLeg.Left = x + (width - legWidth)
                rightLeg.Top = y + height

                Me.leg_move = "L"

            ElseIf leg_move = "L" Then

                leftLeg.Left = x
                leftLeg.Top = y + height

                Me.leg_move = "R"

            End If
        End If
        '------------------------------------------------------------------------------------
    End Sub



    Public Sub can_see_move(ByVal plr1 As player, ByVal tick As Integer, ByVal map As List(Of String))

        'turn the player and enemy coordinates into a 1d array [x, y] so the ray caster can work on it
        Dim plr_coords(2) As Integer
        Dim my_coords(2) As Integer

        plr_coords(0) = plr1.x
        plr_coords(1) = plr1.y

        my_coords(0) = Me.x
        my_coords(1) = Me.y
        '----------------------------------------------------------------------------------------------

        'checks if the enemy can see the player
        If check_in_range(plr1) And Not Me.raycast_obj.ray_blocked(plr_coords, my_coords) Then
            'change the enemies colour when they see the player

            Me.plr1_see = True
        Else
            Me.plr1_see = False

        End If
        '----------------------------------------


        'if the player can be seen a counter it set to 20 allowing the enemy to follow the player
        If Me.plr1_see Then
            Me.see_timer = 20

        ElseIf Me.see_timer > 0 And Me.plr1_see = False Then
            'change the enmies colour depending on how long they can see the player

            Me.see_timer -= 1

        Else
            Me.see_timer = 0

        End If
        '---------------------------------------------------------------------------------------

        'if the counter is checked to be lsrger than zero, if so, the enemy goes towards them
        If Me.see_timer > 0 Then
            move_to_plr(plr1, 2)

        End If
        '------------------------------------------------------------------------------------   

    End Sub

    Public Function check_in_range(ByVal plr1)
        'checks if the player is in a set radius
        If Math.Sqrt((Me.x - plr1.X) * (Me.x - plr1.X) + (Me.y - plr1.Y) * (Me.y - plr1.Y)) < base_enemy.see_radius / 2 Then

            Return True

        End If
        '---------------------------------------
        Return False
    End Function

    Public Sub move_limbs(ByVal tick As Integer, ByVal plr1 As player)

        Me.update_legs(tick)
        Me.update_arms(plr1)
        Me.update_eye()

    End Sub

    Public Sub change_col(ByVal tick)
        'change colour back to normal after being hit
        If tick Mod 6 = 0 Then

            If Me.box.BackColor = Color.LightGray Then
                Me.box.BackColor = Me.colour

            End If

        End If
        '--------------------------------------------------------------------
    End Sub

    Protected Friend Sub load_attacks_fromtxt(Optional ByVal which_atks As String = "plren")

        'load the atributes for each attack into a dictionary
        Me.attack_dict = Me.util.load_attacks(which_atks)
        '-------------------------------------------------

    End Sub



    Protected Friend Overridable Sub update(ByVal all_objects As List(Of wall), ByVal plr1 As player, ByVal enemies As List(Of base_enemy), ByVal map As List(Of String), ByVal tick As Integer)

        move_limbs(tick, plr1)
        can_see_move(plr1, tick, map)
        change_col(tick)
        collisions(all_objects, plr1, tick)

        Me.x += Me.xvel
        Me.y += Me.yvel

        Me.box.Left = Me.x
        Me.box.Top = Me.y
    End Sub
End Class
