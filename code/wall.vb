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

    End Sub


    Public Sub change_img(ByVal map As List(Of String), ByVal image_paths As String)
        Dim endof As String = Me.get_ending(map, image_paths)

        'image path is already known all images are just ordered by number to make them loaded in easily {little code needed}
        'tile_{number}.png
        Dim img As String = image_paths + endof
        img += ".png"

        Dim originalImage As Bitmap = Bitmap.FromFile(img)

        Me.box.Image = originalImage
        Me.box.SizeMode = PictureBoxSizeMode.StretchImage
        '--------------------------------------------------------------------------------------------------------------------

    End Sub

    Public Function find_neighbours(ByVal map As List(Of String))
        'every wall finds all neighbouring tiles

        Dim result As New List(Of Boolean())

        Dim newy As Integer = 0
        Dim newx As Integer = 0

        'not on the edge
        If Me.x <> 0 Then
            newx = Me.x / 50
        End If

        If Me.y <> 0 Then
            newy = Me.y / 50
        End If
        '---------------

        newy += 1

        If map(newy)(newx) = "b" Then
            Dim neighbors(7) As Boolean ' N, NE, E, SE, S, SW, W, NW
            'all possible directions
            Dim directions As Integer(,) = {
                {0, -1}, {1, -1}, {1, 0}, {1, 1},
                {0, 1}, {-1, 1}, {-1, 0}, {-1, -1}
            }
            'iterates through all possible directions
            For ind As Integer = 0 To 7
                '{x, y}
                Dim dx As Integer = directions(ind, 0)
                Dim dy As Integer = directions(ind, 1)

                Dim nx As Integer = (newx) + dx
                Dim ny As Integer = (newy) + dy
                'checks edges
                If nx < 0 Or ny < 1 Or nx > map(1).Length() - 1 Or ny > map.Count() - 2 Then
                    neighbors(ind) = True
                    Continue For
                End If

                If ny >= 0 AndAlso ny < map.Count AndAlso nx >= 0 AndAlso nx < map(ny).Length Then
                    If map(ny)(nx) = "b" Then
                        neighbors(ind) = True

                    End If
                End If
            Next

            result.Add(neighbors)

        End If

        '-------------------------------------------

        Return result
    End Function

    Private Function get_ending(ByVal map As List(Of String), ByVal image_paths As String) As String
        Dim current_cell As List(Of Boolean()) = Me.find_neighbours(map)

        'goes through the neighbours checking which are full
        If current_cell.Count() > 0 Then

            Dim neighbors As Array = current_cell(0)

            Dim N As Boolean = neighbors(0)
            Dim E As Boolean = neighbors(2)
            Dim S As Boolean = neighbors(4)
            Dim W As Boolean = neighbors(6)

            Dim NE = neighbors(1)
            Dim SE = neighbors(3)
            Dim NW = neighbors(7)
            Dim SW = neighbors(5)

            Dim endof As String = ""
            Dim final_img As String = image_paths

            ' check which image to be choosen based on the neighbours the wall has (not all have been done as some are rare to occur and having the image doesnt change them much):
            If NW And N And W And Not SE And Not E And Not S Then
                endof = "2"
            ElseIf W And S And SW And Not NE And Not N And Not E Then
                endof = "3"
            ElseIf E And S And SE And Not N And Not W And Not NW Then
                endof = "4"
            ElseIf N And E And NE And Not SW And Not S And Not W Then
                endof = "5"
            ElseIf N And NE And E And SE And S And SW And W And Not NW Then
                endof = "6"
            ElseIf N And NW And W And NE And E And SE And S And Not SW Then
                endof = "7"
            ElseIf N And NW And W And SW And SW And S And SE And E And Not NE Then
                endof = "36"
            ElseIf N And NE And NW And W And SW And S And E And Not SE Then
                endof = "8"
            ElseIf N And S And Not E And NW And W And SW Or (N And S And Not E And NW And W And SW And (NE Or SE)) Then
                endof = "13"
            ElseIf N And S And NE And E And SE And Not W Or (N And S And NE And E And SE And Not W And (NW Or SW)) Then
                endof = "15"
            ElseIf N And E And W And NE And NW And Not S Or (N And E And W And NE And NW And Not S And (SW Or NE)) Then
                endof = "14"
            ElseIf S And E And W And SW And SE And Not N Or (S And E And W And SW And SE And Not N And (NE Or NW)) Then
                endof = "12"
            ElseIf NW And N And W And SE And S And E And Not N And Not SW Then
                endof = "17"
            ElseIf N And NE And E And S And SW And W And Not NE And Not SE Then
                endof = "16"
            ElseIf Not E And W And S And N And (NW Or SW) Then
                endof = "13"
            ElseIf Not N And Not S And Not SE And Not NE And Not SW And Not NW And E And W Or (Not N And Not S And E And W And (NE Or SE Or SW Or NW)) Then
                endof = "38"
            Else
                endof = "37"
            End If
            '-----------------------------------------------------------------------------------------------------------------------------------------------------------------


            Return endof
        End If
        Return 0
    End Function

    Public Function blocakble(ByVal map, ByVal y, ByVal x) 'g
        'checks if adajaceent tiles are empty for wall images
        Dim empty_tiles() As String = {".", "e", "w", "p", "B", "N", "d"}

        Dim tile_on = map(y)(x)

        For Each pos_tiles In empty_tiles

            If pos_tiles = tile_on Then
                Return True
            End If

        Next

        '----------------------------------------------------
        Return False
    End Function

    Public Function real_wall(ByVal ynew As Integer, ByVal xnew As Integer)

        'checks if any surrounding tiles are empty , for efficiency so the check is not needed.
        'and to differentiate between real walls and fake walls

        For y_add = -1 To 1
            For x_add = -1 To 1

                'check if the wall is, checking somewhere out of range
                If ynew + y_add <= _text_mapp_used.Count() - 2 And xnew + x_add <= _text_mapp_used(1).Length() - 1 And ynew + y_add >= 1 And xnew + x_add >= 0 Then

                    If blocakble(_text_mapp_used, (_text_mapp_used.Count() - 1) - ((ynew + y_add) - 1), xnew + x_add) Then

                        Return True

                    End If

                End If

            Next
        Next
        '--------------------------------------------------------------------------------------

        Return False

    End Function
End Class
