Imports System.Windows.Forms
Imports System.IO

Public Class finish_screen
    Inherits Form

    ' the class used to represent the form that displays you time vs others and ranks them

    Public timera As New Timer()
    Public timer_num As Integer = 0

    Private txtInput As New text(Me, 20, 400, 15, "name here", Color.Black, True, 0)
    Private finiText As New text(Me, 300, 50, 30, "finished the game", Color.Red)
    Private all_text As New List(Of text)

    Private submitbtn As New buttono(Me, 600, 700, 200, 50, "submit")
    Private exitbtn As New buttono(Me, 100, 700, 200, 50, "exit")
    Private rstbtn As New buttono(Me, 350, 700, 200, 50, "restart")

    Private all_buttons As New List(Of buttono)

    Private show_all_text As Boolean = False

    Private leaderboard_dict As New Dictionary(Of String, String)

    Public util As New utilities()

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'loads in the main screen 
        Me.Size = New Size(900, 850)

        Me.all_buttons.Add(Me.submitbtn)
        Me.all_buttons.Add(Me.exitbtn)
        Me.all_buttons.Add(Me.rstbtn)

        Me.all_text.Add(Me.txtInput)
        Me.all_text.Add(Me.finiText)

        For Each txt In Me.all_text
            txt.load()
        Next

        For Each btn In Me.all_buttons
            btn.load()
            AddHandler btn.mybutton.Click, AddressOf btn.Me_OnClick
        Next

        AddHandler Me.timera.Tick, AddressOf Me.Timer2_Tick
        Me.KeyPreview = True

        Me.timera.Interval = 60
        Me.timera.Start()
        '------------------------

    End Sub

    Private Sub load_leaderboard()
        'loads lines from saved scores file
        Me.leaderboard_dict = Me.util.Load_scores(Me.leaderboard_dict)

        'sort the dictionary
        Me.sort_dict()

        Dim ind As Integer = 0
        'print all the rows in the sorted dictionary
        For Each scr In Me.leaderboard_dict

            'only show the first 10
            If ind < 11 Then

                Dim new_row As New text(Me, 350, 150 + (ind * 50), 15, ind + 1 & ". " & scr.Key & " : " & Me.leaderboard_dict(scr.Key) & " seconds", Color.Black)

                new_row.border = True
                new_row.load()

            End If
            '---------------------

            'show the player score/time and where they rank
            If scr.Key = Me.txtInput.textbox.Text Then

                Dim player_score As New text(Me, 600, 150, 15, ind + 1 & ". " & scr.Key & " : " & Me.leaderboard_dict(scr.Key) & " seconds", Color.Red)

                player_score.border = True
                player_score.load()
            End If
            '----------------------------------------------

            ind += 1

        Next
        '----------------------------------------------


    End Sub

    Private Sub sort_dict()

        Dim sortable_list As New List(Of Tuple(Of Integer, String))
        Dim new_dict As New Dictionary(Of String, String)

        'change the dictionary to a list thaat can be sorted
        For Each scr In Me.leaderboard_dict

            Dim val As Integer = 0
            Dim parts() As String = Me.leaderboard_dict(scr.Key).Split(":"c)

            'change hours and minutes to seconds, for ordering
            val = CUInt(parts(0)) * 3600 + CUInt(parts(1)) * 60 + CUInt(parts(2))

            sortable_list.Add(Tuple.Create(val, scr.Key))

        Next
        '---------------------------------------------------

        'sort the new list and store the result as a dictionary
        For Each scr In quick_sort(sortable_list)
            new_dict((scr.item2).ToString()) = scr.item1
        Next

        Me.leaderboard_dict = new_dict
        '-------------------------------------------------------

    End Sub

    Private Function quick_sort(ByVal my_lizt As List(Of Tuple(Of Integer, String)))
        'performs a quick sort to a list

        Dim length As Integer = my_lizt.Count()

        'makes sure the list is big enough : bigger then 1/not smaller then 2
        If length < 2 Then
            Return my_lizt
        End If


        'split the list in halves
        Dim pivot As Tuple(Of Integer, String) = my_lizt(length - 1)

        my_lizt.RemoveAt(length - 1)
        '---------------------

        'order the split list
        Dim items_greater As New List(Of Tuple(Of Integer, String))
        Dim items_lower As New List(Of Tuple(Of Integer, String))

        For Each scr In my_lizt

            If scr.Item1 > pivot.Item1 Then

                items_greater.Add(scr)
            Else

                items_lower.Add(scr)
            End If

        Next
        '----------------------

        'recursively order it
        Dim return_my_lizt As New List(Of Tuple(Of Integer, String))

        return_my_lizt.AddRange(quick_sort(items_lower))
        return_my_lizt.Add(pivot)
        return_my_lizt.AddRange(quick_sort(items_greater))
        '--------------------

        Return return_my_lizt
    End Function

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs)

        'adds functionality to the buttons
        If Me.submitbtn.been_clicked And Me.txtInput.textbox.Text.Count() > 0 Then

            'when the name entered is submitted, it stores the name and score in a text file under all previouse game attempts
            Me.util.save_score(Me.txtInput.textbox.Text)

            Me.submitbtn.been_clicked = False
            Me.show_all_text = True

            Me.submitbtn.destroy()
            Me.txtInput.destroy()

        ElseIf Me.exitbtn.been_clicked Then

            Me.exitbtn.been_clicked = False

            Application.Exit()

        ElseIf Me.rstbtn.been_clicked Then

            Me.rstbtn.been_clicked = False

            Application.Restart()

        End If
        '----------------------------

        If Me.show_all_text Then

            Me.load_leaderboard()
            Me.show_all_text = False

        End If

    End Sub
End Class
