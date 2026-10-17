Public Class utilities
    'used to load from the load save file and attack file, and save to the load save file

    Private Const slmpath As String = "\\cc4sjd8\2024 Intake\24026166\Visual Studio 2010\Projects\practice_courswork_platformer\code\save_games.txt"
    Private Const aspath As String = "\\cc4sjd8\2024 Intake\24026166\Visual Studio 2010\Projects\practice_courswork_platformer\code\attacks.txt"
    Private Const inpath As String = "\\cc4sjd8\2024 Intake\24026166\Visual Studio 2010\Projects\practice_courswork_platformer\code\inentv_items.txt"
    Private Const mipath As String = "\\cc4sjd8\2024 Intake\24026166\Visual Studio 2010\Projects\practice_courswork_platformer\code\all_missions.txt"
    Private Const svscpath As String = "\\cc4sjd8\2024 Intake\24026166\Visual Studio 2010\Projects\practice_courswork_platformer\code\scores.txt"

    Protected Friend loaded_game As Boolean = False
    Protected Friend rand As New Random()

    Private _mousex As Integer
    Public ReadOnly Property mousex
        Get
            Return _mousex
        End Get
    End Property

    Private _mousey As Integer
    Public ReadOnly Property mousey
        Get
            Return _mousey
        End Get
    End Property

    Public Function file_exisists(ByVal file_path As String, ByVal read_write As String, Optional ByVal clear As Boolean = False)
        'checks the file exists and loads it in
        If Not System.IO.File.Exists(file_path) Then
            Debug.Print("file does not exist: " & file_path)
            Stop
        Else
            If read_write = "read" Then
                Return System.IO.File.ReadAllLines(file_path)
            ElseIf read_write = "write" Then
                Return New System.IO.StreamWriter(file_path, clear)
            Else
                Debug.Print("cannot " & read_write)
                Stop
            End If

        End If
        Return Nothing
    End Function

    Public Function add_missions(ByVal all_missions As Dictionary(Of List(Of String), List(Of Integer)))

        Dim all_missions_txt As New List(Of List(Of String))


        'load the title of the mission into a file for random choosing
        For Each mis In all_missions
            all_missions_txt.Add(mis.Key)
        Next

        Return all_missions_txt
    End Function

    Public Function Rand0(Optional ByVal lower_bound As Integer = 1, Optional ByVal upper_bound As Integer = 100) As Integer
        Randomize()

        Return rand.Next(lower_bound, upper_bound)
    End Function


    Public Sub Form1_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)

        Me._mousex = e.X
        Me._mousey = e.Y

    End Sub


    Public Sub save_map(ByVal string_to_save As List(Of String), ByVal plr1 As player)

        Dim objWriter As System.IO.StreamWriter = Me.file_exisists(utilities.slmpath, "write")


        'write the map to the file
        For Each line In string_to_save

            objWriter.WriteLine(line)

        Next
        '------------------------
        'new line, player stats/cutomization
        objWriter.WriteLine("/")

        'write the stat modifiers to the text file
        For ind = 0 To 2
            objWriter.WriteLine(plr1.all_stats(ind))
        Next

        'Write each limbs rgb colour to the text file, each rgb vslue is seperated by a /
        Dim body_cols As Color = plr1.box.BackColor
        objWriter.WriteLine(body_cols.R & "\" & body_cols.G & "\" & body_cols.B)

        Dim arm_cols As Color = plr1.leftArm.BackColor
        objWriter.WriteLine(arm_cols.R & "\" & arm_cols.G & "\" & arm_cols.B)

        Dim leg_cols As Color = plr1.leftLeg.BackColor
        objWriter.WriteLine(leg_cols.R & "\" & leg_cols.G & "\" & leg_cols.B)

        objWriter.Close()

    End Sub

    Public Sub save_score(ByVal textp As String)
        'write to text file

        Dim objWriter As System.IO.StreamWriter = Me.file_exisists(utilities.svscpath, "write", True)


        objWriter.WriteLine(textp)


        objWriter.Close()
    End Sub


    Public Function load_map()

        Dim store_list As New List(Of List(Of String))

        Dim player_stats As Boolean = False
        Dim next_line As Boolean = False

        Dim lines() As String = Me.file_exisists(utilities.slmpath, "read")

        'divide a txt file into lines and read each one then store in a list(map, stats)

        Dim map As New List(Of String)
        Dim stats As New List(Of String)


        For Each line As String In lines

            For Each ind In line

                If ind = "/" Then
                    'if / found save all read things to the stat file
                    player_stats = True
                    next_line = True

                    Exit For
                End If

            Next

            If Not next_line Then
                If player_stats Then
                    stats.Add(line)
                Else
                    map.Add(line)
                End If

            Else
                next_line = False

            End If




        Next
        '------------------------------------------------------------------


        store_list.Add(map)
        store_list.Add(stats)



        Return store_list
    End Function

    Public Function Load_scores(ByVal load_dict As Dictionary(Of String, String))

        'read , in lines, from the text file
        Dim lines() As String = Me.file_exisists(utilities.svscpath, "read")

        For line = 0 To (lines.Count() / 2) - 1


            load_dict(lines(line * 2 + 1)) = lines(line * 2)



        Next
        '------------------------------------------------
        Return load_dict
    End Function

    Public Function load_attacks(ByVal type As String)
        'type=type of entity as enemies ,players and bosses all have different attacks so no point in loading the uneccessary ones
        Dim atk_dict As New Dictionary(Of String, List(Of String))

        Dim lines() As String = Me.file_exisists(utilities.aspath, "read")


        'load the atributes for each attack into a dictionary
        For Each line As String In lines

            Dim parts() As String = line.Split("/"c)

            If parts.Length > 2 AndAlso (parts(8) = type Or "none" = type) Then

                Dim name As String = parts(0)
                Dim values As New List(Of String)

                For i As Integer = 1 To parts.Length - 1
                    values.Add(parts(i))
                Next

                atk_dict(name) = values

            End If

        Next
        '-------------------------------------------------

        Return atk_dict
    End Function

    Public Function load_player_inteface_items()

        Dim ret_dict As New Dictionary(Of String, List(Of Integer))

        Dim lines() As String = Me.file_exisists(utilities.inpath, "read")

        'divide a txt file into lines and read each one then store in a dict
        For Each line As String In lines
            Dim parts() As String = line.Split("/"c)

            'makes sure line it is one has multiple parts, must have more than 1 "/"
            If parts.Length >= 2 Then
                Dim name As String = parts(0)
                Dim values As New List(Of Integer)

                'get all item stats and add to a list
                For sublines As Integer = 1 To parts.Length - 1
                    Dim return_value As Integer

                    If Integer.TryParse(parts(sublines), return_value) Then
                        values.Add(return_value)
                    End If

                Next

                'add the list to its related name in a dictionary

                ret_dict(name) = values


            End If

        Next
        '------------------------------------------------------------------

        Return ret_dict
    End Function

    Public Function load_missions()

        Dim all_missions As New Dictionary(Of List(Of String), List(Of Integer))

        Dim lines() As String = Me.file_exisists(utilities.mipath, "read")
        'read a text file with the missions in it and load into a dictionary in memory
        For Each line As String In lines

            Dim parts() As String = line.Split("/"c)

            If parts.Count() <= 1 Then Continue For

            Dim key As New List(Of String)
            Dim val As New List(Of Integer)

            'splits the line of string from the text file into name/type of enemy/number to kill/reward for completion/how many rewards

            'this is then loaded into a dictionary for easy access
            key.Add(parts(0))
            key.Add(parts(1))
            key.Add(parts(3))

            val.Add(parts(2))
            val.Add(parts(4))

            all_missions(key) = val

        Next
        '-------------------------------------------------------------------------------



        Return all_missions
    End Function

    


End Class
