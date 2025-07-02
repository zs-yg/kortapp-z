using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Security;

namespace AppStore
{
    public class SelfStartingManagerForm : Form
    {
        private DataGridView dataGridView = new DataGridView();
        private Button refreshButton = new Button();
        private Button addButton = new Button();
        private Button removeButton = new Button();
        
        public SelfStartingManagerForm()
        {
            InitializeComponent();
            LoadStartupItems();
        }

        private void InitializeComponent()
        {
            this.Text = "自启动项管理";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            this.SuspendLayout();

            // 主表格
            dataGridView.Dock = DockStyle.Fill;
            dataGridView.Margin = new Padding(10);
            dataGridView.ReadOnly = true;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.BackgroundColor = SystemColors.Window;
            dataGridView.BorderStyle = BorderStyle.Fixed3D;
            
            // 添加列
            var iconColumn = new DataGridViewImageColumn();
            iconColumn.HeaderText = "图标";
            iconColumn.Name = "Icon";
            iconColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;
            iconColumn.FillWeight = 10;
            
            var nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.HeaderText = "名称";
            nameColumn.Name = "Name";
            nameColumn.FillWeight = 25;
            
            var pathColumn = new DataGridViewTextBoxColumn();
            pathColumn.HeaderText = "路径";
            pathColumn.Name = "Path";
            pathColumn.FillWeight = 65;
            
            dataGridView.Columns.AddRange(iconColumn, nameColumn, pathColumn);

            // 按钮面板
            var buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 60;
            buttonPanel.BackColor = SystemColors.Control;
            buttonPanel.Padding = new Padding(10);

            // 按钮样式
            var buttonStyle = new Size(90, 32);
            var buttonFont = new Font("Microsoft YaHei", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);

            // 刷新按钮
            refreshButton.Text = "刷新";
            refreshButton.Size = buttonStyle;
            refreshButton.Font = buttonFont;
            refreshButton.Click += (s, e) => LoadStartupItems();

            // 添加按钮
            addButton.Text = "添加";
            addButton.Size = buttonStyle;
            addButton.Font = buttonFont;
            addButton.Click += AddButton_Click;

            // 删除按钮
            removeButton.Text = "删除";
            removeButton.Size = buttonStyle;
            removeButton.Font = buttonFont;
            removeButton.Click += RemoveButton_Click;

            // 布局按钮
            var flowLayout = new FlowLayoutPanel();
            flowLayout.Dock = DockStyle.Fill;
            flowLayout.FlowDirection = FlowDirection.LeftToRight;
            flowLayout.Controls.AddRange(new Control[] { refreshButton, addButton, removeButton });
            flowLayout.WrapContents = false;
            flowLayout.AutoSize = true;
            
            buttonPanel.Controls.Add(flowLayout);

            // 主布局
            var mainPanel = new Panel();
            mainPanel.Dock = DockStyle.Fill;
            mainPanel.Padding = new Padding(10);
            mainPanel.Controls.Add(dataGridView);
            mainPanel.Controls.Add(buttonPanel);

            this.Controls.Add(mainPanel);
            this.ResumeLayout(false);

            // 初始加载
            LoadStartupItems();
        }

        private void LoadStartupItems()
        {
            dataGridView.Rows.Clear();
            dataGridView.Enabled = false;
            refreshButton.Enabled = false;

            try
            {
                Cursor = Cursors.WaitCursor;
                var items = SelfStartingManager.GetAllStartupItems();
                
                if (items.Count == 0)
                {
                    dataGridView.Rows.Add("未找到自启动项", "");
                }
                else
                {
                    foreach (var item in items)
                    {
                        Image iconImage = null;
                        try
                        {
                            if (!string.IsNullOrEmpty(item.Value) && System.IO.File.Exists(item.Value))
                            {
                                using (Icon icon = Icon.ExtractAssociatedIcon(item.Value))
                                {
                                    iconImage = icon.ToBitmap();
                                }
                            }
                            else
                            {
                                // 使用默认图标
                                iconImage = SystemIcons.Application.ToBitmap();
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogWarning($"无法加载程序图标: {item.Value}", ex);
                            iconImage = SystemIcons.Warning.ToBitmap();
                        }
                        
                        dataGridView.Rows.Add(iconImage, item.Key, item.Value);
                    }
                }
            }
            catch (SecurityException ex)
            {
                MessageBox.Show($"权限不足，无法读取注册表: {ex.Message}", "权限错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载自启动项失败: {ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dataGridView.Enabled = true;
                refreshButton.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new AddStartupItemDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (SelfStartingManager.AddStartupItem(dialog.ItemName, dialog.ItemPath))
                    {
                        MessageBox.Show("添加成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadStartupItems();
                    }
                    else
                    {
                        MessageBox.Show("添加失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择要删除的项", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = dataGridView.SelectedRows[0].Cells["Name"].Value?.ToString() ?? "";
            if (MessageBox.Show($"确定要删除 '{selectedItem}' 吗?", "确认删除", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (SelfStartingManager.RemoveStartupItem(selectedItem))
                {
                    MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStartupItems();
                }
                else
                {
                    MessageBox.Show("删除失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

    public class AddStartupItemDialog : Form
    {
        public string ItemName { get; private set; } = "";
        public string ItemPath { get; private set; } = "";
        
        private TextBox nameTextBox = new TextBox();
        private TextBox pathTextBox = new TextBox();
        private Button browseButton = new Button();
        private Button okButton = new Button();
        private Button cancelButton = new Button();

        public AddStartupItemDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "添加自启动项";
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // 名称标签和文本框
            var nameLabel = new Label();
            nameLabel.Text = "名称:";
            nameLabel.Location = new Point(20, 20);
            nameLabel.Size = new Size(60, 20);

            nameTextBox.Location = new Point(90, 20);
            nameTextBox.Size = new Size(280, 20);

            // 路径标签和文本框
            var pathLabel = new Label();
            pathLabel.Text = "路径:";
            pathLabel.Location = new Point(20, 50);
            pathLabel.Size = new Size(60, 20);

            pathTextBox.Location = new Point(90, 50);
            pathTextBox.Size = new Size(200, 20);

            // 浏览按钮
            browseButton.Text = "浏览...";
            browseButton.Location = new Point(300, 50);
            browseButton.Size = new Size(70, 23);
            browseButton.Click += BrowseButton_Click;

            // 确定按钮
            okButton.Text = "确定";
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(150, 100);
            okButton.Size = new Size(80, 30);
            okButton.Click += OkButton_Click;

            // 取消按钮
            cancelButton.Text = "取消";
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Location = new Point(250, 100);
            cancelButton.Size = new Size(80, 30);

            // 添加控件
            this.Controls.Add(nameLabel);
            this.Controls.Add(nameTextBox);
            this.Controls.Add(pathLabel);
            this.Controls.Add(pathTextBox);
            this.Controls.Add(browseButton);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "可执行文件 (*.exe)|*.exe|所有文件 (*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    pathTextBox.Text = dialog.FileName;
                    if (string.IsNullOrEmpty(nameTextBox.Text))
                    {
                        nameTextBox.Text = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);
                    }
                }
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("请输入名称", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(pathTextBox.Text))
            {
                MessageBox.Show("请选择路径", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ItemName = nameTextBox.Text;
            ItemPath = pathTextBox.Text;
        }
    }
}