using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskManagerClean
{
    /// <summary>
    /// Управляет коллекцией задач: добавление, удаление, обновление, фильтрация и сортировка
    /// </summary>
    public class TaskManagerLogic
    {
        private List<Task> _tasks = new List<Task>();

        /// <summary>
        /// Возвращает список всех задач в виде доступного только для чтения
        /// </summary>
        public IReadOnlyList<Task> Tasks => _tasks.AsReadOnly();

        /// <summary>
        /// Добавляет новую задачу в коллекцию
        /// </summary>
        /// <param name="title">Название задачи. Не может быть пустым или состоять только из пробелов</param>
        /// <param name="description">Описание задачи</param>
        /// <param name="dueDate">Дата выполнения задачи</param>
        /// <param name="priority">Приоритет задачи (Low, Medium, High)</param>
        /// <returns>true, если задача успешно добавлена; false, если название пустое</returns>
        public bool AddTask(string title, string description, DateTime dueDate, TaskPriority priority)
        {
            if (string.IsNullOrWhiteSpace(title))
                return false;

            var task = new Task
            {
                Title = title,
                Description = description,
                DueDate = dueDate,
                Priority = priority
            };

            _tasks.Add(task);
            return true;
        }

        /// <summary>
        /// Удаляет задачу по её идентификатору
        /// </summary>
        /// <param name="id">Идентификатор задачи для удаления</param>
        /// <returns>true, если задача найдена и удалена; false, если задача с таким ID не существует</returns>
        public bool RemoveTask(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            return task != null && _tasks.Remove(task);
        }

        /// <summary>
        /// Удаляет несколько задач по списку идентификаторов
        /// </summary>
        /// <param name="ids">Коллекция идентификаторов задач для удаления</param>
        /// <returns>Количество успешно удалённых задач</returns>
        public int RemoveTasks(IEnumerable<int> ids)
        {
            var tasksToRemove = _tasks.Where(t => ids.Contains(t.Id)).ToList();
            int count = tasksToRemove.Count;
            foreach (var task in tasksToRemove)
                _tasks.Remove(task);
            return count;
        }

        /// <summary>
        /// Обновляет данные существующей задачи
        /// </summary>
        /// <param name="id">Идентификатор задачи для обновления</param>
        /// <param name="title">Новое название задачи</param>
        /// <param name="description">Новое описание задачи</param>
        /// <param name="dueDate">Новая дата выполнения</param>
        /// <param name="priority">Новый приоритет</param>
        /// <param name="status">Новый статус выполнения</param>
        /// <returns>true, если задача найдена и обновлена; false, если задача не найдена или название пустое</returns>
        public bool UpdateTask(int id, string title, string description, DateTime dueDate, TaskPriority priority, TaskStatus status)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null || string.IsNullOrWhiteSpace(title))
                return false;

            task.Title = title;
            task.Description = description;
            task.DueDate = dueDate;
            task.Priority = priority;
            task.Status = status;
            return true;
        }

        /// <summary>
        /// Фильтрует задачи по статусу выполнения
        /// </summary>
        /// <param name="status">Статус для фильтрации (Pending, Completed) или null для отображения всех задач</param>
        /// <returns>Коллекция задач, соответствующих фильтру</returns>
        public IEnumerable<Task> FilterByStatus(TaskStatus? status)
        {
            if (status == null)
                return _tasks;
            return _tasks.Where(t => t.Status == status);
        }

        /// <summary>
        /// Сортирует задачи по указанному критерию
        /// </summary>
        /// <param name="tasks">Коллекция задач для сортировки</param>
        /// <param name="sortBy">Критерий сортировки: "DueDate", "Priority", "Title" или любой другой для сортировки по ID</param>
        /// <returns>Отсортированная коллекция задач</returns>
        public IEnumerable<Task> SortTasks(IEnumerable<Task> tasks, string sortBy)
        {
            if (sortBy == "DueDate")
                return tasks.OrderBy(t => t.DueDate);
            else if (sortBy == "Priority")
                return tasks.OrderByDescending(t => t.Priority);
            else if (sortBy == "Title")
                return tasks.OrderBy(t => t.Title);
            else
                return tasks.OrderBy(t => t.Id);
        }
    }
}