Imports System.IO

Public Class utilities
    'Game data is read from the application folder. Writable save data is kept
    'in the current user's local application-data folder.
    Private Shared ReadOnly dataFolder As String = AppDomain.CurrentDomain.BaseDirectory
    Private Shared ReadOnly saveFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "A-level_coursework_rpg")
    Private Shared ReadOnly slmpath As String = Path.Combine(saveFolder, "save_games.txt")
    Private Shared ReadOnly svscpath As String = Path.Combine(saveFolder, "scores.txt")
    Private Shared ReadOnly placeholderImagePath As String = Path.Combine(Path.GetTempPath(), "A-level_coursework_rpg_placeholder.png")

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

    Private Function data_path(ByVal file_name As String) As String
        Return Path.Combine(dataFolder, file_name)
    End Function

    Private Function get_placeholder_image_path() As String
        If Not File.Exists(placeholderImagePath) Then
            Using image As New Bitmap(16, 16)
                Using graphics As Graphics = Graphics.FromImage(image)
                    graphics.Clear(Color.Magenta)
                End Using
                image.Save(placeholderImagePath, Imaging.ImageFormat.Png)
            End Using
        End If
        Return placeholderImagePath
    End Function

    Public Function file_exisists(ByVal file_path As String, ByVal read_write As String, Optional ByVal clear As Boolean = False)
        If read_write = "read" Then
            If Not File.Exists(file_path) Then
                Return New String() {}
            End If
            Return File.ReadAllLines(file_path)
        ElseIf read_write = "write" Then
            Dim parent As String = Path.GetDirectoryName(file_path)
            If Not String.IsNullOrEmpty(parent) Then Directory.CreateDirectory(parent)
            Return New StreamWriter(file_path, Not clear)
        End If
        Return Nothing
    End Function

    Public Function add_missions(ByVal all_missions As Dictionary(Of List(Of String), List(Of Integer)))
        Dim all_missions_txt As New List(Of List(Of String))
        For Each mis In all_missions
            all_missions_txt.Add(mis.Key)
        Next
        Return all_missions_txt
    End Function

    Public Function Rand0(Optional ByVal lower_bound As Integer = 1, Optional ByVal upper_bound As Integer = 100) As Integer
        If upper_bound <= lower_bound Then Return lower_bound
        Return rand.Next(lower_bound, upper_bound)
    End Function

    Public Sub Form1_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
        Me._mousex = e.X
        Me._mousey = e.Y
    End Sub

    Public Sub save_map(ByVal string_to_save As List(Of String), ByVal plr1 As player)
        Dim objWriter As StreamWriter = Me.file_exisists(utilities.slmpath, "write")
        For Each line In string_to_save
            objWriter.WriteLine(line)
        Next
        objWriter.WriteLine("/")
        For ind = 0 To 2
            objWriter.WriteLine(plr1.all_stats(ind))
        Next
        Dim body_cols As Color = plr1.box.BackColor
        objWriter.WriteLine(body_cols.R & "\" & body_cols.G & "\" & body_cols.B)
        Dim arm_cols As Color = plr1.leftArm.BackColor
        objWriter.WriteLine(arm_cols.R & "\" & arm_cols.G & "\" & arm_cols.B)
        Dim leg_cols As Color = plr1.leftLeg.BackColor
        objWriter.WriteLine(leg_cols.R & "\" & leg_cols.G & "\" & leg_cols.B)
        objWriter.Close()
    End Sub

    Public Sub save_score(ByVal textp As String)
        Dim objWriter As StreamWriter = Me.file_exisists(utilities.svscpath, "write", True)
        objWriter.WriteLine(textp)
        objWriter.Close()
    End Sub

    Public Function load_map()
        Dim store_list As New List(Of List(Of String))
        Dim map As New List(Of String)
        Dim stats As New List(Of String)
        Dim player_stats As Boolean = False

        For Each line As String In DirectCast(Me.file_exisists(utilities.slmpath, "read"), String())
            If line.Trim() = "/" Then
                player_stats = True
            ElseIf player_stats Then
                stats.Add(line)
            Else
                map.Add(line)
            End If
        Next
        store_list.Add(map)
        store_list.Add(stats)
        Return store_list
    End Function

    Public Function Load_scores(ByVal load_dict As Dictionary(Of String, String))
        Dim lines() As String = DirectCast(Me.file_exisists(utilities.svscpath, "read"), String())
        For line As Integer = 0 To (lines.Length \ 2) - 1
            load_dict(lines(line * 2 + 1)) = lines(line * 2)
        Next
        Return load_dict
    End Function

    Public Function load_attacks(ByVal type As String)
        Dim atk_dict As New Dictionary(Of String, List(Of String))
        Dim lines() As String = DirectCast(Me.file_exisists(data_path("attacks.txt"), "read"), String())
        Dim placeholder As String = get_placeholder_image_path()

        For Each line As String In lines
            Dim parts() As String = line.Split("/"c)
            If parts.Length >= 9 AndAlso (parts(8) = type OrElse type = "none") Then
                Dim values As New List(Of String)
                For i As Integer = 1 To parts.Length - 1
                    values.Add(parts(i))
                Next
                'Replace any old computer-specific sprite path with a tiny image
                'generated locally by the game. No downloaded sprite files are used.
                If values.Count > 2 Then values(2) = placeholder
                atk_dict(parts(0)) = values
            End If
        Next
        Return atk_dict
    End Function

    Public Function load_player_inteface_items()
        Dim ret_dict As New Dictionary(Of String, List(Of Integer))
        Dim lines() As String = DirectCast(Me.file_exisists(data_path("inentv_items.txt"), "read"), String())
        For Each line As String In lines
            Dim parts() As String = line.Split("/"c)
            If parts.Length >= 2 Then
                Dim values As New List(Of Integer)
                For subline As Integer = 1 To parts.Length - 1
                    Dim parsed As Integer
                    If Integer.TryParse(parts(subline), parsed) Then values.Add(parsed)
                Next
                If values.Count > 0 Then ret_dict(parts(0)) = values
            End If
        Next
        Return ret_dict
    End Function

    Public Function load_missions()
        Dim all_missions As New Dictionary(Of List(Of String), List(Of Integer))
        Dim lines() As String = DirectCast(Me.file_exisists(data_path("all_missions.txt"), "read"), String())
        For Each line As String In lines
            Dim parts() As String = line.Split("/"c)
            If parts.Length < 5 Then Continue For
            Dim key As New List(Of String) From {parts(0), parts(1), parts(3)}
            Dim amount As Integer
            Dim reward_amount As Integer
            If Integer.TryParse(parts(2), amount) AndAlso Integer.TryParse(parts(4), reward_amount) Then
                all_missions(key) = New List(Of Integer) From {amount, reward_amount}
            End If
        Next
        Return all_missions
    End Function
End Class
