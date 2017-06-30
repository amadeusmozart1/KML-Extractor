Imports System.IO

Public Class Form1

    Dim counter As Integer = 0
    Dim cutOffValue As Double = 0
    Dim cutOffValueEntered = False

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Load

        Me.Icon = New Icon(Path.GetDirectoryName(Application.ExecutablePath) + "\antenna.ico")
        Label4.Text = "Offsett value: 0dBm"
    End Sub


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' List files in the folder.
            ListFiles(FolderBrowserDialog1.SelectedPath, "Button1")
        End If
    End Sub


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            ' List files in the folder.
            ListFiles(FolderBrowserDialog1.SelectedPath, "Button2")
        End If
    End Sub


    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If ListBox2.SelectedItem Is Nothing Or ListBox1.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a file.")
            Exit Sub
        End If

        ' Obtain the file path from the list box selection.
        Dim inputFilePath = ListBox1.SelectedItem.ToString
        Dim outputFilePath = ListBox2.SelectedItem.ToString

        ' Verify that the file was not removed since the
        ' Browse button was clicked.
        If Not My.Computer.FileSystem.FileExists(inputFilePath) Then
            MessageBox.Show("File not found: " & inputFilePath)
            Exit Sub
        End If

        If Not My.Computer.FileSystem.FileExists(outputFilePath) Then
            MessageBox.Show("File not found: " & outputFilePath)
            Exit Sub
        End If

        ' Obtain file information in a string.
        Extract(inputFilePath, outputFilePath)

    End Sub


    Private Sub ListFiles(ByVal folderPath As String, whichButton As String)

        If (whichButton = "Button1") Then
            ListBox1.Items.Clear()

            Dim fileNames = My.Computer.FileSystem.GetFiles(
            folderPath, FileIO.SearchOption.SearchTopLevelOnly)

            For Each fileName As String In fileNames
                ListBox1.Items.Add(fileName)
            Next
        End If

        If (whichButton = "Button2") Then
            ListBox2.Items.Clear()

            Dim fileNames = My.Computer.FileSystem.GetFiles(
            folderPath, FileIO.SearchOption.SearchTopLevelOnly)

            For Each fileName As String In fileNames
                ListBox2.Items.Add(fileName)
            Next
        End If

    End Sub


    Private Sub Extract(inputFilePath As String, outputFilePath As String)

        Try
            If My.Computer.FileSystem.FileExists(inputFilePath) Then
                If My.Computer.FileSystem.FileExists(outputFilePath) Then

                    Dim measurementName As String = InputBox("Enter measurement name for KML:")
                    counter = 0

                    Dim file As System.IO.StreamWriter
                    file = My.Computer.FileSystem.OpenTextFileWriter(outputFilePath, True)
                    file.WriteLine("<?xml version=""1.0"" encoding=""UTF-8""?>")
                    file.WriteLine("<kml>")
                    file.WriteLine("<Document>")
                    If Not measurementName = "" Then
                        file.WriteLine("<name>" + measurementName + "</name>")
                    Else file.WriteLine("<name>DefaultName</name>")
                    End If

                    For Each line As String In System.IO.File.ReadAllLines(inputFilePath)
                        Dim data() As String = line.Split(";")

                        Dim latitude As String = data(0)
                        Dim substringLatitude1 As Integer = latitude.Substring(1, 2)
                        Dim substringLatitude2 As Integer = latitude.Substring(5, 2)
                        Dim substringLatitude3 As Integer = latitude.Substring(8, 4)
                        Dim calculatedLatitude As Double = ((substringLatitude2 + substringLatitude3 / 10000) / 60)

                        Dim longitude As String = data(1)
                        Dim substringLongitude1 As Integer = longitude.Substring(2, 2)
                        Dim substringLongitude2 As Integer = longitude.Substring(6, 2)
                        Dim substringLongitude3 As Integer = longitude.Substring(9, 4)
                        Dim calculatedLongitude As Double = ((substringLongitude2 + substringLongitude3 / 10000) / 60)

                        counter += 1

                        Dim pwrLevelString As String = data(2).Remove(0, 1)
                        pwrLevelString = pwrLevelString.Remove(pwrLevelString.Length - 1, 1)

                        Dim pwrLevel As Double = pwrLevelString / 10
                        pwrLevel = pwrLevel + TrackBar1.Value



                        If (cutOffValueEntered = True And pwrLevel <= cutOffValue) Then

                            Continue For

                        Else
                            If (pwrLevel <= -106) Then

                                file.WriteLine("<Placemark>" +
                                           "<Style><IconStyle><Icon><href>http://maps.google.com/mapfiles/kml/pushpin/red-pushpin.png</href>" +
                                           "</Icon></IconStyle></Style>" +
                                           "<name>" + pwrLevel.ToString + "</name><description>latitude=" +
                                           substringLatitude1.ToString + "." + calculatedLatitude.ToString.Remove(0, 2) +
                                           "  " + "longitude=" +
                                           substringLongitude1.ToString + "." + calculatedLongitude.ToString.Remove(0, 2) +
                                           "</description>" + "<Point><coordinates>" +
                                          substringLongitude1.ToString + "." + calculatedLongitude.ToString.Remove(0, 2) + "," +
                                          substringLatitude1.ToString + "." + calculatedLatitude.ToString.Remove(0, 2) +
                                          "</coordinates></Point></Placemark>")

                            ElseIf (pwrLevel <= -103 And pwrLevel > -106) Then

                                file.WriteLine("<Placemark>" +
                                           "<Style><IconStyle><Icon><href>http://maps.google.com/mapfiles/kml/pushpin/ylw-pushpin.png</href>" +
                                           "</Icon></IconStyle></Style>" +
                                           "<name>" + pwrLevel.ToString + "</name><description>latitude=" +
                                           substringLatitude1.ToString + "." + calculatedLatitude.ToString.Remove(0, 2) +
                                           "  " + "longitude=" +
                                           substringLongitude1.ToString + "." + calculatedLongitude.ToString.Remove(0, 2) +
                                           "</description>" + "<Point><coordinates>" +
                                          substringLongitude1.ToString + "." + calculatedLongitude.ToString.Remove(0, 2) + "," +
                                          substringLatitude1.ToString + "." + calculatedLatitude.ToString.Remove(0, 2) +
                                          "</coordinates></Point></Placemark>")

                            ElseIf (pwrLevel > -103) Then

                                file.WriteLine("<Placemark>" +
                                           "<Style><IconStyle><Icon><href>http://maps.google.com/mapfiles/kml/pushpin/grn-pushpin.png</href>" +
                                           "</Icon></IconStyle></Style>" +
                                           "<name>" + pwrLevel.ToString + "</name><description>latitude=" +
                                           substringLatitude1.ToString + "." + calculatedLatitude.ToString.Remove(0, 2) +
                                           "  " + "longitude=" +
                                           substringLongitude1.ToString + "." + calculatedLongitude.ToString.Remove(0, 2) +
                                           "</description>" + "<Point><coordinates>" +
                                          substringLongitude1.ToString + "." + calculatedLongitude.ToString.Remove(0, 2) + "," +
                                          substringLatitude1.ToString + "." + calculatedLatitude.ToString.Remove(0, 2) +
                                          "</coordinates></Point></Placemark>")

                            End If
                        End If
                    Next

                    file.WriteLine("</Document>")
                    file.WriteLine("</kml>")
                    file.Close()

                    MessageBox.Show("Extraction done!")

                Else MessageBox.Show("File not found: " & outputFilePath)
                End If
            Else MessageBox.Show("File not found: " & inputFilePath)
            End If
        Catch ex As Exception
            ListBox3.Items.Add("Error in line: " + counter.ToString + "     " + ex.ToString)
        End Try

    End Sub


    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        Label4.Text = "Offset value: " + TrackBar1.Value.ToString + "dBm"
    End Sub


    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

        Try
            cutOffValue = InputBox("Enter cutoff value (eg: -110,0):")
            cutOffValueEntered = True
        Catch ex As Exception
            ListBox3.Items.Add("Error: " + ex.ToString)
            cutOffValueEntered = False
        End Try

    End Sub

End Class
