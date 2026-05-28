using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TaskManagerClean
{
    /// <summary>
    /// Главная форма приложения TaskManager
    /// </summary>
    public partial class Form1 : Form
    {
        private TaskManagerLogic _taskManager = new TaskManagerLogic();
        private Task _selectedTask = null;

        /// <summary>
        /// Конструктор формы. Инициализирует компоненты и настраивает начальное состояние
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            AddItemsToComboBoxes();
            SetupForm();
            RefreshTaskList();
        }

        /// <summary>
        /// Принудительно добавляет элементы в выпадающие списки и устанавливает значения по умолчанию
        /// </summary>
        private void AddItemsToComboBoxes()
        {
            cmbPriority.Items.Clear();
            cmbPriority.Items.Add("Low");
            cmbPriority.Items.Add("Medium");
            cmbPriority.Items.Add("High");
            cmbPriority.SelectedIndex = 1;

            cmbSortBy.Items.Clear();
            cmbSortBy.Items.Add("ID");
            cmbSortBy.Items.Add("DueDate");
            cmbSortBy.Items.Add("Priority");
            cmbSortBy.Items.Add("Title");
            cmbSortBy.SelectedIndex = 0;

            cmbFilterStatus.Items.Clear();
            cmbFilterStatus.Items.Add("Все");
            cmbFilterStatus.Items.Add("Pending");
            cmbFilterStatus.Items.Add("Completed");
            cmbFilterStatus.SelectedIndex = 0;
        }

        /// <summary>
        /// Настраивает дополнительные параметры формы: режим рисования списка и обработчик события
        /// </summary>
        private void SetupForm()
        {
            listBoxTasks.DrawMode = DrawMode.OwnerDrawFixed;
            listBoxTasks.DrawItem += ListBoxTasks_DrawItem;
        }

        /// <summary>
        /// Обновляет список задач в ListBox с учётом текущих фильтра и сортировки
        /// </summary>
        private void RefreshTaskList()
        {
            if (listBoxTasks == null) return;

            var tasks = _taskManager.Tasks;

            string filter = cmbFilterStatus.SelectedItem?.ToString();
            TaskStatus? statusFilter = null;
            if (filter == "Pending") statusFilter = TaskStatus.Pending;
            else if (filter == "Completed") statusFilter = TaskStatus.Completed;

            if (statusFilter.HasValue)
                tasks = _taskManager.FilterByStatus(statusFilter).ToList();

            string sortBy = cmbSortBy.SelectedItem?.ToString();
            if (sortBy == "DueDate")
                tasks = tasks.OrderBy(t => t.DueDate).ToList();
            else if (sortBy == "Priority")
                tasks = tasks.OrderByDescending(t => t.Priority).ToList();
            else if (sortBy == "Title")
                tasks = tasks.OrderBy(t => t.Title).ToList();
            else
                tasks = tasks.OrderBy(t => t.Id).ToList();

            listBoxTasks.DataSource = null;
            listBoxTasks.DataSource = tasks.ToList();
            listBoxTasks.DisplayMember = "ToString";
        }

        /// <summary>
        /// Отрисовывает элемент списка с цветовой индикацией: просроченные задачи отображаются красным цветом
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события рисования</param>
        private void ListBoxTasks_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            var task = listBoxTasks.Items[e.Index] as Task;
            if (task == null) return;

            e.DrawBackground();
            Color textColor = task.IsOverdue() ? Color.Red : Color.Black;
            using (var brush = new SolidBrush(textColor))
            {
                e.Graphics.DrawString(task.ToString(), e.Font, brush, e.Bounds);
            }
            e.DrawFocusRectangle();
        }

        /// <summary>
        /// Обработчик выбора задачи в списке. Заполняет поля формы данными выбранной задачи
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        private void listBoxTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedTask = listBoxTasks.SelectedItem as Task;
            if (_selectedTask != null)
            {
                txtTitle.Text = _selectedTask.Title;
                txtDescription.Text = _selectedTask.Description;
                dtpDueDate.Value = _selectedTask.DueDate;
                cmbPriority.SelectedItem = _selectedTask.Priority.ToString();
                chkCompleted.Checked = _selectedTask.Status == TaskStatus.Completed;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Добавить". Создаёт новую задачу и добавляет её в список
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название задачи!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TaskPriority priority = TaskPriority.Medium;
            if (cmbPriority.SelectedItem != null)
            {
                string priorityText = cmbPriority.SelectedItem.ToString();
                if (priorityText == "Low") priority = TaskPriority.Low;
                else if (priorityText == "High") priority = TaskPriority.High;
            }

            bool result = _taskManager.AddTask(
                txtTitle.Text.Trim(),
                txtDescription.Text.Trim(),
                dtpDueDate.Value,
                priority
            );

            if (result)
            {
                ClearFields();
                RefreshTaskList();
                MessageBox.Show("Задача добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Редактировать". Обновляет данные выбранной задачи
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedTask == null)
            {
                MessageBox.Show("Не выбрана задача для выполнения действия!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TaskPriority priority = TaskPriority.Medium;
            if (cmbPriority.SelectedItem != null)
            {
                string priorityText = cmbPriority.SelectedItem.ToString();
                if (priorityText == "Low") priority = TaskPriority.Low;
                else if (priorityText == "High") priority = TaskPriority.High;
            }

            TaskStatus status = chkCompleted.Checked ? TaskStatus.Completed : TaskStatus.Pending;

            bool result = _taskManager.UpdateTask(
                _selectedTask.Id,
                txtTitle.Text.Trim(),
                txtDescription.Text.Trim(),
                dtpDueDate.Value,
                priority,
                status
            );

            if (result)
            {
                ClearFields();
                RefreshTaskList();
                MessageBox.Show("Задача обновлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Удалить выбранные". Удаляет все выделенные задачи из списка
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (listBoxTasks.SelectedItems.Count == 0)
            {
                MessageBox.Show("Не выбрана задача для выполнения действия!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Удалить {listBoxTasks.SelectedItems.Count} задач(и)?", "Подтверждение",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var idsToDelete = listBoxTasks.SelectedItems.Cast<Task>().Select(t => t.Id);
                int deleted = _taskManager.RemoveTasks(idsToDelete);
                ClearFields();
                RefreshTaskList();
                MessageBox.Show($"Удалено задач: {deleted}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Очистить поля". Очищает все поля ввода
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        private void btnClearFields_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        /// <summary>
        /// Очищает все поля ввода и сбрасывает выбранную задачу
        /// </summary>
        private void ClearFields()
        {
            txtTitle.Text = "";
            txtDescription.Text = "";
            dtpDueDate.Value = DateTime.Today;
            if (cmbPriority.Items.Count > 0) cmbPriority.SelectedIndex = 1;
            chkCompleted.Checked = false;
            _selectedTask = null;
            listBoxTasks.ClearSelected();
        }

        /// <summary>
        /// Обработчик изменения выбора в списке сортировки. Обновляет список задач
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        private void cmbSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTaskList();
        }

        /// <summary>
        /// Обработчик изменения выбора в списке фильтрации. Обновляет список задач
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события</param>
        private void cmbFilterStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTaskList();
        }
    }
}