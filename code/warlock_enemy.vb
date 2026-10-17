Public Class warlock_enemy
    Inherits base_enemy

    Private Const fireball_path As String = "\\cc4sjd8\2024 Intake\24026166\Visual Studio 2010\Projects\practice_courswork_platformer\fireball-clipart-pixel-sprite-pixel-transparent-fire-ball--m2H7K9d3N4N4d3H7.png"
    Private fireball_img As New Bitmap(warlock_enemy.fireball_path)

    Public Sub New(ByVal game As Form, ByVal height As Integer, ByVal width As Integer, ByVal x As Integer, ByVal y As Integer, ByVal id As Integer, ByVal hp As Integer, ByVal util As utilities, Optional ByVal magic As Boolean = False)
        MyBase.New(game, height, width, x, y, id, hp, util, magic)

        Me.type_enemy = "warlock"

        Me.colour = Color.Brown
        Me.box.BackColor = Color.Brown

        Me.type_magic = magic
    End Sub

    Protected Overrides Sub update_arms(ByVal plr1 As player)

        Dim centerX As Integer = x + width \ 2
        Dim centerY As Integer = y + height \ 2

        ' warlock arms orbit the entity 
        Me.angle += 0.05

        ' Left arm moves counter-clockwise
        leftArm.Left = centerX + CInt(Math.Cos(angle) * 50) - (leftArm.Width \ 2)
        leftArm.Top = centerY + CInt(Math.Sin(angle) * 50) - (leftArm.Height \ 2)
        leftArm.BackColor = Color.Red

        ' Right arm moves clockwise
        rightArm.Left = centerX + CInt(Math.Cos(-angle) * 50) - (rightArm.Width \ 2)
        rightArm.Top = centerY + CInt(Math.Sin(-angle) * 50) - (rightArm.Height \ 2)
        rightArm.BackColor = Color.Red

    End Sub

    Public Sub attack(ByVal x As Double, ByVal y As Double, ByVal tick As Integer, ByVal angle As Double)

        'creates the same attack type, with the same attributes
        Dim new_attack = New base_attack(Me.game, 25, 25, Me.x, Me.y, 6, "fireball", New Bitmap(Me.attack_dict("fireball")(2)), Me.attack_dict("fireball")(1), angle)

        new_attack.spawn_time = tick

        Me.live_attacks.Add(new_attack)
        new_attack.box.BringToFront()

        new_attack.load()
        new_attack.move(x, y, Me)
        '-------------------------------------------------------

    End Sub

    Public Sub update_attacks(ByVal Timer As Integer)

        'checks all attacks are dead, to be destroyed
        For atk = 0 To Me.live_attacks.Count() - 1

            If Me.live_attacks(atk).check_alive(Timer) Or Me.live_attacks(atk).dead Then

                dead_attacks.Add(live_attacks(atk))
                live_attacks.RemoveAt(atk)

                Exit For

            End If

        Next

    End Sub

    Public Sub load_attacks(ByVal tick, ByVal plr1)

        If Me.plr1_see Then

            'so all attacks from all warlocks dont happen at once, the id is used, reducing lag
            If tick Mod 10 * Me.id = 0 Then

                For num_atk = 0 To 6


                    'creat a circle of fireballs around the warlock
                    angle = num_atk * 60
                    Dim radian As Double = angle * Math.PI / 180

                    Dim usex As Integer = Me.x + 25 + base_enemy.see_radius * Math.Cos(radian)
                    Dim usey As Integer = Me.y + 25 + base_enemy.see_radius * Math.Sin(radian)

                    attack(usex, usey, tick, radian)
                    '----------------------------------------------

                Next

            End If
        End If

    End Sub

    Protected Friend Overrides Sub update(ByVal all_objects As List(Of wall), ByVal plr1 As player, ByVal enemies As List(Of base_enemy), ByVal map As List(Of String), ByVal tick As Integer)

        MyBase.update(all_objects, plr1, enemies, map, tick)

        load_attacks(tick, plr1)
        update_attacks(tick)

        Me.x += Me.xvel
        Me.y += Me.yvel
        Me.box.Left = Me.x
        Me.box.Top = Me.y

    End Sub

End Class
