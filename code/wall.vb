Public Class wall
    Inherits base_block

    Private _text_mapp_used As List(Of String)

    Public WriteOnly Property text_mapp_used
        Set(ByVal value)
            _text_mapp_used = value
        End Set
    End Property

    Public Sub New(ByVal game As Form, ByVal height As Integer, ByVal width As Integer, ByVal x As Integer, ByVal y As Integer)
        MyBase.New(game, height, width, x, y)
        Me.box.BackColor = Color.DarkGray
    End Sub

    'Wall tiles are solid-colour blocks. Keep this method signature because the
    'existing game code calls it, but do not load a sprite or use a file path.
    Public Sub change_img(ByVal map As List(Of String), ByVal image_paths As String)
        Me.box.Image = Nothing
        Me.box.BackColor = Color.DarkGray
    End Sub

    Public Function find_neighbours(ByVal map As List(Of String))
        Dim result As New List(Of Boolean())
        Dim newy As Integer = 0
        Dim newx As Integer = 0

        If Me.x <> 0 Then newx = CInt(Me.x / 50)
        If Me.y <> 0 Then newy = CInt(Me.y / 50)
        newy += 1

        If newy < 0 OrElse newy >= map.Count OrElse newx < 0 OrElse newx >= map(newy).Length Then Return result
        If map(newy)(newx) = "b" Then
            Dim neighbors(7) As Boolean
            Dim directions As Integer(,) = {{0, -1}, {1, -1}, {1, 0}, {1, 1}, {0, 1}, {-1, 1}, {-1, 0}, {-1, -1}}
            For ind As Integer = 0 To 7
                Dim nx As Integer = newx + directions(ind, 0)
                Dim ny As Integer = newy + directions(ind, 1)
                If nx < 0 OrElse ny < 1 OrElse ny > map.Count - 2 OrElse nx >= map(1).Length Then
                    neighbors(ind) = True
                ElseIf nx < map(ny).Length Then
                    neighbors(ind) = (map(ny)(nx) = "b")
                End If
            Next
            result.Add(neighbors)
        End If
        Return result
    End Function

    Public Function blocakble(ByVal map, ByVal y, ByVal x)
        Dim empty_tiles() As String = {".", "e", "w", "p", "B", "N", "d"}
        Dim tile_on = map(y)(x)
        For Each pos_tiles In empty_tiles
            If pos_tiles = tile_on Then Return True
        Next
        Return False
    End Function

    Public Function real_wall(ByVal ynew As Integer, ByVal xnew As Integer)
        For y_add = -1 To 1
            For x_add = -1 To 1
                If ynew + y_add <= _text_mapp_used.Count() - 2 AndAlso xnew + x_add <= _text_mapp_used(1).Length() - 1 AndAlso ynew + y_add >= 1 AndAlso xnew + x_add >= 0 Then
                    If blocakble(_text_mapp_used, (_text_mapp_used.Count() - 1) - ((ynew + y_add) - 1), xnew + x_add) Then Return True
                End If
            Next
        Next
        Return False
    End Function
End Class
