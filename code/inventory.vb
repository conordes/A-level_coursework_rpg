Imports System.Drawing
Imports System.Drawing.Imaging

Public Class inventory
    Public game As Form

    'also controls how missions are used and how UI is controlled

    'inventory:
    Public all_items As New List(Of item)

    Public itemDict As New Dictionary(Of String, List(Of Integer))()
    '----------------------

    'missions:
    Public missions_npcs As New List(Of Npc)
    '----------------------

    'UI:
    Public hp_bar As bar
    Public mana_bar As bar
    Public attack_bar As bar
    Public boss_bar As bar
    Public boss_room As Boolean = False

    Public ui_lis As New List(Of PictureBox)
    Public all_cubes As New List(Of base_block)

    Public chosen_attack As New PictureBox
    '----------------------

    Public Sub New(ByVal form_used As Form, ByVal util As utilities)

        Me.game = form_used

        Me.load_equipped()
        Me.itemDict = util.load_inventory_items()

    End Sub

    'UI:

    Public Sub load_bars()

        hp_bar = New bar(Me.game, 100, 20, 760, 160, Color.Red, Color.Green, -1, True)
        mana_bar = New bar(Me.game, 100, 20, 760, 100, Color.Blue, Color.Yellow, -1, True)
        attack_bar = New bar(Me.game, 60, 5, 765, 90, Color.Black, Color.LightGreen)
        boss_bar = New bar(Me.game, 500, 30, 170, 80, Color.Black, Color.DarkRed, 9, True, 15)

        'create the boss bar but remove it from the screen so it is still stored in memory
        Me.boss_bar.load_all()
        Me.boss_bar.destroy()
        '--------------------------------------------------------------------------------

        Me.attack_bar.load_all()
        Me.hp_bar.load_all()
        Me.mana_bar.load_all()

    End Sub

    Public Sub check_boss_bar()

        'if in a boss room load the boss bar
        If Me.boss_room And Not Me.boss_bar.bar_alive Then
            Me.boss_bar.load_all()
            Me.boss_bar.bar_alive = True

        ElseIf Me.boss_bar.bar_alive And Not Me.boss_room Then
            Me.boss_bar.destroy()
            Me.boss_bar.bar_alive = False

        End If
        '----------------------------------

    End Sub

    Public Sub load_ui(ByVal plr As player)

        'load in the chosen attack image
        chosen_attack.BackColor = Color.Transparent
        chosen_attack.Size = New Size(50, 50)

        chosen_attack.Image = plr.attack_img

        chosen_attack.Left = 770
        chosen_attack.Top = 45
        '--------------------------

    End Sub

    Public Sub load_ui_again(ByVal plr As player)

        'change the image of the attack being used by the player
        chosen_attack.Image = plr.attack_img
        chosen_attack.SizeMode = PictureBoxSizeMode.StretchImage

        Me.game.Controls.Add(chosen_attack)
        '-------------------------------------------------------

        Me.attack_bar.load_all()

        Me.check_boss_bar()

    End Sub

    Public Sub delete_ui()
        'move the hp and mana bars out of the way for the inventory
        Me.hp_bar.stat_text.box.Left = 800
        Me.hp_bar.stat_text.box.Top = 570

        Me.mana_bar.stat_text.box.Left = 800
        Me.mana_bar.stat_text.box.Top = 520
        '---------------------------------------------------------

        Me.attack_bar.destroy()
        Me.game.Controls.Remove(Me.chosen_attack)

    End Sub

    Public Sub load_equipped()
        'create a box for each part of the body that can have armour
        Dim helmet As New PictureBox
        Dim body As New PictureBox
        Dim gauntlets As New PictureBox
        Dim legs As New PictureBox
        Dim boots As New PictureBox
        Dim equipe_r As New PictureBox
        Dim equipe_l As New PictureBox
        '-----------------------------------------------------------

        'addd these peices to a ui list
        ui_lis.Add(helmet)
        ui_lis.Add(body)
        ui_lis.Add(gauntlets)
        ui_lis.Add(legs)
        ui_lis.Add(boots)
        ui_lis.Add(equipe_r)
        ui_lis.Add(equipe_l)
        '----------------------------

        'creates a grey box for each item
        For i = 0 To ui_lis.Count() - 1
            ui_lis(i).BackColor = Color.LightGray
            ui_lis(i).Size = New Size(50, 50)

            ui_lis(i).Left = 765
            ui_lis(i).Top = (i * 55) + 10

            ui_lis(i).SizeMode = PictureBoxSizeMode.StretchImage
        Next
        '--------------------------------

    End Sub

    Public Sub show_equipped()

        'show the armour peices used
        For Each item In Me.ui_lis
            Me.game.Controls.Add(item)
        Next

    End Sub

    Public Sub load_armour_stats()
        For ind = 0 To 8
            Dim new_arm As New base_block(Me.game, 20, 20, 25 + (25 * ind), 750)

            new_arm.box.BackColor = Color.Black
            new_arm.load()

            Me.all_cubes.Add(new_arm)

        Next
    End Sub


    'missions:

    Public Sub load_mis()

        'loads all chosen missions into veiw under the player inventory
        For mis = 0 To Me.missions_npcs.Count() - 1

            'NOT(has finished the mission and claimed the reward)
            If Not Me.missions_npcs(mis).done_mis Then

                Me.missions_npcs(mis).mission_txt.y = 500 + (mis * 50)

                Me.missions_npcs(mis).mission_txt.border = True
                Me.missions_npcs(mis).mission_txt.load()

            End If

        Next
        '--------------------------------------------------------------

    End Sub

    Public Sub delete_mis()

        'gets rid of all the missions so a different ui can be shown
        For mis = 0 To Me.missions_npcs.Count() - 1

            If Not Me.missions_npcs(mis).done_mis Then

                Me.missions_npcs(mis).mission_txt.destroy()

            End If

        Next
        '-----------------------------------------------------------

    End Sub

    'Inventory:

    Public Function darken_bitmap(ByVal original As Bitmap, ByVal darkenfactor As Single)
        'darken the item picture when clicked on, giving feedback to the user

        'the matrix to darken an image
        Dim col_mat As New ColorMatrix(New Single()() {
            New Single() {darkenfactor, 0, 0, 0, 0},
            New Single() {0, darkenfactor, 0, 0, 0},
            New Single() {0, 0, darkenfactor, 0, 0},
            New Single() {0, 0, 0, 1, 0},
            New Single() {0, 0, 0, 0, 1}
        })

        Dim img_atri As New ImageAttributes()
        img_atri.SetColorMatrix(col_mat)

        Dim darkbmp As New Bitmap(original.Width, original.Height)

        Using g As Graphics = Graphics.FromImage(darkbmp)

            g.DrawImage(original, New Rectangle(0, 0, darkbmp.Width, darkbmp.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, img_atri)

        End Using
        '----------------------------------------------------------------------
        Return darkbmp
    End Function

    Public Function not_in(ByVal val1 As String, ByVal lisz As List(Of item))
        'check if an item is in a list
        For Each item In lisz

            If item.name = val1 Then
                Return False
            End If

        Next
        '-----------------------------

        Return True

    End Function

    Public Sub load_inventory_imgs()
        'load all item images in the inventory
        Dim x_coord As Integer = 0
        Dim y_coord As Integer = 0

        For item = 0 To all_items.Count() - 1

            all_items(item).x = 855 + (x_coord * 60)
            all_items(item).y = (y_coord * 60) + 55

            If all_items(item).x > 1250 Then
                x_coord = 0
                y_coord += 1

                all_items(item).x = 855 + (x_coord * 60)
                all_items(item).y = (y_coord * 60) + 55
            End If

            x_coord += 1

            all_items(item).load()
            all_items(item).show_amount()

            all_items(item).loaded_text = True

        Next
        '--------------------------------------------
    End Sub

    Public Sub load_inventory()

        Me.load_inventory_imgs()

        'change location of the bars
        Me.hp_bar.new_loc(800, 550)
        Me.mana_bar.new_loc(800, 500)
        '----------------------------

    End Sub

    Public Sub draw_grid(ByVal frame As PictureBox, ByVal cellSize As Integer, ByVal cols As Integer, ByVal rows As Integer)
        'draws the grid looking area for the inventory

        Dim bit_map As New Bitmap(frame.Width, frame.Height)

        Dim g As Graphics = Graphics.FromImage(bit_map)
        Dim pen As New Pen(Color.LightGray, 5)

        ' Draw vertical lines
        For col As Integer = 0 To cols

            Dim x As Integer = col * cellSize
            g.DrawLine(pen, x, 0, x, rows * cellSize)

        Next

        ' Draw horizontal lines
        For row As Integer = 0 To rows

            Dim y As Integer = row * cellSize
            g.DrawLine(pen, 0, y, cols * cellSize, y)

        Next

        frame.Image = bit_map
        '-----------------------------------------------

    End Sub

    Public Sub delete_inventory()
        'delete all items in inventory
        For Each item In Me.all_items

            Me.game.Controls.Remove(item.box)

            If item.loaded_text Then

                item.amount_text.destroy()

            End If

        Next

        For Each i In Me.ui_lis
            Me.game.Controls.Remove(i)
        Next
        'move hp and mana bar to their og location
        Me.hp_bar.new_loc(755, 160)
        Me.mana_bar.new_loc(755, 100)

        Me.delete_mis()

    End Sub

    Public Sub add(ByVal name As String)
        Dim where As Boolean = Me.not_in(name, Me.all_items)

        If where Then
            'if it is not in the inventory add it
            Dim new_item As New item(Me.game, 50, 50, 0, 0, name)

            new_item.image = New Bitmap("\\cc4sjd8\2024 Intake\24026166\Visual Studio 2010\Projects\practice_courswork_platformer\invent_imgs\" & name & ".png")
            new_item.box.SizeMode = PictureBoxSizeMode.StretchImage

            'make it clickable
            AddHandler new_item.box.MouseClick, AddressOf new_item.item_MouseClick

            Me.all_items.Add(new_item)

        ElseIf Me.itemDict(name)(0) <> 2 And Me.itemDict(name)(0) <> 3 Then

            'if it is in the inventory stack it (if stackable)
            For Each item In Me.all_items

                If item.name = name Then
                    item.amount += 1
                End If

            Next

        End If



    End Sub

    Public Sub clear_inventory()
        'so the list of all_items doesnt chnge whilst being iterated through
        Dim getrifoff As New List(Of Integer)

        'if it is more then 1 then add it to the list
        For item = 0 To all_items.Count() - 1

            If all_items(item).amount < 1 Then
                getrifoff.Add(item)
            End If

        Next
        '--------------------------------------------------------------

        For Each pic In getrifoff

            Me.game.Controls.Remove(all_items(pic).box)
            Me.game.Controls.Remove(all_items(pic).amount_text.box)

            all_items.RemoveAt(pic)
        Next

    End Sub

    Public Function update(ByVal tick As Integer, ByVal plr As player)
        Me.clear_inventory()

        chosen_attack.Image = plr.attack_img

        'update the armour slots depending on wther items are equipped
        For armour_slot = 0 To plr.equipped.Count() - 1

            If plr.equipped(armour_slot) <> Nothing And plr.equipped(armour_slot) <> "0" Then
                ui_lis(armour_slot).Image = New Bitmap("\\cc4sjd8\2024 Intake\24026166\Visual Studio 2010\Projects\practice_courswork_platformer\invent_imgs\" & plr.equipped(armour_slot) & ".png")

            ElseIf plr.equipped(armour_slot) = "0" Then
                'get rid of old slot
                ui_lis(armour_slot).Dispose()

                'add new slot
                Dim new_slot As New PictureBox()

                new_slot.BackColor = Color.LightGray
                new_slot.Size = New Size(50, 50)

                new_slot.Left = 765
                new_slot.Top = (armour_slot * 55) + 10

                new_slot.SizeMode = PictureBoxSizeMode.StretchImage

                ui_lis(armour_slot) = new_slot

            End If

        Next
        '-------------------------------------------------------------

        'check for items clicks
        For Each item In all_items
            item.update()

            If tick Mod 6 = 0 Then
                item.box.Image = item.image
            End If

            If item.touched Then

                'darken the image and return a full tuple

                item.box.Image = darken_bitmap(item.image, 0.6)
                item.touched = False

                Return Tuple.Create(item.name, Me.itemDict(item.name)(0), item)
            End If
        Next
        '------------------------

        'return the item name, how many there are, the item class
        Return Tuple.Create("", -1, "")

    End Function
End Class
