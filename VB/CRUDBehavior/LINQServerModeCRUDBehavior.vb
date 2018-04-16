Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Interactivity
Imports System.Windows
Imports DevExpress.Xpf.Grid
Imports System.Windows.Controls
Imports System.Data.Linq
Imports System.Windows.Input
Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Core.ServerMode
Imports DevExpress.Xpf.Bars

Namespace LINQServer
	Public Class LINQServerModeCRUDBehavior
		Inherits CRUDBehaviorBase.CRUDBehaviorBase
		Public Shared ReadOnly DataSourceProperty As DependencyProperty = DependencyProperty.Register("DataSource", GetType(LinqServerModeDataSource), GetType(LINQServerModeCRUDBehavior), New PropertyMetadata(Nothing))
		Public Property DataSource() As LinqServerModeDataSource
			Get
				Return CType(GetValue(DataSourceProperty), LinqServerModeDataSource)
			End Get
			Set(ByVal value As LinqServerModeDataSource)
				SetValue(DataSourceProperty, value)
			End Set
		End Property
		Protected Overrides Function CanExecuteRemoveRowCommand() As Boolean
            If DataSource Is Nothing OrElse Grid Is Nothing OrElse View Is Nothing OrElse Grid.CurrentItem Is Nothing Then
                Return False
            End If
			Return True
		End Function
		Protected Overrides Sub UpdateDataSource()
			DataSource.Reload()
		End Sub
		Protected Overrides Sub OnAttached()
			MyBase.OnAttached()
			Grid.ItemsSource = DataSource.Data
		End Sub
	End Class
End Namespace