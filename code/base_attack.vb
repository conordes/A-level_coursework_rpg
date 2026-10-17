
Public Class base_attack
    Inherits base_block

    Public life_time As Integer = 0
    Public spawn_time As Integer = 0

    Private _typeofattack As String
    Public ReadOnly Property typeofattack
        Get
            Return _typeofattack
        End Get
    End Property

    Public base_block_obj As base_block

    Public dead As Boolean = False

    Private _damage As Integer = 1
    Public Property damage As Integer
        Get
            Return _damage
        End Get
        Set(ByVal value As Integer)
            _damage = value
        End Set
    End Property

    Public Sub New(ByVal game As Form, ByVal height As Integer, ByVal width As Integer, ByVal x As Integer, ByVal y As Integer, ByVal life_time As Integer, ByVal typo As String, ByVal originalimage As Bitmap, ByVal dmg As Integer, Optional ByVal angle As Double = 0)
        MyBase.New(game, height, width, x, y)

        Me.life_time = life_time

        Me._typeofattack = typo
        Me.damage = dmg

        Me.rotate_img(Me.box, originalimage, angle)

        Me.box.SizeMode = PictureBoxSizeMode.StretchImage

    End Sub

    Public Sub rotate_img(ByVal pic As PictureBox, ByVal original_image As Image, ByVal angle_radians As Double)
        'gets the angle from the sent position to the target
        Dim angle_degrees As Double = angle_radians * (180 / Math.PI)

        'creates a bitmap of the dimensions of the original image
        Dim rotated_bmp As New Bitmap(original_image.Width, original_image.Height)

        'rotates the image by the angle specified
        Using g As Graphics = Graphics.FromImage(rotated_bmp)

            g.TranslateTransform(CSng(original_image.Width / 2), CSng(original_image.Height / 2))
            g.RotateTransform(CSng(angle_degrees))

            g.TranslateTransform(-CSng(original_image.Width / 2), -CSng(original_image.Height / 2))
            g.DrawImage(original_image, New Point(0, 0))

        End Using
        '----------------------------------------

        'applies image to the bitmap so it can be shown on screen
        pic.Image = rotated_bmp
    End Sub

    Function direction_to(ByVal mousex As Integer, ByVal mousey As Integer, ByVal entity As base_block, ByVal speed As Integer)
        ' if the attack is moving towards the player(like firebal) it uses this

        'find the change in x and change in y
        Dim dx As Double = mousex - entity.x
        Dim dy As Double = mousey - entity.y
        Dim dist As Double = Math.Sqrt(dx ^ 2 + dy ^ 2)

        Dim end_coords(2) As Double

        ' Avoid division by zero
        If dx = 0 And dy = 0 Then
            end_coords(0) = 0
            end_coords(1) = 0
            Return end_coords

        ElseIf dx = 0 Then
            end_coords(0) = 0
            end_coords(1) = (dy / dist) * speed
            Return end_coords

        ElseIf dy = 0 Then
            end_coords(1) = 0
            end_coords(0) = (dx / dist) * speed
            Return end_coords
        Else
            end_coords(0) = (dx / dist) * speed
            end_coords(1) = (dy / dist) * speed
            Return end_coords

        End If

        'normalise and multiply by the speed

        '---------------------------------------------------------------------
        Return end_coords
    End Function

    Public Sub move(ByVal mousex As Integer, ByVal mousey As Integer, ByVal entity As base_block, Optional ByVal speed As Integer = 8)

        'chooses if an attack can move
        If Me.typeofattack = "fireball" Or Me.typeofattack = "web" Then

            Dim coords As Array = direction_to(mousex, mousey, entity, speed)

            Me.xvel = coords(0)
            Me.yvel = coords(1)

        End If
        '----------------------------
    End Sub

    Function check_alive(ByVal tick As Integer)
        'check if the attacks been alive too long
        If (tick - spawn_time > life_time) Or Me.dead Then
            Return True
        End If
        '-------------------------------------------
        Return False
    End Function

    Sub update()

        'updates the coordinates in memory for usage
        Me.x += Me.xvel
        Me.y += Me.yvel

        'update the picturebox location on the screen
        Me.box.Left = Me.x
        Me.box.Top = Me.y

    End Sub

End Class
