using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Data.Repositories.Interfaces
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
        Task<int> GetUnreadNotificationsCountAsync(int userId);
    }
}
