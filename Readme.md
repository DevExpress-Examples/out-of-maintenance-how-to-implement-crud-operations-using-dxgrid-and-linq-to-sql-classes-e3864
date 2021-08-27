<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128651102/13.1.4%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E3864)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [CRUDBehaviorBase.cs](./CS/CRUDBehavior/CRUDBehaviorBase.cs) (VB: [CRUDBehaviorBase.vb](./VB/CRUDBehavior/CRUDBehaviorBase.vb))
* [LINQInstantModeCRUDBehavior.cs](./CS/CRUDBehavior/LINQInstantModeCRUDBehavior.cs) (VB: [LINQInstantModeCRUDBehavior.vb](./VB/CRUDBehavior/LINQInstantModeCRUDBehavior.vb))
* [LINQServerModeCRUDBehavior.cs](./CS/CRUDBehavior/LINQServerModeCRUDBehavior.cs) (VB: [LINQServerModeCRUDBehavior.vb](./VB/CRUDBehavior/LINQServerModeCRUDBehavior.vb))
* [MainWindow.xaml](./CS/LINQInstant/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/LINQInstant/MainWindow.xaml))
* [MainWindow.xaml.cs](./CS/LINQInstant/MainWindow.xaml.cs) (VB: [MainWindow.xaml.vb](./VB/LINQInstant/MainWindow.xaml.vb))
* [MainWindow.xaml](./CS/LINQServer/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/LINQServer/MainWindow.xaml))
* [MainWindow.xaml.cs](./CS/LINQServer/MainWindow.xaml.cs) (VB: [MainWindow.xaml.vb](./VB/LINQServer/MainWindow.xaml.vb))
<!-- default file list end -->
# How to implement CRUD operations using DXGrid and LINQ to SQL Classes


<p>This example shows how to use LinqInstantFeedbackDataSource or LinqServerModeDataSource with DXGrid, and how to implement CRUD operations (e.g., add, remove, edit) in your application via special behavior.</p><p><strong>Note</strong> that the test sample requires the SQL Express service to be installed on your machine.</p><p>We have created the LINQServerModeCRUDBehavior and LINQInstantModeCRUDBehavior attached behaviors for GridControl. For instance: </p>

```xml
<dxg:GridControl>

   <dxmvvm:Interaction.Behaviors>

       <crud:LINQServerModeCRUDBehavior ...>

           <crud:LINQServerModeCRUDBehavior.DataSource/>

               <dxsm:LinqServerModeDataSource .../>

           </crud:LINQServerModeCRUDBehavior.DataSource>

       </crud:LINQServerModeCRUDBehavior>

   </dxmvvm:Interaction.Behaviors>

</dxg:GridControl>



```

<p>The LINQServerModeCRUDBehavior and LINQInstantModeCRUDBehavior classes contain the NewRowForm and EditRowForm properties to provide the "Add Row" and "Edit Row" actions. With these properties, you can create the Add and Edit forms according to your requirements:</p>

```xml
<DataTemplate x:Key="EditRecordTemplate">

   <StackPanel Margin="8" MinWidth="200">

       <Grid>

           <Grid.ColumnDefinitions>

               <ColumnDefinition/>

               <ColumnDefinition/>

           </Grid.ColumnDefinitions>

           <Grid.RowDefinitions>

               <RowDefinition/>

               <RowDefinition/>

           </Grid.RowDefinitions>

           <TextBlock Text="ID:" VerticalAlignment="Center" Grid.Row="0" Grid.Column="0" Margin="0,0,6,4" />

           <dxe:TextEdit x:Name="txtID" Grid.Row="0" Grid.Column="1" EditValue="{Binding Path=Id, Mode=TwoWay}" Margin="0,0,0,4" />

           <TextBlock Text="Name:" VerticalAlignment="Center" Grid.Row="1" Grid.Column="0" Margin="0,0,6,4" />

           <dxe:TextEdit x:Name="txtCompany" Grid.Row="1" Grid.Column="1" EditValue="{Binding Path=Name, Mode=TwoWay}" Margin="0,0,0,4" />

       </Grid>

   </StackPanel>

</DataTemplate>

<crud:LINQServerModeCRUDBehavior NewRowForm="{StaticResource ResourceKey=EditRecordTemplate}" EditRowForm="{StaticResource ResourceKey=EditRecordTemplate}"/> 





```

<p>These Behavior classes require the following information from your data model:</p><p>- RowType - the type of rows;</p><p>- DataContext - database entities;</p><p>- DataSource - an object of the LinqInstantFeedbackDataSource or LinqServerModeDataSource type.</p>

```xml
<dxg:GridControl>

   <i:Interaction.Behaviors>

       <crud:LINQServerModeCRUDBehavior RowType="{x:Type local:Item}" DataContext="{Binding Source={StaticResource DataClassesDataContext}}">

           <crud:LINQServerModeCRUDBehavior.DataSource>

               <dxsm:LinqServerModeDataSource KeyExpression="Id" QueryableSource="{Binding Items, Source={StaticResource DataClassesDataContext}}"/>

           </crud:LINQServerModeCRUDBehavior.DataSource>

       </crud:LINQServerModeCRUDBehavior>

   </i:Interaction.Behaviors>

</dxg:GridControl> 





```

<p>See the <a href="http://documentation.devexpress.com/#WPF/clsDevExpressXpfCoreServerModeLinqServerModeDataSourcetopic"><u>LinqServerModeDataSource</u></a> and <a href="http://documentation.devexpress.com/#WPF/clsDevExpressXpfCoreServerModeLinqInstantFeedbackDataSourcetopic"><u>LinqInstantFeedbackDataSource</u></a> classes to learn more about LinqServerModeDataSource and LinqInstantFeedbackDataSource .</p><p>Behavior class descendants support the following commands: NewRowCommand, RemoveRowCommand, EditRowCommand. You can bind your interaction controls with these commands with ease. For instance:</p>

```xml
<crud:LINQServerModeCRUDBehavior x:Name="helper"/>

<StackPanel Grid.Row="1" Orientation="Horizontal" HorizontalAlignment="Center">

   <Button Height="22" Width="60" Command="{Binding Path=NewRowCommand, ElementName=helper}">Add</Button>

   <Button Height="22" Width="60" Command="{Binding Path=RemoveRowCommand, ElementName=helper}" Margin="6,0,6,0">Remove</Button>

   <Button Height="22" Width="60" Command="{Binding Path=EditRowCommand, ElementName=helper}">Edit</Button>

</StackPanel>





```

<p>                    </p><p>By default, the LINQServerModeCRUDBehavior and LINQInstantModeCRUDBehavior solutions support the following end-user interaction capabilities:</p><p>1. An end-user can edit selected row values by double-clicking on a grid row or by pressing the Enter key if the AllowKeyDownActions property is True.</p><p>2. An end-user can remove selected rows via the Delete key press if the AllowKeyDownActions property is True.</p><p>3. An end-user can add new rows, remove and edit them via the NewRowCommand, RemoveRowCommand, and EditRowCommand commands.</p>

<br/>


