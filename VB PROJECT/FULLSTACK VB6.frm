VERSION 5.00
Object = "{67397AA1-7FB1-11D0-B148-00A0C922E820}#6.0#0"; "MSADODC.OCX"
Object = "{CDE57A40-8B86-11D0-B3C6-00A0C90AEA82}#1.0#0"; "MSDATGRD.OCX"
Begin VB.Form Inicio 
   Caption         =   "Usuarios"
   ClientHeight    =   7905
   ClientLeft      =   120
   ClientTop       =   465
   ClientWidth     =   14130
   LinkTopic       =   "Form1"
   ScaleHeight     =   7905
   ScaleWidth      =   14130
   StartUpPosition =   3  'Windows Default
   Begin VB.CommandButton Command2 
      Caption         =   "Novo Registro"
      Height          =   495
      Left            =   1320
      TabIndex        =   3
      Top             =   5400
      Width           =   1215
   End
   Begin VB.CommandButton Command1 
      Caption         =   "Excluir"
      Height          =   495
      Left            =   480
      TabIndex        =   2
      Top             =   5400
      Width           =   615
   End
   Begin MSDataGridLib.DataGrid DataGrid1 
      Bindings        =   "FULLSTACK VB6.frx":0000
      Height          =   4455
      Left            =   0
      TabIndex        =   0
      Top             =   840
      Width           =   14055
      _ExtentX        =   24791
      _ExtentY        =   7858
      _Version        =   393216
      HeadLines       =   1
      RowHeight       =   15
      BeginProperty HeadFont {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ColumnCount     =   2
      BeginProperty Column00 
         DataField       =   ""
         Caption         =   ""
         BeginProperty DataFormat {6D835690-900B-11D0-9484-00A0C91110ED} 
            Type            =   0
            Format          =   ""
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   1046
            SubFormatType   =   0
         EndProperty
      EndProperty
      BeginProperty Column01 
         DataField       =   ""
         Caption         =   ""
         BeginProperty DataFormat {6D835690-900B-11D0-9484-00A0C91110ED} 
            Type            =   0
            Format          =   ""
            HaveTrueFalseNull=   0
            FirstDayOfWeek  =   0
            FirstWeekOfYear =   0
            LCID            =   1046
            SubFormatType   =   0
         EndProperty
      EndProperty
      SplitCount      =   1
      BeginProperty Split0 
         BeginProperty Column00 
         EndProperty
         BeginProperty Column01 
         EndProperty
      EndProperty
   End
   Begin MSAdodcLib.Adodc Adodc1 
      Height          =   615
      Left            =   3960
      Top             =   7080
      Visible         =   0   'False
      Width           =   5295
      _ExtentX        =   9340
      _ExtentY        =   1085
      ConnectMode     =   0
      CursorLocation  =   3
      IsolationLevel  =   -1
      ConnectionTimeout=   15
      CommandTimeout  =   30
      CursorType      =   3
      LockType        =   3
      CommandType     =   8
      CursorOptions   =   0
      CacheSize       =   50
      MaxRecords      =   0
      BOFAction       =   0
      EOFAction       =   0
      ConnectStringType=   3
      Appearance      =   1
      BackColor       =   -2147483643
      ForeColor       =   -2147483640
      Orientation     =   0
      Enabled         =   0
      Connect         =   "DSN=PostgreSQL30"
      OLEDBString     =   ""
      OLEDBFile       =   ""
      DataSourceName  =   "PostgreSQL30"
      OtherAttributes =   ""
      UserName        =   ""
      Password        =   ""
      RecordSource    =   "select * from ""usuarios"" "
      Caption         =   "Usuarios"
      BeginProperty Font {0BE35203-8F91-11CE-9DE3-00AA004BB851} 
         Name            =   "MS Sans Serif"
         Size            =   8.25
         Charset         =   0
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      _Version        =   393216
   End
   Begin VB.Label Label1 
      Caption         =   "Label1"
      DataField       =   "id"
      DataSource      =   "Adodc1"
      Height          =   375
      Left            =   13080
      TabIndex        =   1
      Top             =   0
      Width           =   975
   End
End
Attribute VB_Name = "Inicio"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False


Private Sub Form_Load()
    Dim connectionString As String
    Set cn = New ADODB.Connection
    Set rs = New ADODB.Recordset
    Dim sqlQuery As String
    
    connectionString = "DSN=PostgreSQL30;Uid=postgres;Pwd=123456;"
    cn.Open connectionString
    If cn.State = adStateOpen Then
    
    sqlQuery = "select id from Usuarios limit 1"
    rs.Open sqlQuery, cn, adOpenStatic, adLockReadOnly
    
    Else
        MsgBox "Não foi possível estabelecer conexão com o banco de dados."
    End If
   

    cn.Close
    Set rs = Nothing
    Set cn = Nothing

End Sub

Private Sub Command1_Click()
    Dim xmlhttp As Object
    Dim url As String
    

    url = "https://localhost:5001/api/Usuarios/"
    url = url & Label1

    Set xmlhttp = CreateObject("MSXML2.XMLHTTP")
    

    xmlhttp.Open "DELETE", url, False

    xmlhttp.Send

    If xmlhttp.Status <> 404 Then
        MsgBox "DELETE bem-sucedido!"
            Adodc1.Refresh
    Else
        MsgBox "Erro ao executar DELETE. Código de status: " & xmlhttp.Status & " - " & xmlhttp.statusText
    End If
    

    Set xmlhttp = Nothing
End Sub

Private Sub Command2_Click()
    Dim xmlhttp As Object
    Dim url As String
    Dim responseText As String
    Dim timestamp As String
    timestamp = Now
    
    url = "https://localhost:5001/api/RandomUser/randomuser?timestamp=" & timestamp

    Set xmlhttp = CreateObject("MSXML2.XMLHTTP")

    xmlhttp.Open "GET", url, False
    xmlhttp.Send
    

    If xmlhttp.Status = 200 Then
        responseText = xmlhttp.responseText

        MsgBox "Novo Usuario randômico adicionado com sucesso "
        Adodc1.Refresh
    Else
        MsgBox "Erro ao executar GET. Código de status: " & xmlhttp.Status & " - " & xmlhttp.statusText
    End If
    
    Set xmlhttp = Nothing
End Sub



