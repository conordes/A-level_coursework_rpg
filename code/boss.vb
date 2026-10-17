Public Class boss
    Inherits base_enemy
    '[angle as double] used throughout the code for movement, inherited from base_enemy

    'spider boss:
    Protected Friend thighs As New List(Of List(Of PictureBox))
    Protected Friend femur As New List(Of List(Of PictureBox))
    Protected Friend left As Boolean = False

    Protected Friend next_xer As New List(Of Double)
    Protected Friend next_yer As New List(Of Double)

    Public radian As Double = 0

    'invisi boss:
    Public point As New List(Of Double)
    Public atk_point As New List(Of Double)

    Public body_parts As New List(Of base_block)
    Public invisible As Boolean = False

    Private plr1 As player

    Public Sub New(ByVal game As Form, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal type_of As String, ByVal maxhp As Integer, ByVal plr As player, ByVal utility As utilities, Optional ByVal mgic As Boolean = True)
        MyBase.new(game, height, width, x, y, 0, maxhp, utility, mgic)


        Me.plr1 = plr

        Me.type_enemy = type_of

        'change the body depending on the type of boss
        If type_of = "spider" Then

            Me.box.BackColor = Color.Black
            Me.colour = Color.Black

        ElseIf type_of = "invisi" Then
            Me.box.BackColor = Color.White
            Me.colour = Color.Transparent

        End If
        '----------------------------------------------
    End Sub


    Public Sub update_spider(ByVal tick As Integer)


        'the spider legs attach themselves to an anchor point then every few ticks the hav a new anchor closer to the main body set, simulating crawling

        'moves the upper leg with the spider body, as if its connected to it
        For i = 0 To 5

            angle = i * 60
            Dim rado As Double = angle * Math.PI / 180


            For j = 0 To 3

                Dim xero As Integer = (Me.x + 10) + (j + 3) * 10 * Math.Cos(rado)
                Dim yero As Integer = (Me.y + 10) + (j + 3) * 10 * Math.Sin(rado)

                Me.thighs(i)(j).Left = xero
                Me.thighs(i)(j).Top = yero
            Next
        Next
        '-------------------------------------------------------------------

        'empties the next_list of y and x coords for new ones
        For i = 0 To 5
            Me.next_xer.Add(0)
            Me.next_yer.Add(0)
        Next
        '----------------------------------------------------------------

        'every 8 ticks either the left lower legs or right lower create new anchors to go to
        If tick Mod 8 = 0 Then

            If Not left Then

                For i = 0 To 2
                    angle = i * 60
                    radian = angle * Math.PI / 180

                    Me.next_xer(i) = (Me.x + 8) + 100 * Math.Cos(radian)
                    Me.next_yer(i) = (Me.y + 8) + 100 * Math.Sin(radian)
                Next

                Me.left = True

            Else

                For i = 3 To 5
                    angle = i * 60
                    radian = angle * Math.PI / 180

                    Me.next_xer(i) = (Me.x + 8) + 100 * Math.Cos(radian)
                    Me.next_yer(i) = (Me.y + 8) + 100 * Math.Sin(radian)
                Next

                Me.left = False

            End If

        End If
        '---------------------------------------------------------------------------------------------------------

        'use the new anchor created to simulate moving, if new anchors havent been made stretch the lower leg fom body so the legs still looks alive and not still
        For i = 0 To 5

            angle = i * 60
            radian = angle * Math.PI / 180

            Dim xer As Double = (Me.x + 8) + 60 * Math.Cos(radian)
            Dim yer As Double = (Me.y + 8) + 60 * Math.Sin(radian)

            For j = 0 To 3

                Dim distx As Double = xer - Me.next_xer(i)
                Dim disty As Double = yer - Me.next_yer(i)

                Me.femur(i)(j).Left = Me.next_xer(i) + (distx / (j + 1))
                Me.femur(i)(j).Top = Me.next_yer(i) + (disty / (j + 1))

            Next

        Next
        '---------------------------------------------------------------------------------------------------------------------------------------------------------

        eyebox.BackColor = Color.Purple
    End Sub

    Protected Overrides Sub update_arms(ByVal plr1 As player)
        'empty sub as it inherits base enemy where all other classes use the UpdateArms sub
    End Sub

    Public Sub spider_atks(ByVal plr1 As player, ByVal tick As Integer)
        'shoot webs towards the player and create poison splotges to slow the player and protect from ranged attacks

        'checks the range from the player
        Dim dist = (Me.x - plr1.x) ^ 2 + (Me.y - plr1.y) ^ 2

        'use these coordinates in the raycasting to make the sure the player can be seen
        Dim plr_coords(2) As Integer
        Dim my_coords(2) As Integer

        plr_coords(0) = plr1.x
        plr_coords(1) = plr1.y

        my_coords(0) = Me.x
        my_coords(1) = Me.y

        'create the spider web attack, thet moves towards the player

        If tick Mod 12 = 0 And Not Me.raycast_obj.ray_blocked(my_coords, plr_coords) Then

            Dim dx = Me.x - plr1.x
            Dim dy = Me.y - plr1.y

            Me.angle = Math.Atan2(dy, dx)

            Dim new_attack = New base_attack(Me.game, 25, 25, Me.x, Me.y, 40, "web", New Bitmap(Me.attack_dict("web")(2)), Me.attack_dict("web")(1), angle)

            new_attack.spawn_time = tick
            new_attack.load()
            new_attack.move(plr1.x, plr1.y, Me)
            new_attack.box.BringToFront()

            Me.live_attacks.Add(new_attack)

        End If
        '-----------------------------------------------------------

        'check if the player is in range and make the attack happen every 9 ticks 

        If dist < 100 ^ 4 And tick Mod 9 = 0 Then

            Dim new_attack = New base_attack(Me.game, 25 + Me.util.Rand0(1, 100), 25 + Me.util.Rand0(1, 100), (Me.x - 100) + Me.util.Rand0(1, 100) * 2, (Me.y - 100) + Me.util.Rand0(1, 100) * 2, 30, "stickyweb", New Bitmap(Me.attack_dict("stickyweb")(2)), Me.attack_dict("stickyweb")(1), 0)

            new_attack.spawn_time = tick
            new_attack.load()

            Me.live_attacks.Add(new_attack)


        End If

        '----------------------------------------------------------------------------

    End Sub

    Public Sub load_legs()
        'add upper leg peices to the spider
        For i = 0 To 5

            Dim new_list As New List(Of PictureBox)

            For j = 0 To 4

                Dim new_add As New PictureBox

                AddHandler new_add.MouseClick, AddressOf Me.plr1.click_handler

                new_add.Width = 15
                new_add.Height = 15
                new_add.BackColor = Color.Black



                new_list.Add(new_add)
                new_add.BringToFront()
                Me.game.Controls.Add(new_add)

            Next

            Me.thighs.Add(new_list)
        Next
        '------------------------------------

        'add lower leg peices to the spider
        For i = 0 To 5

            Dim new_list As New List(Of PictureBox)

            For j = 0 To 3

                Dim new_add As New PictureBox

                new_add.Width = 10
                new_add.Height = 10
                new_add.BackColor = Color.Black

                new_list.Add(new_add)
                Me.game.Controls.Add(new_add)
                new_add.BringToFront()

            Next

            Me.femur.Add(new_list)
        Next
        '------------------------------------
    End Sub


    Public Sub invisi_atks(ByVal plr1 As player, ByVal tick As Integer)

        Dim plr_coords(2) As Integer
        Dim my_coords(2) As Integer

        plr_coords(0) = plr1.x + 25
        plr_coords(1) = plr1.y + 25

        my_coords(0) = Me.x + 25
        my_coords(1) = Me.y + 25

        playerx = plr1.x
        enemyx = Me.x

        playery = plr1.y
        enemyy = Me.y


        If Not Me.invisible And Not Me.raycast_obj.ray_blocked(plr_coords, my_coords) And tick Mod 20 = 0 Then

            Dim dy = enemyy - playery
            Dim dx = enemyx - playerx

            If dx = 0 Then dx = 1


            'make a line of attacks (purple orbs) towards the player

            'change the math is the player is in front of or behing the enemy
            If playerx >= enemyx Then
                'player in front
                For new_x = enemyx To playerx Step 20

                    Dim new_y As Integer = (dy / dx) * (new_x - x) + enemyy

                    new_y += (-25 + Me.util.Rand0(1, 50))

                    Dim new_attack = New base_attack(Me.game, 20, 20, new_x, new_y, 30, "invisi_ball", New Bitmap(Me.attack_dict("invisi_ball")(2)), Me.attack_dict("invisi_ball")(1), 0)

                    new_attack.load()
                    new_attack.spawn_time = tick

                    Me.live_attacks.Add(new_attack)

                Next

            Else
                'player behind
                For new_x = playerx To enemyx Step 20

                    Dim new_y As Integer = (dy / dx) * (new_x - x) + enemyy

                    new_y += (-25 + Me.util.Rand0(1, 50))

                    Dim new_attack = New base_attack(Me.game, 20, 20, new_x, new_y, 30, "invisi_ball", New Bitmap(Me.attack_dict("invisi_ball")(2)), Me.attack_dict("invisi_ball")(1), 0)

                    new_attack.load()
                    new_attack.spawn_time = tick

                    Me.live_attacks.Add(new_attack)

                Next

            End If
            '-------------------------------------------------------------------

            'if they hit the player, it teleports the player into the cloud of cubes causing damage
        End If
    End Sub

    Public Sub update_cubes_bodyparts(ByVal tick As Integer)
        'the white cubes that make the circular body follow the main body, so they seem as one
        For i = 0 To 12
            angle = i * (360 / 12)
            Dim rado As Double = angle * Math.PI / 180

            Dim tox = Me.x + 11 + 35 * Math.Cos(rado)
            Dim toy = Me.y + 11 + 35 * Math.Sin(rado)

            Me.body_parts(i).x = tox
            Me.body_parts(i).y = toy

            Me.body_parts(i).box.Left = Me.body_parts(i).x
            Me.body_parts(i).box.Top = Me.body_parts(i).y

            Me.body_parts(i).box.BringToFront()
        Next
        '--------------------------------------------------------------------------------------



        'every 10 ticks, the cubes choose a new point to go to
        If tick Mod 10 = 0 Then

            'new angles to go towards
            Me.point.Clear()

            'create random angles for the floating cubes to go to, new angles each time
            For i = 0 To 20
                angle = i * 18
                Dim rado As Double = angle * Math.PI / 180

                Me.point.Add(rado)

            Next
            '-----------------------------------------------------

            For i = 0 To 20
                Dim rand_int As Integer = rand.Next(Me.point.Count())
                Dim rand_pos As Double = Me.point(rand_int)

                Dim to_x = Me.x + 25 + 160 * Math.Cos(rand_pos)
                Dim to_y = Me.y + 25 + 160 * Math.Sin(rand_pos)
                Dim now_x = Me.cubes(i).x
                Dim now_y = Me.cubes(i).y

                Me.cubes(i).xvel = (to_x - now_x) / 25
                Me.cubes(i).yvel = (to_y - now_y) / 25
            Next
        End If
        '-----------------------------------------------------

        'make it seem as if the cubes are floating towards a point not instantly appearing there
        For i = 0 To 20
            Me.cubes(i).x += Me.cubes(i).xvel
            Me.cubes(i).y += Me.cubes(i).yvel

            Me.cubes(i).box.Left = Me.cubes(i).x
            Me.cubes(i).box.Top = Me.cubes(i).y
            Me.cubes(i).box.BringToFront()
        Next
        '----------------------------------------------------------------------------------------

    End Sub

    Public Sub load_cubes_bodyparts()
        'loads in orbiting cubes around the invisible boss

        'loads in the cubes-------------------------------------------------------------
        For i = 0 To 20
            Dim defense_or_ofense_cubes As Integer = 0
            Dim new_add = New base_block(Me.game, 10, 10, 500, 500)

            If Me.util.Rand0(1, 100) >= 50 Then
                defense_or_ofense_cubes = 1
            End If


            AddHandler new_add.box.MouseClick, AddressOf Me.plr1.click_handler

            If defense_or_ofense_cubes = 1 Then
                new_add.box.BackColor = Color.Red
            Else
                new_add.box.BackColor = Color.Black
            End If

            new_add.load()

            Me.cubes.Add(new_add)

        Next
        '-----------------------------------------------------------------------------

        'gives starting x and y of each cube
        For i = 0 To 20
            angle = i * 18
            Dim rado As Double = angle * Math.PI / 180

            Me.point.Add(rado)

            Me.cubes(i).x = Me.x + 25 + 120 * Math.Cos(rado)
            Me.cubes(i).y = Me.y + 25 + 120 * Math.Sin(rado)

        Next
        '-------------------------------------
        '--------------------------------------------------

        'add white cubes around the body to make it look circle like
        For j = 0 To 12
            Dim new_add As New base_block(Me.game, 25, 25, Me.x, Me.y)

            new_add.box.BackColor = Color.White
            new_add.load()

            Me.body_parts.Add(new_add)

        Next
        '------------------------------------------------------------

    End Sub

    Public Sub update_invisi(ByVal tick As Integer)
        'updates the insibible boss so they can interchange between invisible and visible
        If Not Me.invisible Then

            Me.box.BackColor = Color.White
            Me.eyebox.BackColor = Color.DarkRed
            Me.eye.BackColor = Color.Black

            For Each i In Me.body_parts
                i.box.BackColor = Color.White
            Next

        ElseIf Me.invisible Then

            Me.box.BackColor = Color.Transparent
            Me.eyebox.BackColor = Color.Transparent
            Me.eye.BackColor = Color.Transparent

            For Each i In Me.body_parts
                i.box.BackColor = Color.Transparent
            Next

        End If
        '---------------------------------------------------------------------------------

        'sets a timer so the boss flips between invisible and visible
        If tick Mod 60 = 0 Then

            Me.invisible = Not Me.invisible

        End If
        '------------------------------------------------------------

    End Sub


    Public Overrides Sub collisions(ByVal wallz As List(Of wall), ByVal plr As player, ByVal tick As Integer)
        MyBase.collisions(wallz, plr, tick)

        'handles blocking player attacks using special skills

        For Each i In plr.live_attack

            If Me.type_enemy = "spider" Then

                For Each j In Me.live_attacks
                    If j.typeofattack = "stickyweb" And i.box.Bounds.IntersectsWith(j.box.Bounds) Then
                        If Me.attack_dict(i.typeofattack)(3) = 1 Then
                            i.dead = True
                        End If
                    End If
                Next

                If i.box.Bounds.IntersectsWith(Me.box.Bounds) Then
                    'changes colour to show you have hit
                    Me.box.BackColor = Color.LightGray
                    Me.hp -= i.damage

                    Me.plr1.my_interface.boss_bar.new_size(Me.hp / Me.max_hp)

                End If

            ElseIf Me.type_enemy = "invisi" Then
                'checks if cubes orbiting invisible boss have blocked an attack
                For Each j In Me.cubes
                    If i.box.Bounds.IntersectsWith(j.box.Bounds) Then
                        i.dead = True
                    End If
                Next

                If i.box.Bounds.IntersectsWith(Me.box.Bounds) Then
                    'changes colour to show you have hit
                    Me.box.BackColor = Color.LightGray
                    Me.hp -= i.damage

                    Me.plr1.my_interface.boss_bar.new_size(Me.hp / Me.max_hp)

                End If

            End If

        Next
        '------------------------------------------------------


    End Sub

    Public Sub update_attacks(ByVal Timer)
        'checks if an attack needs to be destroed as it has been alive too long
        For i = 0 To Me.live_attacks.Count() - 1

            If Me.live_attacks(i).check_alive(Timer) Or Me.live_attacks(i).dead Then

                dead_attacks.Add(live_attacks(i))
                live_attacks.RemoveAt(i)

                'only removes one attack per frame so they dont all dissapear at once making it seem laggy and unatural
                Exit For

            End If

        Next
        '-----------------------------------------------------------------------
    End Sub

    Public Overrides Sub load()
        'use the load in the inherited class
        MyBase.load()

        If Me.type_enemy = "spider" Then
            Me.load_legs()

        ElseIf Me.type_enemy = "invisi" Then
            Me.load_cubes_bodyparts()

        End If


    End Sub

    Protected Friend Overrides Sub update(ByVal all_objects As List(Of wall), ByVal plr1 As player, ByVal enemies As List(Of base_enemy), ByVal map As List(Of String), ByVal tick As Integer)

        If Me.type_enemy = "spider" Then

            Me.update_spider(tick)

            Me.spider_atks(plr1, tick)

        ElseIf Me.type_enemy = "invisi" Then

            Me.update_cubes_bodyparts(tick)

            Me.update_invisi(tick)

            Me.invisi_atks(plr1, tick)

        End If

        Me.update_attacks(tick)

        MyBase.update(all_objects, plr1, enemies, map, tick)

    End Sub

End Class
