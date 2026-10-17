
Imports System.Diagnostics
Imports System.IO

Public Class Form1

    Private raycaster As ray_cast
    Private utiliser As New utilities()

    Private timera As New Timer()
    Private timer_num = 0

    Private start_time As Date
    Private end_time As Date

    Private finish_scrn As New finish_screen()

    Private plr1 As New player(Me, 40, 40, 0, 0, Me.utiliser)

    Private char_crea As New character_creator(plr1, Me)
    Private intru_men As New instruction_menu(Me)


    Private walls_list As New List(Of wall)
    Private usable_wallz_list As New List(Of wall)
    Private enemy_list As New List(Of base_enemy)
    Private boss_list As New List(Of boss)
    Private door_list As New List(Of door)
    Private floor_items As New List(Of item)
    Private NPC_list As New List(Of Npc)
    Private new_floor_items As New List(Of item)

    Private type_bos As String = "spider"

    Private dead_enemys As New List(Of base_enemy)

    Private map_generator As New map_maker(utiliser, 14, 16)

    Private text_map1 As New List(Of String)
    Private text_map2 As New List(Of String)
    Private NPC_village As New List(Of String)
    Private text_map3 As New List(Of String)
    Private boss_floor As New List(Of String)
    Private text_mapp_used As List(Of String)

    Private all_missions As New Dictionary(Of List(Of String), List(Of Integer))
    Private all_missions_txt As New List(Of List(Of String))

    Private mousex As Integer
    Private mousey As Integer

    Private all_buttons As New List(Of buttono)

    Private start_btn As New buttono(Me, 475, 350, 200, 90, "start")
    Private exit_btn As New buttono(Me, 250, 350, 200, 90, "exit")
    Private instruct_btn As New buttono(Me, 350, 450, 200, 90, "instructions")
    Private load_btn As New buttono(Me, 350, 250, 200, 90, "load previous save")

    Private save_btn As New buttono(Me, 770, 650, 100, 100, "save")

    Private finished_menu As Boolean = False

    Private start_text As New text(Me, 300, 100, 50, "rpg game", Color.Red)

    Private game_has_start As Boolean = False

    Private id_enemy As Integer = 1
    Private id_npc As Integer = 0

    Private enemy_count_txt As New text(Me, 250, 0, 25, "enemy count:0", Color.Red)



    Private Sub create_map()

        Dim enemy_health As New Dictionary(Of String, Integer) From {
                {"spider", 150},
                {"invisi", 200},
                {"goblin", 2},
                {"warlock", 4}
        }

        'create coordinates for walls for ray_casting, to make the ray casting more optimizes instead of checking each wall seperately

        Me.raycaster = New ray_cast(Me.text_mapp_used)


        'loads the text map into visualized the player can interact with.
        'whilst also making terrain generation and adding in different structure randomly incredibly easy

        For y_add = 1 To text_mapp_used.Count() - 1
            For x_add = 0 To text_mapp_used(y_add).Length() - 1
                If text_mapp_used((text_mapp_used.Count() - 1) - (y_add - 1))(x_add) = "p" Then
                    ' moves the already loaded player image to the starting locations, so i dont need to re-load the player each new map
                    plr1.x = x_add * 50
                    plr1.y = 800 - (y_add * 50)

                ElseIf text_mapp_used((text_mapp_used.Count() - 1) - (y_add - 1))(x_add) = "e" Then

                    Dim new_goblin As New goblin_enemy(Me, 45, 45, x_add * 50, 800 - (y_add * 50), Me.id_enemy, enemy_health("goblin"), Me.utiliser)

                    new_goblin.load_attacks_fromtxt()

                    enemy_list.Add(new_goblin)

                    Me.id_enemy += 1
                ElseIf text_mapp_used((text_mapp_used.Count() - 1) - (y_add - 1))(x_add) = "b" Then
                    Dim new_wall As New wall(Me, 50, 50, x_add * 50, 800 - (y_add * 50))
                    new_wall.text_mapp_used = Me.text_mapp_used

                    If new_wall.real_wall(y_add, x_add) Then

                        Me.usable_wallz_list.Add(new_wall)

                    Else

                        Me.walls_list.Add(new_wall)

                    End If

                ElseIf text_mapp_used((text_mapp_used.Count() - 1) - (y_add - 1))(x_add) = "w" Then

                    Dim new_warlock As New warlock_enemy(Me, 45, 45, x_add * 50, 800 - (y_add * 50), Me.id_enemy, enemy_health("warlock"), Me.utiliser, True)

                    new_warlock.load_attacks_fromtxt()

                    enemy_list.Add(new_warlock)

                    Me.id_enemy += 1
                ElseIf text_mapp_used((text_mapp_used.Count() - 1) - (y_add - 1))(x_add) = "d" Then

                    Dim new_box As New door(Me, 50, 50, x_add * 50, 800 - (y_add * 50), text_mapp_used(0))

                    door_list.Add(new_box)

                ElseIf text_mapp_used((text_mapp_used.Count() - 1) - (y_add - 1))(x_add) = "N" Then

                    Dim new_box As New Npc(Me, Me.plr1, x_add * 50, 800 - (y_add * 50), 50, 50, Me.all_missions_txt(Me.id_npc), Me.utiliser)

                    Me.NPC_list.Add(new_box)

                    Me.id_npc += 1


                ElseIf text_mapp_used((text_mapp_used.Count() - 1) - (y_add - 1))(x_add) = "E" Then
                    'a boss entity
                    Dim new_boss As New boss(Me, x_add * 50, 800 - (y_add * 50), 45, 45, Me.type_bos, enemy_health(Me.type_bos), Me.plr1, Me.utiliser)

                    new_boss.load_attacks_fromtxt("none")

                    Me.enemy_list.Add(new_boss)
                    Me.boss_list.Add(new_boss)

                End If


            Next
        Next
        '--------------------------------------------------------------------------------------------------------------

    End Sub

    Private Sub plr_finished()
        If Me.plr1.equipped(0) = "player_eye" Then
            Me.plr1.finished_game = True
        End If

        'if the user has beaten the game
        If Me.plr1.finished_game Then

            'change the enemy count text 
            Me.enemy_count_txt.my_text = "GAME FINISHED!!!!!!!"
            Me.enemy_count_txt.colour = Color.HotPink

            Me.enemy_count_txt.load()

            Me.plr1.finished_game = False

            If Me.timer_num Mod 20 = 0 Then
                'saves the time taken to finish the game , to a text file so all previous times can be ranked and shown to the user, in the finish_screen
                Me.end_time = DateTime.Now

                Dim elapsed As TimeSpan = Me.end_time - Me.start_time
                Dim elapsedString As String = elapsed.ToString("hh\:mm\:ss")

                Me.utiliser.save_score(elapsedString)

                Me.game_has_start = False
                Me.finished_menu = True

            End If

        End If
        '--------------------------------------
        'checks if the player has died
        If Me.plr1.hp <= 0 Then

            Me.enemy_count_txt.my_text = "U DEAD!!!!!!!"
            Me.enemy_count_txt.colour = Color.Yellow

            Me.enemy_count_txt.load()

            'gives a delay so the user can recognize their death and so it isnt an instant change
            If Me.timer_num Mod 10 = 0 Then
                Application.Restart()
            End If

        End If
        '-------------------------------
    End Sub


    Private Sub change_room()
        If plr1.touch_door And Me.enemy_list.Count() <= 0 Then

            Me.clear_map()

            Dim new_gen_room As New map_maker(Me.utiliser, 14, 16)

            'room for only NPC's
            Dim new_gen_village As New map_maker(Me.utiliser, 14, 16, "vil")

            'loads in a boss room
            Dim new_gen_bossrm As New map_maker(Me.utiliser, 14, 16, "bossrm")

            Me.plr1.boss_room = False

            'checks which room to go to, by looking at the first string in the text map
            If plr1.which_door.goes_to = "floor2" Then

                text_map2 = new_gen_room.load_map()
                text_map2(0) = "vil"

                Me.text_mapp_used = text_map2

            ElseIf plr1.which_door.goes_to = "vil" Then

                NPC_village = new_gen_village.load_map()
                NPC_village(0) = "spider_den"
                Me.type_bos = "spider"
                Me.text_mapp_used = Me.NPC_village

            ElseIf plr1.which_door.goes_to = "floor4" Then

                text_map3 = new_gen_room.load_map()
                text_map3(0) = "invisi_den"

                Me.text_mapp_used = text_map3

                Me.type_bos = "invisi"

            ElseIf plr1.which_door.goes_to = "spider_den" Then

                boss_floor = new_gen_bossrm.load_map(True)
                boss_floor(0) = "floor4"

                Me.plr1.boss_room = True

                Me.text_mapp_used = Me.boss_floor

            ElseIf plr1.which_door.goes_to = "invisi_den" Then

                boss_floor = new_gen_bossrm.load_map(True)
                boss_floor(0) = "invisi_den"

                Me.plr1.boss_room = True

                Me.text_mapp_used = Me.boss_floor

            End If
            '-----------------------------------------------------------------------------

            plr1.touch_door = False

            Me.plr1.my_interface.check_boss_bar(Me.plr1.boss_room)
            Me.load_game()

        End If
    End Sub


    Private Sub amount_of_en_txt()
        'update the text that shows the number of enemies
        If Me.enemy_list.Count() < 1 Then
            Me.enemy_count_txt.colour = Color.LightGreen

        Else
            Me.enemy_count_txt.colour = Color.Red

        End If
        '------------------------------------------------

        Me.enemy_count_txt.update("enemy count:" & (Me.enemy_list.Count()).ToString)
    End Sub


    Private Sub update_flr_items()

        Dim item_id As Integer = plr1.collided_flooritem

        'checks if the player has collided with a floor item
        If item_id <> -1 Then

            'the player gives the item id which can be used to decide what to do with it
            Me.Controls.Remove(Me.floor_items(item_id).box)

            Me.floor_items.RemoveAt(item_id)

        End If
        '---------------------------------------------------

    End Sub

    Private Sub update_npcs()

        For Each Npc In Me.NPC_list
            Npc.update()
        Next

    End Sub

    Private Sub update_enemies()

        For Each enemy In enemy_list

            enemy.update(Me.usable_wallz_list, Me.plr1, Me.enemy_list, Me.text_mapp_used, Me.timer_num)

            If enemy.hp <= 0 Then
                enemy.dead = True
            End If

            'update all attacks in use right now
            For Each atk In enemy.live_attacks
                atk.update()
            Next
            '----------------------------------

            If enemy.dead Then

                'check if an enemy has been killed and which one has been killed ,for missions
                If Me.plr1.killed_enemy_amounts.ContainsKey(enemy.type_enemy) Then
                    Me.plr1.killed_enemy_amounts(enemy.type_enemy) += 1

                Else
                    Me.plr1.killed_enemy_amounts(enemy.type_enemy) = 1

                End If
                '-----------------------------------------------------------------------------

                Dim new_item As item

                'drop an item or add one to the players player_inteface when killed
                If enemy.type_enemy <> "spider" And enemy.type_enemy <> "invisi" Then

                    If Me.utiliser.Rand0() > 50 Then
                        new_item = New item(Me, 35, 35, enemy.x, enemy.y, "hp_potion")
                        Me.new_floor_items.Add(new_item)

                    Else
                        new_item = New item(Me, 35, 35, enemy.x, enemy.y, "mana_potion")
                        Me.new_floor_items.Add(new_item)

                    End If


                ElseIf enemy.type_enemy = "spider" Then

                    new_item = New item(Me, 35, 35, enemy.x, enemy.y, "shield")
                    Me.new_floor_items.Add(new_item)

                    new_item = New item(Me, 35, 35, enemy.x, enemy.y, "helmet")
                    Me.new_floor_items.Add(new_item)

                ElseIf enemy.type_enemy = "invisi" Then

                    new_item = New item(Me, 35, 35, enemy.x, enemy.y, "player_eye")
                    Me.new_floor_items.Add(new_item)

                End If
                '-------------------------------------------------------------

                'add the new floor item to the main floor items list
                For Each item In Me.new_floor_items
                    item.make_image()
                    Me.floor_items.Add(item)

                    item.load()
                Next
                Me.new_floor_items.Clear()
                '----------------------------------------------------

                Me.Controls.Remove(enemy.box)
                Me.dead_enemys.Add(enemy)

                For Each boss In boss_list

                    If boss.type_enemy = "spider" Then

                        For z = 0 To 5
                            For x = 0 To 3
                                Me.Controls.Remove(boss.thighs(z)(x))
                                Me.Controls.Remove(boss.femur(z)(x))
                            Next
                        Next

                        boss.thighs.Clear()
                        boss.femur.Clear()
                    ElseIf boss.type_enemy = "invisi" Then

                        For Each cu In boss.cubes
                            Me.Controls.Remove(cu.box)
                        Next

                        For Each limb_cu In boss.body_parts
                            Me.Controls.Remove(limb_cu.box)
                        Next

                        boss.cubes.Clear()
                        boss.body_parts.Clear()
                    End If
                Next

                If enemy.type_magic Then

                    For Each j In enemy.live_attacks
                        enemy.dead_attacks.Add(j)
                    Next

                    enemy.live_attacks.Clear()
                End If

            End If

            If enemy.type_magic Then

                For Each spell In enemy.dead_attacks
                    Me.Controls.Remove(spell.box)
                Next

                enemy.dead_attacks.Clear()
            End If

        Next

        For Each i In dead_enemys

            Me.enemy_list.Remove(i)
            For Each j In i.all_limbs
                Me.Controls.Remove(j)
            Next

            i.all_limbs.Clear()

        Next
        Me.dead_enemys.Clear()

    End Sub


    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'loads the start of the game, the entry page
        Me.Size = New Size(900, 850)

        'adds the buttons to their own list
        all_buttons.Add(Me.start_btn)
        all_buttons.Add(Me.exit_btn)
        all_buttons.Add(Me.instruct_btn)
        all_buttons.Add(Me.load_btn)

        'loads the missions and sets the possible player mission to the missions now stored in a dictionary
        'the reason this is not done in the player_vlass is because the NPC class uses this data as well

        Me.plr1.all_missions = Me.utiliser.load_missions()
        Me.all_missions_txt = Me.utiliser.add_missions(Me.plr1.all_missions)


        'loads the starting text at the top of the screen
        Me.start_text.load()

        'loads the buttons in the list
        For Each i In all_buttons
            'handles clicking
            AddHandler i.mybutton.Click, AddressOf i.Me_OnClick

            i.load()

        Next

        AddHandler Me.MouseMove, AddressOf Me.utiliser.Form1_MouseMove
        'turn a the timera sub into the main game loop
        AddHandler Me.timera.Tick, AddressOf Me.Timer1_Tick

        timera.Interval = 50
        timera.Start()

    End Sub

    Private Sub load_game()
        'loads all the sprites for each new map
        create_map()

        Me.BackColor = Color.FromArgb(99, 107, 118)


        'loads the enemy_count text
        Me.enemy_count_txt.load()

        'only initializes the player once in the first map, no need to do it again
        'the reason it says floor2 is because this points to the next map , not the one being used
        'This makes doors easy to create and use
        If Me.text_mapp_used(0) = "floor2" Or Me.utiliser.loaded_game Then

            Me.plr1.load_attacks()

            Me.plr1.my_interface = New player_inteface(Me, Me.utiliser)
            Me.plr1.attack_img = New Bitmap(Me.plr1.attack_dict("fireball")(2))

            Me.plr1.my_interface.load_bars()
            Me.plr1.my_interface.load_inventory()
            Me.plr1.my_interface.load_ui(plr1)
            Me.plr1.my_interface.load_armour_stats()


            Me.plr1.my_sheild = New sheild(Me, Me.plr1, 50, 50, 0, 0, 100)


            Me.plr1.my_sheild.sheild_hp_bar = New bar(Me, 100, 20, 755, 230, Color.Brown, Color.LightGray)

            AddHandler Me.MouseClick, AddressOf Me.plr1.click_handler
            AddHandler Me.MouseWheel, AddressOf Me.plr1.scroll_wheel
            AddHandler Me.KeyPress, AddressOf Me.plr1.key_handler

            'if the user finishes the game their time is ranked on their local computer against other scores saved
            Me.start_time = DateTime.Now

            Me.save_btn.load()

            AddHandler Me.save_btn.mybutton.Click, AddressOf Me.save_btn.Me_OnClick

            Me.utiliser.loaded_game = False

            Me.plr1.my_interface.add("helmet")

        Else

            Me.plr1.xvel = 0
            Me.plr1.yvel = 0

            Me.plr1.update(Me.usable_wallz_list, Me.timer_num, Me.mousex, Me.mousey, Me.enemy_list, door_list, floor_items, NPC_list)
        End If
        '---------------------------------------------------------------------------------

        'starts the enemy killed counter again
        Me.plr1.start_level = False

        Const wall_img_path As String = "\\cc4sjd8\2024 Intake\24026166\Visual Studio 2010\Projects\practice_courswork_platformer\wall_imgs\tile_"
        'loads all fake walls
        For Each i In walls_list
            i.load()
            i.change_img(text_mapp_used, wall_img_path)
        Next

        'loads all real walls
        For Each i In Me.usable_wallz_list
            i.load()
            i.change_img(text_mapp_used, wall_img_path)
        Next

        'loads all NPC's and makes it clickable
        For Each npx In NPC_list

            AddHandler npx.box.MouseClick, AddressOf npx.click_handler
            AddHandler npx.eyebox.MouseClick, AddressOf npx.click_handler
            AddHandler npx.eye.MouseClick, AddressOf npx.click_handler

            npx.load()
            npx.limb_load()

            npx.eye.Top += 15
            npx.eyebox.Top += 15

            npx.eyebox.BringToFront()
            npx.eye.BringToFront()

        Next

        'loads each enemy, makes they clickable so if the mouse is over them attacks can still be used

        For Each i In enemy_list

            AddHandler i.box.MouseClick, AddressOf Me.plr1.click_handler
            AddHandler i.eyebox.MouseClick, AddressOf Me.plr1.click_handler
            AddHandler i.eye.MouseClick, AddressOf Me.plr1.click_handler

            'instead of creating a wall map , and ray caster object for each enemy one is just shared among the many
            i.raycast_obj = Me.raycaster

            i.load()

            i.limb_load()

        Next

        'loads each door
        For Each i In Me.door_list

            i.load()

        Next



        Me.DoubleBuffered = True
        Me.KeyPreview = True
    End Sub

    Private Sub clear_map()
        'clear all dead attacks incase attacks that are dead are still store in memory, remove them from their list
        For Each atk In plr1.dead_attack
            Me.Controls.Remove(atk.box)
        Next
        '-----------------------------------------------------------

        'clear all pickup items from graphics memory and their list
        For Each item In floor_items
            Me.Controls.Remove(item.box)
        Next
        floor_items.Clear()
        '----------------------------------

        'clears all the walls from graphics memory and their list
        For Each wall In Me.walls_list
            Me.Controls.Remove(wall.box)
        Next
        Me.walls_list.Clear()
        '--------------------------

        'clear all collidable walls from graphics memory and their list
        For Each u_wall In Me.usable_wallz_list
            Me.Controls.Remove(u_wall.box)
        Next
        Me.usable_wallz_list.Clear()
        '-------------------------------------------------------------

        'clear all enemies from graphics memory and from their list
        For Each ene In Me.enemy_list
            Me.Controls.Remove(ene.box)
            'clear each enemies attacks
            For Each j In ene.live_attacks
                Me.Controls.Remove(j.box)
            Next
            '-------------------------

            'clear all the limbs of enemies
            For Each j In ene.all_limbs
                Me.Controls.Remove(j)
            Next
            '---------------------------

            ene.all_limbs.Clear()
            ene.live_attacks.Clear()
        Next
        enemy_list.Clear()
        '----------------------------------------------------------

        'clear all NPC's in graphics memory
        For Each npc In Me.NPC_list
            Me.Controls.Remove(npc.box)

            'clear their limbs
            For Each limb In npc.all_limbs
                Me.Controls.Remove(limb)
            Next

            npc.del_text_box()
            npc.all_limbs.Clear()
        Next
        NPC_list.Clear()
        '---------------------------------

        'clear all boss entities from graphics memory and clear from their list
        For Each boss In boss_list

            If boss.type_enemy = "spider" And Not boss.dead Then
                'clear the spider legs
                For leg = 0 To 5
                    For leg_part = 0 To 3
                        Me.Controls.Remove(boss.thighs(leg)(leg_part))
                        Me.Controls.Remove(boss.femur(leg)(leg_part))
                    Next
                Next

                boss.thighs.Clear()
                boss.femur.Clear()

            ElseIf boss.type_enemy = "invisi" And Not boss.dead Then
                'clear the orbiting cubes
                For Each cube In boss.cubes
                    Me.Controls.Remove(cube.box)
                Next

                'clear cubes around the main body , that make it seem circular
                For Each limb In boss.body_parts
                    Me.Controls.Remove(limb.box)
                Next

            End If

            boss.cubes.Clear()
            boss.body_parts.Clear()

        Next
        boss_list.Clear()

        '------------------------------------------

        'clear all doors from graphical memory and their list
        For Each door In Me.door_list
            Me.Controls.Remove(door.box)
        Next
        door_list.Clear()
        '----------------------------------------------------

    End Sub

    Private Sub clear_start_menu()

        For Each but In all_buttons
            but.destroy()
        Next

        'removes the title of the game
        Me.start_text.destroy()

        all_buttons.Clear()
    End Sub

    Private Sub load_old_game()
        Dim loaded As List(Of List(Of String)) = Me.utiliser.load_map()

        Me.text_mapp_used = loaded(0)

        For ind = 0 To 2
            Me.plr1.all_stats(ind) = loaded(1)(ind)
        Next

        Dim body_cols() As String = loaded(1)(3).Split("\")
        plr1.box.BackColor = Color.FromArgb(body_cols(0), body_cols(1), body_cols(2))

        Dim arm_cols() As String = loaded(1)(4).Split("\")

        plr1.leftArm.BackColor = Color.FromArgb(arm_cols(0), arm_cols(1), arm_cols(2))
        plr1.rightArm.BackColor = Color.FromArgb(arm_cols(0), arm_cols(1), arm_cols(2))

        Dim leg_cols() As String = loaded(1)(5).Split("\")

        plr1.rightLeg.BackColor = Color.FromArgb(leg_cols(0), leg_cols(1), leg_cols(2))
        plr1.leftLeg.BackColor = Color.FromArgb(leg_cols(0), leg_cols(1), leg_cols(2))

        Me.game_has_start = True

        Me.utiliser.loaded_game = True
        Me.load_btn.been_clicked = False

        Me.plr1.load()
        Me.plr1.limb_load()
    End Sub

    Private Sub game_update()
        'update all entities
        Me.plr1.update(Me.usable_wallz_list, Me.timer_num, Me.mousex, Me.mousey, Me.enemy_list, Me.door_list, Me.floor_items, Me.NPC_list)

        update_flr_items()
        update_enemies()

        amount_of_en_txt()
        change_room()
        update_npcs()

        plr_finished()
        'cannot save the game in a boss room
        If Me.save_btn.been_clicked And Not Me.plr1.boss_room Then

            Me.utiliser.save_map(Me.text_mapp_used, plr1)
            Me.save_btn.been_clicked = False

        End If

        Me.mousex = Me.utiliser.mousex
        Me.mousey = Me.utiliser.mousey

        Me.timer_num += 1
    End Sub

    Private Sub start_screen()
        'gives certain buttons for the enter and end screen functionality
        If Me.finished_menu Then
            'show the end screen, hide this screen
            Me.finish_scrn.Show()
            Me.Hide()

            Me.finished_menu = False

        ElseIf Me.start_btn.been_clicked Then
            clear_start_menu()

            If Not char_crea.created Then

                Me.char_crea.load()
                Me.char_crea.created = True

            End If
            'begin the game, load in the first room
            If Me.char_crea.donebtn.been_clicked Then

                text_map1 = map_generator.load_map()
                text_map1(0) = "floor2"
                Me.text_mapp_used = text_map1

                Me.game_has_start = True
                load_game()

            End If

            Me.char_crea.update()

        ElseIf Me.instruct_btn.been_clicked Then
            Me.clear_start_menu()

            If Not Me.intru_men.created Then

                Me.intru_men.load()
                Me.intru_men.created = True

            Else

                Me.intru_men.update()

            End If
        ElseIf Me.load_btn.been_clicked Then

            clear_start_menu()

            load_old_game()

            load_game()



        ElseIf Me.exit_btn.been_clicked Then
            Application.Exit()

        End If
        '---------------------------------------------------------

    End Sub


    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'if the game has started begin the main loop, else stay on the start screen/s
        If Me.game_has_start Then
            game_update()
        Else
            start_screen()
        End If

    End Sub


End Class
