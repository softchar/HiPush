using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.Infrastructure.Querying
{
    public class Query
    {
        //子查询
        private IList<Query> _subQueries = new List<Query>();

        //查询条件
        private IList<Criterion> _criteria = new List<Criterion>();

        public IEnumerable<Criterion> Criteria {
            get { return _criteria; }
        }

        public IEnumerable<Query> SubQueries {
            get { return _subQueries; }
        }

        /// <summary>
        /// 添加子查询
        /// </summary>
        /// <param name="subQuery"></param>
        public void AddSubQuery(Query subQuery) {
            _subQueries.Add(subQuery);
        }

        /// <summary>
        /// 添加查询条件
        /// </summary>
        /// <param name="criterion"></param>
        public void Add(Criterion criterion) {
            _criteria.Add(criterion);
        }

        /// <summary>
        /// 查询操作 
        ///     and/or
        /// </summary>
        public QueryOperator QueryOperator { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public OrderByClause OrderByClause { get; set; }
    }
}
