using Hi.Model.Application;
using Hi.Model.Devices;
using Hi.Model.Messaging;
using Hi.Model.Pushing;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Repository.DataContextStorage
{
    public class HiDataContext : DbContext
    {
        public HiDataContext()
            : base("name=HiEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
        
        public virtual DbSet<Application> Application { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<MessageBody> MessageBody { get; set; }
        public virtual DbSet<PushMessage> PushMessage { get; set; }
        public virtual DbSet<Device> Device { get; set; }
        public virtual DbSet<MessageEntryEvent> MessageEntryEvent { get; set; }
        public virtual DbSet<MessagePushEvent> MessageResendEvent { get; set; }
        public virtual DbSet<DeviceInvalidEvent> DeviceInvalidEvent { get; set; }
        //public virtual DbSet<DeviceConnection> DeviceConnection { get; set; }
        
    }
}
