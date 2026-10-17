Public Class character_creator

    Public game As Form
    Private plr1 As player

    Private str_bar As bar
    Private str_txt As text

    Private dex_bar As bar
    Private dex_txt As text
    Private wis_bar As bar
    Private wis_txt As text

    Private bar_list As New List(Of bar)
    Private txt_list As New List(Of text)
    Private btn_list As New List(Of List(Of buttono))
    Private image_list As New List(Of PictureBox)
    Private lone_btn_list As New List(Of buttono)

    Private _donebtn As buttono
    Public Property donebtn As buttono
        Get
            Return _donebtn
        End Get
        Set(ByVal value As buttono)
            _donebtn = value
        End Set
    End Property

    Private next_limb As buttono
    Private bef_limb As buttono

    Private attribute_pnts As Integer = 25
    Private attri_txt As text

    'dictionary of each limb and wach rgb value that each one has
    Private colour_limbs As New Dictionary(Of Integer, List(Of Integer))

    Private color_btns As New List(Of buttono)
    Private rgb_vals As New List(Of String)
    Private limb_on As Integer = 0
    Private limb_on_txt As text

    Public created As Boolean = False

    Public ended_char_crea As Boolean = False

    Sub New(ByVal plr As player, ByVal mygame As Form)

        Me.game = mygame
        Me.plr1 = plr

    End Sub

    Private Sub make_bars()

        str_bar = New bar(Me.game, 200, 40, 600, 500, Color.DarkGray, Color.Black)
        dex_bar = New bar(Me.game, 200, 40, 600, 300, Color.DarkGray, Color.Black)
        wis_bar = New bar(Me.game, 200, 40, 600, 100, Color.DarkGray, Color.Black)

        str_txt = New text(Me.game, 600, 450, 20, "strength", Color.DarkGray)
        dex_txt = New text(Me.game, 600, 250, 20, "dexterity", Color.DarkGray)
        wis_txt = New text(Me.game, 600, 50, 20, "wisdom", Color.DarkGray)

        Me.plr1.all_stats(0) = 0
        Me.plr1.all_stats(1) = 0
        Me.plr1.all_stats(2) = 0


        bar_list.Add(str_bar)
        bar_list.Add(dex_bar)
        bar_list.Add(wis_bar)


        txt_list.Add(str_txt)
        txt_list.Add(dex_txt)
        txt_list.Add(wis_txt)

        For Each i In Me.bar_list
            i.load_all()
            i.new_size(0)
        Next

    End Sub

    Private Sub make_buttons()
        'loads in buttons--------------------------
        For butn_num = 0 To bar_list.Count() - 1

            Dim new_btn1 As New buttono(Me.game, bar_list(butn_num).x - 50, bar_list(butn_num).y - 10, 50, 50, "dec")
            Dim new_btn2 As New buttono(Me.game, bar_list(butn_num).x + bar_list(butn_num).width, bar_list(butn_num).y - 10, 50, 50, "inc")

            Dim newli As New List(Of buttono)

            newli.Add(new_btn1)
            newli.Add(new_btn2)

            btn_list.Add(newli)

        Next

        Me.donebtn = New buttono(Me.game, 600, 600, 200, 50, "done")
        Me.lone_btn_list.Add(Me.donebtn)

        rgb_vals.Add("R")
        rgb_vals.Add("G")
        rgb_vals.Add("B")

        For clrbutn = 0 To 2

            Dim new_btn As New buttono(Me.game, 300, 200 + (clrbutn * 100), 100, 50, "+" & Me.rgb_vals(clrbutn))
            Me.lone_btn_list.Add(new_btn)
            Me.color_btns.Add(new_btn)

            Dim new_btn1 As New buttono(Me.game, 400, 200 + (clrbutn * 100), 100, 50, "-" & Me.rgb_vals(clrbutn))
            Me.lone_btn_list.Add(new_btn1)
            Me.color_btns.Add(new_btn1)

        Next

        Me.next_limb = New buttono(Me.game, 340, 480, 150, 50, "Next limb")
        Me.lone_btn_list.Add(Me.next_limb)

        Me.bef_limb = New buttono(Me.game, 140, 480, 150, 50, "Last limb")
        Me.lone_btn_list.Add(Me.bef_limb)

        For Each btn In Me.btn_list

            btn(0).load()
            btn(0).mybutton.Font = New Font("Microsoft Sans Serif", 10)

            btn(1).load()
            btn(1).mybutton.Font = New Font("Microsoft Sans Serif", 10)

            AddHandler btn(0).mybutton.Click, AddressOf btn(0).Me_OnClick
            AddHandler btn(1).mybutton.Click, AddressOf btn(1).Me_OnClick

        Next

        For Each btn In Me.lone_btn_list

            btn.load()

            AddHandler btn.mybutton.Click, AddressOf btn.Me_OnClick

        Next
        '-------------------------------------------------------
    End Sub

    Private Sub make_txt()

        Me.attri_txt = New text(Me.game, 800, 50, 20, Me.attribute_pnts, Color.Red)
        Me.txt_list.Add(attri_txt)

        Me.limb_on_txt = New text(Me.game, 375, 150, 20, "arms" & Me.limb_on, Color.Green)
        Me.txt_list.Add(limb_on_txt)

        For Each i In Me.txt_list
            i.load()
        Next
    End Sub

    Private Sub make_images()
        'draws a box around the player colour changing area (was told it looks pretty)
        Me.plr1.x = 200
        Me.plr1.y = 300

        Me.plr1.load()
        Me.plr1.limb_load()

        Dim pictbox As New PictureBox

        pictbox.Width = 500
        pictbox.Height = 500

        pictbox.Left = 100
        pictbox.Top = 100

        Dim bitmp As New Bitmap(pictbox.Width, pictbox.Height)
        Dim graphic As Graphics = Graphics.FromImage(bitmp)
        Dim pen As New Pen(Color.Black, 5)

        graphic.DrawLine(pen, 0, 0, 400, 0)
        graphic.DrawLine(pen, 0, 450, 400, 450)
        graphic.DrawLine(pen, 0, 0, 0, 450)
        graphic.DrawLine(pen, 400, 0, 400, 450)

        pictbox.Image = bitmp

        Me.image_list.Add(pictbox)

        For Each img In Me.image_list
            Me.game.Controls.Add(img)
        Next
        '---------------------------------------------------
    End Sub

    Private Sub colr_limbs()
        'load the starting values for the colour dictionary 1:[0, 0, 0], 2:[0, 0, 0]...
        For ind = 0 To 2
            Dim new_li As New List(Of Integer)

            new_li.Add(0)
            new_li.Add(0)
            new_li.Add(0)

            Me.colour_limbs.Add(ind, new_li)
        Next
        '------------------------------------------------------------------------------
    End Sub

    Private Sub update_plr_clrs()
        'allows the user to choose wich limbs colour they want to change and only changes that one
        If Me.limb_on = 0 Then

            Me.plr1.leftArm.BackColor = Color.FromArgb(Me.colour_limbs(Me.limb_on)(0), Me.colour_limbs(Me.limb_on)(1), Me.colour_limbs(Me.limb_on)(2))
            Me.plr1.rightArm.BackColor = Color.FromArgb(Me.colour_limbs(Me.limb_on)(0), Me.colour_limbs(Me.limb_on)(1), Me.colour_limbs(Me.limb_on)(2))

            Me.limb_on_txt.update("arms: " & Me.limb_on)

        ElseIf Me.limb_on = 1 Then

            Me.plr1.leftLeg.BackColor = Color.FromArgb(Me.colour_limbs(Me.limb_on)(0), Me.colour_limbs(Me.limb_on)(1), Me.colour_limbs(Me.limb_on)(2))
            Me.plr1.rightLeg.BackColor = Color.FromArgb(Me.colour_limbs(Me.limb_on)(0), Me.colour_limbs(Me.limb_on)(1), Me.colour_limbs(Me.limb_on)(2))

            Me.limb_on_txt.update("legs: " & Me.limb_on)

        ElseIf Me.limb_on = 2 Then

            Me.plr1.box.BackColor = Color.FromArgb(Me.colour_limbs(Me.limb_on)(0), Me.colour_limbs(Me.limb_on)(1), Me.colour_limbs(Me.limb_on)(2))

            Me.limb_on_txt.update("body: " & Me.limb_on)

        End If
        '------------------------------------------------------------------------------------------
    End Sub

    Private Sub finish_maker()
        'destroy all ui so the user can enter the game
        For Each br In Me.bar_list
            br.destroy()
        Next

        For Each btn In Me.btn_list
            btn(0).destroy()
            btn(1).destroy()
        Next

        For Each btn In Me.lone_btn_list
            btn.destroy()
        Next

        For Each txt In Me.txt_list
            txt.destroy()
        Next

        For Each img In Me.image_list
            Me.game.Controls.Remove(img)
        Next
        '-------------------------------------------

        'change the player stats once finished, using percentages so e.g. 1 + 0.3 = 1.3 = 130% increase
        Me.plr1.max_hp *= (1 + plr1.all_stats(0))
        Me.plr1.hp = Me.plr1.max_hp

        Me.plr1.max_mana *= (1 + plr1.all_stats(2))
        Me.plr1.mana = Me.plr1.max_mana
        '-------------------------------------------------------------------------------------
    End Sub

    Public Sub load()
        Me.make_bars()
        Me.make_buttons()
        Me.make_txt()
        Me.make_images()
        Me.colr_limbs()
    End Sub

    Protected Friend Sub update()
        'checks each pair of buttons that allters player stats and changes them accordingly (inc/dec)
        For btnon = 0 To Me.btn_list.Count() - 1

            If Me.btn_list(btnon)(0).been_clicked Then

                If Me.plr1.all_stats(btnon) > 0 Then
                    Me.plr1.all_stats(btnon) -= 1
                    Me.attribute_pnts += 1
                    Me.bar_list(btnon).new_size(Me.plr1.all_stats(btnon) / 20)
                End If

                Me.btn_list(btnon)(0).been_clicked = False

            ElseIf Me.btn_list(btnon)(1).been_clicked Then

                If Me.plr1.all_stats(btnon) < 20 AndAlso Me.attribute_pnts > 0 Then
                    Me.plr1.all_stats(btnon) += 1
                    Me.attribute_pnts -= 1
                    Me.bar_list(btnon).new_size(Me.plr1.all_stats(btnon) / 20)
                End If

                Me.btn_list(btnon)(1).been_clicked = False

            End If
        Next
        '-------------------------------------------------------------------------------------

        'updates all button pairs in the colour changing area and changes the players limbs colours accordingly
        For Each btn In Me.color_btns

            If btn.been_clicked And btn.text = "+R" Then

                Me.colour_limbs(Me.limb_on)(0) += 40
                btn.been_clicked = False
            ElseIf btn.been_clicked And btn.text = "+G" Then

                Me.colour_limbs(Me.limb_on)(1) += 40
                btn.been_clicked = False
            ElseIf btn.been_clicked And btn.text = "+B" Then

                Me.colour_limbs(Me.limb_on)(2) += 40
                btn.been_clicked = False
            ElseIf btn.been_clicked And btn.text = "-R" Then

                Me.colour_limbs(Me.limb_on)(0) -= 40
                btn.been_clicked = False
            ElseIf btn.been_clicked And btn.text = "-G" Then

                Me.colour_limbs(Me.limb_on)(1) -= 40
                btn.been_clicked = False
            ElseIf btn.been_clicked And btn.text = "-B" Then

                Me.colour_limbs(Me.limb_on)(2) -= 40
                btn.been_clicked = False
            End If

        Next
        '--------------------------------------------------------------------------------------------------------

        'checks each limb colour in the dictionary for limb colours, if the one of the RGB values is to big/small it resets it to max/min respectively
        For Each limb In Me.colour_limbs

            For ind = 0 To 2

                If Me.colour_limbs(limb.Key)(ind) < 0 Then

                    Me.colour_limbs(limb.Key)(ind) = 0

                ElseIf Me.colour_limbs(limb.Key)(ind) > 255 Then

                    Me.colour_limbs(limb.Key)(ind) = 255

                End If

            Next

        Next
        '---------------------------------------------------------------------------------------------------------------------------------------------

        'changes the amount of attribute points available
        Me.attri_txt.update(Me.attribute_pnts)

        'checks and updates main buttons
        If Me.donebtn.been_clicked Then

            Me.donebtn.been_clicked = False
            Me.finish_maker()

        ElseIf Me.next_limb.been_clicked Then

            Me.limb_on += 1
            Me.next_limb.been_clicked = False

        ElseIf Me.bef_limb.been_clicked Then

            Me.limb_on -= 1
            Me.bef_limb.been_clicked = False

        End If
        '-----------------------------------

        'makes sure the user can only change the viable amount of limbs, makes it a circular selection e.g. 2->1->0->2->1.....
        If Me.limb_on > 2 Then
            Me.limb_on = 0
        ElseIf Me.limb_on < 0 Then
            Me.limb_on = 2
        End If
        '----------------------------------------------------------------------------------------------------------------------

        Me.update_plr_clrs()

    End Sub
End Class
