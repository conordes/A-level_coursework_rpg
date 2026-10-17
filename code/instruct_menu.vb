Public Class instruct_menu
    Private game As Form

    'this menu gives instruction to the user how to play the game
    Private _created As Boolean = False

    Public Property created As Boolean
        Get
            Return _created
        End Get
        Set(ByVal value As Boolean)
            _created = value
        End Set
    End Property

    Private all_buttons As New List(Of buttono)
    Private done_btn As buttono

    Private all_text As New List(Of text)

    Private game_aim_txt As text

    Private move_keys As text
    Private inv_keys As text
    Private attack_keys As text
    Private scroll_keys As text
    Private sheild_key As text

    Private use_consumables As text

    Public Sub New(ByVal mygame As Form)
        Me.game = mygame
    End Sub

    Private Sub make_buttons()
        Me.done_btn = New buttono(Me.game, 600, 700, 200, 50, "Done")
        Me.all_buttons.Add(Me.done_btn)

        For Each btn In Me.all_buttons
            btn.load()
            AddHandler btn.mybutton.Click, AddressOf btn.Me_OnClick
        Next
    End Sub

    Private Sub make_text()
        Dim game_aim_lines As Integer = 2
        Dim game_aim_text As String = "aim of the game: beat all enemies in each room(level) and beat the bosses"

        Dim move_key_lines As Integer = 2
        Dim move_keys_text As String = "movement: (WASD) keys , uses velocity so only need to press once , double press for speed"

        Dim inv_keys_lines As Integer = 2
        Dim inv_keys_text As String = "player_inteface: press (i) to open the player_inteface and (i) agian to cose the player_inteface, cant open when sheild is equipped"

        Dim attack_keys_lines As Integer = 2
        Dim attack_keys_text As String = "attack click: press(LEFT-CLICK) to shoot whatever attack is in use"

        Dim scroll_keys_line As Integer = 4
        Dim scroll_keys_text As String = "change attack: scroll the wheel on the mouse and look at icon in top right to" & vbNewLine & "change attacks , also a cooldown bar for attacks"

        Dim sheild_key_lines As Integer = 6
        Dim sheild_key_text As String = "sheild use: equippe the sheild by pressing on it in your player_inteface and then" & vbNewLine & "press(RIGHT-CLICK)to use it and pushback enemies or absorb" & vbNewLine & "attacks ,it does have health indicated by the grey bar that appears"

        Dim use_consumables_lines As Integer = 2
        Dim use_consumables_text As String = "use consumables: to heal up , increase mna or euipe items you must open the player_inteface and click on them"

        Me.game_aim_txt = New text(Me.game, 100, 100, 15, game_aim_text, Color.Black, True, game_aim_lines)
        Me.move_keys = New text(Me.game, 100, 200, 15, move_keys_text, Color.Black, True, move_key_lines)
        Me.inv_keys = New text(Me.game, 100, 250, 15, inv_keys_text, Color.Black, True, inv_keys_lines)
        Me.attack_keys = New text(Me.game, 100, 300, 15, attack_keys_text, Color.Black, True, attack_keys_lines)
        Me.scroll_keys = New text(Me.game, 100, 350, 15, scroll_keys_text, Color.Black, True, scroll_keys_line)
        Me.sheild_key = New text(Me.game, 100, 450, 15, sheild_key_text, Color.Black, True, sheild_key_lines)
        Me.use_consumables = New text(Me.game, 100, 600, 15, use_consumables_text, Color.Black, True, use_consumables_lines)

        Me.all_text.Add(Me.game_aim_txt)

        Me.all_text.Add(Me.move_keys)
        Me.all_text.Add(Me.inv_keys)
        Me.all_text.Add(Me.attack_keys)
        Me.all_text.Add(Me.scroll_keys)
        Me.all_text.Add(Me.sheild_key)

        Me.all_text.Add(Me.use_consumables)

        For Each txt In Me.all_text
            txt.load()
        Next

    End Sub

    Protected Friend Sub load()
        Me.make_buttons()
        Me.make_text()
    End Sub

    Protected Friend Sub update()
        'restart the game
        If Me.done_btn.been_clicked Then
            Application.Restart()
        End If
        '---------------
    End Sub
End Class
