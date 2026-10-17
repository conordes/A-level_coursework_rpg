Imports System.Drawing
Imports System.Drawing.Imaging


Public Class player
    Inherits base_block

    Private util As utilities

    Private _max_hp As Integer = 100

    Public Property max_hp
        Get
            Return _max_hp
        End Get
        Set(ByVal value)
            _max_hp = value
        End Set
    End Property

    Private _hp As Integer = 100

    Public Property hp
        Get
            Return _hp
        End Get
        Set(ByVal value)
            _hp = value
        End Set
    End Property

    Private _max_mana As Integer = 100

    Public Property max_mana
        Get
            Return _max_mana
        End Get
        Set(ByVal value)
            _max_mana = value
        End Set
    End Property

    Public _mana As Integer = 100

    Public Property mana
        Get
            Return _mana
        End Get
        Set(ByVal value)
            _mana = value
        End Set
    End Property

    Public max_armour As Integer = 100
    Public armour As Integer = 0

    Public all_stats(3) As Integer

    Public enemy_lisz As List(Of base_enemy)
    Public door_lisz As List(Of door)
    Public wall_lisz As List(Of wall)
    Public floor_items_lisz As List(Of item)
    Public npc_lisz As List(Of Npc)

    Public live_attack As New List(Of base_attack)
    Public dead_attack As New List(Of base_attack)

    Public attack_dict As New Dictionary(Of String, List(Of String))
    Public chosen_attack As String = "fireball"

    Public index_on_attack As Integer = 0

    Public tick As Double

    Public my_interface As player_inteface

    Public open_inv As Boolean = False
    Public opened_ui As Boolean = True
    Public loaded_all As Boolean = True

    Public equipped(8) As String
    Public collided_flooritem As Integer = -1

    Public my_sheild As sheild

    Public touch_door As Boolean = False
    Public which_door As door = Nothing

    Public mousex As Integer = 0
    Public mousey As Integer = 0


    Public leftLeg As New PictureBox()
    Public rightLeg As New PictureBox()
    Public leftArm As New PictureBox()
    Public rightArm As New PictureBox()

    Public legWidth As Integer = width \ 3
    Public legHeight As Integer = height \ 4
    Public armW As Integer = width \ 4
    Public armH As Integer = height \ 3

    Public all_limbs As New List(Of PictureBox)
    Public leg_move As String = "R"

    Public attack_img As Bitmap

    Public all_missions As Dictionary(Of List(Of String), List(Of Integer))

    Public killed_enemy_amounts As New Dictionary(Of String, Integer)
    Public start_level As Boolean = False

    Private _boss_room As Boolean = False

    Public Property boss_room As Boolean
        Get
            Return _boss_room
        End Get
        Set(ByVal value As Boolean)
            _boss_room = value
        End Set
    End Property

    Private _finished_game As Boolean = False

    Public Property finished_game As Boolean
        Get
            Return _finished_game
        End Get
        Set(ByVal value As Boolean)
            _finished_game = value
        End Set
    End Property


    Sub New(ByVal game As Form, ByVal height As Integer, ByVal width As Integer, ByVal x As Integer, ByVal y As Integer, ByVal util As utilities)
        MyBase.New(game, height, width, x, y)

        Me.box.BackColor = Color.Blue
        Me.util = util


        ' Legs
        legWidth = width \ 3
        legHeight = height \ 4
        'arms
        armW = width \ 4
        armH = height \ 3


    End Sub

    Public Sub key_handler(ByVal sender As Object, ByVal e As KeyPressEventArgs)

        Select Case e.KeyChar
            Case "w"c
                Me.yvel -= 3

            Case "s"c
                Me.yvel += 3

            Case "d"c
                Me.xvel += 3

            Case "a"c
                Me.xvel -= 3

            Case "i"c
                'open the player_inteface if the sheild is not being used
                If Not Me.my_sheild.sheild_use Then
                    'open ui when player_inteface closed and vice-versa
                    If Not open_inv Then
                        Me.open_inv = True
                        Me.opened_ui = False
                        loaded_all = True
                    Else
                        Me.open_inv = False
                        Me.opened_ui = True
                        loaded_all = True
                    End If

                End If

        End Select

    End Sub

    Public Function get_angle_to_mouse(ByVal mx As Double, ByVal msy As Double)

        Dim deltaX As Integer = mx - Me.x
        Dim deltaY As Integer = msy - Me.y

        Dim angle As Double = Math.Atan2(deltaY, deltaX)

        Return angle ' Angle in radians

    End Function

    Public Sub click_handler(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        If e.Button = MouseButtons.Left Then
            'use attack atributes saved in a text file to create attack
            If Me.mana >= Me.attack_dict(chosen_attack)(6) And Me.attack_dict(chosen_attack)(5) <= 0 Then

                Me.attack_dict(chosen_attack)(5) = Me.attack_dict(chosen_attack)(4)
                Me.mana -= Me.attack_dict(chosen_attack)(6)

                Select Case chosen_attack
                    Case "fireball"
                        load_fireball()
                    Case "slash"
                        load_slash()
                    Case "lightning"
                        load_lightning()
                End Select

            End If


            'if the sheild is equipped and the player_inteface is closed
        ElseIf e.Button = MouseButtons.Right And Not Me.open_inv Then

            Me.my_sheild.loaded = False

            If Not Me.my_sheild.sheild_use And Me.equipped(6) <> Nothing Then

                Me.my_sheild.sheild_use = True

            ElseIf Me.my_sheild.sheild_use Then

                Me.my_sheild.sheild_use = False

            End If

        End If

    End Sub

    Public Sub scroll_wheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        'use the scroll wheel to choose between attacks

        Me.index_on_attack += 1


        If Me.index_on_attack > attack_dict.Count() - 1 Then
            Me.index_on_attack = 0
        End If

        Me.chosen_attack = Me.attack_dict.Keys(Me.index_on_attack)

        If chosen_attack = "fireball" Then
            Me.attack_img = New Bitmap(Me.attack_dict(chosen_attack)(2))
        ElseIf chosen_attack = "slash" Then
            Me.attack_img = New Bitmap(Me.attack_dict(chosen_attack)(2))
        ElseIf chosen_attack = "lightning" Then
            Me.attack_img = New Bitmap(Me.attack_dict(chosen_attack)(2))
        End If
        '-------------------------------------------------
    End Sub


    Public Sub update_bars()

        Me.my_interface.mana_bar.new_size(mana / max_mana)
        Me.my_interface.hp_bar.new_size(Me.hp / Me.max_hp)
        Me.my_interface.attack_bar.new_size((Me.attack_dict(Me.chosen_attack)(4) - Me.attack_dict(Me.chosen_attack)(5)) / Me.attack_dict(Me.chosen_attack)(4))

    End Sub

    Public Sub update_attacks()
        'make attacks cooldown after being used
        For Each atk In Me.attack_dict



            If Me.attack_dict(atk.Key)(5) > 0 And Me.tick Mod 2 = 0 Then
                Me.attack_dict(atk.Key)(5) -= 0.25
            End If

        Next
        '-----------------------------------------

        'chek all live attacks so they dont live too long
        For i = 0 To Me.live_attack.Count() - 1

            If Me.live_attack(i).check_alive(Me.tick) Then

                dead_attack.Add(live_attack(i))

                Me.game.Controls.Remove(Me.live_attack(i).box)

                live_attack.RemoveAt(i)

                Exit For

            End If

        Next
        '------------------------------------------------

        Me.dead_attack.Clear()

    End Sub

    Public Sub update_invent_or_ui()
        'if the player eye object has been equipped end the game


        'if an item in the player_inteface has been clicked activate it
        Dim new_clicked As Tuple(Of String, Integer, item) = Me.my_interface.update(Me.tick, Me)

        If new_clicked.Item1 <> "" Then

            If Me.my_interface.itemDict(new_clicked.Item1)(0) = 2 Or Me.my_interface.itemDict(new_clicked.Item1)(0) = 3 Then
                'add armour peice

                Me.equipped(Me.my_interface.itemDict(new_clicked.Item1)(1)) = new_clicked.Item1

                new_clicked.Item3.amount -= 1


            ElseIf Me.my_interface.itemDict(new_clicked.Item1)(0) = 1 Then

                If Me.my_interface.itemDict(new_clicked.Item1)(1) <> 0 Then
                    'If hp change, add hp, make it the max hp, if hp is already full dont change anything
                    If Me.hp >= Me.max_hp Then
                        new_clicked.Item3.take_away = False

                    ElseIf Me.hp + Me.my_interface.itemDict(new_clicked.Item1)(1) < Me.max_hp Then
                        Me.hp += Me.my_interface.itemDict(new_clicked.Item1)(1)
                        new_clicked.Item3.take_away = True

                    ElseIf Me.hp + Me.my_interface.itemDict(new_clicked.Item1)(1) > max_hp Then
                        Me.hp = Me.max_hp
                        new_clicked.Item3.take_away = True

                    End If

                    If new_clicked.Item3.take_away = True Then
                        new_clicked.Item3.amount -= 1
                        new_clicked.Item3.take_away = False

                    End If

                End If

                If Me.my_interface.itemDict(new_clicked.Item1)(2) <> 0 Then

                    'If mana change, add mana, make it the max mana, if mana is already full dont allow it to be clicked
                    If Me.mana >= Me.max_mana Then
                        new_clicked.Item3.take_away = False

                    ElseIf Me.mana + Me.my_interface.itemDict(new_clicked.Item1)(2) <= Me.max_mana Then
                        Me.mana += Me.my_interface.itemDict(new_clicked.Item1)(2)
                        new_clicked.Item3.take_away = True

                    ElseIf Me.mana + Me.my_interface.itemDict(new_clicked.Item1)(2) > max_mana Then
                        Me.mana = Me.max_mana
                        new_clicked.Item3.take_away = True

                    End If

                    'if stackable take an item away
                    If new_clicked.Item3.take_away = True Then
                        new_clicked.Item3.amount -= 1
                        new_clicked.Item3.take_away = False

                    End If

                End If

            End If
        End If


    End Sub

    Public Sub update_armour()

        Dim temp_armour As Integer = 0

        For Each arm In Me.equipped

            If (arm <> "0" And arm <> "") AndAlso Me.my_interface.itemDict(arm)(0) = "3" Then
                temp_armour += Me.my_interface.itemDict(arm)(4)
            End If

        Next

        If temp_armour > 100 Then temp_armour = 100

        For block = 0 To Me.my_interface.all_equip_boxes.Count()
            If temp_armour > (block + 1) * 10 Then
                Me.my_interface.all_equip_boxes(block).box.BackColor = Color.White
            End If
        Next

        Me.armour = temp_armour


    End Sub

    Public Sub update_mission()
        'if the npc has been clicked add to player missions
        For Each Npc In Me.npc_lisz

            If Npc.already_clicked And Not Npc.dead And Npc.textbox_open Then

                Dim val_lis As New List(Of Integer)

                val_lis = Me.all_missions(Npc.mission_txt_list)
                val_lis.Add(0)
                Npc.mission_stats = val_lis

                Dim mission_text As String = Npc.mission_txt_list(0) & " " & val_lis(2) & "/" & val_lis(0)
                Npc.mission_txt = New text(Me.game, 950, 0, 15, mission_text, Color.Yellow)

                AddHandler Npc.mission_txt.box.MouseClick, AddressOf Npc.mission_txt.click_handler
                Me.my_interface.missions_npcs.Add(Npc)

                Npc.destroy()

            End If

        Next
        '--------------------------------------------------

        'check if any missions have been completed and therefore make them green
        For Each mis In Me.my_interface.missions_npcs

            If Me.killed_enemy_amounts.ContainsKey(mis.mission_txt_list(1)) Then

                mis.mission_stats(2) = Me.killed_enemy_amounts(mis.mission_txt_list(1))
                mis.mission_txt.my_text = mis.mission_txt_list(0) & " " & mis.mission_stats(2) & "/" & mis.mission_stats(0)

            End If

            'if there are more or equal kills to the number needed
            If mis.mission_stats(2) >= mis.mission_stats(0) Then
                mis.mission_txt.colour = Color.Green
            End If

            'if cliked give reward and tak away from missions
            If mis.mission_txt.button_clicked And mis.mission_stats(2) >= mis.mission_stats(0) Then

                For num = 0 To mis.mission_stats(1) - 1
                    Me.my_interface.add(mis.mission_txt_list(2))
                Next

                mis.done_mis = True

                mis.mission_txt.button_clicked = False
                mis.mission_txt.destroy()

            End If

        Next
        '---------------------------------------------------------------
    End Sub


    Public Sub load_slash()
        Dim xer As Integer = Me.x
        Dim yer As Integer = Me.y


        ' Calculate direction
        Dim dx As Integer = mousex - Me.x
        Dim dy As Integer = mousey - Me.y


        ' Normalize and extend
        Dim scale As Double = 0.3 ' length of the line
        xer = CInt(Me.x + dx * scale)
        yer = CInt(Me.y + dy * scale)


        Dim atk As New base_attack(Me.game, 60, 30, xer, yer, 4, chosen_attack, New Bitmap(Me.attack_dict("slash")(2)), Me.attack_dict("slash")(1) * (1 + Me.all_stats(0) / 10), get_angle_to_mouse(mousex, mousey))

        atk.spawn_time = Me.tick
        atk.load()

        live_attack.Add(atk)

    End Sub

    Public Sub load_fireball()

        Dim atk As New base_attack(Me.game, 30, 30, Me.x, Me.y, 20, chosen_attack, New Bitmap(Me.attack_dict("fireball")(2)), Me.attack_dict("fireball")(1) * (1 + Me.all_stats(2) / 10), get_angle_to_mouse(mousex, mousey))

        atk.move(mousex, mousey, Me, 18)
        atk.spawn_time = Me.tick

        atk.load()

        live_attack.Add(atk)
    End Sub

    Public Sub load_lightning()
        'load lightning into a circle of 6 around the player
        Dim lightning_radius As Integer = 125

        For i = 0 To 6

            Dim angle As Integer = i * 60
            Dim radian As Double = angle * Math.PI / 180

            Dim xer As Integer = Me.x + lightning_radius * Math.Cos(radian)
            Dim yer As Integer = Me.y + lightning_radius * Math.Sin(radian)

            Dim atk As New base_attack(Me.game, 50, 50, xer, yer, 5, chosen_attack, New Bitmap(Me.attack_dict("lightning")(2)), Me.attack_dict("lightning")(1) * (1 + Me.all_stats(2) / 10), radian)

            atk.spawn_time = Me.tick

            atk.load()
            live_attack.Add(atk)

        Next

    End Sub

    Public Sub load_attacks()

        'load the atributes for each attack into a dictionary
        Me.attack_dict = Me.util.load_attacks("plren")
        '-------------------------------------------------

    End Sub

    Public Sub load_invent()

        If Me.loaded_all Then
            'load the inventory if its not been loaded into memeory
            If Me.open_inv Then
                Me.my_interface.show_equipped()
                Me.game.Size = New Size(1300, 850)

                Me.my_interface.load_inventory()
                Me.my_interface.draw_grid(60, 6, 7)
                Me.my_interface.load_mis()

                'close the inventory
            ElseIf Not Me.open_inv Then
                Me.game.Size = New Size(900, 850)
                Me.my_interface.delete_inventory()

            End If
            'open the ui if its been loaded
            If Me.opened_ui Then
                Me.my_interface.load_ui_again(Me, Me.boss_room)
                'close the ui
            ElseIf Not Me.opened_ui Then
                Me.my_interface.delete_ui()

            End If
            'only do these once per open or close
            Me.loaded_all = False

        End If

    End Sub


    Public Sub collisions()
        'colllided with doors and choose where the door leads
        For Each door In door_lisz
            If door.box.Bounds.IntersectsWith(Me.box.Bounds) Then

                If door.goes_to = "bossflr" Then
                    touch_door = True
                    which_door = door
                Else
                    touch_door = True
                    which_door = door
                End If

            End If
        Next
        '--------------------------------------------------


        For Each ene In enemy_lisz

            If ene.type_magic Then

                For Each atk In ene.live_attacks

                    'check if the shield blocks the attack or not
                    If Me.my_sheild.sheild_use And ene.attack_dict(atk.typeofattack)(1) <> -1 Then

                        If Me.my_sheild.collision(-ene.attack_dict(atk.typeofattack)(1), atk) Then
                            atk.dead = True
                        ElseIf Me.player_box_collision(-ene.attack_dict(atk.typeofattack)(1), atk) Then
                            atk.dead = True
                        End If

                    ElseIf Me.my_sheild.sheild_use = False And ene.attack_dict(atk.typeofattack)(1) <> -1 Then

                        If Me.player_box_collision(-ene.attack_dict(atk.typeofattack)(1), atk) Then
                            atk.dead = True
                        End If

                    End If
                    '-----------------------------------

                    'teleport the player to the invisible boss if hit
                    If atk.typeofattack = "invisi_ball" Then

                        If atk.box.Bounds.IntersectsWith(Me.box.Bounds) Then
                            Me.x = ene.x + (25 + (Me.util.Rand0())) - 75
                            Me.y = ene.y + (25 + (Me.util.Rand0())) - 75
                        End If

                    End If

                Next
            End If

            'collisions for the enemy cubes
            If ene.type_enemy = "invisi" Then

                For Each cube In ene.cubes

                    If cube.box.Bounds.IntersectsWith(Me.box.Bounds) Then
                        Me.hp -= 1
                    End If

                Next

            End If
            '----------------------------

            'check if the sheild has collided with an enemy or not
            If Me.my_sheild.sheild_use Then

                If Me.my_sheild.collision(-1, ene) Then
                    pushback(ene, Me.my_sheild.x, Me.my_sheild.y)

                ElseIf Me.player_box_collision(-1, ene) Then
                    pushback(ene, Me.x, Me.y)

                End If

            Else

                If Me.player_box_collision(-1, ene) Then
                    pushback(ene, Me.x, Me.y)
                End If

            End If
            '------------------------------------------------------

        Next


        For Each wal In wall_lisz
            If wal.box.Bounds.IntersectsWith(Me.box.Bounds) Then
                ' Calculate player and wall boundaries
                Dim player_left As Integer = Me.x
                Dim player_right As Integer = Me.x + Me.width
                Dim player_top As Integer = Me.y
                Dim player_bottom As Integer = Me.y + Me.height

                Dim wall_left As Integer = wal.x
                Dim wall_right As Integer = wal.x + wal.width
                Dim wall_top As Integer = wal.y
                Dim wall_bottom As Integer = wal.y + wal.height

                ' Determine overlap distances
                Dim overlap_left As Integer = player_right - wall_left
                Dim overlap_right As Integer = wall_right - player_left
                Dim overlap_top As Integer = player_bottom - wall_top
                Dim overlap_bottom As Integer = wall_bottom - player_top


                Dim min_overlap As Integer = Math.Min(Math.Min(overlap_left, overlap_right), Math.Min(overlap_top, overlap_bottom))

                ' Push the player out depending on which side is smallest
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
            End If
        Next


        'check if the player has collided with a pick up item on the floor

        'update wich item has been collided with (-1 if none)
        Me.collided_flooritem = -1

        For item = 0 To Me.floor_items_lisz.Count() - 1

            If Me.floor_items_lisz(item).box.Bounds.IntersectsWith(Me.box.Bounds) Then

                Me.my_interface.add(Me.floor_items_lisz(item).name)
                Me.collided_flooritem = item

            End If
        Next
        '----------------------------------------------------------------


    End Sub

    Public Function player_box_collision(ByVal hit_taken As Integer, ByVal target As Object)
        'if the collision with the player happens
        If target.box.Bounds.IntersectsWith(Me.box.Bounds) Then
            'changes the damage taken depending on the amount of armour used
            Me.hp += hit_taken * (1 - (Me.armour / Me.max_armour))

            Return True
        End If

        Return False
    End Function

    Public Sub pushback(ByVal entity As base_enemy, ByVal xer As Integer, ByVal yer As Integer)

        If entity.x < xer Then
            entity.x -= entity.width / 2
        End If

        If entity.x > xer Then
            entity.x += entity.width / 2
        End If

        If entity.y < yer Then
            entity.y -= entity.height / 2
        End If

        If entity.y > yer Then
            entity.y += entity.height / 2
        End If

    End Sub


    Public Sub limb_load()

        leftLeg.Width = legWidth
        leftLeg.Height = legHeight

        leftLeg.Left = x
        leftLeg.Top = y + height
        leftLeg.BackColor = Color.Black



        rightLeg.Width = legWidth
        rightLeg.Height = legHeight

        rightLeg.Left = x + (width - legWidth)
        rightLeg.Top = y + height
        rightLeg.BackColor = Color.Black


        leftArm.Width = armW
        leftArm.Height = armH

        leftArm.Left = Me.x - leftArm.Width ' To the left of the box
        leftArm.Top = Me.y + Me.height \ 3
        leftArm.BackColor = Color.Black

        rightArm.Width = armW
        rightArm.Height = armH

        rightArm.Left = Me.x + Me.width ' To the right of the box
        rightArm.Top = Me.y + Me.height \ 3
        rightArm.BackColor = Color.Black


        all_limbs.Add(leftLeg)
        all_limbs.Add(rightLeg)
        all_limbs.Add(leftArm)
        all_limbs.Add(rightArm)


        game.Controls.Add(leftLeg)
        game.Controls.Add(rightLeg)
        game.Controls.Add(leftArm)
        game.Controls.Add(rightArm)

    End Sub

    Public Sub update_arms(ByVal x As Integer, ByVal y As Integer)

        Dim dx As Integer = x - Me.x
        Dim dy As Integer = y - Me.y

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
        Dim armBaseX As Integer = Me.x + 25 + CInt(nx * reach)
        Dim armBaseY As Integer = Me.y + 25 + CInt(ny * reach)

        ' Left arm
        leftArm.Left = armBaseX + CInt(perpX * separation) - leftArm.Width \ 2
        leftArm.Top = armBaseY + CInt(perpY * separation) - leftArm.Height \ 2

        ' Right arm
        rightArm.Left = armBaseX - CInt(perpX * separation) - rightArm.Width \ 2
        rightArm.Top = armBaseY - CInt(perpY * separation) - rightArm.Height \ 2

    End Sub

    Public Sub movement_nums()
        'a max velocity
        If xvel > 6 Then
            Me.xvel = 6

        ElseIf xvel < -6 Then
            Me.xvel = -6

        End If

        If yvel > 6 Then
            Me.yvel = 6

        ElseIf yvel < -6 Then
            Me.yvel = -6

        End If

        'add the user defined stat to the player speed
        Me.x += Me.xvel * (1 + (Me.all_stats(1) / 10))
        Me.y += Me.yvel * (1 + (Me.all_stats(1) / 10))

    End Sub

    Public Sub move_limbs()

        'every 2 seconds change wich leg id move-to simulate walking
        If Me.tick Mod 2 = 0 Then

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

        Me.update_arms(mousex, mousey)

    End Sub


    Public Sub update(ByVal all_wallz As List(Of wall), ByVal timer As Integer, ByVal mouses_x As Integer, ByVal mouse_y As Integer, ByVal enemies As List(Of base_enemy), ByVal doorz As List(Of door), ByVal flooritems As List(Of item), ByVal allnpc As List(Of Npc))

        Me.tick = timer

        Me.touch_door = False

        Me.enemy_lisz = enemies
        Me.door_lisz = doorz
        Me.wall_lisz = all_wallz
        Me.floor_items_lisz = flooritems
        Me.npc_lisz = allnpc

        Me.movement_nums()

        Me.my_sheild.update()

        Me.move_limbs()

        Me.update_mission()

        Me.load_invent()

        Me.update_invent_or_ui()
        Me.update_armour()

        Me.collisions()

        Me.update_attacks()

        Me.update_bars()

        For Each i In Me.live_attack
            i.update()
            i.box.BringToFront()
        Next

        Me.mousex = mouses_x
        Me.mousey = mouse_y

        Me.box.Left = Me.x
        Me.box.Top = Me.y

    End Sub

End Class
