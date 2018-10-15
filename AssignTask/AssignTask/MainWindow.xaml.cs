using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AssignTask
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FillTeamMember(); //加入此專案的TeamMember
            FillTask(); //加入每個Package的工作項目
            this.taskListBox1.SelectedIndex = 1;
        }
        ProjectManagementEntities1 DbContext = new ProjectManagementEntities1();
        private void FillTask()
        {
            var q = from p in this.DbContext.Tasks
                          select p;
            foreach (var taskitem in q.ToList())
            {               
                ImageBrush myBrush = new ImageBrush();
                myBrush.ImageSource = new BitmapImage(new Uri("http://www.zaarapp.com/assets/images/Task.PNG", UriKind.Absolute));
                DockPanel dp = new DockPanel();
                Image task = new Image { Width = 30, Height = 30, Margin = new Thickness(2), Source = myBrush.ImageSource };
                Label taskTitle = new Label { Width = 100, Height = 30, Content = taskitem.TaskName };
                dp.Tag = taskTitle.Content; //tag for taskListBox1_SelectionChanged
                dp.Children.Add(task);
                dp.Children.Add(taskTitle);
                this.taskListBox1.Items.Add(dp);
            }
        }

       
        private void FillTeamMember()
        {
            var q = from p in this.DbContext.Employee
                    select p;

            foreach (var empitems in q.ToList())
            {
                //image之後放員工照片
                Label teamMember = new Label { Width = 60, Height = 70, AllowDrop = true, Content = empitems.EmployeeName ,Foreground =new SolidColorBrush(Colors.Coral)};
                ImageBrush ib = new ImageBrush(); //ToDo 之後加入員工大頭照
                ib.ImageSource = new BitmapImage(new Uri("https://upload.wikimedia.org/wikipedia/commons/thumb/7/7e/Circle-icons-profile.svg/1024px-Circle-icons-profile.svg.png", UriKind.Absolute));
                teamMember.Background = ib;
                teamMember.HorizontalAlignment = HorizontalAlignment.Left;
                teamMember.VerticalAlignment = VerticalAlignment.Bottom;
                teamMember.Margin = new Thickness(5);
                teamMember.Drop += TeamMember_Drop;
                teamMember.MouseDown += TeamMember_MouseDown1;
                teamMember.Tag = empitems.EmployeeName; //Todo 有資料庫之後可能要改(取得員工姓名)
                this.teamMemberWrapPanel.Children.Add(teamMember);
            }
        }

        private void TeamMember_MouseDown1(object sender, MouseButtonEventArgs e)
        {
            var q = from p in this.DbContext.Employee
                    where p.EmployeeName == ((Label)sender).Tag.ToString()
                    select p;

            foreach (var item in q.ToList())
            {
                this.Opacity = 0.5;
                EmployeeDetail employeeDetail = new EmployeeDetail { Height = this.Height - 50, Owner = this, };
                employeeDetail.employeePic.Background = ((Label)sender).Background;
                employeeDetail.employeeName.Content = ((Label)sender).Tag; //ToDo var q 從employeeName找出員工基本資料(部門、工號)                                        
                employeeDetail.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                employeeDetail.employeeID.Content = item.EmployeeID;
                employeeDetail.employeeSkill.Content = "C#";
               employeeDetail.Title = ((Label)sender).Tag + " 的基本資料";
                employeeDetail.Closed += delegate (object sender_eD, EventArgs e_eD)
                {
                    this.Opacity = 1;
                };
                employeeDetail.ShowDialog();
            }
        }

        private void TeamMember_Drop(object sender, DragEventArgs e)
        {
            ((DockPanel)this.taskListBox1.SelectedItem).Children[1].IsEnabled = false;
            ((Label)sender).Foreground = new SolidColorBrush(Colors.Red);
            ((Label)sender).FontSize = 13;
            try //Listviewadd
            {
                var q = from p in this.DbContext.Employee
                        where p.EmployeeName == ((Label)sender).Tag.ToString()
                        select p;

                    foreach (var item in q.ToList())
                {
                    Label lbl = new Label { Content = e.Data.GetData(DataFormats.Text) };
                    this.dataGrid1.Items.Add(new MyItem { EmployeeName = ((Label)sender).Tag + "",
                        Task = lbl.Content + "",
                        Desc = this.taskDesc.Text,
                        EmployeeID = item.EmployeeID,
                        Dep = item.Department.DepartmentName,                     
                    });
                }
                }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.gridLabel1.Visibility = Visibility.Hidden;
        }

        private void taskListBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).Items[0].ToString() != "工作項目")
            {
                try
                {
                    //ToDo Desc Change 作業描述選取後變更

                    this.taskRunInTitle.Content = ((DockPanel)this.taskListBox1.SelectedItems[0]).Tag;
                    var q = from p in this.DbContext.Tasks
                            where p.TaskName == this.taskRunInTitle.Content.ToString()
                            select p;
                    foreach (var descitem in q.ToList())
                    {
                        this.taskDesc.Text = descitem.Description;
                     }
                }
                catch { }
            }
        }
       
        bool mainWindowFlag;
        private void taskAssignImg1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (this.mainWindowFlag != true)
            //{
            //    mainWindowFlag = true;
            //    this.mainWindow.Width = 465;
            //}
            //else
            //{
            //    this.mainWindow.Width = 0;
            //    mainWindowFlag = false;
            //}
        }

        private void homeImg1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //this.mainwarpPanel1.Children.RemoveAt(1);
        }

        bool menuFlag;
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (menuFlag != true)
            {
                Storyboard sb = (Storyboard)this.Resources["sb1"];
                sb.Begin();
                menuFlag = true;
            }
            else
            {
                Storyboard sb = (Storyboard)this.Resources["sb2"];
                sb.Begin();
                menuFlag = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow2.Children.Count <= 5)
            {
                Label remarkLabel = new Label { Height = 30, Width = 463, Content = "備註" };
                TextBox remarkTextBox = new TextBox { Height = 100, Width = 400, Background = new SolidColorBrush(Colors.White), Margin = new Thickness(5) };
                this.mainWindow2.Children.Add(remarkLabel);
                this.mainWindow2.Children.Add(remarkTextBox);
            }
        }

        private void DragActivityName(object sender, MouseButtonEventArgs e)
        {          
            try
            {               
                Label lbl = (Label)sender;
                DragDrop.DoDragDrop(lbl, lbl.Content, DragDropEffects.Copy);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }      
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            #region A.dataGrid To Excel Method
            if (this.gridLabel1.Visibility == Visibility.Visible) return;
            try
                {
                    this.dataGrid1.SelectAllCells();
                    this.dataGrid1.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                    ApplicationCommands.Copy.Execute(null, this.dataGrid1);
                    String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
                    String result = (string)Clipboard.GetData(DataFormats.Text);
                    this.dataGrid1.UnselectAllCells();
                    StreamWriter file1 = new StreamWriter(@"D:\工作分配.xls", false, Encoding.Default);
                    file1.WriteLine(result.Replace(',', ' '));
                    file1.Close();
                    MessageBox.Show("輸出Excel成功");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }                   
             #endregion
        }

        #region MouseMoveEnterClickEvent
        private void menuImg1_MouseEnter(object sender, MouseEventArgs e)
        {          
            this.menuImg1.Opacity = 0.5;
        }

        private void menuImg1_MouseLeave(object sender, MouseEventArgs e)
        {
            this.menuImg1.Opacity = 1;
        }

        private void homeImg1_MouseEnter(object sender, MouseEventArgs e)
        {
            this.homeImg1.Opacity = 0.5;
        }

        private void homeImg1_MouseLeave(object sender, MouseEventArgs e)
        {
            this.homeImg1.Opacity = 1;
        }

        private void taskAssignImg1_MouseEnter(object sender, MouseEventArgs e)
        {
            this.taskAssignImg1.Opacity = 0.5;
        }

        private void taskAssignImg1_MouseLeave(object sender, MouseEventArgs e)
        {
            this.taskAssignImg1.Opacity = 1;
        }

        private void workLoadingImg1_MouseEnter(object sender, MouseEventArgs e)
        {
            this.workLoadingImg1.Opacity = 0.5;
        }

        private void workLoadingImg1_MouseLeave(object sender, MouseEventArgs e)
        {
            this.workLoadingImg1.Opacity = 1;
        }
        #endregion

        private void dataGrid1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {         
            try
            {
                if (this.dataGrid1.SelectedItem != null)
                {                   
                    for (var i = 1; i < this.taskListBox1.Items.Count; i++)
                    {
                        var v = this.taskListBox1.Items[i] as DockPanel;
                        string delLabel = ((Label)v.Children[1]).Content.ToString();                       
                        if (delLabel == ((MyItem)this.dataGrid1.SelectedItem).Task)
                        {
                            ((DockPanel)this.taskListBox1.Items[i]).Children[1].IsEnabled = true;
                        }
                    }
                    this.dataGrid1.Items.Remove((MyItem)this.dataGrid1.SelectedItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e) //Finish
        {
            if (this.gridLabel1.Visibility == Visibility.Visible) return;
            try
            {
                for (int i = 0; i < this.dataGrid1.Items.Count; i++)
                {
                    string taskword = ((MyItem)this.dataGrid1.Items[i]).Task;
                    var q = (from task in this.DbContext.Tasks
                             where task.TaskName == taskword
                             select task).FirstOrDefault();
                    //if (q == null) return;
                    int EmpID= ((MyItem)this.dataGrid1.Items[i]).EmployeeID;
                    q.EmployeeID = EmpID;
                    this.DbContext.SaveChanges();
                }
                MessageBox.Show("分配完成");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void Button_Click_3(object sender, RoutedEventArgs e) // Reassign
        {
                for (int i = 1; i < this.taskListBox1.Items.Count; i++)
                {
                    var v = this.taskListBox1.Items[i] as DockPanel;
                    ((Label)v.Children[1]).IsEnabled = true;
                }
                this.dataGrid1.Items.Clear();
            this.gridLabel1.Visibility = Visibility.Visible; //ref
        }

        #region WorkLoad
        bool wLFlag;
        private void workLoadingImg1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (wLFlag != true)
            {
                this.teamMemberWrapPanel.Children.Clear();
                taskRunInTitle.Visibility = Visibility.Hidden;
                taskDesc.Visibility = Visibility.Hidden;
                this.scr1.Height = 500;            
                wLFlag = true;
                this.taskListBox1.IsEnabled = false;
                FillWorkLoadInformation();
            }
            else
            {
                this.teamMemberWrapPanel.Children.Clear();
                FillTeamMember();
                taskRunInTitle.Visibility = Visibility.Visible;
                taskDesc.Visibility = Visibility.Visible;
                this.scr1.Height = 161;
                wLFlag = false;
                this.taskListBox1.IsEnabled = true;
            }
        }

        private void FillWorkLoadInformation()
        {
            var q = from p in this.DbContext.Tasks
                    select p;

            DataGrid dg = new DataGrid { Width = 400,Height = 200 ,IsReadOnly =true};
            this.teamMemberWrapPanel.Children.Add(dg);           
            foreach (var item in q.ToList())
            {
                dg.Items.Add(new MyItem {
                    EmployeeName = item.Employee.EmployeeName,
                    Task = item.TaskName,
                    StartDate =item.StartDate,
                    EndDate = item.EndDate,
                    ProjectName = item.Works.Project.ProjectName,                    
                });
            }                      
            AddWorkLoadGridColumn(dg);
            AddChart();
        }

        private void AddChart()
        {
            UserControl_Chart_Bar uc = new UserControl_Chart_Bar { Width= 300,Height=200};
            uc.Margin = new Thickness(50);
            this.teamMemberWrapPanel.Children.Add(uc);
        }

        private void AddWorkLoadGridColumn(DataGrid dg)
        {
            DataGridTextColumn NameColumn5 = new DataGridTextColumn();
            NameColumn5.Header = "案子名稱";
            NameColumn5.Binding = new Binding("ProjectName");
            dg.Columns.Add(NameColumn5);
            DataGridTextColumn NameColumn2 = new DataGridTextColumn();
            NameColumn2.Header = "工作";
            NameColumn2.Binding = new Binding(" Task");
            dg.Columns.Add(NameColumn2);
            DataGridTextColumn NameColumn = new DataGridTextColumn();
            NameColumn.Header = "被分配工作員工";
            NameColumn.Binding = new Binding("EmployeeName");
            dg.Columns.Add(NameColumn);
            DataGridTextColumn NameColumn3 = new DataGridTextColumn();
            NameColumn3.Header = "實際開始使間";
            NameColumn3.Binding = new Binding("StartDate");
            dg.Columns.Add(NameColumn3);
            DataGridTextColumn NameColumn4 = new DataGridTextColumn();
            NameColumn4.Header = "實際結束使間";
            NameColumn4.Binding = new Binding("EndDate");
            dg.Columns.Add(NameColumn4);
        }

        #endregion
    }
}





