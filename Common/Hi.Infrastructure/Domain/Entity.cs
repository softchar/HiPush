using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Domain
{
    public class Entity
    {
        /// <summary>
        /// 标识
        /// </summary>
        public Guid Token { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastUpdateTime { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsRemoved { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public byte[] Version { get; set; }

        public Entity() {
            Token = Guid.NewGuid();
            CreateTime = DateTime.Now;
            LastUpdateTime = DateTime.Now;
            IsRemoved = RemoveState.Alive;
            Status = 0;
        }


        /// <summary>
        /// 不满足业务规则的集合
        /// </summary>
        private List<BusinessRule> _brokenRules = new List<BusinessRule>();

        /// <summary>
        /// 验证当前实体是否满足业务规则
        /// </summary>
        protected virtual void Validate() { }

        /// <summary>
        /// 获取不满足业务规则的集合
        /// </summary>
        /// <returns></returns>
        public List<BusinessRule> GetBrokenRules() {
            _brokenRules.Clear();
            Validate();
            return _brokenRules;
        }
        

        /// <summary>
        /// 添加一条业务规则
        /// </summary>
        /// <param name="businessRule"></param>
        public void AddBrokenRule(BusinessRule businessRule) {
            this._brokenRules.Add(businessRule);
        }

        public override bool Equals(object entity)
        {
            return entity != null && entity is Entity && this == (Entity)entity;
        }

        public override int GetHashCode()
        {
            return this.Token.GetHashCode();
        }

        public static bool operator ==(Entity entity1, Entity entity2) {
            if ((object)entity1 == null && (object)entity2 == null) {
                return true;
            }

            if ((object)entity1 == null || (object)entity2 == null) {
                return false;
            }

            //两个实体,如果Token相同,那么就认为是同一个对象
            if (entity1.Token == entity2.Token) {
                return true;
            }

            return false;
        }

        public static bool operator !=(Entity entity1, Entity entity2) {
            return !(entity1 == entity2);
        }
    }
}
