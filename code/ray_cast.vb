Public Class ray_cast
    Private nodes As New List(Of List(Of Array))
    Private wallz As New List(Of List(Of Array))

    Private box As List(Of String)

    Public Sub New(ByVal string_map As List(Of String))
        Me.box = string_map

        Me.load_wallz()
    End Sub

    Private Sub connect_nodes()
        'connect nodes that are next to each other
        For Each node In nodes


            Dim my_coords As Array = node(0)

            Dim all_wallz As List(Of Integer) = find_wall(my_coords, wallz)

            'node: mycoords, north, south, east, west, nw, ne, sw, se
            Dim south_done As Boolean = False
            Dim east_done As Boolean = False
            Dim sw_done As Boolean = False
            Dim se_done As Boolean = False


            For Each wl In all_wallz

                'vertical
                If wallz(wl)(0)(1) < my_coords(1) And wallz(wl)(0)(0) = my_coords(0) Then

                    Dim new_endpoint As Array

                    If node(2)(0) <> -1 And node(2)(1) <> -1 Then
                        new_endpoint = node(2)
                    Else
                        new_endpoint = my_coords
                    End If

                    wallz(wl)(1) = new_endpoint
                    south_done = True
                End If

                'horizontal
                If wallz(wl)(0)(0) < my_coords(0) And wallz(wl)(0)(1) = my_coords(1) Then

                    Dim new_endpoint As Array

                    If node(3)(0) <> -1 And node(3)(1) <> -1 Then
                        new_endpoint = node(3)
                    Else
                        new_endpoint = my_coords
                    End If

                    wallz(wl)(1) = new_endpoint
                    east_done = True
                End If

                'diagnoll left, sw
                If wallz(wl)(0)(0) > my_coords(0) And wallz(wl)(0)(1) < my_coords(1) Then

                    Dim new_endpoint As Array

                    If node(7)(0) <> -1 And node(7)(1) <> -1 Then
                        new_endpoint = node(7)
                    Else
                        new_endpoint = my_coords
                    End If

                    wallz(wl)(1) = new_endpoint
                    sw_done = True
                End If

                'diagnoll right, se
                If wallz(wl)(0)(0) < my_coords(0) And wallz(wl)(0)(1) < my_coords(1) Then

                    Dim new_endpoint As Array

                    If node(8)(0) <> -1 And node(8)(1) <> -1 Then
                        new_endpoint = node(8)
                    Else
                        new_endpoint = my_coords
                    End If

                    wallz(wl)(1) = new_endpoint
                    se_done = True
                End If
            Next
            'node: mycoords, north, south, east, west, nw, ne, sw, se
            'wallz havent been done

            If all_wallz.Count < 1 Then
                Dim start_coords As Array = my_coords
                Dim collumn As New List(Of Array)

                collumn.Add(start_coords)

                wallz.Add(collumn)
            End If

            If Not south_done And Not empty(node(2)) Then
                Dim start_coords As Array = my_coords
                Dim end_coords As Array = node(2)

                Dim collumn As New List(Of Array)

                collumn.Add(start_coords)
                collumn.Add(end_coords)

                wallz.Add(collumn)
            End If

            If Not east_done And Not empty(node(3)) Then
                Dim start_coords As Array = my_coords
                Dim end_coords As Array = node(3)

                Dim collumn As New List(Of Array)

                collumn.Add(start_coords)
                collumn.Add(end_coords)

                wallz.Add(collumn)
            End If

            If Not sw_done And Not empty(node(7)) Then
                Dim start_coords As Array = my_coords
                Dim end_coords As Array = node(7)

                Dim collumn As New List(Of Array)

                collumn.Add(start_coords)
                collumn.Add(end_coords)

                wallz.Add(collumn)
            End If

            If Not se_done And Not empty(node(8)) Then
                Dim start_coords As Array = my_coords
                Dim end_coords As Array = node(8)

                Dim collumn As New List(Of Array)

                collumn.Add(start_coords)
                collumn.Add(end_coords)

                wallz.Add(collumn)
            End If

        Next
        '------------------------------------------------------------------
    End Sub

    Private Sub create_nodes()
        'create nodes for certain walls
        For y_now = 1 To box.Count() - 1
            For x_now = 0 To box(y_now).Length() - 1
                Dim new_node As New List(Of Array)

                Dim on_coords(2) As Integer
                Dim N_coords(2) As Integer
                Dim E_coords(2) As Integer
                Dim S_coords(2) As Integer
                Dim W_coords(2) As Integer
                Dim NW_coords(2) As Integer
                Dim NE_coords(2) As Integer
                Dim SW_coords(2) As Integer
                Dim SE_coords(2) As Integer

                If box(y_now)(x_now) = "b" Then

                    on_coords(0) = x_now
                    on_coords(1) = y_now

                    ' NORTH
                    If y_now - 1 > 0 AndAlso box(y_now - 1)(x_now) = "b" Then
                        N_coords(0) = x_now
                        N_coords(1) = y_now - 1
                    Else
                        N_coords(0) = -1
                        N_coords(1) = -1
                    End If

                    ' SOUTH
                    If y_now + 1 < box.Count() - 1 AndAlso box(y_now + 1)(x_now) = "b" Then
                        S_coords(0) = x_now
                        S_coords(1) = y_now + 1
                    Else
                        S_coords(0) = -1
                        S_coords(1) = -1
                    End If

                    ' EAST 
                    If x_now - 1 > 0 AndAlso box(y_now)(x_now - 1) = "b" Then
                        W_coords(0) = x_now - 1
                        W_coords(1) = y_now
                    Else
                        W_coords(0) = -1
                        W_coords(1) = -1
                    End If

                    ' WEST 
                    If x_now + 1 < box(y_now).Length() - 1 AndAlso box(y_now)(x_now + 1) = "b" Then
                        E_coords(0) = x_now + 1
                        E_coords(1) = y_now
                    Else
                        E_coords(0) = -1
                        E_coords(1) = -1
                    End If

                    ' NORTH-WEST
                    If y_now - 1 > 0 AndAlso x_now - 1 >= 0 AndAlso box(y_now - 1)(x_now - 1) = "b" Then
                        NW_coords(0) = x_now - 1
                        NW_coords(1) = y_now - 1
                    Else
                        NW_coords(0) = -1
                        NW_coords(1) = -1
                    End If

                    ' NORTH-EAST
                    If y_now - 1 > 0 AndAlso x_now + 1 < box(y_now).Length() AndAlso box(y_now - 1)(x_now + 1) = "b" Then
                        NE_coords(0) = x_now + 1
                        NE_coords(1) = y_now - 1
                    Else
                        NE_coords(0) = -1
                        NE_coords(1) = -1
                    End If

                    ' SOUTH-WEST
                    If y_now + 1 < box.Count() AndAlso x_now - 1 >= 0 AndAlso box(y_now + 1)(x_now - 1) = "b" Then
                        SW_coords(0) = x_now - 1
                        SW_coords(1) = y_now + 1
                    Else
                        SW_coords(0) = -1
                        SW_coords(1) = -1
                    End If

                    ' SOUTH-EAST
                    If y_now + 1 < box.Count() AndAlso x_now + 1 < box(y_now).Length() AndAlso box(y_now + 1)(x_now + 1) = "b" Then
                        SE_coords(0) = x_now + 1
                        SE_coords(1) = y_now + 1
                    Else
                        SE_coords(0) = -1
                        SE_coords(1) = -1
                    End If

                    ' Add coordinates to the node
                    new_node.Add(on_coords)
                    new_node.Add(N_coords)
                    new_node.Add(S_coords)
                    new_node.Add(E_coords)
                    new_node.Add(W_coords)
                    new_node.Add(NW_coords)
                    new_node.Add(NE_coords)
                    new_node.Add(SW_coords)
                    new_node.Add(SE_coords)

                    ' Add the node to the main list
                    nodes.Add(new_node)
                End If
            Next
        Next
    End Sub

    Function find_wall(ByVal nodeon As Array, ByVal wallz As List(Of List(Of Array)))
        Dim new_wallz As New List(Of Integer)

        For ind = 0 To wallz.Count() - 1

            If wallz(ind).Count() > 1 AndAlso (wallz(ind)(1)(0) = nodeon(0) And wallz(ind)(1)(1) = nodeon(1)) Then
                new_wallz.Add(ind)
            End If

        Next

        Return new_wallz

    End Function

    Function empty(ByVal coords As Array)
        'the coordd is empty if it is (-1, -1)->they are empty
        If coords(0) = -1 And coords(1) = -1 Then
            Return True
        End If

        Return False
    End Function

    Private Sub normalize_walls()
        Dim cur_walz As New List(Of List(Of Array))

        'turn coord pairs into walls e.g.(1, 1)+(3,3) into a wall
        For Each pair In Me.wallz
            Dim x1 As Integer = 0, y1 As Integer = 0
            Dim x2 As Integer = 0, y2 As Integer = 0

            If pair.Count = 1 Then
                'if the wall is made of 1 coord  create a box(a cross)
                x1 = pair(0)(0) * 50
                y1 = pair(0)(1) * 50

                Dim new_walla As New List(Of Array)

                Dim coorda(2) As Integer
                Dim coordb(2) As Integer

                coorda(0) = x1
                coorda(1) = y1

                coordb(0) = x1 + 50
                coordb(1) = y1 + 50

                new_walla.Add(coorda)
                new_walla.Add(coordb)

                Dim new_wallb As New List(Of Array)

                Dim coordc(2) As Integer
                Dim coordd(2) As Integer

                coordc(0) = x1 + 50
                coordc(1) = y1

                coordd(0) = x1
                coordd(1) = y1 + 50

                new_wallb.Add(coordc)
                new_wallb.Add(coordd)

                '2 walls creat a cross shape
                cur_walz.Add(new_walla)
                cur_walz.Add(new_wallb)

            ElseIf pair.Count = 2 Then
                ' pair of coords creating a wall
                x1 = pair(0)(0) * 50
                y1 = pair(0)(1) * 50

                x2 = pair(1)(0) * 50
                y2 = pair(1)(1) * 50


                Dim new_walla As New List(Of Array)

                Dim coorda(2) As Integer
                Dim coordb(2) As Integer

                coorda(0) = x1
                coorda(1) = y1

                coordb(0) = x2
                coordb(1) = y2

                new_walla.Add(coorda)
                new_walla.Add(coordb)

                cur_walz.Add(new_walla)

            End If

        Next

        Me.wallz = cur_walz
    End Sub

    Public Function ray_blocked(ByVal from_coords As Array, ByVal to_coords As Array)
        Dim coorda(2) As Integer
        Dim coordb(2) As Integer

        coorda = from_coords

        coordb = to_coords

        Dim colis As Boolean = False
        For Each wl In Me.wallz
            'uses fancy math to find if an intersection even exists
            Dim x1 = coorda(0)
            Dim x2 = coordb(0)
            Dim x3 = wl(0)(0)
            Dim x4 = wl(1)(0)

            Dim y1 = coorda(1)
            Dim y2 = coordb(1)
            Dim y3 = wl(0)(1)
            Dim y4 = wl(1)(1)

            Dim den As Double = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4)

            'division by 0
            If den = 0 Then
                Exit For
            End If

            Dim t As Double = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / den
            Dim u As Double = -(((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / den)

            If t >= 0 And t <= 1 And u >= 0 And u <= 1 Then
                colis = True
                Exit For
            End If
            '----------------------------------------------------------
        Next

        Return colis



    End Function

    Private Sub load_wallz()
        Me.create_nodes()
        Me.connect_nodes()
        Me.normalize_walls()
    End Sub

End Class
