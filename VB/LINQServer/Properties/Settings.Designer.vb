'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------
Namespace LINQServer.Properties

    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.8.1.0")>
    Friend NotInheritable Partial Class Settings
        Inherits Global.System.Configuration.ApplicationSettingsBase

        Private Shared defaultInstance As LINQServer.Properties.Settings = CType((Global.System.Configuration.ApplicationSettingsBase.Synchronized(New LINQServer.Properties.Settings())), LINQServer.Properties.Settings)

        Public Shared ReadOnly Property [Default] As Settings
            Get
                Return LINQServer.Properties.Settings.defaultInstance
            End Get
        End Property

        <Global.System.Configuration.ApplicationScopedSettingAttribute()>
        <Global.System.Diagnostics.DebuggerNonUserCodeAttribute()>
        <Global.System.Configuration.SpecialSettingAttribute(Global.System.Configuration.SpecialSetting.ConnectionString)>
        <Global.System.Configuration.DefaultSettingValueAttribute("Data Source=(localdb)\mssqllocaldb;AttachDbFilename=|DataDirectory|\Database.mdf;" & "Integrated Security=True")>
        Public ReadOnly Property DatabaseConnectionString1 As String
            Get
                Return(CStr((Me("DatabaseConnectionString1"))))
            End Get
        End Property
    End Class
End Namespace
