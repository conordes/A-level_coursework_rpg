Public Class map_maker

    Private width As Integer = 16
    Private height As Integer = 14

    Private new_map As New List(Of String)

    Private type_room As String = "normal"

    Private util As utilities

    Public Sub New(ByVal utl As utilities, ByVal wid As Integer, ByVal hei As Integer, Optional ByVal typrm As String = "normal")

        Me.type_room = typrm

        Me.width = wid
        Me.height = hei

        Me.util = utl


    End Sub


    Public Function generate_random_map() As List(Of List(Of String))

        Dim density As Integer = 60
        Dim iterations As Integer = 100 'higher the number is the less unique rooms may be

        Dim tempMap As New List(Of List(Of String))

        ' randomly add walls to the empty grid----------------
        For y As Integer = 0 To height

            Dim row As New List(Of String)
            For x As Integer = 0 To width

                'add walls depending on density requested:
                If Me.util.Rand0(1, 100) < density Then
                    row.Add("b")
                Else
                    row.Add(".")
                End If

            Next

            tempMap.Add(row)

        Next
        '-----------------------------------------------------

        'amount of iterations:
        For iteration As Integer = 0 To iterations - 1

            Dim newMap As New List(Of List(Of String))

            ' use cellular automata to make the grid look more room-like----------------------------------------------
            For y As Integer = 0 To tempMap.Count - 1

                Dim newRow As New List(Of String)
                For x As Integer = 0 To tempMap(0).Count - 1

                    Dim wallCount As Integer = 0

                    'count how many neibouring cells are walls--------------------------------------------------------
                    For dy As Integer = -1 To 1
                        For dx As Integer = -1 To 1

                            Dim ny As Integer = y + dy
                            Dim nx As Integer = x + dx

                            If ny >= 0 AndAlso ny < tempMap.Count AndAlso nx >= 0 AndAlso nx < tempMap(0).Count Then
                                If (dy <> 0 OrElse dx <> 0) And tempMap(ny)(nx) = "b" Then
                                    wallCount += 1
                                End If
                            Else
                                wallCount += 1
                            End If

                        Next
                    Next
                    '-------------------------------------------------------------------------------------------------

                    'add rules so only cells with enough neighbours live--------
                    If wallCount >= 5 Then
                        newRow.Add("b")
                    Else
                        newRow.Add(".")
                    End If
                    '-----------------------------------------------------------
                Next

                newMap.Add(newRow)

            Next

            tempMap = newMap

        Next
        '----------------------------------------------------------------------------------------------------------

        Return tempMap
    End Function

    Public Function is_navig(ByVal map As List(Of List(Of String)), ByVal start_pos As Integer(), ByVal end_pos As Integer())

        Dim path As List(Of Integer()) = find_path(start_pos(0), start_pos(1), end_pos(0), end_pos(1), map)

        ' returns true if their is a path present and there are more than 0 steps:
        Return Not path Is Nothing AndAlso path.Count > 0
    End Function

    Public Function load_map(Optional ByVal bossrm As Boolean = False) As List(Of String)

        Dim validMap As List(Of List(Of String))
        Dim floorTiles As New List(Of Integer())

        Dim start_pos As Integer()
        Dim end_pos As Integer()

        Dim sum_of_enemies As Integer = 0
        Dim sum_of_npcs As Integer = 0

        Dim spawned As Boolean = False

        Do

            floorTiles = New List(Of Integer())


            spawned = False

            validMap = generate_random_map()

            sum_of_enemies = 0
            sum_of_npcs = 0

            spawned = False

            'adds the border-----------------
            For i As Integer = 0 To Me.width - 1
                validMap(1)(i) = "b"
                validMap(Me.height)(i) = "b"
            Next

            For j As Integer = 1 To Me.height - 1
                validMap(j)(0) = "b"
                validMap(j)(Me.width) = "b"
            Next
            '-------------------------------

            ' Gather all floor tiles-----------------------------------

            For y As Integer = 1 To height - 1
                For x As Integer = 0 To width - 1
                    If validMap(y)(x) = "." Then


                        floorTiles.Add(New Integer() {x, y})


                    End If
                Next
            Next
            '----------------------------------------------------------


            ' adds doors (to bossroom or to next room)-----------------------------------------------------

            'check if there is enough empty space, if there is not it re-does the loop:
            If floorTiles.Count < 2 Then Continue Do

            'checks if there is a path to the doors.
            start_pos = floorTiles(Me.util.Rand0(1, (floorTiles.Count - 1) * 0.5))
            end_pos = floorTiles(floorTiles.Count - 1)


            If is_navig(validMap, start_pos, end_pos) Then

                spawned = True

                validMap(start_pos(1))(start_pos(0)) = "p"

                validMap(end_pos(1))(end_pos(0)) = "d"

            End If

            If Not spawned Then Continue Do

            '---------------------------------------------------------------------------------------------


            If Not bossrm Then

                If Me.type_room = "normal" Then
                    'spawn in goblins/warlocks-----------------------------------------------------------------------------------------------------------------------------

                    While sum_of_enemies < 10

                        Dim en_location As Integer() = floorTiles(Me.util.Rand0(1, floorTiles.Count - 1))

                        'choose between goblin enemies and warlocks and check for a path----------

                        If Not (en_location Is start_pos) AndAlso is_navig(validMap, start_pos, en_location) And validMap(en_location(1))(en_location(0)) = "." Then

                            Select Case Me.util.Rand0(1, 100)
                                Case Is > 50
                                    validMap(en_location(1))(en_location(0)) = "w"

                                Case Is <= 50
                                    validMap(en_location(1))(en_location(0)) = "e"
                            End Select

                            sum_of_enemies += 1
                            '----------------------------------------------------
                        End If

                    End While

                    '------------------------------------------------------------------------------------------------------------------------------------------------------
                Else
                    'spawn in NPC's------------------------------------------------------------


                    While sum_of_npcs < 3

                        Dim npc_location As Integer() = floorTiles(Me.util.Rand0(1, floorTiles.Count - 1))

                        If Not (npc_location Is start_pos) AndAlso is_navig(validMap, start_pos, npc_location) And validMap(npc_location(1))(npc_location(0)) = "." Then
                            validMap(npc_location(1))(npc_location(0)) = "N"
                        Else
                            Continue While
                        End If

                        sum_of_npcs += 1

                    End While
                    '---------------------------------------------------------------------------
                End If
            Else
                'spawn in the bosses--------------------------------------------------------------------------------------------

                Dim boss_location As Integer() = floorTiles(Me.util.Rand0(3, floorTiles.Count - 1))

                If Not (boss_location Is start_pos) AndAlso is_navig(validMap, start_pos, boss_location) And validMap(boss_location(1))(boss_location(0)) = "." Then
                    validMap(boss_location(1))(boss_location(0)) = "E"
                    sum_of_enemies += 1
                End If


                '-------------------------------------------------------------------------------------------------------------
            End If

        Loop Until floorTiles.Count() > (Me.width * Me.height) / 4 And (sum_of_enemies > 0 Or sum_of_npcs > 2)

        ' convert list of list of strings("char") -> list of strings("full string")--
        For Each row In validMap
            new_map.Add(String.Join("", row.ToArray()))
        Next
        '----------------------------------------------------------------------------

        Return Me.new_map
    End Function

    Public Function find_path(ByVal startx As Integer, ByVal starty As Integer, ByVal endx As Integer, ByVal endy As Integer, ByVal map As List(Of List(Of String)))
        Dim openList As New List(Of Integer())

        Dim cameFrom As New Dictionary(Of String, Integer())
        'arrays to check whether cells have scores and if they are higher than the new scores 
        Dim gScore As New Dictionary(Of String, Integer)
        Dim fScore As New Dictionary(Of String, Integer)
        '------------------------------------------------------------------------------------
        Dim path As New List(Of Integer())
        Dim startKey As String = startx.ToString() & "," & starty.ToString()

        openList.Add(New Integer() {startx, starty})
        gScore(startKey) = 0
        fScore(startKey) = Math.Abs(startx - endx) + Math.Abs(starty - endy)

        While openList.Count > 0

            Dim current As Integer() = openList(0)
            Dim currentKey As String = current(0).ToString() & "," & current(1).ToString()

            'scores all cells that have not got a score but are being checked-------------------
            For num As Integer = 1 To openList.Count - 1
                Dim node As Integer() = openList(num)
                Dim nodeKey As String = node(0).ToString() & "," & node(1).ToString()

                If fScore.ContainsKey(nodeKey) AndAlso fScore(nodeKey) < fScore(currentKey) Then
                    current = node
                    currentKey = nodeKey
                End If

            Next
            '-----------------------------------------------------------------------------------

            'checks if the end node is in the the list of checked cells, so it ends the program and finds the best path depending on scores
            If current(0) = endx AndAlso current(1) = endy Then
                path.Add(current)

                While cameFrom.ContainsKey(currentKey)
                    current = cameFrom(currentKey)
                    path.Insert(0, current)
                    currentKey = current(0).ToString() & "," & current(1).ToString()
                End While

                Return path
            End If
            '-------------------------------------------------------------------------------------------------------------------------------
            openList.Remove(current)
            'list of all neighbouring cels to one being checked:
            Dim offsets As Integer()() = {New Integer() {1, 0}, New Integer() {-1, 0}, New Integer() {0, 1}, New Integer() {0, -1}}
            'iterates between all neighbours changing their scores-----------------------------
            For j As Integer = 0 To offsets.Length - 1

                Dim nx As Integer = current(0) + offsets(j)(0)
                Dim ny As Integer = current(1) + offsets(j)(1)

                'checks the cell being checked is empty(not a wall) and in the array:
                If nx < 0 Or nx >= map(0).Count Or ny < 0 Or ny >= map.Count Then Continue For
                If map(ny)(nx) = "b" Then Continue For

                Dim nKey As String = nx.ToString() & "," & ny.ToString()
                Dim tentativeG As Integer = gScore(currentKey) + 1

                'changes the score of cells already been checked, or adds a score to cells that have not been checked---
                If Not gScore.ContainsKey(nKey) OrElse tentativeG < gScore(nKey) Then
                    cameFrom(nKey) = current
                    gScore(nKey) = tentativeG
                    fScore(nKey) = tentativeG + Math.Abs(nx - endx) + Math.Abs(ny - endy)

                    Dim found As Boolean = False

                    For k As Integer = 0 To openList.Count - 1
                        If openList(k)(0) = nx AndAlso openList(k)(1) = ny Then
                            found = True
                            Exit For
                        End If
                    Next

                    If Not found Then
                        openList.Add(New Integer() {nx, ny})
                    End If

                End If
                '-------------------------------------------------------------------------------------------------------
            Next
            '----------------------------------------------------------------------------------
        End While

        Return Nothing
    End Function
End Class

