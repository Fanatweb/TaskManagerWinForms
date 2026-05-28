using System;

namespace TaskManagerClean
{
    /// <summary>
    /// Представляет задачу с названием, описанием, датой, приоритетом и статусом выполнения
    /// </summary>
    public class Task
    {
        private static int _nextId = 1;

        /// <summary>
        /// Уникальный идентификатор задачи
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Название задачи
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Описание задачи
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Дата выполнения задачи
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Приоритет задачи (Low, Medium, High)
        /// </summary>
        public TaskPriority Priority { get; set; }

        /// <summary>
        /// Статус выполнения задачи (Pending, Completed)
        /// </summary>
        public TaskStatus Status { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр задачи с автоматической генерацией ID
        /// </summary>
        public Task()
        {
            Id = _nextId++;
            Status = TaskStatus.Pending;
        }

        /// <summary>
        /// Возвращает строковое представление задачи для отображения в списке
        /// </summary>
        /// <returns>Строка вида "[ ] 1: Название (01.01.2026, High)" для невыполненных или "[✔] 1: Название (01.01.2026, High)" для выполненных</returns>
        public override string ToString()
        {
            string checkMark = Status == TaskStatus.Completed ? "✔" : " ";
            return $"[{checkMark}] {Id}: {Title} ({DueDate:dd.MM.yyyy}, {Priority})";
        }

        /// <summary>
        /// Определяет, просрочена ли задача
        /// </summary>
        /// <returns>true, если дата выполнения меньше сегодняшней и задача не выполнена; иначе false</returns>
        public bool IsOverdue()
        {
            return DueDate.Date < DateTime.Today && Status == TaskStatus.Pending;
        }
    }
}